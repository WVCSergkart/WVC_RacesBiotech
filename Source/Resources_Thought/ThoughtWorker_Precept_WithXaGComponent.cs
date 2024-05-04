using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public abstract class ThoughtWorker_PreceptWithXaGComponent : ThoughtWorker_Precept
	{

		[Unsaved(false)]
		private XaG_GameComponent cachedGameComponent;

		public XaG_GameComponent GameComponent
		{
			get
			{
				if (cachedGameComponent == null || Current.Game != cachedGameComponent.currentGame)
				{
					cachedGameComponent = Current.Game.GetComponent<XaG_GameComponent>();
				}
				return cachedGameComponent;
			}
		}

		// public XaG_GameComponent GameComponent => Current.Game.GetComponent<XaG_GameComponent>();

	}

	public class ThoughtWorker_Precept_HasAnyXenotypesAndCount : ThoughtWorker_PreceptWithXaGComponent
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (GameComponent.cachedXenotypesCount > 0)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return GameComponent.cachedXenotypesCount;
		}

	}

	public class ThoughtWorker_Precept_HasAnyNonHumanlikeAndCount : ThoughtWorker_PreceptWithXaGComponent
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (GameComponent.cachedNonHumansCount > 0)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return GameComponent.cachedNonHumansCount;
		}

	}

	public class ThoughtWorker_Precept_MoreWhenFiveColonistInFaction : ThoughtWorker_PreceptWithXaGComponent
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (GameComponent.cachedPawnsCount > 5)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			if (GameComponent.cachedPawnsCount > 5)
			{
				return GameComponent.cachedPawnsCount - 5;
			}
			return 0f;
		}

	}

}
