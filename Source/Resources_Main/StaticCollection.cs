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

	}

}
