// Verse.Gene_Healing
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_UnrealFastHealing : XaG_Gene
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(57581, delta))
			{
				HealingUtility.Regeneration(pawn, regeneration: 1, tick: 1);
			}
		}

	}

	[Obsolete]
	public class Gene_BodyPartsRestoration : Gene_UnrealFastHealing
	{
		//private int ticksToHealBodyPart;

		//// 15-30 days
		//private static readonly IntRange HealingIntervalTicksRange = new(900000, 1800000);

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	ResetInterval();
		//}

		//public override void TickInterval(int delta)
		//{
		//	ticksToHealBodyPart -= delta;
		//	if (ticksToHealBodyPart <= 0)
		//	{
		//		HealingUtility.TryHealRandomPermanentWound(pawn, this);
		//		ResetInterval();
		//	}
		//}

		//private void ResetInterval()
		//{
		//	ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: Restore lost limb",
		//			action = delegate
		//			{
		//				if (Active)
		//				{
		//					HealingUtility.TryHealRandomPermanentWound(pawn, this);
		//				}
		//				ResetInterval();
		//			}
		//		};
		//	}
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		//}
	}

}
