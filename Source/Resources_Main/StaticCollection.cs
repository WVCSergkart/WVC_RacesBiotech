using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public static int cachedXenotypesCount = 0;
		public static int cachedNonHumansCount = 0;
		public static int cachedColonyMechsCount = 0;
		public static bool haveAssignedWork = false;

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

		public static List<Pawn> hideMechanitorButton = new();
		//public static List<Pawn> duplicates = new();

		public static void ResetCollection()
		{
			cachedDuplicatesCount = 0;
			cachedPlayerPawnsCount = 0;
			cachedNonDeathrestingColonistsCount = 0;
			cachedColonistsDuplicatesDeathrestingCount = 0;
			cachedDeathrestingColonistsCount = 0;
			cachedXenotypesCount = 0;
			cachedNonHumansCount = 0;
			cachedColonyMechsCount = 0;
			haveAssignedWork = false;
			oneManArmyMode = false;
			hideMechanitorButton = new();
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

	}

}
