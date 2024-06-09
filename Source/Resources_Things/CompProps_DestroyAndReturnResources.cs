using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompDestroyAndRefund : ThingComp
	{

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!parent.def.costList.NullOrEmpty())
			{
				foreach (ThingDefCountClass thingDefCountClass in parent.def.costList)
				{
					Thing thing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
					thing.stackCount = thingDefCountClass.count;
					GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near, null, null, default);
				}
			}
			parent.Destroy();
		}

	}

}
