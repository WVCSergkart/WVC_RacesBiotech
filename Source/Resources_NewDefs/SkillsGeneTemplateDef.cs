using System;
using System.Collections.Generic;
using Verse;


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

}
