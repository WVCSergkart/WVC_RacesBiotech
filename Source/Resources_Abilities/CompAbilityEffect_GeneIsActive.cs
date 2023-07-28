using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_AbilityGeneIsActive : CompProperties_AbilityEffect
    {
        public List<GeneDef> anyOfGenes;

        public List<GeneDef> eachOfGenes;

        public Gender gender = Gender.None;

        public CompProperties_AbilityGeneIsActive()
        {
            compClass = typeof(CompAbilityEffect_GeneIsActive);
        }
    }

    public class CompAbilityEffect_GeneIsActive : CompAbilityEffect
    {
        public new CompProperties_AbilityGeneIsActive Props => (CompProperties_AbilityGeneIsActive)props;

        public override bool GizmoDisabled(out string reason)
        {
            Pawn pawn = parent.pawn;
            if (Props.gender != Gender.None)
            {
                if (Props.gender != pawn.gender)
                {
                    reason = "WVC_XaG_AbilityGeneIsActive_PawnWrongGender".Translate(pawn);
                    return true;
                }
            }
            if (pawn?.genes == null)
            {
                reason = "WVC_XaG_AbilityGeneIsActive_PawnBaseliner".Translate(pawn);
                return true;
            }
            if (Props.eachOfGenes != null && Props.eachOfGenes.Count > 0)
            {
                foreach (GeneDef allSelectedItem in Props.eachOfGenes)
                {
                    if (!MechanoidizationUtility.HasActiveGene(allSelectedItem, pawn))
                    {
                        reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn) + ": " + allSelectedItem.label;
                        return true;
                    }
                }
            }
            if (Props.anyOfGenes != null && Props.anyOfGenes.Count > 0)
            {
                foreach (GeneDef allSelectedItem in Props.anyOfGenes)
                {
                    if (MechanoidizationUtility.HasActiveGene(allSelectedItem, pawn))
                    {
                        reason = null;
                        return false;
                    }
                }
                reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn);
                return true;
            }
            reason = null;
            return false;
        }
    }

}
