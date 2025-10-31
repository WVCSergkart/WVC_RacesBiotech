using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_EntitiesGenes : CompProperties
	{

		public GeneDef geneDef;

		public JobDef devourJob;

		public string warningText = "WVC_XaG_GeneChimeraDevourFleshmassNucleusWarning";

		public CompProperties_EntitiesGenes()
		{
			compClass = typeof(CompEntitiesGenes);
		}
	}

	public class CompEntitiesGenes : ThingComp
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
				CompStudyUnlocks study = parent.TryGetComp<CompStudyUnlocks>();
				if (selPawn.IsChimerkin() && (study == null || study.Completed))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Props.warningText.Translate(), delegate
					{
						MiscUtility.MakeJobWithGeneDef(selPawn, Props.devourJob, Props.geneDef, parent);
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
