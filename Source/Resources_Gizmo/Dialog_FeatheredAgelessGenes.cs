using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	//public class Dialog_FeatheredAgelessGenes : Dialog_EditShiftGenes
	//{

	//	private Gene_FeatheredAgeless gene_FeatheredAgeless;

	//	public Dialog_FeatheredAgelessGenes(Gene pawnGene) : base(pawnGene)
	//	{
	//		geneMatStats = new GeneMatStatData[2]
	//		{
	//			new GeneMatStatData("WVC_XaG_GeneticMaterial_FeatherAge", "WVC_XaG_GeneticMaterial_FeatherAgeDesc", ReqTex.Texture),
	//			new GeneMatStatData("WVC_XaG_GeneticMaterial_Genes", "WVC_XaG_GeneticMaterial_FeatherGenesDesc", HasTex.Texture),
	//		};
	//	}

	//	public override CachedTexture HasTex => XaG_UiUtility.FeatheredGenMatHasTex;
	//	public override CachedTexture ReqTex => XaG_UiUtility.FeatheredGenMatReqTex;

	//	//public override int ReqGeneMat => base.ReqGeneMat;

	//	public int CostFactor => 4;
	//	private int GetGeneCost(GeneDefWithChance newGene)
	//	{
	//		return newGene.Cost * CostFactor;
	//	}

	//	private static List<Pawn> Colonists => PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists;

	//	private int? cachedGeneMat;
	//	public override int AllGeneMat
	//	{
	//		get
	//		{
	//			if (cachedGeneMat == null)
	//			{
	//				try
	//				{
	//					SimpleCurve curve = new()
	//					{
	//						new CurvePoint(1, 1),
	//						new CurvePoint(100, 60),
	//						new CurvePoint(200, 70),
	//						new CurvePoint(400, 80),
	//						new CurvePoint(1000, 120)
	//					};
	//					cachedGeneMat = Gene_FeatheredAgeless.collectedYears + Colonists.Where(pawn => pawn.genes?.GetFirstGeneOfType<Gene_FeatheredAgeless>() != null).Sum(pawn => (int)curve.Evaluate(pawn.ageTracker.AgeChronologicalYears) + pawn.ageTracker.AgeBiologicalYears);
	//				}
	//				catch (Exception arg)
	//				{
	//					Log.Error("Failed count all player pawn summary years. Reason: " + arg.Message);
	//					cachedGeneMat = 0;
	//				}
	//			}
	//			return cachedGeneMat.Value;
	//		}
	//	}

	//	protected override void SwitchButton(Rect rect3)
	//	{

	//	}

	//	public override void OnGenesChanged()
	//	{
	//		cachedReqGeneMat = null;
	//		base.OnGenesChanged();
	//	}

	//	private int? cachedReqGeneMat;
	//	public override int ReqGeneMat
	//	{
	//		get
	//		{
	//			if (cachedReqGeneMat == null)
	//			{
	//				int value = base.ReqGeneMat;
	//				foreach (Pawn pawn in Colonists)
	//				{
	//					if (pawn.genes?.GetFirstGeneOfType<Gene_FeatheredAgeless>() == null)
	//					{
	//						continue;
	//					}
	//					foreach (Gene gene in pawn.genes.GenesListForReading)
	//					{
	//						if (gene_FeatheredAgeless.Undead.geneDefs.Contains(gene.def))
	//						{
	//							GeneDefWithChance newGene = new()
	//							{
	//								geneDef = gene.def
	//							};
	//							value += GetGeneCost(newGene);
	//						}
	//					}
	//				}
	//				cachedReqGeneMat = value;
	//			}
	//			return cachedReqGeneMat.Value;
	//		}
	//	}

	//	public override void Accept()
	//	{
	//		Log.Error("Accpeted");
	//	}

	//	protected override void SetupAvailableGenes(Gene gene)
	//	{
	//		if (gene is Gene_FeatheredAgeless ageless)
	//		{
	//			this.gene_FeatheredAgeless = ageless;
	//		}
	//		else
	//		{
	//			return;
	//		}
	//		foreach (GeneDef item in gene_FeatheredAgeless.Undead.geneDefs)
	//		{
	//			if (item.prerequisite != null && !XaG_GeneUtility.HasActiveGene(item.prerequisite, gene.pawn))
	//			{
	//				continue;
	//			}
	//			GeneDefWithChance geneDefWithChance = new();
	//			geneDefWithChance.geneDef = item;
	//			geneDefWithChance.disabled = pawnGenes.Contains(item);
	//			//GeneExtension_Undead geneExtension_Undead = item.GetModExtension<GeneExtension_Undead>();
	//			if (item.biostatArc == 0)
	//			{
	//				geneDefWithChance.displayCategory = GeneCategoryDefOf.Miscellaneous;
	//			}
	//			else
	//			{
	//				geneDefWithChance.displayCategory = GeneCategoryDefOf.Archite;
	//			}
	//			geneDefWithChance.Cost = GetGeneCost(geneDefWithChance);
	//			if (DebugSettings.ShowDevGizmos)
	//			{
	//				geneDefWithChance.Cost = 0;
	//			}
	//			allGenes.Add(geneDefWithChance);
	//		}
	//	}

	//}

}
