using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenogermSerum : Xenogerm
	{

		public XenotypeDef endotypeDef;

		public XenotypeDef xenotypeDef;

		public Pawn targetPawn;

		public static readonly CachedTexture ImplantTex = new("UI/Gizmos/ImplantGenes");

		public static readonly CachedTexture CancelIcon = new("UI/Designators/Cancel");

		public override string LabelNoCount
		{
			get
			{
				if (xenotypeDef == null)
				{
					return def.label + " (ERR)";
				}
				return def.label + " (" + xenotypeDef.label + ")";
			}
		}

		public int RequiredMedicineForImplanting
		{
			get
			{
				int num = 0;
				for (int i = 0; i < WVC_GenesDefOf.WVC_ImplantXenogermSerum.ingredients.Count; i++)
				{
					IngredientCount ingredientCount = WVC_GenesDefOf.WVC_ImplantXenogermSerum.ingredients[i];
					if (ingredientCount.filter.Allows(ThingDefOf.MedicineIndustrial))
					{
						num += (int)ingredientCount.GetBaseCount();
					}
				}
				return num;
			}
		}

		public override void Notify_DebugSpawned()
		{
			SetGeneSet();
			xenotypeName = "DEBUG";
			iconDef = XenotypeIconDefOf.Basic;
		}

		public override void PostMake()
		{
			base.PostMake();
			SetGeneSet();
		}

		public void SetGeneSet()
		{
			xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true).RandomElement();
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
			geneSet.SortGenes();
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
				HealthCardUtility.CreateSurgeryBill(newTarget, WVC_GenesDefOf.WVC_ImplantXenogermSerum, null).xenogerm = this;
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
					defaultLabel = "TEST " + "ImplantXenogerm".Translate() + "...",
					defaultDesc = "ImplantXenogermDesc".Translate(RequiredMedicineForImplanting.Named("MEDICINEAMOUNT")),
					icon = ImplantTex.Texture,
					action = delegate
					{
						List<FloatMenuOption> list = new();
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
										SetTargetPawnForXenoChanger(pawn);
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
				icon = CancelIcon.Texture,
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

		private void SendImplantationLetter(Pawn targetPawn)
		{
			string arg = string.Empty;
			if (!targetPawn.InBed() && !targetPawn.Map.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed bed && RestUtility.CanUseBedEver(targetPawn, x.def) && bed.Medical))
			{
				arg = "XenogermOrderedImplantedBedNeeded".Translate(targetPawn.Named("PAWN"));
			}
			int requiredMedicineForImplanting = RequiredMedicineForImplanting;
			string arg2 = string.Empty;
			if (targetPawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine).Sum((Thing x) => x.stackCount) < requiredMedicineForImplanting)
			{
				arg2 = "XenogermOrderedImplantedMedicineNeeded".Translate(requiredMedicineForImplanting.Named("MEDICINENEEDED"));
			}
			Find.LetterStack.ReceiveLetter("LetterLabelXenogermOrderedImplanted".Translate(), "LetterXenogermOrderedImplanted".Translate(targetPawn.Named("PAWN"), requiredMedicineForImplanting.Named("MEDICINENEEDED"), arg.Named("BEDINFO"), arg2.Named("MEDICINEINFO")), LetterDefOf.NeutralEvent);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref targetPawn, "targetPawn");
			Scribe_Defs.Look(ref endotypeDef, "endotypeDef");
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
		}

	}

	public class Recipe_ImplantXenogermSerum : Recipe_ImplantXenogerm
	{
		public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		{
			if (!base.AvailableOnNow(thing, part))
			{
				return false;
			}
			if (thing is not Pawn pawn || !pawn.Spawned)
			{
				return false;
			}
			// List<Thing> list = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Xenogerm);
			// if (list.Any())
			// {
				// foreach (Thing item in list)
				// {
					// if (!item.IsForbidden(pawn) && !item.Position.Fogged(pawn.Map))
					// {
						// return true;
					// }
				// }
			// }
			if (!thing.IsForbidden(pawn) && !thing.Position.Fogged(pawn.Map))
			{
				return true;
			}
			return false;
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (!CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
			{
				if (bill.xenogerm != null && bill.xenogerm is XenogermSerum serum)
				{
					// GeneUtility.ImplantXenogermItem(pawn, serum);
					if (serum.endotypeDef != null)
					{
						ReimplanterUtility.SetXenotype(pawn, serum.endotypeDef);
					}
					if (serum.xenotypeDef != null)
					{
						ReimplanterUtility.SetXenotype(pawn, serum.xenotypeDef);
					}
				}
				if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
				{
					ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
				}
				if (ModsConfig.IdeologyActive)
				{
					Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
					SerumUtility.PostSerumUsedHook(pawn);
				}
			}
		}
	}

}
