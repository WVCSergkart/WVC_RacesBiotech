using RimWorld;
using RimWorld.Planet;
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

	public class Gene_Energyshifter_SolarCharger : Gene_Energyshifter_SubGene, IGeneDevourer
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				Charge();
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

		private void Charge()
		{
			if (!pawn.TryGetNeedFood(out Need_Food need_Food))
			{
				return;
			}
			if (pawn.MapHeld == null)
			{
				if (pawn.GetCaravan()?.NightResting == true)
				{
					need_Food.CurLevelPercentage += 0.05f;
					Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.05f, 1, 1f);
				}
				else
				{
					need_Food.CurLevelPercentage += 0.01f;
					Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.001f, 1, 1f);
				}
			}
			else
			{
				if (pawn.PositionHeld.InSunlight(pawn.MapHeld))
				{
					need_Food.CurLevelPercentage += 0.025f;
					Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.025f, 1, 1f);
				}
			}
		}

		public void Notify_DevouredHuman(Pawn victim)
		{

		}

		public void Notify_DevouredFlesh(Pawn victim)
		{

		}

		public void Notify_DevouredMech(Pawn victim)
		{
			Need_MechEnergy energy = victim.needs?.energy;
			if (energy != null)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, energy.CurLevel);
			}
		}

	}

	// DEV
	public class Gene_Energyshifter_Learning : Gene_Energyshifter_SubGene
	{

		public override void TickMasterGene(int factorDelayTicks, int outTicks)
		{

		}

	}

	public class Gene_Energyshifter_SlowAging : Gene_Energyshifter_SubGene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			Gene_ElfAging.SetupChronoAge(pawn, this);
		}

	}

}
