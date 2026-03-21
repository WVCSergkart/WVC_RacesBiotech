using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_StylingStation : CompProperties
	{

		public JobDef jobDef;

		public CompProperties_StylingStation()
		{
			compClass = typeof(CompStylingStation);
		}
	}

	public class CompStylingStation : ThingComp
	{

		private CompProperties_StylingStation Props => (CompProperties_StylingStation)props;


		public static List<Pawn> cachedPawns;
		public static List<Pawn> Pawns
		{
			get
			{
				if (cachedPawns == null)
				{
					cachedPawns = PawnsFinder.AllMapsAndWorld_Alive.Where((p) => p.genes?.GetFirstGeneOfType<Gene_CustomHair>() != null).ToList();
				}
				return cachedPawns;
			}
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!Pawns.Contains(selPawn))
			{
				yield break;
			}
			if (!selPawn.CanReach(parent, PathEndMode.OnCell, Danger.Deadly))
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_CustomHairGene_FloatOption".Translate().CapitalizeFirst(), delegate
			{
				selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(Props.jobDef, parent), JobTag.Misc);
			}), selPawn, parent);
		}

	}

}
