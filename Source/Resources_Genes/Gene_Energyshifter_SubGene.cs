using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Energyshifter_SubGene : Gene_Disconnectable
	{

		[Unsaved(false)]
		private Gene_Energyshifter cachedShapeshifterGene;
		public Gene_Energyshifter Shapeshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Energyshifter>();
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

	}

}
