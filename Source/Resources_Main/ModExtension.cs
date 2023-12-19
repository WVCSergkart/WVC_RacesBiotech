using RimWorld;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	public class GeneExtension_Background : DefModExtension
	{
		public string backgroundPathEndogenes;
		public string backgroundPathXenogenes;
		public string backgroundPathEndoArchite;
		public string backgroundPathXenoArchite;
	}

	public class GeneExtension_Spawner : DefModExtension
	{
		public ThingDef thingDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;
		public int stackCount = 1;
		public IntRange spawnIntervalRange = new(120000, 300000);
		public QuestScriptDef summonQuest;
		// public QuestScriptDef resurrectionQuest;
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
		// public bool perfectImmunity = false;
		// public bool diseaseFree = false;
		public List<GeneDef> inheritableGeneDefs;
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
		// Gestator
		// public float matchPercent = 0.4f;
		// public float matchPercent = 0.4f;
		// Special Food
		public List<ThingDef> specialFoodDefs;
	}

	public class GeneExtension_XenotypeGestator : DefModExtension
	{
		public HediffDef gestationHediffDef;
		public HediffDef cooldownHediffDef;
		public float matchPercent = 0.4f;
		public int minimumDays = 3;
		public int cooldownDays = 15;
	}

	public class XenotypeExtension_SubXenotype : DefModExtension
	{
		public float shapeshiftChance = 0.1f;
		public bool xenotypeCanShapeshiftOnDeath = false;
		public List<XenotypeDef> xenotypeDefs;
	}

	public class XenotypeExtension_XenotypeShapeShift : DefModExtension
	{
		public SubXenotypeDef subXenotypeDef = null;
	}

	public class ThingExtension_Golems : DefModExtension
	{
		public bool removeButcherRecipes = false;
		public bool removeRepairComp = false;
		public bool removeDormantComp = false;
	}

	// public class FoodExtension_GeneFood : DefModExtension
	// {
		// public bool requireAnyGene = false;
		// public List<GeneDef> geneDefs;
		// public List<ThingDef> foodDefs;
	// }

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
		public List<HediffDef> hediffsThatPreventUndeadResurrection;
		// public List<ThingDef> listedGolems;
		// public List<GeneDef> gene_IsNotAcceptablePrey;
		// public List<GeneDef> gene_IsAngelBeauty;
		// public List<GeneDef> gene_PawnSkillsNotDecay;
		public List<string> mechDefNameShouldNotContain;
		// public List<GeneDef> xenoTree_PollutionReq_GeneDefs;
		public List<ThingDef> plantsToNotOverwrite_SpawnSubplant;
	}
}
