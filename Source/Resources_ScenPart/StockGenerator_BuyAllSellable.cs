// RimWorld.StatPart_Age
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class StockGenerator_BuyAllSellable : StockGenerator
	{

		public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
		{
			return Enumerable.Empty<Thing>();
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return true;
		}

		public override Tradeability TradeabilityFor(ThingDef thingDef)
		{
			if (thingDef.tradeability == Tradeability.None || !HandlesThingDef(thingDef))
			{
				return Tradeability.None;
			}
			return Tradeability.Sellable;
		}

	}

}
