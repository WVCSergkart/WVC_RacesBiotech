using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_FeatheredAgeless : Gene_Ageless, IGeneRemoteControl
	{

		public string RemoteActionName => "WVC_XaG_FeatheredAgelessLabel".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_FeatheredAgelessDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Find.WindowStack.Add(new Dialog_FeatheredAgelessGenes(this));
			genesSettings.Close();
		}

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
			ResetCache();
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		//======================================

		public override string LabelCap => base.LabelCap + " - (" + "WVC_XaG_FeatheredAgelees_TotalGeneMatLabel".Translate(AllGeneMat) + ")";

		private static int? cachedGeneMat;

		public static void ResetCache()
		{
			cachedGeneMat = null;
		}

		public static List<Pawn> Colonists => PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists;
		public int AllGeneMat
		{
			get
			{
				if (cachedGeneMat == null)
				{
					try
					{
						SimpleCurve curveChrono = new()
						{
							new CurvePoint(1, 1),
							new CurvePoint(100, 60),
							new CurvePoint(200, 85),
							new CurvePoint(400, 100),
							new CurvePoint(1000, 150)
						};
						SimpleCurve curveBio = new()
						{
							new CurvePoint(1, 1),
							new CurvePoint(18, 80),
							new CurvePoint(20, 20),
							new CurvePoint(100, 40)
						};
						cachedGeneMat = Colonists.Where(pawn => pawn.genes?.GetFirstGeneOfType<Gene_FeatheredAgeless>() != null).Sum(pawn => (int)curveChrono.Evaluate(pawn.ageTracker.AgeChronologicalYears) + (int)curveBio.Evaluate(pawn.ageTracker.AgeBiologicalYears));
					}
					catch (Exception arg)
					{
						Log.Error("Failed count all player pawn summary years. Reason: " + arg.Message);
						cachedGeneMat = 0;
					}
				}
				return cachedGeneMat.Value;
			}
		}

		//======================================

		private GeneExtension_Undead cachedGeneExtension;
		public GeneExtension_Undead Undead
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Undead>();
				}
				return cachedGeneExtension;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
			if (MiscUtility.GameStarted())
			{
				nextTick = Cooldown;
			}
			else
			{
				nextTick = new IntRange(10000, Cooldown).RandomInRange;
			}
		}

		private int Cooldown => 300000; // 5 days

		public int nextTick = 120;

		public override void TickInterval(int delta)
		{
			if (GeneResourceUtility.CanTick(ref nextTick, Cooldown, delta))
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 120);
		}

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
