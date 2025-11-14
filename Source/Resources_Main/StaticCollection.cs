using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class StaticCollectionsClass
	{

		public static int cachedDuplicatesCount = 0;
		public static int cachedPlayerPawnsCount = 0;
		public static int cachedDeathrestingColonistsCount = 0;
		public static int cachedNonDeathrestingColonistsCount = 0;
		public static int cachedColonistsDuplicatesDeathrestingCount = 0;
		//public static int cachedXenotypesCount = 0;
		public static int cachedNonHumansCount = 0;
		public static int cachedColonyMechsCount = 0;
		public static bool haveAssignedWork = false;
		public static int cachedDownedColonists = 0;

		//[Obsolete]
		//public static int cachedColonistsCount => cachedNonDeathrestingColonistsCount;
		//public static bool leaderIsUndead = false;
		//public static bool leaderIsShapeshifter = false;
		//public static bool presentUndead = false;
		//public static bool presentShapeshifter = false;

		//public static bool shapeshifterAppear = false;
		public static bool oneManArmyMode = false;

		//public static Pawn voidLinkNewGamePlusPawn;

		//public static List<XenotypeDef> knownXenotypeDefs = new() { XenotypeDefOf.Baseliner };

		//public static XaG_GameComponent currentGameComponent;

		private static List<Pawn> hideMechanitorButton = new();
		public static bool HideMechanitor(Pawn pawn)
		{
			return StaticCollectionsClass.hideMechanitorButton.Contains(pawn);
		}

		//public static List<DuplicateSet> duplicatesSets = new();

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
			hideMechanitorButton = new();
			//duplicates = new();
			ThoughtWorker_Precept_Social_Duplicates.duplicatePawns = new();
			ThoughtWorker_Precept_Social_Duplicates.ignoredPawns = new();
			ThoughtWorker_Precept_Social_Duplicates.duplicateSets = new();
			//ThoughtWorker_Precept_Family.families = new();
			//ThoughtWorker_Precept_WithCollectionCheck.UpdCollection();
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
			ThoughtWorker_Precept_Family.lastRecacheTick = -1;
			GeneshiftUtility.lastRecacheTick = -1;
			Gene_Switcher.cachedHolders = null;
		}

		public static void AddHideMechanitors(Pawn pawn)
		{
			if (!StaticCollectionsClass.hideMechanitorButton.Contains(pawn))
			{
				StaticCollectionsClass.hideMechanitorButton.Add(pawn);
			}
		}

		public static void AddOrRemoveHideMechanitors(Pawn pawn)
		{
			if (StaticCollectionsClass.hideMechanitorButton.Contains(pawn))
			{
				StaticCollectionsClass.hideMechanitorButton.Remove(pawn);
			}
			else
			{
				StaticCollectionsClass.hideMechanitorButton.Add(pawn);
			}
		}

		public static void RemoveHideMechanitors(Pawn pawn)
		{
			if (StaticCollectionsClass.hideMechanitorButton.Contains(pawn))
			{
				StaticCollectionsClass.hideMechanitorButton.Remove(pawn);
			}
		}

		//private static List<Pawn> mechanitors = new();

		//public static bool IsGeneticMechanitor(this Pawn pawn)
		//{
		//	return mechanitors.Contains(pawn);
		//}
		//public static void AddMechanitor(Pawn pawn)
		//{
		//	if (!mechanitors.Contains(pawn))
		//	{
		//		Gene gene = pawn.genes.GetFirstGeneOfType<Gene_Mechlink>();
		//		if (gene != null)
		//		{
		//			mechanitors.Add(pawn);
		//			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn);
		//		}
		//	}
		//}
		//public static void RemoveMechanitor(Pawn pawn)
		//{
		//	if (mechanitors.Contains(pawn))
		//	{
		//		Gene gene = pawn.genes.GetFirstGeneOfType<Gene_Mechlink>();
		//		if (gene == null)
		//		{
		//			mechanitors.Remove(pawn);
		//			PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn);
		//		}
		//	}
		//}

	}

}
