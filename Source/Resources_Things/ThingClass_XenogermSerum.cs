using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenogermSerum : Xenogerm
	{

		public XenotypeDef endotypeDef;

		public XenotypeDef xenotypeDef;

		public override void Notify_DebugSpawned()
		{
			geneSet = new();
			xenotypeName = "DEBUG";
			iconDef = XenotypeIconDefOf.Basic;
		}

		public override void PostMake()
		{
			base.PostMake();
			geneSet = new();
			if (endotypeDef != null)
			{
				List<GeneDef> genesListForReading = endotypeDef.genes;
				for (int j = 0; j < genesListForReading.Count; j++)
				{
					geneSet.AddGene(genesListForReading[j]);
				}
				xenotypeName = endotypeDef.label;
			}
			if (xenotypeDef != null)
			{
				List<GeneDef> genesListForReading = xenotypeDef.genes;
				for (int j = 0; j < genesListForReading.Count; j++)
				{
					geneSet.AddGene(genesListForReading[j]);
				}
				xenotypeName = xenotypeDef.label;
			}
		}

		public void SetTargetPawnForXenoChanger(Pawn newTarget)
		{
			int trueMax = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.TrueMax;
			TaggedString text = "ImplantXenogermWarningDesc".Translate(newTarget.Named("PAWN"), trueMax.ToStringTicksToPeriod().Named("COMADURATION"));
			if (newTarget.genes.Xenogenes.Any())
			{
				text += "\n\n" + "ImplantXenogermWarningOverwriteXenogenes".Translate(newTarget.Named("PAWN"), newTarget.genes.XenotypeLabelCap.Named("XENOTYPE"), newTarget.genes.Xenogenes.Select((Gene x) => x.LabelCap).ToLineList("  - ").Named("XENOGENES"));
			}
			int num = GeneUtility.MetabolismAfterImplanting(newTarget, geneSet);
			text += "\n\n" + "ImplantXenogermWarningNewMetabolism".Translate(newTarget.Named("PAWN"), num.Named("MET"), GeneTuning.MetabolismToFoodConsumptionFactorCurve.Evaluate(num).ToStringPercent().Named("CONSUMPTION"));
			text += "\n\n" + "WouldYouLikeToContinue".Translate();
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
			{
				Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x.xenogerm == this);
				if (bill != null)
				{
					targetPawn.BillStack.Delete(bill);
				}
				HealthCardUtility.CreateSurgeryBill(newTarget, RecipeDefOf.ImplantXenogerm, null).xenogerm = this;
				targetPawn = newTarget;
				SendImplantationLetter(newTarget);
			}, destructive: true));
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (xenotypeDef == null && endotypeDef == null)
			{
				yield break;
			}
			if (targetPawn == null)
			{
				yield return new Command_Action
				{
					defaultLabel = "ImplantXenogerm".Translate() + "...",
					defaultDesc = "ImplantXenogermDesc".Translate(RequiredMedicineForImplanting.Named("MEDICINEAMOUNT")),
					icon = ImplantTex.Texture,
					action = delegate
					{
						List<FloatMenuOption> list = new List<FloatMenuOption>();
						foreach (Pawn item in base.Map.mapPawns.AllPawnsSpawned)
						{
							Pawn pawn = item;
							if (!pawn.IsQuestLodger() && pawn.genes != null && (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony || (pawn.IsColonyMutant && pawn.IsGhoul)))
							{
								int num = GeneUtility.MetabolismAfterImplanting(pawn, geneSet);
								if (num < GeneTuning.BiostatRange.TrueMin)
								{
									list.Add(new FloatMenuOption((string)(pawn.LabelShortCap + ": " + "ResultingMetTooLow".Translate() + " (") + num + ")", null, pawn, Color.white));
								}
								else if (PawnIdeoDisallowsImplanting(pawn))
								{
									list.Add(new FloatMenuOption(pawn.LabelShortCap + ": " + "IdeoligionForbids".Translate(), null, pawn, Color.white));
								}
								else
								{
									list.Add(new FloatMenuOption(pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap, delegate
									{
										SetTargetPawn(pawn);
									}, pawn, Color.white));
								}
							}
						}
						if (!list.Any())
						{
							list.Add(new FloatMenuOption("NoImplantablePawns".Translate(), null));
						}
						Find.WindowStack.Add(new FloatMenu(list));
					}
				};
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "CancelImplanting".Translate(),
				defaultDesc = "CancelImplantingDesc".Translate(targetPawn.Named("PAWN")),
				icon = CancelIcon,
				action = delegate
				{
					Bill bill = targetPawn.BillStack?.Bills.FirstOrDefault((Bill x) => x.xenogerm == this);
					if (bill != null)
					{
						targetPawn.BillStack.Delete(bill);
					}
				}
			};
		}

	}

}
