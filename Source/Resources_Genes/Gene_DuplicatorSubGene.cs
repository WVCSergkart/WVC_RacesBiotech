using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DuplicatorSubGene : Gene
	{

		private bool? cachedIsDuplicate;
		public bool IsDuplicate
		{
			get
			{
				if (!cachedIsDuplicate.HasValue)
				{
					cachedIsDuplicate = pawn.IsDuplicate;
					//if (cachedIsDuplicate == null)
					//{
					//	return false;
					//}
				}
				return cachedIsDuplicate.Value;
			}
		}

		private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}

		[Unsaved(false)]
		private Gene_Duplicator cachedDuplicatorGene;
		public Gene_Duplicator Duplicator
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator>();
				}
				return cachedDuplicatorGene;
			}
		}

		public bool SourceIsAlive
		{
			get
			{
				if (Duplicator == null)
				{
					return false;
				}
				return Duplicator.SourceIsAlive;
			}
		}

		public override void TickInterval(int delta)
		{

		}

	}

	// =====================================

	public class Gene_Duplicator_DeathChain : Gene_DuplicatorSubGene
	{

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active || Duplicator == null)
			{
				return;
			}
			Duplicator.Notify_GenesChanged(null);
			foreach (Pawn dupe in Duplicator.PawnDuplicates_WithSource.ToList())
			{
				if (!dupe.Dead)
				{
					dupe.Kill(null);
				}
			}
		}

	}

}
