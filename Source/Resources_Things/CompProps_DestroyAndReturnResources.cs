using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_OutdatedThing : CompProperties
	{

		public ThingDef newDef;

		public int count = 1;

		public CompProperties_OutdatedThing()
		{
			compClass = typeof(CompReplaceWithThing);
		}
	}

	public class CompReplaceWithThing : ThingComp
	{

		public CompProperties_OutdatedThing Props => (CompProperties_OutdatedThing)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			for (int i = 0; i < parent.stackCount; i++)
			{
				Thing thing = ThingMaker.MakeThing(Props.newDef);
				thing.stackCount = Props.count;
				GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near, null, null, default);
			}
			parent.Destroy();
		}

	}

	public class CompUseEffect_SimpleDestroySelf : CompUseEffect
	{

		public override void DoEffect(Pawn usedBy)
		{
			if (!parent.Destroyed)
			{
				parent?.SplitOff(1)?.Destroy();
			}
		}

	}

	public class CompDestroyAndRefund : ThingComp
	{

		public CompProperties_OutdatedThing Props => (CompProperties_OutdatedThing)props;

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

	public class CompDestroyAndRefundStack : ThingComp
	{

		public CompProperties_OutdatedThing Props => (CompProperties_OutdatedThing)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!parent.def.costList.NullOrEmpty())
			{
				for (int i = 0; i < parent.stackCount; i++)
				{
					foreach (ThingDefCountClass thingDefCountClass in parent.def.costList)
					{
						Thing thing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
						thing.stackCount = thingDefCountClass.count;
						GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near, null, null, default);
					}
				}
			}
			parent.Destroy();
		}

	}

}
