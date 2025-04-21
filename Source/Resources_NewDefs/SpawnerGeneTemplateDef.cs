using System;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	[Obsolete]
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

}
