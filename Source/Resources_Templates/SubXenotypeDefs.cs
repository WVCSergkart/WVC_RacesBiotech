using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace WVC_XenotypesAndGenes
{

	// public class EndotypeDef : XenotypeDef
	// {
	// }

	// public class RandomGenes
	// {
		// public bool inheritable = false;
		// public List<GeneDef> genes = new();
	// }

	public class SubXenotypeDef : Def
	{

		public List<GeneDef> endogenes = new();

		public List<GeneDef> xenogenes = new();

		public List<GeneDef> removeGenes = new();

		public float selectionWeight = 1f;

		// public XenotypeIconDef xenotypeIconDef = null;

		// public List<RandomGenes> randomGenes = new();

		public override void ResolveReferences()
		{
			if (endogenes.NullOrEmpty() || xenogenes.NullOrEmpty())
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
			if (!xenogenes.NullOrEmpty())
			{
				foreach (GeneDef gene in xenogenes)
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
			if (!endogenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Endogenes".Translate().CapitalizeFirst(), endogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Endogenes_Desc".Translate() + "\n\n" + endogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1010);
			}
			if (!xenogenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Xenogenes".Translate().CapitalizeFirst(), xenogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Xenogenes_Desc".Translate() + "\n\n" + xenogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
			}
			if (!removeGenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RemovedGenes".Translate().CapitalizeFirst(), removeGenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RemovedGenes_Desc".Translate() + "\n\n" + removeGenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 990);
			}
			// if (!randomGenes.NullOrEmpty())
			// {
				// foreach (RandomGenes item in randomGenes)
				// {
					// yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_RandomGenes".Translate().CapitalizeFirst(), item.genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "WVC_XaG_SubXeno_RandomGenes_Desc".Translate() + "\n\n" + item.genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 980);
				// }
			// }
		}
	}
}
