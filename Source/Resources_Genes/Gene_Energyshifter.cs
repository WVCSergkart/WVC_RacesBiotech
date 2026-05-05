using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Energyshifter : Gene_Shapeshifter, IGeneChargeable
	{

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
				if (ShaperResource_Raw >= 1f)
				{
					return 1f;
				}
				return 0f;
			}
		}

		public override void PreShapeshift(bool genesRegrowing)
		{
			Update(false, 1f);
			base.PreShapeshift(genesRegrowing);
		}
		public override List<XenotypeHolder> Xenotypes
		{
			get
			{
				return base.Xenotypes.Where(holder =>
				{
					bool isSpecifiedXenotype = Giver.xenotypeDefs != null && Giver.xenotypeDefs.Contains(holder.XenotypeDef_Safe.defName);
					bool inAnyCategory = Giver.geneCategoryDefs != null && holder.genes.Any(def => Giver.geneCategoryDefs.Contains(def.displayCategory));
					bool isUnlocked = unlockedXenotypes != null && unlockedXenotypes.Contains(holder.Label);
					return holder.Baseliner || inAnyCategory || isUnlocked || isSpecifiedXenotype;
				}).ToList();
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				Update(false, (Consumption / 60000) * 2500);
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
				// 15000 / 2500 = 6
				geneDisconnectable.TickMasterGene(6);
			}
		}

		private float? cachedConsumption;
		public float Consumption
		{
			get
			{
				if (cachedConsumption == null)
				{
					float newConsumption = 0.1f;
					foreach (IGeneDisconnectable geneDisconnectable in SubGenes)
					{
						if (geneDisconnectable.Active)
						{
							newConsumption += geneDisconnectable.ResourceConsumption;
						}
					}
					cachedConsumption = newConsumption;
				}
				return cachedConsumption.Value;
			}
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
			cachedConsumption = null;
		}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	UpdateCache();
		//	base.Notify_OverriddenBy(overriddenBy);
		//}

		//public override void Notify_Override()
		//{
		//	UpdateCache();
		//	base.Notify_Override();
		//}

		//public override void Notify_GenesRecache(Gene changedGene)
		//{
		//	UpdateCache();
		//	base.Notify_GenesRecache(changedGene);
		//}

		//public override void PostShapeshift(bool genesRegrowing)
		//{
		//	base.PostShapeshift(genesRegrowing);
		//	UpdateCache();
		//}

		public override void CopyFrom(Gene_Shapeshifter oldShapeshifter)
		{
			if (oldShapeshifter is Gene_Energyshifter gene_Fleshshaper)
			{
				this.unlockedXenotypes = gene_Fleshshaper.unlockedXenotypes;
			}
			base.CopyFrom(oldShapeshifter);
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
				MiscUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
			}
		}

	}

}
