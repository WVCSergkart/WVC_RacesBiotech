using System;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Energyshifter_SubGene : Gene_Disconnectable, IGeneRecacheable
	{

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					return Energyshifter != null;
				}
				return false;
			}
		}

		private Gene_Energyshifter cachedShapeshifterGene;
		public Gene_Energyshifter Energyshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Energyshifter>();
					if (cachedShapeshifterGene == null && MiscUtility.GameStarted())
					{
						Disabled = true;
					}
				}
				return cachedShapeshifterGene;
			}
		}

		public override Type MasterClass => typeof(Gene_Energyshifter);

		public override void UpdateCache()
		{
			base.UpdateCache();
			Energyshifter?.UpdateCache();
		}

		public override void TickInterval(int delta)
		{

		}

		public virtual void Notify_GenesRecache(Gene changedGene)
		{
			cachedShapeshifterGene = null;
		}

	}

	public class Gene_Energyshifter_Regen : Gene_Energyshifter_SubGene
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(1511, delta))
			{
				HealingUtility.Regeneration(pawn, 100, 1511);
			}
		}

	}

	public class Gene_Energyshifter_Solar : Gene_Energyshifter_SubGene
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				UpdatePower();
			}
		}

		private float? cachedPowerFactor;
		public override float ResourceConsumption_Factor
		{
			get
			{
				if (cachedPowerFactor == null)
				{
					if (pawn.MapHeld == null)
					{
						cachedPowerFactor = 0.5f;
					}
					else
					{
						cachedPowerFactor = Mathf.Clamp(1f - pawn.MapHeld.glowGrid.GroundGlowAt(pawn.PositionHeld), 0f, 1f);
					}
				}
				return cachedPowerFactor.Value;
			}
		}

		private void UpdatePower()
		{
			cachedPowerFactor = null;
			Energyshifter?.UpdateConsumption();
		}

	}

}
