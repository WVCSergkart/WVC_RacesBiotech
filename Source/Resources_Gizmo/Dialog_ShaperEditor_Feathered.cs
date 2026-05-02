using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Dialog_ShaperEditor_Feathered : Dialog_ShaperEditor
	{

		private Gene_FeatheredAgeless gene_FeatheredAgeless;
		private bool isArchite;

		public Dialog_ShaperEditor_Feathered(IGeneShaperEditor pawnGene) : base(pawnGene)
		{
			Gene_FeatheredAgeless.ResetCache();
			//geneMatStats = new GeneMatStatData[2]
			//{
			//	new GeneMatStatData("WVC_XaG_GeneticMaterial_FeatherAge", "WVC_XaG_GeneticMaterial_FeatherAgeDesc", HasTex.Texture),
			//	new GeneMatStatData("WVC_XaG_GeneticMaterial_FeatherReq", "WVC_XaG_GeneticMaterial_FeatherReqDesc", ReqTex.Texture),
			//};
		}

		//public override CachedTexture HasTex => XaG_UiUtility.FeatheredGenMatHasTex;
		//public override CachedTexture ReqTex => XaG_UiUtility.FeatheredGenMatReqTex;

		//public override int ReqGeneMat => base.ReqGeneMat;

		//public override void DrawBiostats(int geneMat, ref float curX, float curY, float margin = 6)
		//{
		//	float num2 = 0f;
		//	float num3 = Text.LineHeightOf(GameFont.Small);
		//	Rect iconRect = new(curX, curY + margin + num2, num3, num3);
		//	if (geneMat > 0)
		//	{
		//		XaG_UiUtility.DrawStat(iconRect, ReqTex, geneMat.ToString(), num3);
		//	}
		//	curX += 34f;
		//}

		public float CostFactor => WVC_Biotech.settings.featheredAgeless_GenesCostFactor;
		private int GetGeneCost(GeneDefWithChance newGene)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return 0;
			}
			return (int)(newGene.Cost * CostFactor) + 7;
		}

		//private int? cachedGeneMat;
		//public override int AllGeneMat
		//{
		//	get
		//	{
		//		if (cachedGeneMat == null)
		//		{
		//			try
		//			{
		//				SimpleCurve curveChrono = new()
		//				{
		//					new CurvePoint(1, 1),
		//					new CurvePoint(100, 40),
		//					new CurvePoint(200, 45),
		//					new CurvePoint(400, 50),
		//					new CurvePoint(1000, 70)
		//				};
		//				SimpleCurve curveBio = new()
		//				{
		//					new CurvePoint(1, 1),
		//					new CurvePoint(18, 40),
		//					new CurvePoint(20, 20),
		//					new CurvePoint(100, 40)
		//				};
		//				cachedGeneMat = Colonists.Where(pawn => pawn.genes?.GetFirstGeneOfType<Gene_FeatheredAgeless>() != null).Sum(pawn => (int)curveChrono.Evaluate(pawn.ageTracker.AgeChronologicalYears) + (int)curveBio.Evaluate(pawn.ageTracker.AgeBiologicalYears));
		//			}
		//			catch (Exception arg)
		//			{
		//				Log.Error("Failed count all player pawn summary years. Reason: " + arg.Message);
		//				cachedGeneMat = 0;
		//			}
		//		}
		//		return cachedGeneMat.Value;
		//	}
		//}
		//public override int AllGeneMat => gene_FeatheredAgeless.GeneticMaterial;

		protected override void SwitchButton(Rect rect3)
		{

		}

		public override void OnGenesChanged()
		{
			cachedReqGeneMat = null;
			base.OnGenesChanged();
		}

		private int? cachedReqGeneMat;
		public override int ReqGeneMat
		{
			get
			{
				if (cachedReqGeneMat == null)
				{
					int value = base.ReqGeneMat;
					foreach (Pawn pawn in Gene_FeatheredAgeless.Colonists)
					{
						if (pawn.genes?.GetFirstGeneOfType<Gene_FeatheredAgeless>() == null)
						{
							continue;
						}
						foreach (Gene gene in pawn.genes.GenesListForReading)
						{
							if (gene_FeatheredAgeless.Undead.geneDefs.Contains(gene.def))
							{
								GeneDefWithChance newGene = new()
								{
									geneDef = gene.def
								};
								value += GetGeneCost(newGene);
							}
						}
					}
					cachedReqGeneMat = value;
				}
				return cachedReqGeneMat.Value;
			}
		}

		public override bool CanAccept(bool throwMessage = false)
		{
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.Pawn, throwMessage))
			{
				return false;
			}
			if (SelectedGenes.Empty())
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_NullGeneSet".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (SelectedGenes.Any((geneChance) => geneChance.disabled))
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_DialogFeatheredAgelessGenes_Disabled".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (AllGeneMat < ReqGeneMat)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_DialogEditShiftGenes_NeedMoreAmount".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			List<GeneDefWithChance> selectedGenes = SelectedGenes;
			foreach (GeneDefWithChance selectedGene in SelectedGenes)
			{
				if (selectedGene.geneDef.prerequisite != null && !Contains(selectedGenes, selectedGene.geneDef.prerequisite) && !pawnGenes.Contains(selectedGene.geneDef.prerequisite))
				{
					if (throwMessage)
					{
						Messages.Message("MessageGeneMissingPrerequisite".Translate(selectedGene.geneDef.label).CapitalizeFirst() + ": " + selectedGene.geneDef.prerequisite.LabelCap, null, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			return true;
		}

		//public override void Accept()
		//{
		//	foreach (GeneDefWithChance geneDefWithChance in selectedGenes)
		//	{
		//		if (geneDefWithChance.disabled)
		//		{
		//			continue;
		//		}
		//		if (XaG_GeneUtility.TryRemoveAllConflicts(gene.pawn, geneDefWithChance.geneDef))
		//		{
		//			gene.pawn.genes.AddGene(geneDefWithChance.geneDef, !inheritable);
		//		}
		//	}
		//	ReimplanterUtility.PostImplantDebug(gene.pawn);
		//	Close();
		//}

		protected override void SetupAvailableGenes(IGeneShaperEditor gene)
		{
			if (gene is Gene_FeatheredAgeless ageless)
			{
				this.gene_FeatheredAgeless = ageless;
			}
			else
			{
				return;
			}
			isArchite = pawnGenes.Any(def => def.biostatArc != 0);
			foreach (GeneDef item in gene_FeatheredAgeless.Undead.geneDefs)
			{
				if (!isArchite && item.biostatArc != 0)
				{
					continue;
				}
				if (item.prerequisite != null && !XaG_GeneUtility.HasActiveGene(item.prerequisite, gene.Pawn))
				{
					continue;
				}
				GeneDefWithChance geneDefWithChance = new();
				geneDefWithChance.geneDef = item;
				geneDefWithChance.disabled = pawnGenes.Contains(item);
				geneDefWithChance.displayCategory = item.displayCategory;
				geneDefWithChance.Cost = GetGeneCost(geneDefWithChance);
				allGenes.Add(geneDefWithChance);
			}
		}

	}

}
