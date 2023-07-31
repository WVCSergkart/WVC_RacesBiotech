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

        public string suffixDefName = "XaG";

        public List<GeneDef> inheritableGeneDefs = null;

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
            if (inheritableGeneDefs == null)
            {
                yield return "inheritableGeneDefs is null.";
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
}
