using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	//[Obsolete]
	//public class CompProperties_EntitiesGenes : CompProperties_CustomFloatMenu
	//{



	//}

	[Obsolete]
	public class CompEntitiesGenes : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.xenotypeDef == null || Props.jobDef == null)
			{
				yield break;
			}
			if (!selPawn.IsChimerkin())
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimeraEntitiesDevour".Translate(), delegate
			{
				CompStudyUnlocks study = parent.TryGetComp<CompStudyUnlocks>();
				if (study == null || study.Completed)
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Props.warningText.Translate(), delegate
					{
						MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef);
					});
					Find.WindowStack.Add(window);
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
