// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Noise;
using static UnityEngine.Scripting.GarbageCollector;

namespace WVC_XenotypesAndGenes
{
	public class ScenPart_PawnModifier_LongRunningJoke : ScenPart_PawnModifier_DynamicXenotypes
	{

		private int nextTick = 4000;

		private Pawn startingPawn;

		private float? cachedDynamicChance;
		public override float DynamicChance
		{
			get
			{
				if (cachedDynamicChance == null)
				{
					if (MiscUtility.GameStarted())
					{
						cachedDynamicChance = Mathf.Clamp(0.01f * (Find.TickManager.TicksGame / 1000000), 0.01f, 0.12f);
					}
					else
					{
						cachedDynamicChance = base.DynamicChance;
					}
				}
				return cachedDynamicChance.Value;
			}
		}

		public override void PostGameStart()
		{
			startingPawn = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists?.First();
			SetGeneral();
		}

		public override void Tick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 59999, 1))
			{
				return;
			}
			try
			{
				cachedDynamicChance = null;
				TryDoLeave();
			}
			catch (Exception arg)
			{
				Log.Warning("Failed trigger colony leave event. Reason: " + arg.Message);
			}
		}

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			if (startingPawn?.genes != null && Rand.Chance(0.03f))
			{
				SetXenotype(pawn, startingPawn.genes.Xenotype);
			}
			base.ModifyPawnPostGenerate(pawn, redressed);
		}

		private void TryDoLeave()
		{
			if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount <= 5)
			{
				return;
			}
			//List<Pawn> colonists = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists;
			foreach (Pawn colonist in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
			{
				if (Rand.Chance(0.75f))
				{
					continue;
				}
				if (colonist == startingPawn)
				{
					continue;
				}
				if (CanLeaveColony(colonist))
				{
					DoLeave(colonist);
					return;
				}
			}
		}

		public static void DoLeave(Pawn colonist)
		{
			if (PawnUtility.ShouldSendNotificationAbout(colonist))
			{
				Find.LetterStack.ReceiveLetter("WVC_Label_LongRunningJoke_DoLeave".Translate(), "WVC_Desc_LongRunningJoke_DoLeave".Translate(colonist), LetterDefOf.NegativeEvent, new LookTargets(colonist));
			}
			colonist.GetLord()?.Notify_PawnLost(colonist, PawnLostCondition.Undefined);
			if (colonist.Faction != null && colonist.Faction.IsPlayer)
			{
				colonist.SetFaction(null);
			}
			LordMaker.MakeNewLord(colonist.Faction, new LordJob_ExitMapBest(LocomotionUrgency.Walk), colonist.Map).AddPawn(colonist);
		}

		public static bool CanLeaveColony(Pawn colonist)
		{
			if (colonist.Map == null || colonist.InSpace())
			{
				return false;
			}
			if (!colonist.Spawned || !colonist.Map.CanEverExit)
			{
				return false;
			}
			if (!colonist.IsHuman() || colonist.IsMutant || colonist.IsCreepJoiner || colonist.IsColonyChild())
			{
				return false;
			}
			if (colonist.Downed || colonist.IsQuestLodger() || colonist.Drafted)
			{
				return false;
			}
			if (!colonist.ageTracker.Adult)
			{
				return false;
			}
			if (!RCellFinder.TryFindBestExitSpot(colonist, out _, TraverseMode.ByPawn, false))
			{
				return false;
			}
			return colonist.Map.Tile.LayerDef == PlanetLayerDefOf.Surface;
		}

		private void SetGeneral()
		{
			HivemindUtility.nonPlayerHivemindSize = 15;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref startingPawn, "mainHero", saveDestroyedThings: true);
			Scribe_Values.Look(ref nextTick, "nextTick", defaultValue: 60000);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGeneral();
			}
		}

	}

}
