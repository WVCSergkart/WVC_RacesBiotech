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
		public bool shouldAddMainGenes = true;

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
				XenotypeExtension_SubXenotype modExtension = Xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
				if (modExtension != null)
				{
					if (Rand.Chance(modExtension.subXenotypeChance))
					{
						SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
						RandomXenotype(pawn, subXenotypeDef, Xenotype);
					}
					if (modExtension.mainSubXenotypeDef != null && shouldAddMainGenes)
					{
						SubXenotypeDef subXenotypeDef = modExtension.mainSubXenotypeDef;
						MainXenotype(pawn, subXenotypeDef, Xenotype);
					}
				}
			}
			if (this != null)
			{
				pawn.genes.RemoveGene(this);
			}
		}

		public void RandomXenotype(Pawn pawn, SubXenotypeDef xenotype, XenotypeDef mainXenotype)
		{
			shouldAddMainGenes = false;
			if (xenotype.removeGenes != null)
			{
				RemoveGenes(pawn, xenotype);
			}
			if (xenotype.mainGenes != null)
			{
				MainXenotype(pawn, xenotype, mainXenotype);
			}
			Pawn_GeneTracker genes = pawn.genes;
			List<GeneDef> geneDefs = xenotype.genes;
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
			if (xenotype.xenotypeIconDef != null)
			{
				pawn.genes.iconDef = xenotype.xenotypeIconDef;
			}
		}

		public void MainXenotype(Pawn pawn, SubXenotypeDef xenotype, XenotypeDef mainXenotype)
		{
			List<GeneDef> geneDefs = xenotype.mainGenes;
			for (int i = 0; i < geneDefs.Count; i++)
			{
				pawn.genes?.AddGene(geneDefs[i], !mainXenotype.inheritable);
			}
		}

		public void RemoveGenes(Pawn pawn, SubXenotypeDef xenotype)
		{
			List<GeneDef> geneDefs = xenotype.removeGenes;
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (geneDefs.Contains(genesListForReading[i].def))
				{
					pawn.genes?.RemoveGene(genesListForReading[i]);
				}
			}
		}

	}

}
