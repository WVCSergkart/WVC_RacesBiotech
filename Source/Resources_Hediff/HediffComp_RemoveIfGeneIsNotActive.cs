using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_RemoveIfGeneIsNotActive : HediffCompProperties
	{
		public GeneDef geneDef;

		// 1 day
		public int checkInterval = 60000;

		public HediffCompProperties_RemoveIfGeneIsNotActive()
		{
			compClass = typeof(HediffComp_RemoveIfGeneIsNotActive);
		}
	}

	public class HediffComp_RemoveIfGeneIsNotActive : HediffComp
	{
		public GeneDef geneDef;

		public HediffCompProperties_RemoveIfGeneIsNotActive Props => (HediffCompProperties_RemoveIfGeneIsNotActive)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.checkInterval))
			{
				return;
			}
			if (geneDef == null && Props.geneDef != null)
			{
				// base.Pawn.health.RemoveHediff(parent);
				// return;
				geneDef = Props.geneDef;
			}
			if (!MechanoidizationUtility.HasActiveGene(geneDef, parent.pawn))
			{
				base.Pawn.health.RemoveHediff(parent);
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Defs.Look(ref geneDef, "geneDef");
		}

	}

}
