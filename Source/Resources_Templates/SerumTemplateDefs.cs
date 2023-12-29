using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;


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

		public bool canGenerateInGeneSet = true;

		public GeneSymbolPack symbolPack;

		public float marketValueFactor = 1f;

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
