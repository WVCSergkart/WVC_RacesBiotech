using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Energyshifter : Gene_Shapeshifter, IGeneChargeable
	{

		public XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;

		private List<IGeneDisconnectable> cachedSubGenes;
		public List<IGeneDisconnectable> SubGenes
		{
			get
			{
				if (cachedSubGenes == null)
				{
					List<IGeneDisconnectable> list = new();
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (gene is IGeneDisconnectable geneDisconnectable && def.IsGeneDefOfType(geneDisconnectable.MasterClass))
						{
							list.Add(geneDisconnectable);
						}
					}
					cachedSubGenes = list;
				}
				return cachedSubGenes;
			}
		}

		private bool disabled = false;
		public override bool Active
		{
			get
			{
				if (disabled)
				{
					return false;
				}
				return base.Active;
			}
		}

		public override float GeneticMatchOffset
		{
			get
			{
				Update(true);
				SimpleCurve geneticCurve = new()
				{
					new CurvePoint(0f, -100),
					new CurvePoint(0.2f, -100),
					new CurvePoint(0.5f, -70),
					new CurvePoint(0.7f, 0),
					new CurvePoint(1f, 100)
				};
				return geneticCurve.Evaluate(ShaperResource_Raw) * 0.01f;
				//if (ShaperResource_Raw >= 1f)
				//{
				//	return 1f;
				//}
				//return 0f;
			}
		}

		// In Dev?
		//private static bool? cachedLocalMode;
		public bool LocalMode
		{
			get
			{
				//if (cachedLocalMode == null)
				//{
				//	cachedLocalMode = MechanoidsUtility.CerebrexCoreDefeated;
				//}
				//return cachedLocalMode.Value;
				return MechanoidsUtility.CerebrexCoreDefeated;
			}
		}

		public override void PreShapeshift(bool genesRegrowing)
		{
			Update(false, 1f);
			base.PreShapeshift(genesRegrowing);
		}

		public override void UnlockGeneDef(GeneDef geneDef)
		{
			if (LocalMode)
			{
				base.UnlockGeneDef(geneDef);
			}
			else
			{
				GameComponent.UnlockGeneDef(geneDef);
			}
		}

		public override void UnlockXenotype(string xenotypeName)
		{
			if (LocalMode)
			{
				base.UnlockXenotype(xenotypeName);
			}
			else
			{
				GameComponent.UnlockXenotype(xenotypeName);
			}
		}

		public List<GeneDef> CollectedGenes
		{
			get
			{
				if (LocalMode)
				{
					if (collectedGeneDefs == null)
					{
						collectedGeneDefs = new();
					}
					return collectedGeneDefs;
				}
				if (collectedGeneDefs != null && GameComponent != null)
				{
					foreach (GeneDef val in collectedGeneDefs)
					{
						GameComponent.UnlockGeneDef(val);
					}
					collectedGeneDefs = null;
				}
				return GameComponent.UnlockedGeneDefs;
			}
		}

		public List<string> UnlockedXenotypes
		{
			get
			{
				if (LocalMode)
				{
					if (unlockedXenotypes == null)
					{
						unlockedXenotypes = new();
					}
					return unlockedXenotypes;
				}
				if (unlockedXenotypes != null && GameComponent != null)
				{
					foreach (string val in unlockedXenotypes)
					{
						GameComponent.UnlockXenotype(val);
					}
					unlockedXenotypes = null;
				}
				return GameComponent.UnlcokedXenotypes;
			}
		}

		public override List<XenotypeHolder> Xenotypes
		{
			get
			{
				return base.Xenotypes.Where(holder =>
				{
					bool isSpecifiedXenotype = Giver.xenotypeDefs != null && Giver.xenotypeDefs.Contains(holder.XenotypeDef_Safe.defName);
					//bool inAnyCategory = Giver.geneCategoryDefs != null && holder.genes.Any(def => Giver.geneCategoryDefs.Contains(def.displayCategory));
					bool isUnlocked = UnlockedXenotypes != null && UnlockedXenotypes.Contains(holder.Label);
					return holder.Baseliner || isUnlocked || isSpecifiedXenotype;
				}).ToList();
			}
		}

		//private static List<XenotypeHolder> cachedXenotypeHolders;
		//public List<XenotypeHolder> XenotypeHolders
		//{
		//	get
		//	{
		//		if (cachedXenotypeHolders == null)
		//		{
		//			cachedXenotypeHolders = base.Xenotypes;
		//		}
		//		return cachedXenotypeHolders;
		//	}
		//}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				Update(false, (TotalOffset / 60000) * 2500);
				if (pawn.IsNestedHashIntervalTick(2500, 15000))
				{
					TickSubGenes();
				}
			}
		}

		private void TickSubGenes()
		{
			foreach (IGeneDisconnectable geneDisconnectable in SubGenes)
			{
				if (!geneDisconnectable.Active)
				{
					continue;
				}
				// 15000 / 2500 = 6
				geneDisconnectable.TickMasterGene(6, 15000);
			}
		}

		private float? cachedResourceOffset;
		public float TotalOffset
		{
			get
			{
				if (cachedResourceOffset == null)
				{
					float newOffset = def.resourceLossPerDay + (TotalMetabolism * -0.01f) + (OverridenByMe * 0.01f);
					//float factor = 1f;
					foreach (IGeneDisconnectable geneDisconnectable in SubGenes)
					{
						if (geneDisconnectable.Active)
						{
							newOffset += geneDisconnectable.ResourceConsumption_Offset;
							//factor *= geneDisconnectable.ResourceConsumption_Factor;
						}
					}
					//newOffset *= factor;
					if (newOffset > 0)
					{
						newOffset *= GenesFactor;
					}
					else
					{
						newOffset /= GenesFactor;
					}
					cachedResourceOffset = newOffset;
				}
				return cachedResourceOffset.Value;
			}
		}

		public float GenesFactor
		{
			get
			{
				//float factor = 1f;
				//foreach (IGeneDisconnectable geneDisconnectable in gene_Energyshifter.SubGenes)
				//{
				//	if (geneDisconnectable.Active)
				//	{
				//		factor *= geneDisconnectable.ResourceConsumption_Factor;
				//	}
				//}
				SimpleCurve geneticCurve = new()
				{
					new CurvePoint(21, 0.9f),
					new CurvePoint(28, 1f),
					new CurvePoint(29, 1.01f),
					new CurvePoint(35, 1.07f),
					new CurvePoint(36, 1.20f),
					new CurvePoint(42, 2.00f),
					new CurvePoint(200, 5.00f)
					//new CurvePoint(100, 1.72f),
					//new CurvePoint(128, 2.00f)
				};
				float factor = geneticCurve.Evaluate(pawn.genes.Endogenes.Count(gene => !gene.IsMelanin()));
				//if (pawn.HasGenesRegrowing())
				//{
				//	factor *= 1.2f;
				//}
				return factor;
			}
		}

		public int OverridenByMe => pawn.genes.GenesListForReading.Count(gene => gene.overriddenByGene == this) * 5;
		public int TotalMetabolism => pawn.genes.GenesListForReading.Where(gene => !gene.Overridden).Sum(gene => gene.def.biostatMet);

		public void UpdateConsumption()
		{
			cachedResourceOffset = null;
		}

		public void Update(bool direct, float percentageOffset = 0.01f)
		{
			if (GeneResourceUtility.TryGetNeedFood(pawn, out Need_Food food))
			{
				if (!direct)
				{
					food.CurLevelPercentage -= percentageOffset;
				}
				geneticMaterial = food.CurLevelPercentage;
			}
			else
			{
				disabled = true;
			}
		}

		public override void UpdateCache()
		{
			base.UpdateCache();
			disabled = false;
			cachedSubGenes = null;
			UpdateConsumption();
		}

		public override bool TryOffsetResource(float count)
		{
			return false;
		}

		public void Notify_Charging(float chargePerTick, int tick, float factor)
		{
			Update(true);
		}

		public override void DoEffects()
		{
			if (pawn.SpawnedOrAnyParentSpawned)
			{
				EffectsUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
			}
		}

	}

}
