using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_CustomFloatMenu : CompProperties
	{

		public GeneDef geneDef;

		public JobDef jobDef;

		public string warningText = "WVC_XaG_GeneChimeraDevourFleshmassNucleusWarning";

		public float factor = 1;

		public CompProperties_CustomFloatMenu()
		{
			compClass = typeof(CompEntitiesGenes);
		}
	}

	[Obsolete]
	public class CompProperties_EntitiesGenes : CompProperties_CustomFloatMenu
	{



	}

	public class CompEntitiesGenes : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.geneDef == null || Props.jobDef == null)
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
						MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, Props.geneDef);
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

	public class CompChimeraArchiteLimit : ThingComp
	{

		public CompProperties_CustomFloatMenu Props => (CompProperties_CustomFloatMenu)props;

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (Props.jobDef == null)
			{
				yield break;
			}
			if (!selPawn.IsChimerkin())
			{
				yield break;
			}
			//Log.Error("01");
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate(), delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, factor: Props.factor);
			}), selPawn, parent);
			if (parent.stackCount <= 1)
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_GeneChimera_ArchiteDevour".Translate() + " x" + parent.stackCount, delegate
			{
				MiscUtility.MakeCustomJob(selPawn, parent, Props.jobDef, null, true, factor: Props.factor);
			}), selPawn, parent);
		}

	}

}
