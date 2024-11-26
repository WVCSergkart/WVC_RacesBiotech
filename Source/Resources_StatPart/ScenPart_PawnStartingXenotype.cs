// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Xenotype : ScenPart_PawnModifier
	{

		public List<XenotypeChance> xenotypeChances = new();

		protected override void ModifyNewPawn(Pawn p)
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
            if (xenotypes.Contains(p.genes.Xenotype))
            {
                return;
            }
            if (xenotypeChances.TryRandomElementByWeight((XenotypeChance xenoChance) => xenoChance.chance, out XenotypeChance xenotypeChance))
            {
                ReimplanterUtility.SetXenotype(p, xenotypeChance.xenotype);
            }
        }

        public override string Summary(Scenario scen)
        {
            return "WVC_AllowedXenotypes".Translate().CapitalizeFirst() + ":\n" + xenotypeChances.Select((XenotypeChance x) => x.xenotype.LabelCap.ToString()).ToLineList(" - ");
        }

    }

    public class ScenPart_ForcedMechanitor : ScenPart_PawnModifier
    {

        protected override void ModifyNewPawn(Pawn p)
        {
            if (!p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
            {
                p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
            }
        }

    }

}
