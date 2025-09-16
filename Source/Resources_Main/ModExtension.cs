using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

    public class GeneralHolder
	{
		// GenePacks
		public int genesCount = 0;
		public int architeCount = 0;
		public float chance = 1.0f;
		public List<GeneCategoryDef> allowedGeneCategoryDefs;
		// Tratis
		public TraitDef traitDef;
		public float weight = 1f;
		// StatPart
		public ThingDef thingDef;
		public float bandwidth = 1f;
		public bool cosmeticOnly = false;
		public bool prerequisitesOnly = false;
		// Duplicated Genes
		public GeneDef sourceGeneDef;
		public List<GeneDef> dupGeneDefs;
		// Holoface
		public Color color = Color.white;
		public bool visible = true;
		[MustTranslate]
		public string label;
		// Chimera
		public GeneDef reqGeneDef;
		public bool reqOneManArmy = false;
		public List<GeneDef> genes;
		public List<BiomeDef> biomeDefs;
		public XenotypeDef xenotypeDef;
		public IntRange? metRange;
		// Duplicator
		public GeneDef geneDef;
		public int reqDupesCount;
		public bool forDuplicates = false;
		// CopyPaste
		public GeneDef copyFromGeneDef;
		public bool copyExclusionTags = true;
		public bool copyMakeImmuneTo = true;
		public bool copyHediffGiversCannotGive = true;
		public bool copySuppressedTraits = true;
		public bool copyForcedTraits = true;
		public bool copyConditionalStatAffecters = true;
		public bool copyStatFactors = true;
		public bool copyStatOffsets = true;
		public bool copySymbolPack = true;

		public bool CanAddGene(Pawn pawn, Gene_Chimera chimera)
		{
            if (genes == null)
            {
				return false;
            }
            if (reqOneManArmy && !StaticCollectionsClass.oneManArmyMode)
			{
				return false;
			}
            if (reqGeneDef != null && !XaG_GeneUtility.HasActiveGene(reqGeneDef, pawn))
			{
				return false;
			}
            if (metRange.HasValue && (metRange.Value.TrueMin != chimera.ReqMetRange.TrueMin || metRange.Value.TrueMax != chimera.ReqMetRange.TrueMax))
			{
				return false;
			}
            return true;
		}

	}

	// Exten

	public class GeneExtension_Spawner : DefModExtension
	{
		// Thing Spawner
		public ThingDef thingDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;
		public int stackCount = 1;
		public IntRange spawnIntervalRange = new(120000, 300000);
		public string spawnMessage = "MessageCompSpawnerSpawnedItem";
		public bool showMessageIfOwned = true;
		public HediffDef initialHediffDef;
		public bool useMatchPercentFromSettings = false;
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
		public bool showInspectInfoIfOwned = true;
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
		public ThingDef defaultDryadThingDef;
		public StatDef dryadsStatLimit;
		public HediffDef postGestationSickness;
		public ThoughtDef dryadDiedMemoryDef;
		//BloodyGrowths
		public float hemogenPerThing = 0.2f;
		public ThingStyleDef styleDef = null;
		// GeneDigester
		public IntRange digestRange = new(1, 2);
		public float chance = 0.66f;
		public SoundDef soundDef;
		// Golems
		public List<PawnKindDef> mechTypes;
		public List<GolemModeDef> golemModeDefs;
		//public GolemlinkModeDef defaultGolemlinkModeDef;
		// Golems
		public IntRange durationIntervalRange = new(240000, 300000);
		// Void
		public HediffDef mechanitorHediff;
		public HediffDef mechHediff;
		public StatDef voidMechsLimit_StatDef;
		public StatDef voidDamageChance_StatDef;
		public StatDef voidMaxResource_StatDef;
		// Mechs
		public List<MechWeightClassDef> allowedMechWeightClasses;
	}

	public class GeneExtension_Undead : DefModExtension
	{
		// Undead
		public List<HediffDef> hediffDefs;
		public List<ThoughtDef> thoughtDefs;
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
		public List<GeneralHolder> possibleTraits;
		public StatDef shiftStatDef;
		public int reqGeneMat = -1;
		public GeneCategoryDef overrideGeneCategory;
		// Special Food
		public List<ThingDef> specialFoodDefs;
		// GeneticThief
		public XenotypeDef xenotypeDef;
		//public Color collectedGenesColor;
		//public Color destroyedGenesColor;
		// Regen
		public float regeneration = -1f;
		public float immunization = -1f;
		//public float deathrestBoost = -1f;
		// Regen
		// public ShapeshiftModeDef defaultShapeMode;
		// public List<ShapeshiftModeDef> initialShapeModes;
		// Chimera
		public List<GeneDef> chimeraGenesTools;
		public List<GeneDef> humanBasicGenes;
		//public List<GeneDef> chimeraOneManArmyGenes;
		public List<GeneralHolder> chimeraConditionalGenes;
		// Fleshmass
		//public int maxMutationLevel = 5;
		//Chimera
		public List<GeneDef> addedGenes;
		public List<GeneDef> removedGenes;
	}

	public class GeneExtension_Opinion : DefModExtension
	{
		public ThoughtDef thoughtDef;
		public ThoughtDef packMemberLost;
		public ThoughtDef AboutMeThoughtDef;
		public ThoughtDef MeAboutThoughtDef;
		public ThoughtDef myTargetInGeneralThought;
		public bool targetShouldBePsySensitive = false;
		public bool targetShouldBeFamily = false;
		public bool ignoreIfHasGene = false;
		public bool onlySameXenotype = false;
		public ThoughtDef sameAsMe_AboutMeThoughtDef;
		//Recluse
		public int colonistsLimit = 1;
	}

	public class GeneExtension_General : DefModExtension
	{
		public PawnKindDef pawnKindDef;
		public float recruitChance = 0.5f;
		public bool canBePredatorPrey = true;
		public List<GeneDef> inheritableGeneDefs;
		public List<GeneralHolder> copyFromGeneDefs;
		//public bool removeExclusionTags = false;
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
		public List<GeneralHolder> genesCountProbabilities;
		public RulePackDef genepackNamer;
		public bool supportMutants = true;
		public List<MutantDef> supportedMutantDefs;
		// implanter
		public float reimplantChance = 0.02f;
		// weight flatter
		public float selectionWeight = 1f;
		public bool isAptitude = false;
		// Birth
		public float birthQualityOffset = 0f;
		//outcome
		public List<XenotypeChance> xenotypeChances;
		//Food
		public bool isDustogenic = false;
		//Fleshmass
		public bool isFleshmass = false;
		//Xenos
		public bool isChimerkin = false;
		//public bool isEvolvedChimerkin = false;
		// Quests
		//public string questDescription = null;
		//Abilities
		//public bool isBloodfeed = false;
	}

	public class GeneExtension_Graphic : DefModExtension
	{
		public string backgroundPathEndogenes = "WVC/UI/Genes/GeneBackground_Endogene";
		public string backgroundPathXenogenes = "WVC/UI/Genes/GeneBackground_Xenogene";
		public string backgroundPathEndoArchite = "WVC/UI/Genes/GeneBackground_ArchiteGene";
		public string backgroundPathXenoArchite = "WVC/UI/Genes/GeneBackground_XenoArchiteGene";
		// BodySkin
		public bool furIsSkinWithHair = false;
		public bool furIsSkinWithMask = false;
		public bool furIsSkin = false;
		public bool furIsSkinTransparent = false;
		public bool invisible = false;
		public float alpha = 0.8f;
		//public bool glowingHair = false;
		[Obsolete]
		public bool furCanRot = true;
		// BodySize
		[Obsolete]
		public float bodyScaleFactor = 1f;
		[Obsolete]
		public float headScaleFactor = 1f;
	}

	public class GeneExtension_Giver : DefModExtension
	{
		public HediffDef hediffDefName;
		public HediffDef hediffDef;
		public HediffDef dupeHediffDef;
		public List<HediffDef> hediffDefs;
		public List<BodyPartDef> bodyparts;
		public BackstoryDef childBackstoryDef;
		public BackstoryDef adultBackstoryDef;
		public XenotypeDef xenotypeForcerDef = null;
		public bool xenotypeIsInheritable = true;
		public List<HeadTypeDef> headTypeDefs;
		public HediffDef metHediffDef;
		// Scarifier
		//public int scarsCount = 0;
		public StatDef scarsStatDef;
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
		//public Color filledBlockColor = ColorLibrary.Orange;
		//public Color excessBlockColor = ColorLibrary.Red;
		//public float gizmoOrder = -90f;
		//public int recacheFrequency = 231;
		//public string tipSectionTitle = "WVC_XaG_GolemBandwidth";
		//public string tipSectionTip = "WVC_XaG_GolemBandwidthGizmoTip";
		//public int golemistTypeIndex = -1;
		public QuestScriptDef summonQuest;
		// Bloodeater
		public float nutritionPerBite = 0.8f;
		public JobDef bloodeaterFeedingJobDef;
		// Photosynthesis
		public float passivelyReplenishedNutrition = 0.04f;
		// Bloodfeeder
		public int ticksToDisappear = 60000;
		// Energy
		public bool foodPoisoningFromFood = false;
		public JobDef rechargeableStomachJobDef;
		public JobDef repairJobDef;
		public ThingDef xenoChargerDef;
		public float chargeSpeedFactor = 1f;
		// Morpher
		public List<XenotypeDef> xenotypeDefs;
		// Colorable Eyes
		public List<GeneralHolder> holofaces;
		public Color defaultColor;
		//Morpher
		public GeneDef morpherTriggerGene;
		public List<GeneDef> morpherTriggerGenes;
		public JobDef morpherTriggerChangeJob;
		public List<XenotypeDef> morpherXenotypeDefs;
		public List<XenotypeChance> morpherXenotypeChances;
		//Voidlink
		//public float maxVoidEnergyOffset = 0f;
		//Subhumans
		public MutantDef mutantDef;
		//Skinshaper
		public List<GeneSetPresets> geneSetPresets;
		public GeneSetPresets geneSetByDefault;
		//
		public StatDef statDef;
		//
		//public List<AbilityDef> abilityDefs;
		//public List<AbilityDef> abilityDefs;
		//public bool chimeraCanEatGenes = true;
		public List<NeedDef> needDefs;
		public List<GeneDef> geneDefs;
		public List<GeneralHolder> geneDefWithChances;
		public List<GeneCategoryDef> geneCategoryDefs;
	}

	public class GeneExtension_Obsolete : DefModExtension
	{
		public bool logInDevMode = true;
	}
}
