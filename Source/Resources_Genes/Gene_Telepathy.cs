using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Telepathy : Gene_Speaker
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new(6600, 22000));
		}

		public override void TryInteract()
		{
			//if (!WVC_Biotech.settings.enableHarmonyTelepathyGene)
			//{
			//}
			GeneInteractionsUtility.TryInteractRandomly(pawn, true, true, false, out _);
			ResetInterval(new(6600, 22000));
		}

	}

	public class Gene_ArchiverMindsConnection : Gene_Telepathy
	{


		[Unsaved(false)]
		private Gene_Archiver cachedArchiverGene;
		public Gene_Archiver Archiver
		{
			get
			{
				if (cachedArchiverGene == null || !cachedArchiverGene.Active)
				{
					cachedArchiverGene = pawn?.genes?.GetFirstGeneOfType<Gene_Archiver>();
				}
				return cachedArchiverGene;
			}
		}

		public override void TryInteract()
		{
			List<Pawn> archivedPawns = new();
			foreach (PawnGeneSetHolder holder in Archiver.SavedGeneSets)
            {
				if (holder is PawnContainerHolder container && !archivedPawns.Contains(container.holded))
                {
					archivedPawns.Add(container.holded);
				}
            }
			GeneInteractionsUtility.TryInteractRandomly(pawn, archivedPawns, true, true, false, out _);
			ResetInterval(new(6600, 22000));
		}

	}

	public class Gene_Psyfeeder : Gene
	{

		private Gene_Hemogen cachedHemogenGene = null;

		public Gene_Hemogen Gene_Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(8000, delta))
			{
				return;
			}
			GeneFeaturesUtility.TryPsyFeedRandomly(pawn, Gene_Hemogen);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryPsyFeed",
					action = delegate
					{
						GeneFeaturesUtility.TryPsyFeedRandomly(pawn, Gene_Hemogen);
					}
				};
			}
		}

	}

}
