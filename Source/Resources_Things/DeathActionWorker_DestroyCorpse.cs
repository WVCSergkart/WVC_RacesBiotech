using Verse;
using Verse.AI.Group;

namespace WVC_XenotypesAndGenes
{

	public class DeathActionProperties_DestroyCorpse : DeathActionProperties
	{

		public bool deSpawn = false;

		public DeathActionProperties_DestroyCorpse()
		{
			workerClass = typeof(DeathActionWorker_DestroyCorpse);
		}

	}

	public class DeathActionWorker_DestroyCorpse : DeathActionWorker
	{

		public DeathActionProperties_DestroyCorpse Props => (DeathActionProperties_DestroyCorpse)props;

		public override void PawnDied(Corpse corpse, Lord prevLord)
		{
			if (corpse != null)
			{
				if (Props.deSpawn)
				{
					corpse.Destroy(DestroyMode.Vanish);
				}
				else
				{
					corpse.DeSpawn(DestroyMode.Vanish);
				}
			}
		}

	}

	// [Obsolete]
	// public class DeathActionWorker_SpawnThing : DeathActionWorker
	// {
	// public override void PawnDied(Corpse corpse)
	// {
	// FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, ThingDefOf.Filth_CorpseBile, 5);
	// Thing thing = ThingMaker.MakeThing(corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn);
	// GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near, null, null, default);
	// corpse.DeSpawn();
	// }
	// }

	// [Obsolete]
	// public class DeathActionWorker_SpawnRandomThingFromList : DeathActionWorker
	// {
	// public override void PawnDied(Corpse corpse)
	// {
	// FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn, 5);
	// Thing thing = ThingMaker.MakeThing(corpse.InnerPawn.def.GetModExtension<GeneExtension_Spawner>().thingDefsToSpawn.RandomElement());
	// GenPlace.TryPlaceThing(thing, corpse.Position, corpse.Map, ThingPlaceMode.Near, null, null, default);
	// corpse.DeSpawn();
	// }
	// }

}
