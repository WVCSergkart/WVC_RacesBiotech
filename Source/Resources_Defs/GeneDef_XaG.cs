using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class GeneDef_XaG : GeneDef
	{

		public List<GeneralHolder> genesTrans;

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats(req))
			{
				yield return statDrawEntry;
			}
			if (genesTrans != null)
			{
				foreach (GeneralHolder holder in genesTrans)
				{
					yield return new StatDrawEntry(StatCategoryDefOf.Genetics, holder.geneDef.LabelCap, holder.genes.Count.ToString(), "WVC_GeneticTransform_Desc".Translate(), 910, hyperlinks: holder.genes.Select((gene) => new Dialog_InfoCard.Hyperlink(gene)));
				}
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (genesTrans != null)
			{
				if (customEffectDescriptions == null)
				{
					customEffectDescriptions = new();
				}
				List<GeneDef> genesList = new();
				foreach (GeneralHolder generalHolder in genesTrans)
				{
					genesList.Add(generalHolder.geneDef);
				}
				customEffectDescriptions.Add("WVC_GeneticTransform_GenesList".Translate(genesList.Select(gene => gene.label).ToCommaList(useAnd: true)));
			}
		}

	}

}
