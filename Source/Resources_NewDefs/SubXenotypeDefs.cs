using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    [Obsolete]
	public class ThralltypeDef : DevXenotypeDef
	{

	}

	[Obsolete]
	public class SubXenotypeDef : XenotypeDef
	{

		public List<GeneDef> endogenes = new();

		public List<GeneDef> removeGenes = new();

		public float selectionWeight = 1f;

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
            GeneDef geneticShifter = MainDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
            SubXenotype(geneticShifter);
            genes.Add(geneticShifter);
        }

        private void SubXenotype(GeneDef geneticShifter)
        {
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
			if (!endogenes.NullOrEmpty())
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "WVC_XaG_SubXeno_Endogenes".Translate().CapitalizeFirst(), endogenes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + "WVC_XaG_SubXeno_Endogenes_Desc".Translate() + "\n\n" + endogenes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1030);
			}
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
