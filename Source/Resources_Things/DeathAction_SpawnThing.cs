using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class DeathActionWorker_SpawnThing : DeathActionWorker
	{
		public override void PawnDied(Corpse corpse)
		{
			FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, ThingDefOf.Filth_CorpseBile, 5);
			Thing thing = ThingMaker.MakeThing(corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn);
			// thing.stackCount = corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;
			GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near, null, null, default);
			corpse.DeSpawn();
		}
	}

}
