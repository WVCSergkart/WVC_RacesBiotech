using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class SubXenotypeUtility
	{

		// ========================================================

		public static void XenotypeShapeShift(Pawn pawn, Gene shapeShiftGene)
		{
			if (TryGetXenotypeDef(pawn, out XenotypeDef subXenotypeDef))
			{
				//if (subXenotypeDef is SubXenotypeDef sub && !sub.doubleXenotypeChances.NullOrEmpty())
				//{
				//	SetXenotypeGenes(pawn, sub, xenogermReplicationChance);
				//}
				if (subXenotypeDef is DevXenotypeDef thrall && thrall.isThrall)
				{
					SetThrallGenes(pawn, thrall);
				}
				else if (subXenotypeDef is DevXenotypeDef hybrid && hybrid.isHybrid)
				{
					SetHybridGenes(pawn, hybrid);
				}
			}
			if (shapeShiftGene != null)
			{
				pawn.genes.RemoveGene(shapeShiftGene);
			}
		}

		public static void SetHybridGenes(Pawn pawn, DevXenotypeDef xenotype)
		{
			XenotypeDef firstXenotype = xenotype.xenotypeDefs.RandomElement();
			if (xenotype.xenotypeDefs.TryRandomElement((xenos) => xenos != firstXenotype && xenos.inheritable == firstXenotype.inheritable, out var result))
            {
                SetHybridXenotype(pawn, xenotype, firstXenotype, result);
            }
            else
            {
				ReimplanterUtility.SetXenotype(pawn, ListsUtility.GetAllXenotypesExceptAndroids().RandomElement());
            }
		}

        private static void SetHybridXenotype(Pawn pawn, DevXenotypeDef devXenotypeDef, XenotypeDef firstXenotype, XenotypeDef secondXenotype)
        {
            pawn.genes.Endogenes.RemoveAllGenes();
            pawn.genes.Xenogenes.RemoveAllGenes();
            AddGenes(pawn, firstXenotype.genes, firstXenotype.inheritable, new());
            AddGenes(pawn, secondXenotype.genes, firstXenotype.inheritable, new());
			DuplicateUtility.RemoveAllGenes_Overridden(pawn);
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			foreach (Gene gene in genesListForReading.ToList())
            {
				if (devXenotypeDef.exceptedGenes.Contains(gene.def))
                {
					pawn.genes.RemoveGene(gene);
                }
            }
            int limit = (int)(firstXenotype.genes.Count * 0.5f) + (int)(secondXenotype.genes.Count * 0.5f);
            if (genesListForReading.Count > limit)
            {
                genesListForReading.Shuffle();
                for (int i = 0; i < genesListForReading.Count - limit; i++)
                {
                    pawn.genes?.RemoveGene(genesListForReading[i]);
                }
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
			ReimplanterUtility.PostImplantDebug(pawn);
        }

        public static void SetThrallGenes(Pawn pawn, DevXenotypeDef xenotype)
		{
			if (xenotype.thrallDefs.Where((ThrallDef x) => x.mutantDef == null).TryRandomElementByWeight((ThrallDef x) => x.selectionWeight, out var result))
			{
				CompAbilityEffect_ReimplanterThrallMaker.ThrallMaker(pawn, result);
			}
			AddGenes(pawn, xenotype.guaranteedGenes, true, new());
		}

		[Obsolete]
		public static void SetXenotypeGenes(Pawn pawn, SubXenotypeDef xenotype, float xenogermReplicationChance)
		{
			if (!xenotype.removeGenes.NullOrEmpty())
			{
				RemoveGenes(pawn, xenotype);
			}
			List<GeneDef> endogenes = GetEndogenesFromXenotypes(xenotype);
			if (!endogenes.NullOrEmpty())
			{
				AddGenes(pawn, endogenes, true, xenotype.removeGenes);
			}
			RemoveAllUnnecessaryGenes(pawn, endogenes, xenotype.genes);
			if (Rand.Chance(xenogermReplicationChance))
			{
				GeneUtility.UpdateXenogermReplication(pawn);
			}
		}

		// ========================================================

		public static void RemoveAllUnnecessaryGenes(Pawn pawn, List<GeneDef> endogenes, List<GeneDef> xenogenes)
		{
			List<GeneDef> xenoTypeGenes = new();
			xenoTypeGenes.AddRange(endogenes);
			xenoTypeGenes.AddRange(xenogenes);
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (!xenoTypeGenes.Contains(genesListForReading[i].def))
				{
					pawn.genes?.RemoveGene(genesListForReading[i]);
				}
			}
		}

		[Obsolete]
		public static List<GeneDef> GetEndogenesFromXenotypes(SubXenotypeDef xenotype)
		{
			List<GeneDef> list = new();
			XenotypeDef xenotypeDef = xenotype.doubleXenotypeChances.RandomElementByWeight((XenotypeChance x) => x.chance).xenotype;
			foreach (GeneDef item in xenotypeDef.genes)
			{
				if (!xenotype.removeGenes.Contains(item))
				{
					list.Add(item);
				}
			}
			foreach (GeneDef item in xenotype.endogenes)
			{
				if (!xenotype.removeGenes.Contains(item) && !list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static void AddGenes(Pawn pawn, List<GeneDef> genes, bool inheritable, List<GeneDef> removeGenes)
		{
			List<GeneDef> genesListForReading = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading);
			for (int i = 0; i < genes.Count; i++)
			{
				if (!removeGenes.Contains(genes[i]) && !genesListForReading.Contains(genes[i]))
				{
					pawn.genes?.AddGene(genes[i], !inheritable);
				}
			}
		}

		[Obsolete]
		public static void RemoveGenes(Pawn pawn, SubXenotypeDef xenotype)
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

		// ========================================================

		public static bool TryGetXenotypeDef(Pawn pawn, out XenotypeDef subXenotypeDef)
		{
			subXenotypeDef = null;
			Pawn_GeneTracker genes = pawn?.genes;
			if (genes == null || genes.CustomXenotype != null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			return (subXenotypeDef = genes.Xenotype) != null;
		}

	}
}
