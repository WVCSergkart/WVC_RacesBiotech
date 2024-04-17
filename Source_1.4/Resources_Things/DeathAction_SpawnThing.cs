using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class DeathActionWorker_SpawnThing : DeathActionWorker
	{
		public override void PawnDied(Corpse corpse)
		{
			FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, ThingDefOf.Filth_CorpseBile, 5);
			Thing thing = ThingMaker.MakeThing(corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn);
			GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near, null, null, default);
			corpse.DeSpawn();
		}
	}

	[Obsolete]
	public class DeathActionWorker_SpawnRandomThingFromList : DeathActionWorker
	{
		public override void PawnDied(Corpse corpse)
		{
			FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn, 5);
			Thing thing = ThingMaker.MakeThing(corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near, null, null, default);
			corpse.DeSpawn();
		}
	}

}
