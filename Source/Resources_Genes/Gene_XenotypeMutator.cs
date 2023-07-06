using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenotypeShapeshifter : Gene
	{

		public bool geneIsXenogene = false;

		public XenotypeDef Xenotype => pawn.genes?.Xenotype;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.genes != null)
			{
				geneIsXenogene = pawn.genes.IsXenogene(this);
			}
			if (Xenotype != null)
			{
				GeneExtension_Giver modExtension = Xenotype.GetModExtension<GeneExtension_Giver>();
				if (modExtension != null)
				{
					if (Rand.Chance(modExtension.subXenotypeChance))
					{
						SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
						RandomXenotype(pawn, subXenotypeDef);
					}
				}
			}
			if (this != null)
			{
				pawn.genes.RemoveGene(this);
			}
		}

		public void RandomXenotype(Pawn pawn, SubXenotypeDef xenotype)
		{
			Pawn_GeneTracker genes = pawn.genes;
			List<GeneDef> geneDefs = xenotype.AllGenes;
			if (xenotype.overrideExistingGenes)
			{
				if (!xenotype.inheritable)
				{
					for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
					{
						pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
					}
				}
				if (xenotype.inheritable)
				{
					for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
					{
						pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
					}
				}
			}
			if ((xenotype.inheritable && genes.Endogenes.Count <= 0) || (!xenotype.inheritable && genes.Xenogenes.Count <= 0) || xenotype.ignoreExistingGenes)
			{
				for (int i = 0; i < geneDefs.Count; i++)
				{
					pawn.genes?.AddGene(geneDefs[i], !xenotype.inheritable);
				}
			}
		}

	}

}
