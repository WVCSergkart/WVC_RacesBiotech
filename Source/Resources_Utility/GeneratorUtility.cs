using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using static Verse.GeneSymbolPack;

namespace WVC_XenotypesAndGenes
{

	public static class GeneratorUtility
	{

		// HarmonyHooks

		//public static void Geneline(List<GeneDef> geneDefList)
		//{
		//	//if (!WVC_Biotech.settings.generateSkillGenes)
		//	//{
		//	//	return;
		//	//}
		//	List<SkillDef> skillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
		//	foreach (SkillsGeneTemplateDef template in DefDatabase<SkillsGeneTemplateDef>.AllDefsListForReading)
		//	{
		//		foreach (SkillDef skillDef in skillDefs)
		//		{
		//			geneDefList.Add(GeneratorUtility.GetFromTemplate_Skills(template, skillDef, skillDef.index * 1000));
		//		}
		//	}
		//}

		public static void HybridForcerGenes(List<GeneDef> geneDefList)
		{
			if (!WVC_Biotech.settings.generateXenotypeForceGenes)
			{
				return;
			}
			foreach (XenotypeForcerGeneTemplateDef template in DefDatabase<XenotypeForcerGeneTemplateDef>.AllDefsListForReading)
			{
				foreach (XenotypeDef allDef in ListsUtility.GetWhiteListedXenotypes(true, true))
				{
					geneDefList.Add(GeneratorUtility.GetFromTemplate_XenotypeForcer(template, allDef, allDef.index * 1000));
				}
			}
		}

		public static void Aptitudes(List<GeneDef> geneDefList)
		{
			if (!WVC_Biotech.settings.generateSkillGenes)
			{
				return;
			}
			List<SkillDef> skillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
			foreach (SkillsGeneTemplateDef template in DefDatabase<SkillsGeneTemplateDef>.AllDefsListForReading)
			{
				foreach (SkillDef skillDef in skillDefs)
				{
					geneDefList.Add(GeneratorUtility.GetFromTemplate_Skills(template, skillDef, skillDef.index * 1000));
				}
			}
		}

		public static void GauranlenTreeModeDef()
		{
			if (!ModLister.IdeologyInstalled || !WVC_Biotech.settings.enable_dryadQueenMechanicGenerator)
			{
				return;
			}
			List<GauranlenTreeModeDef> exceptions = ListsUtility.GetGauranlenTreeModeDefExceptions();
			foreach (GauranlenTreeModeDef gauranlenTreeModeDef in DefDatabase<GauranlenTreeModeDef>.AllDefsListForReading)
			{
				if (exceptions.Contains(gauranlenTreeModeDef))
				{
					continue;
				}
				DefGenerator.AddImpliedDef(GeneratorUtility.GetFromGauranlenTreeModeTemplate(gauranlenTreeModeDef));
			}
			foreach (DummyDryadTemplateDef dummyDryad in DefDatabase<DummyDryadTemplateDef>.AllDefsListForReading)
			{
				if (dummyDryad.dryadDefs.NullOrEmpty())
				{
					continue;
				}
				foreach (PawnKindDef dryad in dummyDryad.dryadDefs)
				{
					TrySetDryadComp(dryad.race);
				}
			}
		}

		// public static void PatchThinkTree(ThinkTreeDef dryadTree)
		// {
			// if (dryadTree.thinkRoot == null)
			// {
				// Log.Error("Failed to patch " + dryadTree.defName + " root is null.");
				// return;
			// }
			// ThinkNode node = GetReturnToGauranlenTreeSubNode(dryadTree.thinkRoot);
			// if (node == null)
			// {
				// Log.Error("Failed to patch " + dryadTree.defName + " node is null.");
				// return;
			// }
		// }

