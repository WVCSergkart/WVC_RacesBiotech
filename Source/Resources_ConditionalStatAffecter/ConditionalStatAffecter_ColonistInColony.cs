// RimWorld.StatPart_Age
using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ConditionalStatAffecter_ColonistInColony : ConditionalStatAffecter
	{

		// public override string Label => "WVC_XaG_StatsReport_NoSunlight".Translate();

		public override string Label
		{
			get
			{
				return "WVC_XaG_StatsReport_ColonistsInColony".Translate(colonistsLimit + 1);
			}
		}

		public int colonistsLimit = 5;

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

		public override bool Applies(StatRequest req)
		{
			if (req.HasThing && req.Thing.Spawned)
			{
				return GameComponent.cachedPawnsCount <= colonistsLimit;
			}
			return false;
		}

	}

}
