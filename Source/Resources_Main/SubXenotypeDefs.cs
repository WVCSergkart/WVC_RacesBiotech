using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class ChimeraDef : Def
	//{

	//	public List<GeneDef> addedGenes = new();

	//	public List<GeneDef> removedGenes = new();

	//}

	public class GolemModeDef : Def
	{

		public List<StatDef> displayedStats;

		public List<DefHyperlink> hyperlinks = new();

		public PawnKindDef pawnKindDef;

		public bool canBeSummoned = false;

		public bool canBeAnimated = true;

		public bool changeable = true;

		public float iconSize = 1f;

		public float order = 0f;

		private bool? isWorkGolemnoid;

		private string cachedDescription;

		private float? cachedGolembondCost;

		public bool Worker
		{
			get
			{
				if (isWorkGolemnoid == null)
				{
					isWorkGolemnoid = !pawnKindDef.race.race.mechEnabledWorkTypes.NullOrEmpty();
				}
				return isWorkGolemnoid.Value;
			}
		}

		public float GolembondCost
		{
			get
			{
				if (cachedGolembondCost == null)
				{
					cachedGolembondCost = pawnKindDef.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost);
				}
				return cachedGolembondCost.Value;
			}
		}

		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(pawnKindDef.race.description);
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Stats".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
					if (!displayedStats.NullOrEmpty())
					{
						for (int j = 0; j < displayedStats.Count; j++)
						{
							StatDef statDef = displayedStats[j];
							stringBuilder.AppendLine(" - " + statDef.LabelCap + ": " + statDef.ValueToString(pawnKindDef.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
						}
					}
					//stringBuilder.AppendLine(" - " + StatDefOf.ArmorRating_Blunt.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt).ToStringPercent());
					//stringBuilder.AppendLine(" - " + StatDefOf.ArmorRating_Sharp.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp).ToStringPercent());
					//stringBuilder.AppendLine(" - " + StatDefOf.MoveSpeed.LabelCap + ": " + StatDefOf.MoveSpeed.ValueToString(pawnKindDef.race.GetStatValueAbstract(StatDefOf.MoveSpeed), ToStringNumberSense.Absolute, !StatDefOf.MoveSpeed.formatString.NullOrEmpty()));
					//stringBuilder.AppendLine(" - " + WVC_GenesDefOf.WVC_GolemBondCost.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost));
					//stringBuilder.AppendLine(" - " + StatDefOf.BandwidthCost.LabelCap + ": " + pawnKindDef.race.GetStatValueAbstract(StatDefOf.BandwidthCost));
					if (Worker)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("MechWorkActivities".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						stringBuilder.AppendLine(" - " + pawnKindDef.race.race.mechEnabledWorkTypes.Select((WorkTypeDef w) => w.gerundLabel).ToCommaList(useAnd: true).CapitalizeFirst());
					}
					if (!pawnKindDef.weaponTags.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Weapon".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						stringBuilder.AppendLine(" - " + DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef thing) => !thing.weaponTags.NullOrEmpty() && thing.weaponTags.Contains(pawnKindDef.weaponTags.FirstOrDefault())).FirstOrDefault().label.CapitalizeFirst());
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_MechWeightClass".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + pawnKindDef.race.race.mechWeightClass.ToStringHuman().CapitalizeFirst());
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			label = pawnKindDef.label;
			description = pawnKindDef.description;
			hyperlinks.Add(new DefHyperlink(pawnKindDef.race));
			CompProperties_Spawner compProperties_Spawner = pawnKindDef?.race?.GetCompProperties<CompProperties_Spawner>();
			if (compProperties_Spawner != null)
			{
				hyperlinks.Add(new DefHyperlink(compProperties_Spawner.thingToSpawn));
			}
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

		public GeneDef reqGeneDef;

		public MutantDef mutantDef = null;

		public bool addGenesFromAbility = true;
		public bool addGenesFromMaster = true;

		public bool resurrectAsShambler = false;

		public XenotypeDef xenotypeDef;

		public List<RotStage> acceptableRotStages = new() { RotStage.Fresh, RotStage.Rotting };

		public float selectionWeight = 1f;

		[Obsolete]
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

	[Obsolete]
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

	public class XenotypeHolder
	{

		public string name = null;

		public XenotypeIconDef iconDef = null;

		public XenotypeDef xenotypeDef = null;

		public List<GeneDef> genes = new();

		public bool inheritable;

		public float displayPriority;

		public bool shouldSkip = false;

		public bool isTrueShiftForm;

		public bool isOverriden;

		public float? matchPercent;

		//public string customEffectsDesc = null;

		public bool Baseliner => xenotypeDef == XenotypeDefOf.Baseliner && genes.NullOrEmpty();

		public bool CustomXenotype => xenotypeDef == XenotypeDefOf.Baseliner && !genes.NullOrEmpty();

		public XenotypeHolder()
		{

		}

		public XenotypeHolder(XenotypeDef xenotypeDef)
		{
			this.xenotypeDef = xenotypeDef;
			genes = xenotypeDef.genes;
			inheritable = xenotypeDef.inheritable;
		}

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					if (name.NullOrEmpty())
					{
						cachedLabel = xenotypeDef.label;
					}
					else
					{
						cachedLabel = name;
					}
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		[Unsaved(false)]
		private string cachedDescription;

		public virtual string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					if (xenotypeDef != XenotypeDefOf.Baseliner)
					{
						stringBuilder.AppendLine(!xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.descriptionShort : xenotypeDef.description);
						if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine(("WVC_DoubleXenotypes".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + "\n" + xenotypeDef.doubleXenotypeChances.Select((XenotypeChance x) => "WVC_XaG_DoubleXenotypeWithChanceText".Translate(x.xenotype.LabelCap, (x.chance * 100f).ToString()).ToString()).ToLineList(" - "));
						}
					}
					else
					{
						stringBuilder.AppendLine("UniqueXenotypeDesc".Translate());
					}
					stringBuilder.AppendLine();
					//if (customEffectsDesc != null)
					//{
					//	stringBuilder.AppendLine(customEffectsDesc);
					//	stringBuilder.AppendLine();
					//}
					XaG_GeneUtility.GetBiostatsFromList(genes, out int biostatCpx, out int biostatMet, out int biostatArc);
					bool flag2 = false;
					if (biostatCpx != 0)
					{
						stringBuilder.AppendLineTagged("Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + biostatCpx.ToStringWithSign());
						flag2 = true;
					}
					if (biostatMet != 0)
					{
						stringBuilder.AppendLineTagged("Metabolism".Translate().CapitalizeFirst().Colorize(GeneUtility.METColor) + ": " + biostatMet.ToStringWithSign());
						flag2 = true;
					}
					if (biostatArc != 0)
					{
						stringBuilder.AppendLineTagged("ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + biostatArc.ToStringWithSign());
						flag2 = true;
					}
					if (flag2)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(("WVC_Inheritable".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + inheritable.ToStringYesNo());
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

	public class ThrallHolder : XenotypeHolder
	{

		public ThrallDef thrallDef;

		[Unsaved(false)]
		private string cachedDescription;

		public override string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(thrallDef.description);
					if (thrallDef.xenotypeDef != null && !thrallDef.xenotypeDef.descriptionShort.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(thrallDef.xenotypeDef.descriptionShort);
					}
					if (thrallDef.reqGeneDef != null)
                    {
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Requires".Translate() + ": " + thrallDef.reqGeneDef.LabelCap);
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_XaG_AcceptableRotStages".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + thrallDef.acceptableRotStages.Select((RotStage x) => x.ToStringHuman()).ToLineList(" - "));
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

    public class MechanoidHolder
    {

        public PawnKindDef pawnKindDef = null;

        public List<StatDef> displayedStats = new() { StatDefOf.WorkSpeedGlobal, StatDefOf.BandwidthCost, StatDefOf.ArmorRating_Blunt, StatDefOf.ArmorRating_Sharp };


		private float? voidEnergyCost;

		public float VoidEnergyCost
		{
			get
			{
				if (!voidEnergyCost.HasValue)
                {
                    voidEnergyCost = GetVoidMechCost(pawnKindDef);
                }
                return voidEnergyCost.Value;
			}
		}

        public static float GetVoidMechCost(PawnKindDef pawnKindDef, float limit = 99f)
		{
			float voidCost = (float)Math.Round((pawnKindDef.race.race.baseBodySize + pawnKindDef.race.race.baseHealthScale + (pawnKindDef.race.race.mechEnabledWorkTypes != null ? pawnKindDef.race.race.mechEnabledWorkTypes.Count : 0)) * 2f * pawnKindDef.race.race.mechWeightClass.ToFloatFactor() + 0.51f, 0);
			if (voidCost > limit)
			{
				return limit;
			}
			return voidCost;
		}

		private bool? isWorkGolemnoid;

        public bool Worker
        {
            get
            {
                if (isWorkGolemnoid == null)
                {
                    isWorkGolemnoid = pawnKindDef?.race?.race?.mechEnabledWorkTypes?.NullOrEmpty() == false;
                }
                return isWorkGolemnoid.Value;
            }
        }

        [Unsaved(false)]
        private string cachedDescription;

        public virtual string Description
        {
            get
            {
                if (cachedDescription == null)
                {
					string phase = "start";
                    try
					{
						StringBuilder stringBuilder = new();
						stringBuilder.AppendLine(pawnKindDef.race.description);
						stringBuilder.AppendLine();
						phase = "get stats";
						stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Stats".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
						if (!displayedStats.NullOrEmpty())
						{
							for (int j = 0; j < displayedStats.Count; j++)
							{
								StatDef statDef = displayedStats[j];
								stringBuilder.AppendLine(" - " + statDef.LabelCap + ": " + statDef.ValueToString(pawnKindDef.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
							}
						}
						phase = "get mechEnabledWorkTypes";
						if (Worker)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("MechWorkActivities".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
							stringBuilder.AppendLine(" - " + pawnKindDef.race.race.mechEnabledWorkTypes.Select((WorkTypeDef w) => w.gerundLabel).ToCommaList(useAnd: true).CapitalizeFirst());
						}
						phase = "get weaponTags";
						if (!pawnKindDef.weaponTags.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine("WVC_XaG_Dialog_Golemlink_Weapon".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":");
							stringBuilder.AppendLine(" - " + DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef thing) => !thing.weaponTags.NullOrEmpty() && thing.weaponTags.Contains(pawnKindDef.weaponTags.FirstOrDefault()))?.FirstOrDefault()?.label.CapitalizeFirst());
						}
						stringBuilder.AppendLine();
						phase = "get MechWeightClass";
						stringBuilder.Append("WVC_MechWeightClass".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + pawnKindDef.race.race.mechWeightClass.ToStringHuman().CapitalizeFirst());
						cachedDescription = stringBuilder.ToString();
					}
                    catch (Exception arg)
                    {
						Log.Error("Failed get description for mechanoid. On phase: " + phase + " Reason: " + arg);
						cachedDescription = "WVC_XaG_NoDescError".Translate() + " Cause failed get description for mechanoid. On phase: " + phase;
					}
                }
                return cachedDescription;
            }
        }

	}

}
