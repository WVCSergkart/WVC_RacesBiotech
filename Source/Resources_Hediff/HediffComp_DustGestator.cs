using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_ImmaculateConception : HediffCompProperties
	{
		public float requiredSeverity = 0.9f;

		public float removeSeverity = 1.0f;

		public string completeMessage = "WVC_XaG_Gene_ImmaculateConception";

		public bool endogeneTransfer = true;

		public bool xenogeneTransfer = true;

		public HediffCompProperties_ImmaculateConception()
		{
			compClass = typeof(HediffComp_ImmaculateConception);
		}
	}

	public class HediffComp_ImmaculateConception : HediffComp
	{
		public HediffCompProperties_ImmaculateConception Props => (HediffCompProperties_ImmaculateConception)props;

		private bool canBirth = true;

		// protected int GestationIntervalDays => Props.gestationIntervalDays;

		// public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			Pawn pawn = parent.pawn;
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				base.Pawn.health.RemoveHediff(parent);
				return;
			}
			if (canBirth && parent.Severity >= Props.requiredSeverity)
			{
				canBirth = false;
				GestationUtility.GenerateNewBornPawn(parent.pawn, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer);
			}
			if (parent.Severity >= Props.removeSeverity)
			{
				base.Pawn.health.RemoveHediff(parent);
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref canBirth, "canBirth");
		}

		// public string GetLabel()
		// {
		// Pawn pawn = parent.pawn;
		// if (pawn.Faction == Faction.OfPlayer && pawn.ageTracker.CurLifeStage.reproductive)
		// {
		// float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
		// return percent.ToStringPercent();
		// }
		// return "";
		// }
	}

}
