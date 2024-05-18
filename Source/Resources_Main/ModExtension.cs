using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
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
		// StatPart
		public ThingDef thingDef;
		public float bandwidth = 1f;
		// public RulePackDef genepackNamer;
		// public ThingStyleDef styleDef;
		// public List<XaG_CountWithChance> genesCountProbabilities;
		// public int AllGenesCount
		// {
			// get
			// {
				// int count = 0;
				// count += genesCount;
				// count += architeCount;
				// return count;
			// }
		// }
		public bool cosmeticOnly = false;
		public bool prerequisitesOnly = false;
	}

	// Exten

	// public class GeneExtension_Background : DefModExtension
	// {
		// public string backgroundPathEndogenes;
		// public string backgroundPathXenogenes;
		// public string backgroundPathEndoArchite;
		// public string backgroundPathXenoArchite;
	// }

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
		//incidents
		public IncidentDef incidentDef;
		// public ThingDef specialTree;
		public int specialTreesMax = 3;
		public int specialTreesMin = 1;
		// public float skipChance = 0.5f;
		// Gauranlen
		// public List<GauranlenTreeModeDef> blacklistedGauranlenTreeModeDefs;
		public int connectedDryadsLimit = 6;
		public PawnKindDef defaultDryadPawnKindDef;
		public StatDef dryadsStatLimit;
		public HediffDef postGestationSickness;
		public ThoughtDef dryadDiedMemoryDef;
		//BloodyGrowths
		public float hemogenPerThing = 0.2f;
		public ThingStyleDef styleDef = null;
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
		public float chance = 0.37f;
		// Shapeshifter
		public SoundDef soundDefOnImplant;
		public List<HediffDef> duplicateHediffs;
		public List<TraitDef> duplicateTraits;
		public List<HediffDef> blockingHediffs;
		public List<TraitDef> blockingTraits;
		public List<string> trustedXenotypes;
		public List<XaG_CountWithChance> possibleTraits;
		// Special Food
		public List<ThingDef> specialFoodDefs;
		// GeneticThief
		public XenotypeDef xenotypeDef;
		public Color collectedGenesColor;
		public Color destroyedGenesColor;
		// Regen
		public float regeneration = -1f;
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
		// public PawnKindDef inheritFromPawnKind;
		// Undead Resurrection Component
		public bool shouldResurrect = false;
		// 4-6 hours
		public IntRange resurrectionDelay = new(6000, 9000);
		public string uniqueTag = "XaG_Undead";
		// Golems Component
		public bool removeButcherRecipes = false;
		public bool removeRepairComp = false;
		public bool removeDormantComp = false;
		public bool generateCorpse = true;
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
		public bool supportMutants = true;
		public List<MutantDef> supportedMutantDefs;
		// implanter
		public float reimplantChance = 0.02f;
		// weight flatter
		// public float selectionWeight = 1f;
		public bool isAptitude = false;
	}

	public class GeneExtension_Graphic : DefModExtension
	{
		public string backgroundPathEndogenes = "WVC/UI/Genes/GeneBackground_Endogene";
		public string backgroundPathXenogenes = "WVC/UI/Genes/GeneBackground_Xenogene";
		public string backgroundPathEndoArchite = "WVC/UI/Genes/GeneBackground_ArchiteGene";
		public string backgroundPathXenoArchite = "WVC/UI/Genes/GeneBackground_XenoArchiteGene";
		// BodySkin
		public bool furIsSkinWithHair = false;
		public bool furIsSkin = false;
		[Obsolete]
		public bool furCanRot = true;
		// BodySize
		public float bodyScaleFactor = 1f;
		public float headScaleFactor = 1f;
	}

	public class GeneExtension_Giver : DefModExtension
	{
		public HediffDef hediffDefName;
		public List<HediffDef> hediffDefs;
		public List<BodyPartDef> bodyparts;
		public BackstoryDef childBackstoryDef;
		public BackstoryDef adultBackstoryDef;
		public XenotypeDef xenotypeForcerDef = null;
		public bool xenotypeIsInheritable = true;
		public List<HeadTypeDef> headTypeDefs;
		// Scarifier
		public int scarsCount = 0;
		// Special Food
		public List<ThingDef> specialFoodDefs;
		public Gender gender = Gender.None;
		public IntRange intervalRange = new(120000, 300000);
		public bool showMessageIfOwned = false;
		public string message = "WVC_XaG_Extreme_GeneticInstabilityMessage";
		public JobDef jobDef;
		public bool defaultBoolValue = false;
		public SimpleCurve curve;
		// Golems Gizmo
		public Color filledBlockColor = ColorLibrary.Orange;
		public Color excessBlockColor = ColorLibrary.Red;
		public float gizmoOrder = -90f;
		public int recacheFrequency = 231;
		public string tipSectionTitle = "WVC_XaG_GolemBandwidth";
		public string tipSectionTip = "WVC_XaG_GolemBandwidthGizmoTip";
		public int golemistTypeIndex = -1;
		public QuestScriptDef summonQuest;
		// Bloodeater
		public float nutritionPerBite = 0.8f;
		// Photosynthesis
		public float passivelyReplenishedNutrition = 0.04f;
		// Bloodfeeder
		public int ticksToDisappear = 60000;
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
		public List<string> blackListedXenotypesForSerums = new();
		// public List<XenotypeDef> blackListedXenotypesForSingleSerums;
		// public List<XenotypeDef> blackListedXenotypesForHybridSerums;
		public List<XenotypeDef> whiteListedXenotypesForResurrectorSerums = new();
		public List<string> whiteListedXenotypesForFilter = new();
		// public List<GeneDef> whiteListedExoskinGenes;
		public List<BackstoryDef> blackListedBackstoryForChanger = new();
		// public List<ThingDef> listedGolems;
		// public List<GeneDef> gene_IsNotAcceptablePrey;
		// public List<GeneDef> gene_IsAngelBeauty;
		// public List<GeneDef> gene_PawnSkillsNotDecay;
		public List<string> mechDefNameShouldNotContain = new();
		// public List<GeneDef> xenoTree_PollutionReq_GeneDefs;
		public List<ThingDef> plantsToNotOverwrite_SpawnSubplant = new();
		public List<Type> shapeShift_IgnoredGeneClasses = new();
		// public List<TraitDef> shapeShift_ProhibitedTraits;
		// public List<PreceptDef> shapeShift_ProhibitedPrecepts;
		// public List<HediffDef> hediffsRemovedByGenesRestorationSerum;
		public List<MutantDef> xenoGenesMutantsExceptions = new();
		public List<GeneDef> anomalyXenoGenesExceptions = new();

		// [Obsolete]
		// public List<HediffDef> hediffsThatPreventUndeadResurrection;
		// [Obsolete]
		// public List<HediffDef> blackListedHediffDefForReimplanter;
		// [Obsolete]
		// public List<ThingDef> blackListedDefsForSerums;
		// [Obsolete]
		// public List<GeneDef> perfectCandidatesForSerums;
		// [Obsolete]
		// public List<GeneDef> nonCandidatesForSerums;
	}
}
