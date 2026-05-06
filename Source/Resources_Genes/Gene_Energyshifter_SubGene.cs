using System;
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
					return Shapeshifter != null;
				}
				return false;
			}
		}

		private Gene_Energyshifter cachedShapeshifterGene;
		public Gene_Energyshifter Shapeshifter
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
			Shapeshifter?.UpdateCache();
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

}
