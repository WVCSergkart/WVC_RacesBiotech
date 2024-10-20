// RimWorld.StatPart_Age
using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ConditionalStatAffecter_NoSunlight : ConditionalStatAffecter
	{
		public override string Label => "WVC_XaG_StatsReport_NoSunlight".Translate();

		public override bool Applies(StatRequest req)
		{
			if (req.HasThing && req.Thing.Spawned)
			{
				return !req.Thing.Position.InSunlight(req.Thing.Map);
			}
			return false;
		}
	}

	public class ConditionalStatAffecter_InLight : ConditionalStatAffecter
	{
		public override string Label => "WVC_XaG_StatsReport_InLight".Translate();

		public override bool Applies(StatRequest req)
		{
			if (req.HasThing && req.Thing.Spawned)
			{
				return req.Thing.MapHeld.glowGrid.PsychGlowAt(req.Thing.PositionHeld) != PsychGlow.Dark;
			}
			return false;
		}
	}

}
