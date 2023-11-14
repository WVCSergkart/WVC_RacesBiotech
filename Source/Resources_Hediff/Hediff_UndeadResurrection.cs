using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_UndeadResurrection : HediffCompProperties
	{

		// public bool shouldSendNotification = false;

		public int refreshTicks = 6000;

		// public FloatRange tendingQualityRange = new(0.1f, 1.0f);

		public HediffCompProperties_UndeadResurrection()
		{
			compClass = typeof(HediffCompUndeadResurrection);
		}
	}

	public class HediffCompUndeadResurrection : HediffComp
	{

		private int ticksToResurrection = 0;

		public HediffCompProperties_UndeadResurrection Props => (HediffCompProperties_UndeadResurrection)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			ticksToResurrection++;
			if (Props.refreshTicks > ticksToResurrection)
			{
				return;
			}
			UndeadUtility.Resurrect(parent.pawn);
			ticksToResurrection = 0;
			base.Pawn.health.RemoveHediff(parent);
		}
	}

}
