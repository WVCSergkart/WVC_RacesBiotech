using RimWorld;
using Verse;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class RecipeWorkerCounter_SerumRetune : RecipeWorkerCounter
	{

		public new bool CountValidThing(Thing thing, Bill_Production bill, ThingDef def)
		{
			ThingDef def2 = thing.def;
			if (def2 != def)
			{
				return false;
			}
			if (!bill.includeTainted && def2.IsApparel && ((Apparel)thing).WornByCorpse)
			{
				return false;
			}
			if (thing.def.useHitPoints && !bill.hpRange.IncludesEpsilon((float)thing.HitPoints / (float)thing.MaxHitPoints))
			{
				return false;
			}
			CompQuality compQuality = thing.TryGetComp<CompQuality>();
			if (compQuality != null && !bill.qualityRange.Includes(compQuality.Quality))
			{
				return false;
			}
			if (bill.limitToAllowedStuff && !bill.ingredientFilter.Allows(thing.Stuff))
			{
				return false;
			}
			if (thing.SpawnedOrAnyParentSpawned && thing.PositionHeld.Fogged(thing.MapHeld))
			{
				return false;
			}
			return true;
		}

	}

}