		// public static bool IsGestatedDryadThinkTree(ThinkTreeDef dryadTree, out ThinkTreeDef checkedTree)
		// {
			// checkedTree = null;
			// foreach (ThinkNode subNode in dryadTree.thinkRoot.subNodes)
			// {
				// if (subNode is ThinkNode_ConditionalGestatedDryad)
				// {
					// checkedTree = dryadTree;
					// return true;
				// }
			// }
			// return false;
		// }

		// Dryads

		public static GauranlenGeneModeDef GetFromGauranlenTreeModeTemplate(GauranlenTreeModeDef def)
		{
			GauranlenGeneModeDef gauranlenGeneModeDef = new()
			{
				defName = "WVC_XaG_" + def.defName,
				label = def.label,
				description = def.description,
				pawnKindDef = def.pawnKindDef,
				requiredMemes = def.requiredMemes,
				displayedStats = def.displayedStats,
				// hyperlinks = def.hyperlinks,
				modContentPack = WVC_Biotech.settings.Mod.Content,
				modExtensions = def.modExtensions
			};
			if (def.pawnKindDef != null)
			{
				// ThingDef newDryadDef = GetFromGauranlenGeneModeTemplate(def.pawnKindDef.race);
				// DefGenerator.AddImpliedDef(newDryadDef);
				// gauranlenGeneModeDef.newDryadDef = newDryadDef;
				// def.pawnKindDef.race.race.allowedOnCaravan = true;
				// def.pawnKindDef.race.race.disableAreaControl = false;
				TrySetDryadComp(def.pawnKindDef.race);
				// if (!TrySetDryadComp(def.pawnKindDef.race))
				// {
					// Log.Warning("Failed set CompProperties_GauranlenDryad for " + def.pawnKindDef.race.defName + ". These dryads will not work with the dryad queen gene.");
				// }
			}
			return gauranlenGeneModeDef;
		}

		public static bool TrySetDryadComp(ThingDef thingDef)
		{
			try
			{
				thingDef.race.allowedOnCaravan = true;
				thingDef.race.disableAreaControl = false;
				if (thingDef.comps == null)
				{
					thingDef.comps = new();
				}
				if (thingDef.GetCompProperties<CompProperties_GestatedDryad>() == null)
				{
					CompProperties_GestatedDryad dryad_comp = new();
					dryad_comp.defaultDryadPawnKindDef = PawnKindDefOf.Dryad_Basic;
					thingDef.comps.Add(dryad_comp);
				}
				return true;
			}
			catch
			{
				Log.Warning("Failed set CompProperties_GauranlenDryad for " + thingDef.defName + ". These dryads will not work with the dryad queen gene.");
			}
			return false;
		}

		//[Obsolete]
		//public static ThingDef GetFromGauranlenGeneModeTemplate(ThingDef thingDef)
		//{
		//	ThingDef dryadDef = new()
		//	{
		//		defName = "WVC_XaG_" + thingDef.defName,
		//		label = thingDef.label,
		//		description = thingDef.description + "\n\n" + "WVC_XaG_GestatedDryadDescription".Translate().Resolve(),
		//		descriptionHyperlinks = thingDef.descriptionHyperlinks,
		//		modContentPack = WVC_Biotech.settings.Mod.Content,
		//		statBases = thingDef.statBases,
		//		uiIconScale = thingDef.uiIconScale,
		//		tools = thingDef.tools,
		//		tradeTags = thingDef.tradeTags,
		//		race = thingDef.race,
		//		comps = new(),
		//		thingCategories = thingDef.thingCategories,
		//		recipes = thingDef.recipes,
		//		thingClass = thingDef.thingClass,
		//		category = thingDef.category,
		//		selectable = thingDef.selectable,
		//		containedItemsSelectable = thingDef.containedItemsSelectable,
		//		containedPawnsSelectable = thingDef.containedPawnsSelectable,
		//		tickerType = thingDef.tickerType,
		//		altitudeLayer = thingDef.altitudeLayer,
		//		useHitPoints = thingDef.useHitPoints,
		//		hasTooltip = thingDef.hasTooltip,
		//		soundImpactDefault = thingDef.soundImpactDefault,
		//		drawHighlight = thingDef.drawHighlight,
		//		inspectorTabs = thingDef.inspectorTabs,
		//		drawGUIOverlay = thingDef.drawGUIOverlay,
		//		modExtensions = thingDef.modExtensions
		//	};
		//	dryadDef.race.allowedOnCaravan = true;
		//	dryadDef.race.disableAreaControl = false;
		//	if (!thingDef.comps.NullOrEmpty())
		//	{
		//		foreach (CompProperties item in thingDef.comps)
		//		{
		//			dryadDef.comps.Add(item);
		//		}
		//	}
		//	if (dryadDef.GetCompProperties<CompProperties_GestatedDryad>() == null)
		//	{
		//		CompProperties_GestatedDryad dryad_comp = new();
		//		dryad_comp.defaultDryadPawnKindDef = PawnKindDefOf.Dryad_Basic;
		//		dryadDef.comps.Add(dryad_comp);
		//	}
		//	return dryadDef;
		//}

