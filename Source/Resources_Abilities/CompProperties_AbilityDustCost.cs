using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_AbilityDustCost : CompProperties_AbilityEffect
    {
        public float dustCost = 0.2f;

        public CompProperties_AbilityDustCost()
        {
            compClass = typeof(CompAbilityEffect_DustCost);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return (string)("WVC_XaG_AbilityDustCost".Translate() + ": ") + Mathf.RoundToInt(dustCost * 100f);
        }
    }

    public class CompAbilityEffect_DustCost : CompAbilityEffect
    {
        public new CompProperties_AbilityDustCost Props => (CompProperties_AbilityDustCost)props;

        private bool HasEnoughDust
        {
            get
            {
                Gene_Dust gene_Dust = parent.pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
                if (gene_Dust == null || gene_Dust.Value < Props.dustCost)
                {
                    return false;
                }
                return true;
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Gene_Dust gene_Dust = parent.pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
            if (gene_Dust != null)
            {
                DustUtility.OffsetDust(parent.pawn, 0f - Props.dustCost);
            }
        }

        public override bool GizmoDisabled(out string reason)
        {
            Gene_Dust gene_Dust = parent.pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
            if (gene_Dust == null)
            {
                reason = "WVC_XaG_AbilityDisabledNoDustGene".Translate(parent.pawn);
                return true;
            }
            if (gene_Dust.Value < Props.dustCost)
            {
                reason = "WVC_XaG_AbilityDisabledNoDust".Translate(parent.pawn);
                return true;
            }
            float num = TotalDustCostOfQueuedAbilities();
            float num2 = Props.dustCost + num;
            if (Props.dustCost > float.Epsilon && num2 > gene_Dust.Value)
            {
                reason = "WVC_XaG_AbilityDisabledNoDust".Translate(parent.pawn);
                return true;
            }
            reason = null;
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return HasEnoughDust;
        }

        private float TotalDustCostOfQueuedAbilities()
        {
            float num = (parent.pawn.jobs?.curJob?.verbToUse is not Verb_CastAbility verb_CastAbility) ? 0f : DustCost(verb_CastAbility.ability.comps);
            if (parent.pawn.jobs != null)
            {
                for (int i = 0; i < parent.pawn.jobs.jobQueue.Count; i++)
                {
                    if (parent.pawn.jobs.jobQueue[i].job.verbToUse is Verb_CastAbility verb_CastAbility2)
                    {
                        num += DustCost(verb_CastAbility2.ability.comps);
                    }
                }
            }
            return num;
        }

        public float DustCost(List<AbilityComp> comps)
        {
            if (comps != null)
            {
                foreach (AbilityComp comp in comps)
                {
                    if (comp is CompAbilityEffect_DustCost compAbilityEffect_DustCost)
                    {
                        return compAbilityEffect_DustCost.Props.dustCost;
                    }
                }
            }
            return 0f;
        }
    }

}
