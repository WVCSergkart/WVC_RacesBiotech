using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentCellsGain : Gene, IGeneResourceDrain
	{
		[Unsaved(false)]
		private Gene_ResurgentCells cachedHemogenGene;

		public Gene_Resource Resource
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes.GetFirstGeneOfType<Gene_ResurgentCells>();
				}
				return cachedHemogenGene;
			}
		}

		public bool CanOffset
		{
			get
			{
				if (Active)
				{
					return true;
				}
				return false;
			}
		}

		public float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
		}

	}

}
