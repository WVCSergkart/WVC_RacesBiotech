using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_ReplaceWithHediff : HediffCompProperties
	{

		public GeneDef geneDef;

		public HediffDef hediffDef;

		public List<BodyPartDef> bodyparts;

		public HediffCompProperties_ReplaceWithHediff()
		{
			compClass = typeof(HediffComp_ReplaceWithHediff);
		}
	}

	public class HediffComp_ReplaceWithHediff : HediffComp
	{

		private bool removeHediff = false;

		public override bool CompShouldRemove => removeHediff;

		public HediffCompProperties_ReplaceWithHediff Props => (HediffCompProperties_ReplaceWithHediff)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (Props.hediffDef == null || Props.bodyparts.NullOrEmpty() || Props.geneDef == null)
			{
				Log.Error("Failed update hediff for the " + Props.geneDef.LabelCap + " gene.");
				removeHediff = true;
				return;
			}
			Gene_PermanentHediff.BodyPartsGiver(Props.bodyparts, Pawn, Props.hediffDef, Pawn.genes.GetGene(Props.geneDef));
			Log.Warning("Successfully updated hediff for the " + Props.geneDef.LabelCap + " gene.");
			removeHediff = true;
		}

	}

}
