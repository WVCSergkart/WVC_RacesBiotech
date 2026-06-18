using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class StaticCollectionsClass
	{

		private static XaG_GameComponent cachedGameComponent;
		public static XaG_GameComponent GameComponent
		{
			get
			{
				if (cachedGameComponent == null)
				{
					cachedGameComponent = Current.Game?.GetComponent<XaG_GameComponent>();
				}
				return cachedGameComponent;
			}
		}

		public static int cachedDuplicatesCount = 0;
		public static int cachedPlayerPawnsCount = 0;
		public static int cachedDeathrestingColonistsCount = 0;
		public static int cachedNonDeathrestingColonistsCount = 0;
		public static int cachedColonistsDuplicatesDeathrestingCount = 0;
		public static int cachedNonHumansCount = 0;
		public static int cachedColonyMechsCount = 0;
		public static bool haveAssignedWork = false;
		public static int cachedDownedColonists = 0;

		public static bool oneManArmyMode = false;

		public static bool HideMechanitor(Pawn pawn)
		{
			return MechanoidsUtility.NonMechanitorGUIPawns.Contains(pawn);
		}

		public static void ResetCollection()
		{
			cachedDuplicatesCount = 0;
			cachedPlayerPawnsCount = 0;
			cachedNonDeathrestingColonistsCount = 0;
			cachedColonistsDuplicatesDeathrestingCount = 0;
			cachedDeathrestingColonistsCount = 0;
			ThoughtWorker_Precept_HasAnyXenotypesAndCount.cachedXenotypesCount = 0;
			cachedNonHumansCount = 0;
			cachedColonyMechsCount = 0;
			cachedDownedColonists = 0;
			haveAssignedWork = false;
			oneManArmyMode = false;
			MechanoidsUtility.ResetCollection();
			Gene_Wings.ResetCollection();
			Gene_PredatorRepellent.ResetCollection();
			ThoughtWorker_Precept_Social_Duplicates.duplicatePawns = new();
			ThoughtWorker_Precept_Social_Duplicates.ignoredPawns = new();
			ThoughtWorker_Precept_Social_Duplicates.duplicateSets = new();
			GeneshiftUtility.ResetXenotypesCollection();
			HealingUtility.UpdRegenCollection();
			GeneResourceUtility.UpdUndeads();
			HivemindUtility.ResetCollection();
			ResetStaticCache_PerSave();
		}

		public static void ResetStaticCache_PerSave()
		{
			ThoughtWorker_Precept_HasAnyXenotypesAndCount.lastRecacheTick = -1;
			ThoughtWorker_Precept_WithCollectionCheck.lastRecacheTick = -1;
			FamilyUtility.lastRecacheTick = -1;
			GeneshiftUtility.lastRecacheTick = -1;
			Gene_Switcher.cachedHolders = null;
			Gene_PackMentality.ResetCache();
			Gene_Recluse.lastRecacheTick = -1;
			CompStylingStation.cachedPawns = null;
			cachedGameComponent = null;
		}

		//[Obsolete]
		//public static int CountAllPlayerXenos()
		//{
		//	int mult = 0;
		//	foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
		//	{
		//		if (!item.IsHuman())
		//		{
		//			continue;
		//		}
		//		if (XaG_GeneUtility.PawnIsBaseliner(item))
		//		{
		//			continue;
		//		}
		//		mult++;
		//	}
		//	return mult;
		//}

		//[Obsolete]
		//public static void CountAllPlayerControlledPawns_StaticCollection()
		//{
		//	UpdateStaticCollection();
		//}

		//public static void UpdateStaticCollection(bool updAllCollections = false)
		//{
		//	if (updAllCollections)
		//	{
		//		ThoughtWorker_Precept_Shapeshifter.UpdCollection();
		//	}
		//	UpdateStaticCollection();
		//}

		public static void UpdateStaticCollection()
		{
			int playerDuplicates = 0;
			int allPawns = 0;
			int activeColonists = 0;
			int activeColonistsDuplicatesDeathresting = 0;
			int deathresters = 0;
			//int xenos = 0;
			int nonHumans = 0;
			int colonyMechs = 0;
			int downedColonists = 0;
			//int mutants = 0;
			bool anyAssignedWork = false;
			//bool leaderIsShapeshifterOrSimilar = false;
			//bool presentShapeshifter = false;
			//bool leaderIsUndead = false;
			//bool presentUndead = false;
			List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
			foreach (Pawn item in pawns)
			{
				if (item.IsQuestLodger())
				{
					continue;
				}
				if (item.IsPrisoner)
				{
					continue;
				}
				allPawns++;
				if (!item.RaceProps.Humanlike)
				{
					if (!item.IsMutant)
					{
						nonHumans++;
					}
					else
					{
						SubHumansCount(item);
					}
					if (item.RaceProps.IsMechanoid)
					{
						colonyMechs++;
					}
					continue;
				}
				if (item.IsMutant)
				{
					if (item.IsGhoul)
					{
						nonHumans++;
					}
					if (item.mutant.Def.thinkTree == null && item.mutant.Def.thinkTreeConstant == null)
					{
						Colonists(item);
					}
					//mutants++;
					//SubHumansCount(ref colonists, ref nonHumans, item);
					continue;
				}
				Colonists(item);
				if (anyAssignedWork)
				{
					continue;
				}
				for (int i = 0; i < 24; i++)
				{
					if (item.timetable.GetAssignment(i) == TimeAssignmentDefOf.Work)
					{
						anyAssignedWork = true;
						break;
					}
				}
			}
			StaticCollectionsClass.cachedDuplicatesCount = playerDuplicates;
			StaticCollectionsClass.cachedPlayerPawnsCount = allPawns;
			StaticCollectionsClass.cachedNonDeathrestingColonistsCount = activeColonists;
			StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount = activeColonistsDuplicatesDeathresting;
			StaticCollectionsClass.cachedDeathrestingColonistsCount = deathresters;
			//StaticCollectionsClass.cachedXenotypesCount = xenos;
			StaticCollectionsClass.cachedNonHumansCount = nonHumans;
			StaticCollectionsClass.haveAssignedWork = anyAssignedWork;
			StaticCollectionsClass.cachedColonyMechsCount = colonyMechs;
			StaticCollectionsClass.cachedDownedColonists = downedColonists;
			StaticCollectionsClass.oneManArmyMode = activeColonistsDuplicatesDeathresting <= 1;
			Gene_Sacrificer.ResetCache();

			void SubHumansCount(Pawn item)
			{
				if (!item.IsSubhuman)
				{
					nonHumans++;
				}
				else
				{
					activeColonists++;
				}
			}
			void Colonists(Pawn item)
			{
				//if (!XaG_GeneUtility.PawnIsBaseliner(item) && item.IsHuman())
				//{
				//	xenos++;
				//}
				if (item.IsDuplicate)
				{
					playerDuplicates++;
				}
				if (!item.Deathresting)
				{
					if (item.Downed)
					{
						downedColonists++;
					}
					activeColonists++;
				}
				else
				{
					deathresters++;
				}
				activeColonistsDuplicatesDeathresting++;
			}
		}

	}

}
