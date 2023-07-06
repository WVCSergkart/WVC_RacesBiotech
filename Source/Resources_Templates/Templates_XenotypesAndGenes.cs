using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;
using WVC;
using WVC_XenotypesAndGenes;


namespace WVC_XenotypesAndGenes
{

	public class SerumGeneTemplateDef : Def
	{

		public IntRange spawnIntervalRange = new(120000, 300000);

		public int stackCount = 1;

		public int displayOrderOffset = 1;

		public Type geneClass = typeof(Gene);

		public Color? iconColor;

		[MustTranslate]
		public string labelShortAdj;

		[MustTranslate]
		public List<string> customEffectDescriptions;

		[NoTranslate]
		public string iconPath;

		public GeneCategoryDef displayCategory;

		public float displayOrderInCategory;

		public float minAgeActive;

		public bool randomChosen;

		public HistoryEventDef deathHistoryEvent;

		public List<StatModifier> statOffsets;

		public List<StatModifier> statFactors;

		public List<ConditionalStatAffecter> conditionalStatAffecters;

		public List<PawnCapacityModifier> capMods;

		public int biostatCpx = 1;

		public int biostatMet;

		public int biostatArc;

		public List<string> exclusionTags;

		public GeneDef prerequisite;

		public float selectionWeight = 1f;

		// public float selectionWeightFactorDarkSkin = 1f;

		public bool canGenerateInGeneSet = true;

		public GeneSymbolPack symbolPack;

		public float marketValueFactor = 1f;

		// public GeneGraphicData graphicData;

		// public bool neverGrayHair;

		// public SoundDef soundCall;

		// public SoundDef soundDeath;

		// public SoundDef soundWounded;

		// public Type resourceGizmoType = typeof(GeneGizmo_Resource);

		// public float resourceLossPerDay;

		// [MustTranslate]
		// public string resourceLabel;

		// [MustTranslate]
		// public string resourceDescription;

		// public List<float> resourceGizmoThresholds;

		// public bool showGizmoOnWorldView;

		// public bool showGizmoWhenDrafted;

		// public bool showGizmoOnMultiSelect;

		// public List<AbilityDef> abilities;

		// public List<GeneticTraitData> forcedTraits;

		// public List<GeneticTraitData> suppressedTraits;

		// public List<NeedDef> disablesNeeds;

		// public NeedDef causesNeed;

		// public WorkTags disabledWorkTags;

		// public bool ignoreDarkness;

		// public EndogeneCategory endogeneCategory;

		// public bool dislikesSunlight;

		// public float lovinMTBFactor = 1f;

		// public bool immuneToToxGasExposure;

		// public List<Aptitude> aptitudes;

		// public PassionMod passionMod;

		// public bool removeOnRedress;

		// public bool passOnDirectly = true;

		// public bool preventPermanentWounds;

		// public bool dontMindRawFood;

		// public Color? hairColorOverride;

		// public Color? skinColorBase;

		// public Color? skinColorOverride;

		// public float randomBrightnessFactor;

		// public TagFilter hairTagFilter;

		// public TagFilter beardTagFilter;

		// public GeneticBodyType? bodyType;

		// public List<HeadTypeDef> forcedHeadTypes;

		// public float minMelanin = -1f;

		// public HairDef forcedHair;

		// public bool womenCanHaveBeards;

		// public float socialFightChanceFactor = 1f;

		// public float aggroMentalBreakSelectionChanceFactor = 1f;

		// public float mentalBreakMtbDays;

		// public MentalBreakDef mentalBreakDef;

		// public float missingGeneRomanceChanceFactor = 1f;

		// public float prisonBreakMTBFactor = 1f;

		// public float painOffset;

		// public float painFactor = 1f;

		// public float foodPoisoningChanceFactor = 1f;

		// public List<DamageFactor> damageFactors;

		// public SimpleCurve biologicalAgeTickFactorFromAgeCurve;

		// public List<HediffDef> makeImmuneTo;

		// public List<HediffDef> hediffGiversCannotGive;

		// public ChemicalDef chemical;

		// public float addictionChanceFactor = 1f;

		// public float overdoseChanceFactor = 1f;

		// public float toleranceBuildupFactor = 1f;

		// public bool sterilize;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
			if (!typeof(Gene).IsAssignableFrom(geneClass))
			{
				yield return "geneClass is not Gene or child thereof.";
			}
		}
	}

	public class SerumRecipeTemplateDef : Def
	{

		[MustTranslate]
		public string jobString = "Doing an unknown recipe.";

		public List<ThingDef> recipeUsers;

		public SurgeryOutcomeEffectDef surgeryOutcomeEffect;

		public bool dontShowIfAnyIngredientMissing;

		public float workAmount = -1f;

		public bool targetsBodyPart = true;

		public bool anesthetize = true;

		public Type workerClass = typeof(RecipeWorker);

		[Unsaved(false)]
		public int adjustedCount = 1;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
		}

	}

	public class SerumTemplateDef : Def
	{

		public Type thingClass;

		public ResourceCountPriority resourceReadoutPriority;

		public bool useHitPoints = true;

		public GraphicData graphicData;

		public List<StatModifier> statBases;

		public int stackLimit = 1;

		public float marketValueOffset = 1f;

		public List<ThingDefCountClass> costList;

		public RecipeMakerProperties recipeMaker;

		public TechLevel techLevel;

		public ResearchProjectDef basicResearchProjectDef;

		public ResearchProjectDef architeResearchProjectDef;

		[NoTranslate]
		public List<string> tradeTags;

		public Tradeability tradeability = Tradeability.All;

		[NoTranslate]
		public List<string> thingSetMakerTags;

		public int allowedArchonexusCount;

		public int pathCost;

		public bool rotatable = true;

		public bool drawGUIOverlay;

		public bool alwaysHaulable;

		public AltitudeLayer altitudeLayer = AltitudeLayer.Item;

		public bool selectable;

		public DrawerType drawerType = DrawerType.RealtimeOnly;

		public List<ThingCategoryDef> thingCategories;

		public ThingCategory category;

		public List<CompProperties> comps = new();

		// public IssueDef issue;

		public XenotypeForcerType xenotypeForcerType;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public SerumRecipeTemplateDef recipeTemplateDef;

		public SerumGeneTemplateDef geneTemplateDef;

		public enum XenotypeForcerType
		{
			Base,
			Hybrid,
			Custom,
			CustomHybrid
		}

		public string serumTagName = "AnySerum";

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
		}

	}
}
