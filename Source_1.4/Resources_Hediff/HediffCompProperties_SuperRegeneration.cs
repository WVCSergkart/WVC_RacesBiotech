using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class HediffCompProperties_SuperRegeneration : HediffCompProperties
	{

		public bool shouldSendNotification = false;

		public int refreshTicks = 6000;

		public FloatRange tendingQualityRange = new(0.1f, 1.0f);

		public HediffCompProperties_SuperRegeneration()
		{
			compClass = typeof(HediffCompSuperRegeneration);
		}
	}

	[Obsolete]
	public class HediffCompSuperRegeneration : HediffComp
	{

		public HediffCompProperties_SuperRegeneration Props => (HediffCompProperties_SuperRegeneration)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			Pawn pawn = parent.pawn;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = hediffs.Count - 1; num >= 0; num--)
			{
				if (hediffs[num].TendableNow() && !hediffs[num].IsTended())
				{
					hediffs[num].Tended(Props.tendingQualityRange.RandomInRange, Props.tendingQualityRange.TrueMax, 1);
				}
			}
			TaggedString taggedString = HealingUtility.FixWorstHealthCondition(pawn, null, false);
			if (Props.shouldSendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			}
		}
	}

}
