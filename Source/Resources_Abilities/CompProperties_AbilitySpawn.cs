using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilitySpawn : CompProperties_AbilityEffect
	{
		public ThingDef thingDef;

		public bool allowOnBuildings = false;

		public bool allowOnNonSoil = true;

		public CompProperties_AbilitySpawn()
		{
			compClass = typeof(CompAbilityEffect_Spawn);
		}
	}

}
