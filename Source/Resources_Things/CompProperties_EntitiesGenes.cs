using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_EntitiesGenes : CompProperties
	{

		public GeneDef geneDef;

		public JobDef devourJob;

		public CompProperties_EntitiesGenes()
		{
			compClass = typeof(CompFleshmassGene);
		}
	}

	public class CompFleshmassGene : ThingComp
	{

		public CompProperties_EntitiesGenes Props => (CompProperties_EntitiesGenes)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.geneDef == null || Props.devourJob == null)
            {
				yield break;
			}
			//if (!selPawn.IsChimera())
			//{
			//	yield break;
			//}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimeraEntitiesDevour".Translate(), delegate
			{
				if (selPawn.IsChimera())
				{
					MiscUtility.MakeJobWithGeneDef(selPawn, Props.devourJob, Props.geneDef, parent);
				}
				else
                {
					Messages.Message("WVC_XaG_GeneChimeraReqGene".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
			}), selPawn, parent);
			//return Enumerable.Empty<FloatMenuOption>();
		}

	}

}
