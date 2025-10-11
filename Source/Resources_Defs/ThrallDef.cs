using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class ThrallDef : Def
	{

		public XenotypeIconDef xenotypeIconDef;

		public List<GeneDef> genes;

		public List<GeneDef> blacklistGenes;

		public List<GeneCategoryDef> blacklistCategories;

		public GeneDef reqGeneDef;

		public MutantDef mutantDef = null;

		//public bool isBasic = false;

		public List<MutantByRotStage> mutantByRotStage;

		public class MutantByRotStage
        {

            public MutantDef mutantDef;

            public RotStage rotStage;

			public List<GeneDef> genes;

		}

		public MutantDef GetMutantFromStage(RotStage rotStage)
        {
			if (mutantByRotStage == null)
            {
				return null;
            }
			foreach (MutantByRotStage item in mutantByRotStage)
            {
				if (rotStage == item.rotStage)
                {
					return item.mutantDef;
				}
            }
			return null;
		}

		public List<GeneDef> GetGenesFromStage(RotStage rotStage)
		{
			if (mutantByRotStage == null)
			{
				return null;
			}
			foreach (MutantByRotStage item in mutantByRotStage)
			{
				if (rotStage == item.rotStage && !item.genes.NullOrEmpty())
				{
					return item.genes;
				}
			}
			return null;
		}

		public bool addGenesFromAbility = true;
		public bool addGenesFromMaster = true;

		public bool resurrectAsShambler = false;

		//public bool isOverlordMutant = false;

		public XenotypeDef xenotypeDef;

		public List<RotStage> acceptableRotStages = new() { RotStage.Fresh, RotStage.Rotting };

		public float selectionWeight = 1f;

		private string cachedDescription = null;
		public string Description
        {
			get
            {
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					if (mutantDef != null)
					{
						stringBuilder.AppendLine("WVC_Mutation".Translate(mutantDef.LabelCap).Colorize(ColoredText.TipSectionTitleColor) + ": " + mutantDef.description);
					}
					else
					{
						stringBuilder.AppendLine(description);
					}
					if (xenotypeDef != null && !xenotypeDef.descriptionShort.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(xenotypeDef.descriptionShort);
					}
					if (reqGeneDef != null)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Requires".Translate() + ": " + reqGeneDef.LabelCap);
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_XaG_AcceptableRotStages".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + acceptableRotStages.Select((RotStage x) => x.ToStringHuman()).ToLineList(" - "));
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
        }
        //[Obsolete]
        //public string generalDesc;

        //public override void ResolveReferences()
        //{
        //	base.ResolveReferences();
        //	if (genes.NullOrEmpty())
        //	{
        //		return;
        //	}
        //	if (descriptionHyperlinks == null)
        //	{
        //		descriptionHyperlinks = new List<DefHyperlink>();
        //	}
        //	foreach (GeneDef gene in genes)
        //	{
        //		descriptionHyperlinks.Add(new DefHyperlink(gene));
        //	}
        //}

        //public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
        //{
        //	foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
        //	{
        //		yield return item;
        //	}
        //	yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Genes".Translate().CapitalizeFirst(), genes.Select((GeneDef x) => x.label).ToCommaList().CapitalizeFirst(), "GenesDesc".Translate() + "\n\n" + genes.Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true), 1000);
        //}

    }

}
