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
                else if (subXenotypeDef is DevXenotypeDef hybrid && hybrid.isHybrid && hybrid.isRandom)
                {
                    SetHybridGenes(pawn, hybrid);
                }
            }
			if (shapeShiftGene != null)
			{
				pawn.genes.RemoveGene(shapeShiftGene);
			}
		}

		//public static Dictionary<XenotypeDef, XenotypeDef> failedCombos = new();
		public static void SetHybridGenes(Pawn pawn, DevXenotypeDef xenotype)
		{
			int cycleTry = 0;
			while (cycleTry < 10)
			{
				XenotypeDef firstXenotype = xenotype.xenotypeDefs.RandomElement();
				if (xenotype.xenotypeDefs.TryRandomElement((xenos) => xenos != firstXenotype && xenos.inheritable == firstXenotype.inheritable, out var result))
				{
					if (TrySetHybridXenotype(pawn, xenotype, firstXenotype, result))
                    {
						//string xenos = "";
						//foreach (var item in failedCombos)
						//{
						//	xenos += item.Key + " ";
						//	xenos += item.Value + " ";
						//}
						//Log.Error("Failed xenotyps: " + xenos);
						return;
                    }
				}
				cycleTry++;
			}
			ReimplanterUtility.SetXenotype(pawn, ListsUtility.GetAllXenotypesExceptAndroids().RandomElement());
		}

        public static bool TrySetHybridXenotype(Pawn pawn, DevXenotypeDef devXenotypeDef, XenotypeDef firstXenotype, XenotypeDef secondXenotype)
		{
			//pawn.genes.GetFirstGeneOfType<Gene_HybridImplanter>()?.SetXenotypes(firstXenotype, secondXenotype);
			return TrySetHybridXenotype(pawn, devXenotypeDef.genes, firstXenotype.genes, secondXenotype.genes, devXenotypeDef.inheritable, devXenotypeDef.exceptedGenes);
		}

		public static bool TrySetHybridXenotype(Pawn caster, Pawn victim, List<Gene> ignoredGenes, bool inheritable)
		{
			return TrySetHybridXenotype(caster, ignoredGenes.ConvertToDefs(), XaG_GeneUtility.ConvertToDefs(caster.genes.GenesListForReading), XaG_GeneUtility.ConvertToDefs(victim.genes.GenesListForReading), inheritable, new());
		}

		private static bool TrySetHybridXenotype(Pawn pawn, List<GeneDef> mainXenotypeGenes, List<GeneDef> firstXenotypeGenes, List<GeneDef> secondXenotypeGenes, bool inheritable, List<GeneDef> exceptedGenes)
        {
            if (!TryGetHybridGenes(firstXenotypeGenes, secondXenotypeGenes, out List<GeneDef> allNewGenes, exceptedGenes))
            {
                return false;
            }
			if (!inheritable)
			{
				pawn.genes.Endogenes.RemoveAllGenes(mainXenotypeGenes);
			}
            pawn.genes.Xenogenes.RemoveAllGenes(mainXenotypeGenes);
            AddGenes(pawn, allNewGenes, inheritable, new());
			//List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			//foreach (Gene gene in genesListForReading.ToList())
			//{
			//	if (exceptedGenes.Contains(gene.def))
			//	{
			//		pawn.genes.RemoveGene(gene);
			//	}
			//}
            ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
            ReimplanterUtility.PostImplantDebug(pawn);
            return true;
        }

        public static bool TryGetHybridGenes(List<GeneDef> firstXenotypeGenes, List<GeneDef> secondXenotypeGenes, out List<GeneDef> allNewGenes, List<GeneDef> exceptedGenes)
        {
            allNewGenes = new();
			string phase = "start";
            try
            {
                List<GeneDef> allSubGenes = new();
                phase = "sort genes";
                // sort genes
                List<GeneDef> firstGenes = new();
                foreach (GeneDef item in firstXenotypeGenes)
                {
                    if (exceptedGenes.Contains(item))
                    {
                        continue;
                    }
                    if (item.prerequisite == null)
                    {
                        firstGenes.Add(item);
                    }
                    else
                    {
                        allSubGenes.Add(item);
                    }
                }
                List<GeneDef> secondGenes = new();
                foreach (GeneDef item in secondXenotypeGenes)
                {
                    if (exceptedGenes.Contains(item))
                    {
                        continue;
                    }
                    if (item.prerequisite == null)
                    {
                        secondGenes.Add(item);
                    }
                    else
                    {
                        allSubGenes.Add(item);
                    }
                }
                phase = "add main genes";
                // add genes
                int firstGenesCount = firstGenes.Count / 2;
                for (int i = 0; i < firstGenesCount; i++)
                {
                    if (!XaG_GeneUtility.ConflictWith(firstGenes[i], allNewGenes))
                    {
                        allNewGenes.Add(firstGenes[i]);
                    }
                }
                int secondGenesCount = secondGenes.Count / 2;
                for (int i = 0; i < secondGenesCount; i++)
                {
                    if (!XaG_GeneUtility.ConflictWith(secondGenes[i], allNewGenes))
                    {
                        allNewGenes.Add(secondGenes[i]);
                    }
                }
                phase = "add sub genes";
                // add sub-genes
                int maxGenesCount = firstGenesCount + secondGenesCount;
                phase = "count all tries";
                int tryReq = CountTries(allSubGenes);
                if (tryReq > 0)
                {
                    phase = "add sub genes";
                    for (int i = 0; i < tryReq; i++)
                    {
                        AddSubGenes(allNewGenes, allSubGenes, maxGenesCount);
                    }
                }
                SetSkinAndHairGenes(firstXenotypeGenes, secondXenotypeGenes, allNewGenes);
                XaG_GeneUtility.GetBiostatsFromList(allNewGenes, out _, out int met, out _);
                if (met > 5 || met < -5)
                {
                    return false;
                }
            }
            catch (Exception arg)
            {
				Log.Error("Failed create hybrid. On phase: " + phase + " Reason: " + arg);
				return false;
			}
			return !allNewGenes.NullOrEmpty();
        }

        private static void SetSkinAndHairGenes(List<GeneDef> firstXenotypeGenes, List<GeneDef> secondXenotypeGenes, List<GeneDef> allNewGenes)
		{
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			foreach (GeneDef item in allNewGenes)
			{
				ReimplanterUtility.IsSkinOrHairGene(ref xenotypeHasSkinColor, ref xenotypeHasHairColor, item);
			}
			if (!xenotypeHasSkinColor)
            {
				List<GeneDef> allGenes = new();
				allGenes.AddRange(firstXenotypeGenes.Where((geneDef) => XaG_GeneUtility.IsSkinGeneDef(geneDef)));
				allGenes.AddRange(secondXenotypeGenes.Where((geneDef) => XaG_GeneUtility.IsSkinGeneDef(geneDef)));
				if (!allGenes.Empty())
				{
					allNewGenes.Add(allGenes.First());
				}
				else
				{
					allNewGenes.Add(WVC_GenesDefOf.Skin_SheerWhite);
				}
			}
            if (!xenotypeHasHairColor)
			{
				List<GeneDef> allGenes = new();
				allGenes.AddRange(firstXenotypeGenes.Where((geneDef) => XaG_GeneUtility.IsHairGeneDef(geneDef)));
				allGenes.AddRange(secondXenotypeGenes.Where((geneDef) => XaG_GeneUtility.IsHairGeneDef(geneDef)));
				if (!allGenes.Empty())
				{
					allNewGenes.Add(allGenes.First());
				}
				else
				{
					allNewGenes.Add(WVC_GenesDefOf.Hair_SnowWhite);
				}
			}
        }

        private static int CountTries(List<GeneDef> allSubGenes)
        {
            int tryReq = 1;
            foreach (GeneDef subGene in allSubGenes)
            {
				//if (subGene.prerequisite?.prerequisite != null)
				//{
				//    tryReq++;
				//}
				XaG_GeneUtility.GeneDefHasSubGenes_WithCount(subGene.prerequisite, ref tryReq);
			}
			return tryReq;
        }

        private static void AddSubGenes(List<GeneDef> allNewGenes, List<GeneDef> allSubGenes, int maxGenesCount)
		{
            int maxGenes = maxGenesCount - allNewGenes.Count;
			if (maxGenes < 1)
            {
				return;
            }
			int breaker = 0;
   //         for (int i = 0; i < maxGenes; i++)
			//{
			//	if (breaker > maxGenesCount)
			//	{
			//		break;
			//	}
			//	breaker++;
			//	if (!XaG_GeneUtility.ConflictWith(allSubGenes[i], allNewGenes) && allNewGenes.Contains(allSubGenes[i].prerequisite))
			//	{
			//		allNewGenes.Add(allSubGenes[i]);
			//	}
			//	else
   //             {
			//		maxGenes++;
			//	}
			//}
            foreach (GeneDef item in allSubGenes)
            {
                if (!XaG_GeneUtility.ConflictWith(item, allNewGenes) && allNewGenes.Contains(item.prerequisite))
                {
                    allNewGenes.Add(item);
                }
				if (breaker > maxGenes)
                {
					break;
                }
				breaker++;
			}
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
			List<GeneDef> genesListForReading = XaG_GeneUtility.ConvertToDefs(pawn.genes.GenesListForReading);
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
