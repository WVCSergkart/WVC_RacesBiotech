using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_HiveSpawnAnimals : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(60000,120000);

		public int maxNumber = 4;

		public int maxLivingThings = 20;

		public int maxNumberOfSpawns = 1;

		public bool ignoreFaction = true;

		public string uniqueTag = "PlayerHive";

		public string inspectString = "WVC_XaG_Label_CompHiveSpawnAnimals_WalkingCorpses";

		public List<PawnKindDef> pawnsList = new();

		public CompProperties_HiveSpawnAnimals()
		{
			compClass = typeof(CompHiveSpawnAnimals);
		}
	}

}
