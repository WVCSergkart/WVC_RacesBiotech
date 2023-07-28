using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_HiveSpawnAnimals : CompProperties
    {
        public IntRange ticksBetweenSpawn = new(60000, 120000);

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

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string item in base.ConfigErrors(parentDef))
            {
                yield return item;
            }
            if (maxNumberOfSpawns < 0)
            {
                yield return "Invalid maxNumberOfSpawns value. The valid range is zero or more.";
            }
            if (maxLivingThings < maxNumber)
            {
                yield return "Invalid maxLivingThings value. maxLivingThings must be greater than maxNumber.";
            }
            if (parentDef.tickerType == TickerType.Never)
            {
                yield return "has CompHiveSpawnAnimals, but its TickerType is set to Never";
            }
        }
    }

}