		// XenoForcers

		//[Obsolete]
		public static GeneDef GetFromTemplate_XenotypeForcer(XenotypeForcerGeneTemplateDef template, XenotypeDef def, int displayOrderBase)
		{
			GeneDef geneDef = new()
			{
				defName = template.defName + "_" + def.defName + "_XenoForcer",
				geneClass = template.geneClass,
				label = template.label.Formatted(def.label),
				// iconPath = template.iconPath.Formatted(def.defName),
				iconPath = def.iconPath,
				description = template.description.Formatted(def.label),
				labelShortAdj = template.labelShortAdj.Formatted(def.label),
				selectionWeight = template.selectionWeight,
				// iconColor = template.iconColor,
				biostatCpx = template.biostatCpx,
				biostatMet = template.biostatMet,
				biostatArc = template.biostatArc,
				displayCategory = template.displayCategory,
				// displayOrderInCategory = template.displayOrderInCategory,
				customEffectDescriptions = template.customEffectDescriptions,
				displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
				minAgeActive = template.minAgeActive,
				modContentPack = template.modContentPack,
				exclusionTags = new List<string>(),
				modExtensions = new List<DefModExtension>
				{
					new GeneExtension_Giver
					{
						xenotypeForcerDef = def,
						xenotypeIsInheritable = def.inheritable
					}
				}
			};
			string exclusionTag = "WVC_XenotypesAndGenes_XenotypeTemplateCollection";
			if (def.inheritable)
			{
				exclusionTag += "_Endo";
			}
			else
			{
				exclusionTag += "_Xeno";
			}
			geneDef.exclusionTags.Add(exclusionTag);
			if (template.exclusionTags != null)
			{
				foreach (string item in template.exclusionTags)
				{
					geneDef.exclusionTags.Add(item);
				}
			}
			if (template.modExtensions != null)
			{
				foreach (DefModExtension item in template.modExtensions)
				{
					geneDef.modExtensions.Add(item);
				}
			}
			return geneDef;
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
				statOffsets = template.statOffsets,
				statFactors = template.statFactors,
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

		// ============================ GENELINE ============================

		//public static GeneDef GetGenelineGene(GeneDef geneDef)
		//{
		//	GeneDef genelineGeneDef = geneDef.Clone() as GeneDef;
		//	genelineGeneDef.defName = geneDef.defName + "_Geneline";
		//	genelineGeneDef.selectionWeight = 0f;
		//	genelineGeneDef.canGenerateInGeneSet = false;
		//	genelineGeneDef.biostatCpx = 0;
		//	genelineGeneDef.biostatMet = 0;
		//	genelineGeneDef.biostatArc = 0;
		//	genelineGeneDef.ResolveDefNameHash();
		//	return genelineGeneDef;
		//}

		public static object Clone(this object obj)
		{
			return obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, null);
		}

	}
}
