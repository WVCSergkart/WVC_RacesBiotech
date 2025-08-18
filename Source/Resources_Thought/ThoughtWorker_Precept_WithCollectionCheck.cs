using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public abstract class ThoughtWorker_Precept_WithCollectionCheck : ThoughtWorker_Precept
	{

		public static int lastRecacheTick = -1;

		public void UpdCollection()
		{
            if (lastRecacheTick < Find.TickManager.TicksGame)
            {
				//Log.Error("ShouldHaveThought Tick");
                MiscUtility.UpdateStaticCollection();
                lastRecacheTick = Find.TickManager.TicksGame + 6000;
            }
		}

	}

    public class ThoughtWorker_Precept_HasAnyXenotypesAndCount : ThoughtWorker_Precept_WithCollectionCheck
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (StaticCollectionsClass.cachedXenotypesCount > 0)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return StaticCollectionsClass.cachedXenotypesCount;
		}

	}

	public class ThoughtWorker_Precept_HasAnyNonHumanlikeAndCount : ThoughtWorker_Precept_WithCollectionCheck
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (StaticCollectionsClass.cachedNonHumansCount > 0)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return StaticCollectionsClass.cachedNonHumansCount;
		}

	}

	public class ThoughtWorker_Precept_MoreThanFiveColonistsInFaction : ThoughtWorker_Precept_WithCollectionCheck
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 5)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 5)
			{
				return StaticCollectionsClass.cachedNonDeathrestingColonistsCount - 5;
			}
			return 0f;
		}

	}

	public class ThoughtWorker_Precept_MoreThanOneColonistsInFaction : ThoughtWorker_Precept_WithCollectionCheck
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1)
			{
				return StaticCollectionsClass.cachedNonDeathrestingColonistsCount - 1;
			}
			return 0f;
		}

	}

	public class ThoughtWorker_Precept_OneManArmy : ThoughtWorker_Precept_WithCollectionCheck
	{

		private static int PawnsCount => StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount;

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (PawnsCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (PawnsCount > 1)
			{
				return PawnsCount - 1;
			}
			return 0f;
		}

	}

	public class ThoughtWorker_Precept_Deathwatch : ThoughtWorker_Precept_WithCollectionCheck
	{

		private static int PawnsCount => StaticCollectionsClass.cachedNonDeathrestingColonistsCount - StaticCollectionsClass.cachedDownedColonists;

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
            if (PawnsCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
        {
            if (PawnsCount > 1)
            {
                return PawnsCount - 1;
            }
            return 0f;
        }
	}

	public class ThoughtWorker_Precept_Duplicates : ThoughtWorker_Precept_WithCollectionCheck
	{

		private static int PawnsCount => StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount - StaticCollectionsClass.cachedDuplicatesCount;

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (PawnsCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (PawnsCount > 1)
			{
				return PawnsCount - 1;
			}
			return 0f;
		}

	}

}
