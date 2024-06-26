using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	public class XaG_CountWithChance
	{
		// GenePacks
		public int genesCount = 0;
		public int architeCount = 0;
		public float chance = 1.0f;
		// Tratis
		public TraitDef traitDef;
		public float weight = 1f;
	}

	// Exten

	public class GeneExtension_Background : DefModExtension
	{
		public string backgroundPathEndogenes;
		public string backgroundPathXenogenes;
		public string backgroundPathEndoArchite;
		public string backgroundPathXenoArchite;
	}

	public class GeneExtension_Spawner : DefModExtension
	{
		// Thing Spawner
		public ThingDef thingDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;
		public int stackCount = 1;
		public IntRange spawnIntervalRange = new(120000, 300000);
		public string spawnMessage = "MessageCompSpawnerSpawnedItem";
		public bool showMessageIfOwned = true;
		// WIP
		public bool customizable = false;
		public float stackPercent = 0.1f;
		public StuffCategoryDef stuffCategoryDef;
		// Pawn Spawner
		public QuestScriptDef summonQuest;
		public IntRange summonRange = new(1, 5);
		public HediffDef gestationHediffDef;
		public HediffDef cooldownHediffDef;
		public float matchPercent = 0.4f;
		public float gestationPeriodFactor = 0.5f;
		public float xenotypeComplexityFactor = 0.1f;
		public int cooldownDays = 15;
		public List<GeneDef> canGestateAnyIfHas;
		public bool useMatchPercentFromSettings = false;
	}

	public class GeneExtension_Undead : DefModExtension
	{
		// Undead
		public List<HediffDef> hediffDefs;
		public BackstoryDef childhoodDef;
		public BackstoryDef adulthoodDef;
		public bool ignoreHediffs = false;
		public IntRange additionalDelay = new(6000, 9000);
		// Reincarnation
		public QuestScriptDef summonQuest;
		public int minChronoAge = 54;
		// Shapeshifter
		public SoundDef soundDefOnImplant;
		public List<HediffDef> duplicateHediffs;
		public List<TraitDef> duplicateTraits;
		public List<HediffDef> blockingHediffs;
		public List<TraitDef> blockingTraits;
		public List<string> trustedXenotypes;
		public List<XaG_CountWithChance> possibleTraits;
	}

	public class GeneExtension_Opinion : DefModExtension
	{
		public ThoughtDef AboutMeThoughtDef;
		public ThoughtDef MeAboutThoughtDef;
		public bool targetShouldBePsySensitive = false;
		public bool targetShouldBeFamily = false;
		public bool ignoreIfHasGene = false;
		public bool onlySameXenotype = false;
		public ThoughtDef sameAsMe_AboutMeThoughtDef;
	}

	public class GeneExtension_General : DefModExtension
	{
		public PawnKindDef pawnKindDef;
		public float recruitChance = 0.5f;
		public bool canBePredatorPrey = true;
		public List<GeneDef> inheritableGeneDefs;
		// Undead Resurrection Component
		public bool shouldResurrect = false;
		// 4-6 hours
		public IntRange resurrectionDelay = new(6000, 9000);
		public string uniqueTag = "XaG_Undead";
		// Golems Component
		public bool removeButcherRecipes = false;
		public bool removeRepairComp = false;
		public bool removeDormantComp = false;
		// Job Components
		public int ticksToAbsorb = 180;
		public ThingDef warmupMote;
		public SoundDef warmupStartSound;
		public EffecterDef warmupEffecter;
		public bool reimplantEndogenes = true;
		public bool reimplantXenogenes = true;
		// Genepack Components
		public List<XaG_CountWithChance> genesCountProbabilities;
		public RulePackDef genepackNamer;
	}

	public class GeneExtension_Graphic : DefModExtension
	{
		// public FurDef furDef;
		public bool furIsSkinWithHair = false;
		public bool furIsSkin = false;
		public bool furCanRot = true;
	}

	public class GeneExtension_Giver : DefModExtension
	{
		public HediffDef hediffDefName;
		public List<HediffDef> hediffDefs;
		// public BodyPartDef bodypart;
		public List<BodyPartDef> bodyparts;
		public BackstoryDef childBackstoryDef;
		public BackstoryDef adultBackstoryDef;
		// public List<GeneDef> randomizerGenesList;
		// public string exclusionTag;
		// public GeneDef randomizerGene;
		// public bool geneIsRandomized = false;
		// public IntRange intRange = new IntRange(1, 1);
		public XenotypeDef xenotypeForcerDef = null;
		public bool xenotypeIsInheritable = true;
		public List<HeadTypeDef> headTypeDefs;
		// Scarifier
		public int scarsCount = 0;
		// public List<GeneDef> scarGeneDefs;
		// Rand Hediff
		// public HediffStatRandDef hediffStatRandDef;
		// Gestator
		// public float matchPercent = 0.4f;
		// public float matchPercent = 0.4f;
		// Special Food
		public List<ThingDef> specialFoodDefs;
		// public SoundDef soundDefOnImplant;
		// public ThoughtDef geneOpinion_thoughtDef;
		public Gender gender = Gender.None;
		public IntRange intervalRange = new(120000, 300000);
		public bool showMessageIfOwned = false;
		public string message = "WVC_XaG_Extreme_GeneticInstabilityMessage";
		public JobDef jobDef;
		public bool defaultBoolValue = false;
		// public ThingDef jobTarget;
		public SimpleCurve curve;
		// public List<HediffDef> hediffsThatPreventUndeadResurrection;
	}

	// public class GeneExtension_Shapeshifter : DefModExtension
	// {
		// public SoundDef soundDefOnImplant;
		// public List<HediffDef> duplicateHediffs;
		// public List<TraitDef> duplicateTraits;
		// public List<HediffDef> blockingHediffs;
		// public List<TraitDef> blockingTraits;
		// public List<string> trustedXenotypes;
		// public List<TraitDefWithWeight> possibleTraits;

		// public class TraitDefWithWeight
		// {
			// public TraitDef traitDef;
			// public float weight = 1f;
		// }
	// }

	// public class GeneExtension_XenotypeGestator : DefModExtension
	// {
		// public HediffDef gestationHediffDef;
		// public HediffDef cooldownHediffDef;
		// public float matchPercent = 0.4f;
		// public float gestationPeriodFactor = 0.5f;
		// public float xenotypeComplexityFactor = 0.1f;
		// public int cooldownDays = 15;
		// public List<GeneDef> canGestateAnyIfHas;
	// }

	// public class XenotypeExtension_SubXenotype : DefModExtension
	// {
		// public float shapeshiftChance = 0.1f;
		// public bool xenotypeCanShapeshiftOnDeath = false;
		// public List<XenotypeDef> xenotypeDefs;
	// }

	// public class PreceptExtension_General : DefModExtension
	// {
		// public bool blesslinkCannotSummonMechanoids = false;
	// }

	// public class XenotypeExtension_XenotypeShapeShift : DefModExtension
	// {
		// public SubXenotypeDef subXenotypeDef = null;
	// }

	// public class JobExtension_Reimplanter : DefModExtension
	// {
		// public int ticksToAbsorb = 180;
		// public ThingDef warmupMote;
		// public SoundDef warmupStartSound;
		// public EffecterDef warmupEffecter;
		// public bool reimplantEndogenes = true;
		// public bool reimplantXenogenes = true;
	// }

	// public class ThingExtension_Golems : DefModExtension
	// {
		// public bool removeButcherRecipes = false;
		// public bool removeRepairComp = false;
		// public bool removeDormantComp = false;
	// }

	// public class ThingExtension_Undead : DefModExtension
	// {
		// public bool shouldResurrect = false;
		// public IntRange resurrectionDelay = new(6000, 9000);
		// public string uniqueTag = "XaG_Undead";
	// }

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
		// public List<GeneDef> whiteListedExoskinGenes;
		public List<BackstoryDef> blackListedBackstoryForChanger;
		// public List<ThingDef> listedGolems;
		// public List<GeneDef> gene_IsNotAcceptablePrey;
		// public List<GeneDef> gene_IsAngelBeauty;
		// public List<GeneDef> gene_PawnSkillsNotDecay;
		public List<string> mechDefNameShouldNotContain;
		// public List<GeneDef> xenoTree_PollutionReq_GeneDefs;
		public List<ThingDef> plantsToNotOverwrite_SpawnSubplant;
		public List<Type> shapeShift_IgnoredGeneClasses;
		// public List<TraitDef> shapeShift_ProhibitedTraits;
		// public List<PreceptDef> shapeShift_ProhibitedPrecepts;
		// public List<HediffDef> hediffsRemovedByGenesRestorationSerum;

		[Obsolete]
		public List<HediffDef> hediffsThatPreventUndeadResurrection;
		[Obsolete]
		public List<HediffDef> blackListedHediffDefForReimplanter;
		[Obsolete]
		public List<ThingDef> blackListedDefsForSerums;
		[Obsolete]
		public List<GeneDef> perfectCandidatesForSerums;
		[Obsolete]
		public List<GeneDef> nonCandidatesForSerums;
	}
}
