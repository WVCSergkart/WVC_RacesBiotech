using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// public abstract class ThoughtWorker_PreceptWithXaGComponent : ThoughtWorker_Precept
	// {

		// [Unsaved(false)]
		// private XaG_GameComponent cachedGameComponent;

		// public XaG_GameComponent GameComponent
		// {
			// get
			// {
				// if (cachedGameComponent == null || Current.Game != cachedGameComponent.currentGame)
				// {
					// cachedGameComponent = Current.Game.GetComponent<XaG_GameComponent>();
				// }
				// return cachedGameComponent;
			// }
		// }

	// }

	public class ThoughtWorker_Precept_HasAnyXenotypesAndCount : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			// if (p.Faction != Faction.OfPlayer)
			// {
				// return false;
			// }
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

	public class ThoughtWorker_Precept_HasAnyNonHumanlikeAndCount : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			// if (p.Faction != Faction.OfPlayer)
			// {
				// return false;
			// }
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

	public class ThoughtWorker_Precept_MoreThanFiveColonistsInFaction : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
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

	public class ThoughtWorker_Precept_MoreThanOneColonistsInFaction : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
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
