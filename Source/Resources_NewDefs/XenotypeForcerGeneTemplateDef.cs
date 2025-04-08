using System;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{
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

}
