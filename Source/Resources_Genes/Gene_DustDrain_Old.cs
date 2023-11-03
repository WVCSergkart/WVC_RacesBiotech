using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DustDrain : Gene, IGeneResourceDrain
	{
		[Unsaved(false)]
		private Gene_Dust cachedDustGene;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		// private const float MinAgeForDrain = 3f;

		public Gene_Resource Resource
		{
			get
			{
				if (cachedDustGene == null || !cachedDustGene.Active)
				{
					cachedDustGene = pawn.genes.GetFirstGeneOfType<Gene_Dust>();
				}
				return cachedDustGene;
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

		// public float ResourceLossPerDay => ResourceLoss();
		public float ResourceLossPerDay => ResourceLoss();

		public float ResourceLoss()
		{
			if (cachedDustGene.ResourceLossPerDay > 0f)
			{
				return def.resourceLossPerDay;
			}
			return 0f;
		}

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
		}
	}

}
