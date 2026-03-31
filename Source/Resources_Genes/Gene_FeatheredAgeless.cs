using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_FeatheredAgeless : Gene_Ageless
	{

		//public string RemoteActionName => "WVC_XaG_FeatheredAgelessLabel".Translate();

		//public TaggedString RemoteActionDesc => "WVC_XaG_FeatheredAgelessDesc".Translate();

		//public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		//{
		//	Find.WindowStack.Add(new Dialog_FeatheredAgelessGenes(this));
		//	genesSettings.Close();
		//}

		//public bool RemoteControl_Hide => !Active;

		//public bool RemoteControl_Enabled
		//{
		//	get
		//	{
		//		return enabled;
		//	}
		//	set
		//	{
		//		enabled = value;
		//		remoteControllerCached = false;
		//	}
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		//}

		//public bool enabled = true;
		//public bool remoteControllerCached = false;

		//public void RemoteControl_Recache()
		//{
		//	XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (enabled)
		//	{
		//		yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
		//	}
		//}

		////======================================

		//private GeneExtension_Undead cachedGeneExtension;
		//public GeneExtension_Undead Undead
		//{
		//	get
		//	{
		//		if (cachedGeneExtension == null)
		//		{
		//			cachedGeneExtension = def.GetModExtension<GeneExtension_Undead>();
		//		}
		//		return cachedGeneExtension;
		//	}
		//}

		//public static int collectedYears = 0;

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(59955, delta))
			{
				AgeReversal();
			}
		}

		private void AgeReversal()
		{
			if (AgelessUtility.CanAgeReverse(pawn))
			{
				AgelessUtility.AgeReverse(pawn);
				//if (pawn.Faction == Faction.OfPlayerSilentFail)
				//{
				//	collectedYears++;
				//}
			}
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref collectedYears, "generalCollectedYears", 0);
		//}

	}

	[Obsolete]
	public class Gene_DustAgeless : Gene_FeatheredAgeless
	{
		//public readonly long oneYear = 3600000L;

		//public readonly long humanAdultAge = 18L;

		//private int ticksToAgeReversal;

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	AgelessUtility.InitialRejuvenation(pawn);
		//	ResetInterval();
		//}

		//public override void TickInterval(int delta)
		//{
		//	//base.delta();
		//	ticksToAgeReversal -= delta;
		//	if (ticksToAgeReversal > 0)
		//	{
		//		return;
		//	}
		//	ResetInterval();
		//	AgeReversal();
		//}

		//public void AgeReversal()
		//{
		//	if (AgelessUtility.CanAgeReverse(pawn))
		//	{
		//		AgelessUtility.AgeReverse(pawn);
		//	}
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: Revers age",
		//			action = delegate
		//			{
		//				//AgeReversal();
		//				//ResetInterval();
		//				ticksToAgeReversal = 0;
		//			}
		//		};
		//	}
		//}

		//private void ResetInterval()
		//{
		//	IntRange intRange = new(300000, 900000);
		//	ticksToAgeReversal = intRange.RandomInRange;
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref ticksToAgeReversal, "ticksToAgeReversal", 0);
		//}
	}

}
