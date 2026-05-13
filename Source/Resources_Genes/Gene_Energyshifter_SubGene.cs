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

	public class Gene_Energyshifter_SolarCharger : Gene_Energyshifter_SubGene
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				Energyshifter?.UpdateConsumption();
			}
		}

		public override float ResourceConsumption_Offset
		{
			get
			{
				if (pawn.MapHeld != null)
				{
					SimpleCurve curve = new()
					{
						new CurvePoint(0f, def.resourceLossPerDay),
						new CurvePoint(0.5f, def.resourceLossPerDay),
						new CurvePoint(1f, -0.6f)
					};
					return curve.Evaluate(pawn.MapHeld.glowGrid.GroundGlowAt(pawn.PositionHeld));
				}
				return base.ResourceConsumption_Offset;
			}
		}

		public override float ResourceConsumption_Factor => 1f;

		public override void TickMasterGene(int factorDelayTicks, int outTicks)
		{
			Gene_Rechargeable.NotifySubGenes_Charging(pawn, Mathf.Clamp(Energyshifter.Consumption, 0f, 999f), 1, 0.5f);
		}

		//private void UpdatePower()
		//{
		//	Energyshifter?.UpdateConsumption();
		//}

		//private void Charge()
		//{
		//	if (!pawn.TryGetNeedFood(out Need_Food need_Food))
		//	{
		//		return;
		//	}
		//	if (pawn.MapHeld == null)
		//	{
		//		if (pawn.GetCaravan()?.NightResting == true)
		//		{
		//			need_Food.CurLevelPercentage += 0.05f;
		//			Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.05f, 1, 1f);
		//		}
		//		else
		//		{
		//			need_Food.CurLevelPercentage += 0.01f;
		//			Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.001f, 1, 1f);
		//		}
		//	}
		//	else
		//	{
		//		if (pawn.PositionHeld.InSunlight(pawn.MapHeld))
		//		{
		//			need_Food.CurLevelPercentage += 0.025f;
		//			Gene_Rechargeable.NotifySubGenes_Charging(pawn, 0.025f, 1, 1f);
		//		}
		//	}
		//}

		//public void Notify_DevouredHuman(Pawn victim)
		//{

		//}

		//public void Notify_DevouredFlesh(Pawn victim)
		//{

		//}

		//public void Notify_DevouredMech(Pawn victim)
		//{
		//	Need_MechEnergy energy = victim.needs?.energy;
		//	if (energy != null)
		//	{
		//		GeneResourceUtility.OffsetNeedFood(pawn, energy.CurLevel);
		//	}
		//}

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
