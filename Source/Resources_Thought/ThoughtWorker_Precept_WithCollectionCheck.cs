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
			if (StaticCollectionsClass.cachedColonistsCount > 5)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (StaticCollectionsClass.cachedColonistsCount > 5)
			{
				return StaticCollectionsClass.cachedColonistsCount - 5;
			}
			return 0f;
		}

	}

	public class ThoughtWorker_Precept_MoreThanOneColonistsInFaction : ThoughtWorker_Precept_WithCollectionCheck
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			UpdCollection();
			if (StaticCollectionsClass.cachedColonistsCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (StaticCollectionsClass.cachedColonistsCount > 1)
			{
				return StaticCollectionsClass.cachedColonistsCount - 1;
			}
			return 0f;
		}

	}

}
