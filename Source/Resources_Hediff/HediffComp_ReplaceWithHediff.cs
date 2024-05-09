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

		public List<GeneDef> geneDefs;

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
			HediffUtility.BodyPartsGiver(Props.bodyparts, Pawn, Props.hediffDef, Props.geneDef);
			Log.Warning("Successfully updated hediff for the " + Props.geneDef.LabelCap + " gene.");
			removeHediff = true;
		}

	}

	public class HediffComp_ReplaceWhenRemoved : HediffComp
	{

		public HediffCompProperties_ReplaceWithHediff Props => (HediffCompProperties_ReplaceWithHediff)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			Pawn.health.RemoveHediff(parent);
		}

		public override void CompPostPostRemoved()
		{
			if (XaG_GeneUtility.HasAnyActiveGene(Props.geneDefs, Pawn))
			{
				HediffUtility.TryAddHediff(Props.hediffDef, Pawn, null, Props.bodyparts);
			}
		}

	}

	public class HediffComp_ReplaceWhenBrainDestroyed : HediffComp
	{

		public HediffCompProperties_ReplaceWithHediff Props => (HediffCompProperties_ReplaceWithHediff)props;

		public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (Pawn.health.hediffSet.GetBrain() == null)
			{
				HediffUtility.TryAddHediff(Props.hediffDef, Pawn, null);
				Pawn.health.RemoveHediff(parent);
			}
		}

	}

}
