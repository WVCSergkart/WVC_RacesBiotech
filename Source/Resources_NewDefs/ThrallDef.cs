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

		public GeneDef reqGeneDef;

		public MutantDef mutantDef = null;

		public List<MutantByRotStage> mutantByRotStage;

		public class MutantByRotStage
        {
            public MutantDef mutantDef;

            public RotStage rotStage;

        }

		public MutantDef GetMutantFromStage(RotStage rotStage)
        {
			foreach (MutantByRotStage item in mutantByRotStage)
            {
				if (rotStage == item.rotStage)
                {
					return item.mutantDef;
				}
            }
			return null;
        }

		public bool addGenesFromAbility = true;
		public bool addGenesFromMaster = true;

		public bool resurrectAsShambler = false;

		public bool isOverlordMutant = false;

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

}
