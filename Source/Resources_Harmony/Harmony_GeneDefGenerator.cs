using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
// using static RimWorld.BaseGen.SymbolStack;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	[HarmonyPatch(typeof(GeneDefGenerator), "ImpliedGeneDefs")]
	public static class WVC_Genes_GeneDefGenerator_ImpliedGeneDefs_Patch
	{
		[HarmonyPostfix]
		public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
		{
			List<GeneDef> geneDefList = values.ToList();
			foreach (SkillsGeneTemplateDef template in DefDatabase<SkillsGeneTemplateDef>.AllDefsListForReading)
			{
				List<SkillDef> skillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
				foreach (SkillDef skillDef in skillDefs)
				{
					geneDefList.Add(GetFromTemplate_Skills(template, skillDef, skillDef.index * 1000));
				}
			}
			foreach (InheritableImmuneGeneTemplateDef template in DefDatabase<InheritableImmuneGeneTemplateDef>.AllDefsListForReading)
			{
				geneDefList.Add(GetFromTemplate_InheritableImmune(template));
			}
			// List<string> whiteListedTraits = new List<string>();
			// foreach (Template_WhiteListedTraitsDef item in DefDatabase<Template_WhiteListedTraitsDef>.AllDefsListForReading)
			// {
				// whiteListedTraits.AddRange(item.whiteListedTraits);
			// }
			return geneDefList;
		}

		public static GeneDef GetFromTemplate_Skills(SkillsGeneTemplateDef template, SkillDef def, int displayOrderBase)
		{
			GeneDef geneDef = new()
            {
				defName = template.defName + "_" + def.defName,
				geneClass = template.geneClass,
				label = template.label.Formatted(def.label),
				iconPath = template.iconPath.Formatted(def.defName),
				description = template.description.Formatted(def.label),
				labelShortAdj = template.labelShortAdj.Formatted(def.label),
				selectionWeight = template.selectionWeight,
				// iconColor = template.iconColor,
				biostatCpx = template.biostatCpx,
				biostatMet = template.biostatMet,
				biostatArc = template.biostatArc,
				displayCategory = template.displayCategory,
				displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
				minAgeActive = template.minAgeActive,
				modContentPack = template.modContentPack,
				modExtensions = template.modExtensions
			};
			if (!template.exclusionTagPrefix.NullOrEmpty())
			{
				geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix + "_" + def.defName };
			}
			if (def is SkillDef skill)
			{
				if (template.aptitudeOffset != 0)
				{
					geneDef.aptitudes = new List<Aptitude>
					{
						new Aptitude(skill, template.aptitudeOffset)
					};
				}
				if (template.passionModType != 0)
				{
					geneDef.passionMod = new PassionMod(skill, template.passionModType);
				}
			}
			return geneDef;
		}

		public static GeneDef GetFromTemplate_InheritableImmune(InheritableImmuneGeneTemplateDef template)
		{
			GeneDef geneDef = new()
            {
				defName = template.defName + "_" + template.inheritableGeneDef.defName,
				geneClass = template.geneClass,
				label = template.label,
				iconPath = template.iconPath,
				description = template.description,
				labelShortAdj = template.labelShortAdj,
				selectionWeight = template.selectionWeight,
				customEffectDescriptions = template.customEffectDescriptions,
				// iconColor = template.iconColor,
				biostatCpx = template.biostatCpx,
				biostatMet = template.biostatMet,
				biostatArc = template.biostatArc,
				prerequisite = template.prerequisite,
				displayCategory = template.displayCategory,
				displayOrderInCategory = template.displayOrderInCategory,
				resourceLossPerDay = template.resourceLossPerDay,
				makeImmuneTo = new List<HediffDef>(),
				hediffGiversCannotGive = new List<HediffDef>(),
				exclusionTags = new List<string>(),
				minAgeActive = template.minAgeActive,
				modContentPack = template.modContentPack,
				modExtensions = template.modExtensions
			};
			// if (!template.exclusionTagPrefix.NullOrEmpty())
			// {
				// geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix + "_" + def.defName };
			// }
			// template
			if (template.exclusionTags != null)
			{
				foreach (string item in template.exclusionTags)
				{
					geneDef.exclusionTags.Add(item);
				}
			}
			if (template.makeImmuneTo != null)
			{
				foreach (HediffDef item in template.makeImmuneTo)
				{
					geneDef.makeImmuneTo.Add(item);
				}
			}
			if (template.hediffGiversCannotGive != null)
			{
				foreach (HediffDef item in template.hediffGiversCannotGive)
				{
					geneDef.hediffGiversCannotGive.Add(item);
				}
			}
			// inherit
			if (template.inheritableGeneDef.exclusionTags != null)
			{
				foreach (string item in template.inheritableGeneDef.exclusionTags)
				{
					geneDef.exclusionTags.Add(item);
				}
			}
			if (template.inheritableGeneDef.makeImmuneTo != null)
			{
				foreach (HediffDef item in template.inheritableGeneDef.makeImmuneTo)
				{
					geneDef.makeImmuneTo.Add(item);
				}
			}
			if (template.inheritableGeneDef.hediffGiversCannotGive != null)
			{
				foreach (HediffDef item in template.inheritableGeneDef.hediffGiversCannotGive)
				{
					geneDef.hediffGiversCannotGive.Add(item);
				}
			}
			// if (def is SkillDef skill)
			// {
				// if (template.aptitudeOffset != 0)
				// {
					// geneDef.aptitudes = new List<Aptitude>
					// {
						// new Aptitude(skill, template.aptitudeOffset)
					// };
				// }
				// if (template.passionModType != 0)
				// {
					// geneDef.passionMod = new PassionMod(skill, template.passionModType);
				// }
			// }
			return geneDef;
		}
	}

}
