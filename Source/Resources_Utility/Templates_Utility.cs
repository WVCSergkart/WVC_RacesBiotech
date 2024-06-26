using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using static Verse.GeneSymbolPack;

namespace WVC_XenotypesAndGenes
{

	public static class GeneratorUtility
	{

		// Dryads

		public static void GauranlenTreeModeDef()
		{
			if (!ModLister.IdeologyInstalled || !WVC_Biotech.settings.enable_dryadQueenMechanicGenerator)
			{
				return;
			}
			List<GauranlenTreeModeDef> exceptions = XenotypeFilterUtility.GauranlenTreeModeDefExceptions();
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
				// GeneExtension_Spawner modExtension = dummyDryad.geneDef?.GetModExtension<GeneExtension_Spawner>();
				// if (modExtension == null)
				// {
					// continue;
				// }
				// ThingDef newBasicDryad = GeneratorUtility.GetFromGauranlenGeneModeTemplate(dummyDryad.dryadDef.race);
				// DefGenerator.AddImpliedDef(newBasicDryad);
				// modExtension.defaultDryadThingDef = newBasicDryad;
				foreach (PawnKindDef dryad in dummyDryad.dryadDefs)
				{
					TrySetDryadComp(dryad.race);
					// if (!)
					// {
						// Log.Warning("Failed set CompProperties_GauranlenDryad for " + dryad.race.defName + ". These dryads will not work with the dryad queen gene.");
					// }
				}
			}
		}

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
				if (thingDef.GetCompProperties<CompProperties_GauranlenDryad>() == null)
				{
					CompProperties_GauranlenDryad dryad_comp = new();
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

		public static ThingDef GetFromGauranlenGeneModeTemplate(ThingDef thingDef)
		{
			ThingDef dryadDef = new()
			{
				defName = "WVC_XaG_" + thingDef.defName,
				label = thingDef.label,
				description = thingDef.description + "\n\n" + "WVC_XaG_GestatedDryadDescription".Translate().Resolve(),
				descriptionHyperlinks = thingDef.descriptionHyperlinks,
				modContentPack = WVC_Biotech.settings.Mod.Content,
				statBases = thingDef.statBases,
				uiIconScale = thingDef.uiIconScale,
				tools = thingDef.tools,
				tradeTags = thingDef.tradeTags,
				race = thingDef.race,
				comps = new(),
				thingCategories = thingDef.thingCategories,
				recipes = thingDef.recipes,
				thingClass = thingDef.thingClass,
				category = thingDef.category,
				selectable = thingDef.selectable,
				containedItemsSelectable = thingDef.containedItemsSelectable,
				containedPawnsSelectable = thingDef.containedPawnsSelectable,
				tickerType = thingDef.tickerType,
				altitudeLayer = thingDef.altitudeLayer,
				useHitPoints = thingDef.useHitPoints,
				hasTooltip = thingDef.hasTooltip,
				soundImpactDefault = thingDef.soundImpactDefault,
				drawHighlight = thingDef.drawHighlight,
				inspectorTabs = thingDef.inspectorTabs,
				drawGUIOverlay = thingDef.drawGUIOverlay,
				modExtensions = thingDef.modExtensions
			};
			dryadDef.race.allowedOnCaravan = true;
			dryadDef.race.disableAreaControl = false;
			if (!thingDef.comps.NullOrEmpty())
			{
				foreach (CompProperties item in thingDef.comps)
				{
					dryadDef.comps.Add(item);
				}
			}
			if (dryadDef.GetCompProperties<CompProperties_GauranlenDryad>() == null)
			{
				CompProperties_GauranlenDryad dryad_comp = new();
				dryad_comp.defaultDryadPawnKindDef = PawnKindDefOf.Dryad_Basic;
				dryadDef.comps.Add(dryad_comp);
			}
			return dryadDef;
		}

		// public static T DeepClone<T>(T obj)
		// {
			// using var ms = new MemoryStream();
			// var formatter = new BinaryFormatter();
			// formatter.Serialize(ms, obj);
			// ms.Position = 0;
			// return (T)formatter.Deserialize(ms);
		// }

		// public static void CopyRaceProperties(this RaceProperties target, RaceProperties source)
		// {
			// RaceProperties newRace = new();
			// newRace = source;
			// target = newRace;
		// }

		// public static void CopyRaceProperties(this RaceProperties target, RaceProperties source)
		// {
			// foreach (var property in source.GetType().GetProperties())
			// {
				// PropertyInfo propertyS = target.GetType().GetProperty(property.Name);
				// var value = property.GetValue(source, null);
				// try
				// {
					// propertyS.SetValue(target, value, null);
				// }
				// catch
				// {
					// Log.Error(property.Name);
				// }
			// }
		// }

		// public static T CopyClass<T>(T obj)
		// {
			// T objcpy = (T)Activator.CreateInstance(typeof(T));
			// foreach (var prop in obj.GetType().GetProperties())
			// {
				// var value = prop.GetValue(obj);
				// try
				// {
					// objcpy.GetType().GetProperty(prop.Name).SetValue(objcpy, value);
				// }
				// catch
				// {
				// }
			// }
			// return objcpy;
		// }

		// public static object DeepClone(object objSource)
		// {
			// Type type = objSource.GetType();
			// object obj = Activator.CreateInstance(type);
			// PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			// foreach (PropertyInfo propertyInfo in properties)
			// {
				// if (!propertyInfo.CanWrite)
				// {
					// continue;
				// }
				// if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.IsEnum || propertyInfo.PropertyType == typeof(string))
				// {
					// propertyInfo.SetValue(obj, propertyInfo.GetValue(objSource, null), null);
					// continue;
				// }
				// object value = propertyInfo.GetValue(objSource, null);
				// if (value == null)
				// {
					// propertyInfo.SetValue(obj, null, null);
				// }
				// else
				// {
					// propertyInfo.SetValue(obj, DeepClone(value), null);
				// }
			// }
			// return obj;
		// }

		// XenoForcers

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

		// ============================ SPAWNER ============================

		public static GeneDef GetFromTemplate_SpawnerGenes_Resources(SpawnerGeneTemplateDef template, ThingDef thingDef, int displayOrderBase)
		{
			GeneDef geneDef = new()
			{
				defName = template.defName + "_" + thingDef.defName + "_RB",
				label = template.label.Formatted(thingDef.LabelCap),
				labelShortAdj = template.labelShortAdj.Formatted(thingDef.label),
				description = template.description.Formatted(thingDef.label),
				geneClass = template.geneClass,
				iconPath = template.iconPath,
				// Icon = thingDef.uiIcon,
				hairColorOverride = thingDef.stuffProps.color,
				randomBrightnessFactor = 0f,
				customEffectDescriptions = new(),
				selectionWeight = template.selectionWeight,
				marketValueFactor = template.marketValueFactor,
				randomChosen = template.randomChosen,
				exclusionTags = template.exclusionTags,
				canGenerateInGeneSet = template.canGenerateInGeneSet,
				biostatCpx = template.biostatCpx,
				biostatMet = template.biostatMet,
				biostatArc = template.biostatArc,
				displayCategory = template.displayCategory,
				displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
				minAgeActive = template.minAgeActive,
				modContentPack = template.modContentPack,
				modExtensions = new List<DefModExtension>
				{
					new GeneExtension_Spawner
					{
						thingDefToSpawn = thingDef,
						stackCount = (int)(thingDef.stackLimit * template.stackCountPercent),
						spawnIntervalRange = template.spawnIntervalRange
					}
				}
			};
			if (template.modExtensions != null)
			{
				foreach (DefModExtension item in template.modExtensions)
				{
					geneDef.modExtensions.Add(item);
				}
			}
			if (template.customEffectDescriptions != null)
			{
				foreach (string item in template.customEffectDescriptions)
				{
					geneDef.customEffectDescriptions.Add(item.Formatted(thingDef.label, (template.spawnIntervalRange.min / 60000).ToString(), (template.spawnIntervalRange.max / 60000).ToString()));
				}
			}
			return geneDef;
		}

		// ============================ HAIR AND BODY COLOR OVERRIDE ============================

		public static GeneDef GetFromTemplate_SkinHairColorGenes_FromResources(ColorGeneTemplateDef template, ThingDef thingDef, int displayOrderBase)
		{
			GeneDef geneDef = new()
			{
				defName = template.defName + "_" + thingDef.defName + "_RB",
				label = template.label.Formatted(thingDef.label),
				// labelShortAdj = template.labelShortAdj.Formatted(thingDef.label),
				description = template.description.Formatted(thingDef.label),
				iconPath = template.iconPath,
				geneClass = typeof(Gene),
				randomBrightnessFactor = 0f,
				selectionWeight = template.selectionWeight,
				randomChosen = template.randomChosen,
				exclusionTags = template.exclusionTags,
				canGenerateInGeneSet = template.canGenerateInGeneSet,
				biostatCpx = template.biostatCpx,
				biostatMet = template.biostatMet,
				biostatArc = template.biostatArc,
				displayCategory = template.displayCategory,
				displayOrderInCategory = displayOrderBase + template.displayOrderOffset,
				modContentPack = template.modContentPack
			};
			if (template.modExtensions != null)
			{
				foreach (DefModExtension item in template.modExtensions)
				{
					geneDef.modExtensions.Add(item);
				}
			}
			if (template.skinColor)
			{
				geneDef.skinColorOverride = thingDef.stuffProps.color;
			}
			else
			{
				geneDef.hairColorOverride = thingDef.stuffProps.color;
			}
			return geneDef;
		}

	}
}
