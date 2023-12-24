using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace WVC_XenotypesAndGenes
{

	// public class EndotypeDef : XenotypeDef
	// {

		// public override void ResolveReferences()
		// {
			// if (endogenes.NullOrEmpty() || mainXenotypeDef == null)
			// {
				// return;
			// }
			// if (descriptionHyperlinks == null)
			// {
				// descriptionHyperlinks = new List<DefHyperlink>();
			// }
			// if (!endogenes.NullOrEmpty())
			// {
				// foreach (GeneDef gene in endogenes)
				// {
					// descriptionHyperlinks.Add(new DefHyperlink(gene));
				// }
			// }
			// if (mainXenotypeDef != null && !mainXenotypeDef.genes.NullOrEmpty())
			// {
				// foreach (GeneDef gene in mainXenotypeDef.genes)
				// {
					// if (!removeGenes.Contains(gene))
					// {
						// descriptionHyperlinks.Add(new DefHyperlink(gene));
					// }
				// }
			// }
		// }

	// }

	public class RandomGenes
	{
		public bool inheritable = false;
		public List<GeneDef> genes = new();
	}

	public class SubXenotypeDef : XenotypeDef
	{

		// public new List<GeneDef> genes = new() { WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter };

		public List<GeneDef> endogenes = new();

		public List<GeneDef> removeGenes = new();

		public float selectionWeight = 1f;

		// public XenotypeDef mainXenotypeDef;

		// public XenotypeIconDef xenotypeIconDef = null;

		public List<RandomGenes> randomGenes = new();

		public override void ResolveReferences()
		{
			if (endogenes.NullOrEmpty() || genes.NullOrEmpty())
			{
				return;
			}
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
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
					if (!removeGenes.Contains(gene) && gene != WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter)
					{
						descriptionHyperlinks.Add(new DefHyperlink(gene));
					}
				}
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
			{
				yield return item;
			}
			if (!removeGenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RemovedGenes".Translate().CapitalizeFirst(), removeGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RemovedGenes_Desc".Translate() + "\n\n" + removeGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1020);
			}
			// if (!genes.NullOrEmpty())
			// {
				// yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Xenogenes".Translate().CapitalizeFirst(), genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Xenogenes_Desc".Translate() + "\n\n" + genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
			// }
			if (!endogenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Endogenes".Translate().CapitalizeFirst(), endogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Endogenes_Desc".Translate() + "\n\n" + endogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1010);
			}
			// if (!randomGenes.NullOrEmpty())
			// {
				// foreach (RandomGenes item in randomGenes)
				// {
					// yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RandomGenes".Translate().CapitalizeFirst(), item.genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RandomGenes_Desc".Translate() + "\n\n" + item.genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 980);
				// }
			// }
		}

		// public override IEnumerable<string> ConfigErrors()
		// {
			// foreach (string item in base.ConfigErrors())
			// {
				// yield return item;
			// }
			// GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
			// if (removeGenes.Contains(geneticShifter))
			// {
				// removeGenes.Remove(geneticShifter);
				// yield return defName + " contains " + geneticShifter.defName + " in removeGenes. Fixing..";
			// }
			// if (endogenes.Contains(geneticShifter))
			// {
				// endogenes.Remove(geneticShifter);
				// yield return defName + " contains " + geneticShifter.defName + " in endogenes. Fixing..";
			// }
			// if (genes.Contains(geneticShifter))
			// {
				// genes.Remove(geneticShifter);
				// yield return defName + " contains " + geneticShifter.defName + " in genes. Fixing..";
			// }
			// if (!genes.Contains(geneticShifter))
			// {
				// genes.Add(geneticShifter);
			// }
			// if (inheritable)
			// {
				// inheritable = false;
				// Log.Warning(defName + " is inheritable. Fixing..");
			// }
			// if (doubleXenotypeChances.NullOrEmpty() || doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) == 1f)
			// {
				// yield return defName + " has null doubleXenotypeChances. doubleXenotypeChances must contain at least one xenotype with a chance of 1.0";
			// }
		// }

	}
}
