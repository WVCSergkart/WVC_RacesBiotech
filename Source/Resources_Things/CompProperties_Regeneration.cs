using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_Regeneration : CompProperties
	{

		public bool shouldSendNotification = false;

		public int refreshTicks = 6000;

		public int regenAmount = 6;

		public CompProperties_Regeneration()
		{
			compClass = typeof(CompRegeneration);
		}
	}

	public class CompRegeneration : ThingComp
	{

		public CompProperties_Regeneration Props => (CompProperties_Regeneration)props;

		public override void CompTick()
		{
			base.CompTick();
			Pawn pawn = parent as Pawn;
			if (!pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			// Pawn pawn = parent.pawn;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = hediffs.Count - 1; num >= 0; num--)
			{
				// if (hediffs[num].TendableNow() && !hediffs[num].IsTended())
				// {
					// hediffs[num].Tended(Props.tendingQualityRange.RandomInRange, Props.tendingQualityRange.TrueMax, 1);
				// }
				// Log.Error("1");
				if (hediffs[num].def.isBad)
				{
					// Log.Error("2");
					hediffs[num].Heal(Props.regenAmount);
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
