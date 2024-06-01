using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// public class ThingCopyDef : ThingDef, ICloneable
	// {

		// public object Clone()
		// {
			// return (ThingDef) this.MemberwiseClone();
		// }

		// public ThingCopyDef()
		// {
		// }

		// public ThingCopyDef(ThingDef thingDef)
		// {
			// descriptionHyperlinks = thingDef.descriptionHyperlinks;
			// statBases = thingDef.statBases;
			// uiIconScale = thingDef.uiIconScale;
			// tools = thingDef.tools;
			// tradeTags = thingDef.tradeTags;
			// race = thingDef.race;
			// comps = thingDef.comps;
			// thingCategories = thingDef.thingCategories;
			// recipes = thingDef.recipes;
			// thingClass = thingDef.thingClass;
			// category = thingDef.category;
			// selectable = thingDef.selectable;
			// containedItemsSelectable = thingDef.containedItemsSelectable;
			// containedPawnsSelectable = thingDef.containedPawnsSelectable;
			// tickerType = thingDef.tickerType;
			// altitudeLayer = thingDef.altitudeLayer;
			// useHitPoints = thingDef.useHitPoints;
			// hasTooltip = thingDef.hasTooltip;
			// soundImpactDefault = thingDef.soundImpactDefault;
			// drawHighlight = thingDef.drawHighlight;
			// inspectorTabs = thingDef.inspectorTabs;
			// drawGUIOverlay = thingDef.drawGUIOverlay;
			// modExtensions = thingDef.modExtensions;
		// }

	// }

	[Obsolete]
	public class DryadKindDef : PawnKindDef
	{

		public PawnKindDef sourceDryadDef;

		public override void ResolveReferences()
		{
			if (sourceDryadDef != null)
			{
				label = sourceDryadDef.label;
				lifeStages = sourceDryadDef.lifeStages;
				race.statBases = sourceDryadDef.race.statBases;
				race.race.trainability = sourceDryadDef.race.race.trainability;
				race.race.lifeStageAges = sourceDryadDef.race.race.lifeStageAges;
				race.race.headPosPerRotation = sourceDryadDef.race.race.headPosPerRotation;
				race.race.trainableTags = sourceDryadDef.race.race.trainableTags;
				race.race.untrainableTags = sourceDryadDef.race.race.untrainableTags;
				race.race.baseBodySize = sourceDryadDef.race.race.baseBodySize;
				race.race.baseHealthScale = sourceDryadDef.race.race.baseHealthScale;
				race.tools = sourceDryadDef.race.tools;
				race.label = sourceDryadDef.race.label;
				race.description = sourceDryadDef.race.description;
			}
			label = "WVC_XaG_GestatedDryad".Translate() + " " + label;
			race.label = "WVC_XaG_GestatedDryad".Translate() + " " + race.label;
			race.description = race.description + "\n\n" + "WVC_XaG_GestatedDryadDescription".Translate();
			base.ResolveReferences();
		}

	}

	public class GauranlenGeneModeDef : Def
	{

		public GauranlenTreeModeDef useDescriptionFromDef = null;

		public GauranlenGeneModeDef previousStage;

		public PawnKindDef pawnKindDef;

		public ThingDef newDryadDef;

		public List<MemeDef> requiredMemes;

		public List<GeneDef> requiredGenes;

		public List<StatDef> displayedStats;

		public List<DefHyperlink> hyperlinks = new();

		private string cachedDescription;

		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					cachedDescription = description;
					CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race.GetCompProperties<CompProperties_Spawner>();
					if (compProperties_Spawner != null)
					{
						cachedDescription = cachedDescription + "\n\n" + "DryadProducesResourcesDesc".Translate(NamedArgumentUtility.Named(pawnKindDef, "DRYAD"), GenLabel.ThingLabel(compProperties_Spawner.thingToSpawn, null, compProperties_Spawner.spawnCount).Named("RESOURCES"), compProperties_Spawner.spawnIntervalRange.max.ToStringTicksToPeriod().Named("DURATION")).Resolve().CapitalizeFirst();
					}
				}
				return cachedDescription;
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (useDescriptionFromDef != null)
			{
				description = useDescriptionFromDef.description;
			}
			hyperlinks.Add(new DefHyperlink(pawnKindDef.race));
			CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race?.GetCompProperties<CompProperties_Spawner>();
			if (compProperties_Spawner != null)
			{
				hyperlinks.Add(new DefHyperlink(compProperties_Spawner.thingToSpawn));
			}
		}

	}

	public class ThralltypeDef : XenotypeDef
	{

		public List<ThrallDef> thrallDefs = new();

		public List<GeneDef> guaranteedGenes;

		public override void ResolveReferences()
		{
			if (genes.NullOrEmpty())
			{
				genes = new();
			}
			GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
			if (genes.Contains(geneticShifter))
			{
				genes.Remove(geneticShifter);
			}
			if (guaranteedGenes.NullOrEmpty())
			{
				guaranteedGenes = genes;
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			foreach (GeneDef gene in guaranteedGenes)
			{
				descriptionHyperlinks.Add(new DefHyperlink(gene));
			}
			foreach (ThrallDef thrallDef in DefDatabase<ThrallDef>.AllDefsListForReading)
			{
				// if (thrallDef.mutantDef != null)
				// {
					// continue;
				// }
				descriptionHyperlinks.Add(new DefHyperlink(thrallDef));
			}
			inheritable = true;
			doubleXenotypeChances = null;
			genes.Add(geneticShifter);
		}

	}

	public class ThrallDef : Def
	{

		public XenotypeIconDef xenotypeIconDef;

		public List<GeneDef> genes;

		public MutantDef mutantDef = null;

		public bool addGenesFromAbility = true;
		public bool addGenesFromMaster = true;

		public bool resurrectAsShambler = false;

		public XenotypeDef xenotypeDef;

		public List<RotStage> acceptableRotStages = new() { RotStage.Fresh, RotStage.Rotting };

		public float selectionWeight = 1f;

		[MustTranslate]
		public string generalDesc;

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
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
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Genes".Translate().CapitalizeFirst(), genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
		}

	}

	public class EvotypeDef : XenotypeDef
	{

		public float shapeshiftChance = 0.1f;

		public bool xenotypeCanShapeshiftOnDeath = false;
		public bool xenotypeCanEvolveOvertime = false;

		public List<XenotypeDef> subXenotypeDefs;

		public override void ResolveReferences()
		{
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (!subXenotypeDefs.NullOrEmpty())
			{
				foreach (XenotypeDef subXenotypeDef in subXenotypeDefs)
				{
					descriptionHyperlinks.Add(new DefHyperlink(subXenotypeDef));
				}
			}
			if (!genes.NullOrEmpty())
			{
				foreach (GeneDef gene in genes)
				{
					descriptionHyperlinks.Add(new DefHyperlink(gene));
				}
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}
			if (!subXenotypeDefs.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_SubXenotypesList".Translate().CapitalizeFirst(), subXenotypeDefs.Select((XenotypeDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_SubXenotypesList_Desc".Translate() + "\n\n" + subXenotypeDefs.Select((XenotypeDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 810);
			}
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_SubXenotypes_CanEvolveOnDeath".Translate(), xenotypeCanShapeshiftOnDeath.ToStringYesNo(), "WVC_XaG_SubXeno_SubXenotypes_CanEvolveOnDeath_Desc".Translate(), 800);
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_SubXenotypes_CanEvolveOvertime".Translate(), xenotypeCanEvolveOvertime.ToStringYesNo(), "WVC_XaG_SubXeno_SubXenotypes_CanEvolveOvertime_Desc".Translate(), 790);
			yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_EvolveChance".Translate(), (shapeshiftChance * 100).ToString() + "%", "WVC_XaG_SubXeno_EvolveChance_Desc".Translate(), 780);
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
			if (subXenotypeDefs.NullOrEmpty())
			{
				 yield return defName + " has null subXenotypeDefs. subXenotypeDefs must contain at least one xenotype.";
			}
			if (shapeshiftChance <= 0f)
			{
				 yield return defName + " shapeshiftChance must be > 0. If this is intended, then use XenotypeDef or SubXenotypeDef.";
			}
			if (!xenotypeCanEvolveOvertime && !xenotypeCanShapeshiftOnDeath)
			{
				 yield return defName + " xenotypeCanEvolveOvertime and xenotypeCanShapeshiftOnDeath is false. At least one must be true.";
			}
		}

	}

	public class SubXenotypeDef : XenotypeDef
	{

		// public new List<GeneDef> genes = new() { WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter };

		public List<GeneDef> endogenes = new();

		// public List<GeneDef> xenogenes = new();

		public List<GeneDef> removeGenes = new();

		// public List<XenotypeChance> inheritFromXenotypes;

		public float selectionWeight = 1f;

		// public XenotypeDef mainXenotypeDef;

		// public XenotypeIconDef xenotypeIconDef = null;

		[Obsolete]
		public List<RandomGenes> randomGenes = new();

		[Obsolete]
		public class RandomGenes
		{
			public bool inheritable = false;
			public List<GeneDef> genes = new();
		}

		public override void ResolveReferences()
		{
			if (endogenes.NullOrEmpty() || genes.NullOrEmpty())
			{
				return;
			}
			GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
			if (removeGenes.Contains(geneticShifter))
			{
				removeGenes.Remove(geneticShifter);
			}
			if (endogenes.Contains(geneticShifter))
			{
				endogenes.Remove(geneticShifter);
			}
			if (genes.Contains(geneticShifter))
			{
				genes.Remove(geneticShifter);
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (!doubleXenotypeChances.NullOrEmpty())
			{
				foreach (XenotypeChance xenotypeChance in doubleXenotypeChances)
				{
					descriptionHyperlinks.Add(new DefHyperlink(xenotypeChance.xenotype));
					foreach (GeneDef xenotype_gene in xenotypeChance.xenotype.genes)
					{
						if (!removeGenes.Contains(xenotype_gene))
						{
							descriptionHyperlinks.Add(new DefHyperlink(xenotype_gene));
						}
					}
				}
			}
			if (!endogenes.NullOrEmpty())
			{
				foreach (GeneDef gene in endogenes)
				{
					descriptionHyperlinks.Add(new DefHyperlink(gene));
				}
			}
			if (!genes.NullOrEmpty())
			{
				foreach (GeneDef gene in genes)
				{
					descriptionHyperlinks.Add(new DefHyperlink(gene));
				}
			}
			inheritable = false;
			genes.Add(geneticShifter);
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}
			if (!removeGenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RemovedGenes".Translate().CapitalizeFirst(), removeGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RemovedGenes_Desc".Translate() + "\n\n" + removeGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1050);
			}
			// if (!xenogenes.NullOrEmpty())
			// {
				// yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Xenogenes".Translate().CapitalizeFirst(), xenogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Xenogenes_Desc".Translate() + "\n\n" + xenogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1040);
			// }
			if (!endogenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Endogenes".Translate().CapitalizeFirst(), endogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Endogenes_Desc".Translate() + "\n\n" + endogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1030);
			}
			// if (!randomGenes.NullOrEmpty())
			// {
				// foreach (RandomGenes item in randomGenes)
				// {
					// yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RandomGenes".Translate().CapitalizeFirst(), item.genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RandomGenes_Desc".Translate() + "\n\n" + item.genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1020);
				// }
			// }
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
			if (doubleXenotypeChances.NullOrEmpty() || doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) != 1f)
			{
				yield return defName + " has null doubleXenotypeChances. doubleXenotypeChances must contain at least one xenotype with a chance 1.0";
			}
		}

	}
}
