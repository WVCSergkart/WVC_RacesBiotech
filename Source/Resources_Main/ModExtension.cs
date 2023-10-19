using RimWorld;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

    // public class GeneExtension_Background : DefModExtension
    // {
        // public string backgroundPathEndogenes;
        // public string backgroundPathXenogenes;
        // public string backgroundPathArchite;
        // public bool replaceBackground = false;
    // }

	public class GeneExtension_Spawner : DefModExtension
	{
		public ThingDef thingDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;
		public int stackCount = 1;
		public IntRange spawnIntervalRange = new(120000, 300000);
		public QuestScriptDef summonQuest;
		// public bool writeTimeLeftToSpawn = true;
		// public string customLabel = "Resource";
	}

    public class GeneExtension_General : DefModExtension
    {
        // public bool geneIsMechaskin = false;
        // public bool geneIsPowerSource = false;
        // public bool geneIsSubcore = false;
        // public bool eyesShouldBeInvisble = false;
        public bool canBePredatorPrey = true;
        public bool noSkillDecay = false;
        public bool shouldSendNotificationAbout = true;
        public bool geneIsAngelBeauty = false;
        // public bool geneIsIncestous = false;
    }

    public class GeneExtension_Giver : DefModExtension
    {
        public HediffDef hediffDefName;
        public List<HediffDef> hediffDefs;
        // public BodyPartDef bodypart;
        public List<BodyPartDef> bodyparts;
        public BackstoryDef childBackstoryDef;
        public BackstoryDef adultBackstoryDef;
        public List<GeneDef> randomizerGenesList;
        // public string exclusionTag;
        // public GeneDef randomizerGene;
        // public bool geneIsRandomized = false;
        // public IntRange intRange = new IntRange(1, 1);
        public XenotypeDef xenotypeForcerDef = null;
        public bool xenotypeIsInheritable = true;
        public List<HeadTypeDef> headTypeDefs;
		// Scarifier
		public int scarsCount = 3;
		public List<GeneDef> scarGeneDefs;
		// Rand Hediff
		// public HediffStatRandDef hediffStatRandDef;
    }

    public class XenotypeExtension_SubXenotype : DefModExtension
    {
        public SubXenotypeDef mainSubXenotypeDef = null;
        public List<SubXenotypeDef> subXenotypeDefs;
        public float subXenotypeChance = 0.1f;
        public bool xenotypeCanShapeshiftOnDeath = false;
    }

    // public class BlackListedXenotypesDef : Def
    public class XenotypesAndGenesListDef : Def
    {
        public List<string> blackListedXenotypesForSerums;
        // public List<XenotypeDef> blackListedXenotypesForSingleSerums;
        // public List<XenotypeDef> blackListedXenotypesForHybridSerums;
        public List<XenotypeDef> whiteListedXenotypesForResurrectorSerums;
        public List<string> whiteListedXenotypesForFilter;
        public List<ThingDef> blackListedDefsForSerums;
        public List<GeneDef> perfectCandidatesForSerums;
        public List<GeneDef> nonCandidatesForSerums;
        public List<GeneDef> whiteListedExoskinGenes;
        public List<BackstoryDef> blackListedBackstoryForChanger;
        public List<HediffDef> blackListedHediffDefForReimplanter;
        // public List<ThingDef> listedGolems;
        // public List<GeneDef> gene_IsNotAcceptablePrey;
        // public List<GeneDef> gene_IsAngelBeauty;
        // public List<GeneDef> gene_PawnSkillsNotDecay;
        public List<string> mechDefNameShouldNotContain;
    }
}
