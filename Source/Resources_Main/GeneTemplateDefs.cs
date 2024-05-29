using System;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	// public class GeneratorDef : Def
	// {

		// public bool isSkillGene = false;
		// public bool isXenoFrocer = false;
		// public bool isSpawnerGene = false;
		// public bool isDryadPatch = false;
		// public bool isColorGene = false;

		// public List<PawnKindDef> dryadDefs;

		// public override IEnumerable<string> ConfigErrors()
		// {
			// foreach (string item in base.ConfigErrors())
			// {
				// yield return item;
			// }
			// if (!isSkillGene && !isSpawnerGene && !isXenoFrocer && !isColorGene)
			// {
				// yield break;
			// }
			// if (!typeof(Gene).IsAssignableFrom(geneClass))
			// {
				// yield return "geneClass is not Gene or child thereof.";
			// }
		// }

	// }

	public class DummyDryadTemplateDef : Def
	{

		// public GeneDef geneDef;

		// public PawnKindDef dryadDef;

		public List<PawnKindDef> dryadDefs;

	}

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

	public class SpawnerGeneTemplateDef : Def
	{

		public IntRange spawnIntervalRange = new(120000, 300000);

		public int stackCount = 1;

		public float stackCountPercent = 0.1f;

		public int displayOrderOffset = 1;

		public Type geneClass = typeof(Gene_Spawner);

		[NoTranslate]
		public string iconPath;

		[MustTranslate]
		public string labelShortAdj;

		[MustTranslate]
		public List<string> customEffectDescriptions;

		public GeneCategoryDef displayCategory;

		public float displayOrderInCategory;

		public float minAgeActive;

		public bool randomChosen;

		public int biostatCpx = 0;

		public int biostatMet = 0;

		public int biostatArc = 0;

		public List<string> exclusionTags;

		public float selectionWeight = 0.0001f;

		public bool canGenerateInGeneSet = true;

		public float marketValueFactor = 1.1f;

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

	public class ColorGeneTemplateDef : Def
	{

		public int displayOrderOffset = 1;

		[NoTranslate]
		public string iconPath;

		public GeneCategoryDef displayCategory;

		public float displayOrderInCategory;

		public bool randomChosen = true;

		public int biostatCpx = 0;

		public int biostatMet = 0;

		public int biostatArc = 0;

		public bool skinColor = true;

		public List<string> exclusionTags;

		public float selectionWeight = 0.0001f;

		public bool canGenerateInGeneSet = true;

	}

}
