// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_XenotypesAndGenes : ScenPart_PawnModifier
	{

		public List<XenotypeChance> xenotypeChances = new();
        public List<XenotypeDef> allowedXenotypes = new();
        public List<GeneDef> geneDefs = new();
        public bool addMechlink = false;
        public bool nullifyBackstory = false;
        public List<GeneDef> chimeraGeneDefs = new();

        protected override void ModifyNewPawn(Pawn p)
        {
            SetXenotype(p);
            SetGenes(p);
            AddMechlink(p);
            NullifyBackstory(p);
            ChimeraGenes(p);
        }

        private void ChimeraGenes(Pawn p)
        {
            if (chimeraGeneDefs.NullOrEmpty())
            {
                return;
            }
            XaG_GeneUtility.AddGenesToChimera(p, chimeraGeneDefs);
        }

        private void SetGenes(Pawn p)
        {
            if (geneDefs.NullOrEmpty())
            {
                return;
            }
            foreach (GeneDef geneDef in geneDefs)
            {
                if (XaG_GeneUtility.HasGene(geneDef, p))
                {
                    continue;
                }
                p.genes.AddGene(geneDef, xenogene: !p.genes.Xenotype.inheritable);
            }
        }

        private void NullifyBackstory(Pawn p)
        {
            if (nullifyBackstory)
            {
                DuplicateUtility.NullifyBackstory(p);
            }
        }

        private void AddMechlink(Pawn p)
        {
            if (addMechlink && !p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
            {
                p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
            }
        }

        private void SetXenotype(Pawn p)
        {
            if (xenotypeChances.NullOrEmpty())
            {
                return;
            }
            List<XenotypeDef> xenotypes = new();
            foreach (XenotypeChance xenoChance in xenotypeChances)
            {
                xenotypes.Add(xenoChance.xenotype);
            }
            if (xenotypes.Contains(p.genes.Xenotype) || !allowedXenotypes.NullOrEmpty() && allowedXenotypes.Contains(p.genes.Xenotype))
            {
                return;
            }
            if (xenotypeChances.TryRandomElementByWeight((XenotypeChance xenoChance) => xenoChance.chance, out XenotypeChance xenotypeChance))
            {
                ReimplanterUtility.SetXenotype_DoubleXenotype(p, xenotypeChance.xenotype);
            }
        }

        public override string Summary(Scenario scen)
        {
            if (xenotypeChances.NullOrEmpty())
            {
                return base.Summary(scen);
            }
            return "WVC_AllowedXenotypes".Translate().CapitalizeFirst() + ":\n" + xenotypeChances.Select((XenotypeChance x) => x.xenotype.LabelCap.ToString()).ToLineList(" - ") + (!allowedXenotypes.NullOrEmpty() ? "\n" + allowedXenotypes.Select((XenotypeDef x) => x.LabelCap.ToString()).ToLineList(" - ") : "");
        }

    }

    //public class ScenPart_ForcedMechanitor : ScenPart_PawnModifier
    //{

    //    protected override void ModifyNewPawn(Pawn p)
    //    {
    //        if (!p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
    //        {
    //            p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
    //        }
    //    }

    //}

}
