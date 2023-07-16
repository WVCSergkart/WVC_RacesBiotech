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

	public class SkillsGeneTemplateDef : Def
	{
		public Type geneClass = typeof(Gene);

		public int biostatCpx;

		public int biostatMet;

		public int biostatArc;

		public int aptitudeOffset;

		// public Color? iconColor;

		// public float addictionChanceFactor = 1f;

		public PassionMod.PassionModType passionModType;

		public float minAgeActive;

		public GeneCategoryDef displayCategory;

		public int displayOrderOffset;

		public float selectionWeight = 1f;

		[MustTranslate]
		public string labelShortAdj;

		[NoTranslate]
		public string iconPath;

		[NoTranslate]
		public string exclusionTagPrefix;

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

	public class InheritableImmuneGeneTemplateDef : Def
	{
		public Type geneClass = typeof(Gene);

		public int biostatCpx;

		public int biostatMet;

		public int biostatArc;

		public float minAgeActive;

		public GeneCategoryDef displayCategory;

		// public int displayOrderOffset;

		public float selectionWeight = 1f;

		public GeneDef inheritableGeneDef = null;

		public List<HediffDef> makeImmuneTo;

		public List<HediffDef> hediffGiversCannotGive;

		public GeneDef prerequisite;

		[MustTranslate]
		public string labelShortAdj;

		[NoTranslate]
		public string iconPath;

		// [NoTranslate]
		// public string exclusionTagPrefix;

		[MustTranslate]
		public List<string> customEffectDescriptions;

		public List<string> exclusionTags;

		public float displayOrderInCategory;

		public float resourceLossPerDay;

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
			if (inheritableGeneDef == null)
			{
				yield return "inheritableGeneDef is null.";
			}
		}
	}

	public class XenotypeForcerGeneTemplateDef : Def
	{
		public Type geneClass = typeof(Gene_XenotypeForcer);

		public int biostatCpx = 0;

		public int biostatMet = 0;

		public int biostatArc = 1;

		public float selectionWeight = 0.0f;

		public bool canGenerateInGeneSet = false;

		public float minAgeActive;

		public GeneCategoryDef displayCategory;

		// public float displayOrderInCategory;

		public int displayOrderOffset;

		[MustTranslate]
		public string labelShortAdj;

		[MustTranslate]
		public List<string> customEffectDescriptions;

		[NoTranslate]
		public string iconPath;

		public List<string> exclusionTags;

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

	// public class ExoskinistPreceptTemplateDef : Def
	// {
		// public Type preceptClass = typeof(Precept);

		// public IssueDef issue;

		// public PreceptImpact impact;

		// public List<MemeDef> associatedMemes = new List<MemeDef>();

		// public List<MemeDef> conflictingMemes = new List<MemeDef>();

		// public List<MemeDef> requiredMemes = new List<MemeDef>();

		// public int displayOrderInImpact;

		// public int displayOrderInIssue;

		// public List<PreceptComp> comps = new List<PreceptComp>();

		// public List<PreceptApparelRequirement> roleApparelRequirements;

		// public WorkTags roleDisabledWorkTags;
	// }

	// public class ExoskinistThoughtTemplateDef : Def
	// {
		// public string postfixName;

		// public Type workerClass;

		// public List<TraitDef> nullifyingTraits;

		// public List<TraitRequirement> nullifyingTraitDegrees;

		// public List<PreceptDef> nullifyingPrecepts;

		// public List<HediffDef> nullifyingHediffs;

		// public List<GeneDef> nullifyingGenes;

		// public ExpectationDef minExpectation;

		// public bool validWhileDespawned;

		// public List<ThoughtStage> stages = new List<ThoughtStage>();
	// }

	public class SubXenotypeDef : Def
	{
		public List<GeneDef> genes = new();

		public List<GeneDef> removeGenes = null;

		public List<GeneDef> mainGenes = null;

		public bool inheritable = false;

		public bool overrideExistingGenes = false;

		public bool ignoreExistingGenes = false;

		// public bool useMainAdditionalGenes = false;

		public List<GeneDef> AllGenes => genes;

		public XenotypeIconDef xenotypeIconDef = null;

		public override void ResolveReferences()
		{
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (mainGenes != null)
			{
				foreach (GeneDef gene in mainGenes)
				{
					descriptionHyperlinks.Add(new DefHyperlink(gene));
				}
			}
			foreach (GeneDef gene in genes)
			{
				descriptionHyperlinks.Add(new DefHyperlink(gene));
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}
			if (mainGenes != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_MainGenes".Translate().CapitalizeFirst(), mainGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_MainGenes_Desc".Translate() + "\n\n" + mainGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1010);
			}
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Genes".Translate().CapitalizeFirst(), genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
			if (removeGenes != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RemovedGenes".Translate().CapitalizeFirst(), removeGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RemovedGenes_Desc".Translate() + "\n\n" + removeGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 990);
			}
			if (!genes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "GenesAreInheritable".Translate(), inheritable.ToStringYesNo(), "GenesAreInheritableXenotypeDef".Translate(), 980);
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_OverrideExistingGenes".Translate(), overrideExistingGenes.ToStringYesNo(), "WVC_XaG_SubXeno_OverrideExistingGenes_Desc".Translate(), 970);
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_IgnoreExistingGenes".Translate(), ignoreExistingGenes.ToStringYesNo(), "WVC_XaG_SubXeno_IgnoreExistingGenes_Desc".Translate(), 960);
			}
		}
	}
}
