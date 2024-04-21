using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class ThrallDef : Def
	{

		public XenotypeIconDef xenotypeIconDef;

		public List<GeneDef> genes;

		public MutantDef mutantDef = null;

		public bool addGenesFromAbility = true;
		public bool addGenesFromMaster = true;

		public bool resurrectAsShambler = false;

		[MustTranslate]
		public string generalDesc;

		public override void ResolveReferences()
		{
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

		public List<SubXenotypeDef> subXenotypeDefs;

		public override void ResolveReferences()
		{
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (!subXenotypeDefs.NullOrEmpty())
			{
				foreach (SubXenotypeDef subXenotypeDef in subXenotypeDefs)
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
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_SubXenotypesList".Translate().CapitalizeFirst(), subXenotypeDefs.Select((SubXenotypeDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_SubXenotypesList_Desc".Translate() + "\n\n" + subXenotypeDefs.Select((SubXenotypeDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 810);
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
