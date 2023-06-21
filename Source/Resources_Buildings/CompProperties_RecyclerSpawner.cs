using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_RecyclerSpawner : CompProperties
	{
		public IntRange ticksUntilSpawn = new(5000,5000);

		public ThingDef productDef;

		public int productCount = 1;

		public bool writeTimeLeftToSpawn = true;

		public CompProperties_RecyclerSpawner()
		{
			compClass = typeof(CompRecyclerSpawner);
		}
	}

}
