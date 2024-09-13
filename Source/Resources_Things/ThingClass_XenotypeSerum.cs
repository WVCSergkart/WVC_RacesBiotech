using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeSerum : ThingWithComps
	{

		public Pawn targetPawn;

		public static readonly CachedTexture ImplantTex = new("UI/Gizmos/ImplantGenes");

		public static readonly CachedTexture CancelIcon = new("UI/Designators/Cancel");

		public override IEnumerable<DefHyperlink> DescriptionHyperlinks
		{
			get
			{
				if (this?.TryGetComp<CompUseEffect_XenogermSerum>()?.xenotype != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenogermSerum>().xenotype);
				}
				if (this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef);
				}
				if (this?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>() != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().endotype);
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().xenotype);
				}
				if (this?.TryGetComp<CompUseEffect_GeneGiver>()?.geneDef != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_GeneGiver>().geneDef);
				}
				// if (this?.TryGetComp<CompUseEffect_GeneRestoration>()?.Props?.hediffsToRemove != null)
				// {
					// foreach (HediffDef item in this.TryGetComp<CompUseEffect_GeneRestoration>().Props.hediffsToRemove)
					// {
						// yield return new DefHyperlink(item);
					// }
				// }
				if (def.descriptionHyperlinks != null)
				{
					for (int i = 0; i < def.descriptionHyperlinks.Count; i++)
					{
						yield return def.descriptionHyperlinks[i];
					}
				}
			}
		}

		public void SetTargetPawnForXenoChanger(Pawn newTarget)
		{
			int trueMax = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.TrueMax;
			TaggedString text = "ImplantXenogermWarningDesc".Translate(newTarget.Named("PAWN"), trueMax.ToStringTicksToPeriod().Named("COMADURATION"));
			// if (newTarget.genes.Xenogenes.Any())
			// {
				// text += "\n\n" + "ImplantXenogermWarningOverwriteXenogenes".Translate(newTarget.Named("PAWN"), newTarget.genes.XenotypeLabelCap.Named("XENOTYPE"), newTarget.genes.Xenogenes.Select((Gene x) => x.LabelCap).ToLineList("  - ").Named("XENOGENES"));
			// }
			// int num = GeneUtility.MetabolismAfterImplanting(newTarget, geneSet);
			// text += "\n\n" + "ImplantXenogermWarningNewMetabolism".Translate(newTarget.Named("PAWN"), num.Named("MET"), GeneTuning.MetabolismToFoodConsumptionFactorCurve.Evaluate(num).ToStringPercent().Named("CONSUMPTION"));
			text += "\n\n" + "WouldYouLikeToContinue".Translate();
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
			{
				// List<Thing> things = new() { this };
				// Log.Error("0");
				Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x is Bill_Medical bill_Medical && bill_Medical.uniqueRequiredIngredients != null && bill_Medical.uniqueRequiredIngredients.Contains(this));
				if (bill != null)
				{
					targetPawn.BillStack.Delete(bill);
				}
				// Log.Error("1");
				HealthCardUtility.CreateSurgeryBill(newTarget, WVC_GenesDefOf.WVC_ImplantXenogermSerum, null, new() { this });
				// Log.Error("2");
				targetPawn = newTarget;
				// SendImplantationLetter(newTarget);
			}, destructive: true));
		}

		// public void SendImplantationLetter(Pawn targetPawn)
		// {
			// string arg = string.Empty;
			// if (!targetPawn.InBed() && !targetPawn.Map.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed bed && RestUtility.CanUseBedEver(targetPawn, x.def) && bed.Medical))
			// {
				// arg = "XenogermOrderedImplantedBedNeeded".Translate(targetPawn.Named("PAWN"));
			// }
			// int requiredMedicineForImplanting = RequiredMedicineForImplanting;
			// string arg2 = string.Empty;
			// if (targetPawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine).Sum((Thing x) => x.stackCount) < requiredMedicineForImplanting)
			// {
				// arg2 = "XenogermOrderedImplantedMedicineNeeded".Translate(requiredMedicineForImplanting.Named("MEDICINENEEDED"));
			// }
			// Find.LetterStack.ReceiveLetter("LetterLabelXenogermOrderedImplanted".Translate(), "LetterXenogermOrderedImplanted".Translate(targetPawn.Named("PAWN"), requiredMedicineForImplanting.Named("MEDICINENEEDED"), arg.Named("BEDINFO"), arg2.Named("MEDICINEINFO")), LetterDefOf.NeutralEvent);
		// }

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (targetPawn == null)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_ImplantXenogermSerum".Translate(),
					defaultDesc = "WVC_ImplantXenogermSerumDesc".Translate(),
					icon = ImplantTex.Texture,
					action = delegate
					{
						List<FloatMenuOption> list = new();
						foreach (Pawn item in base.Map.mapPawns.AllPawnsSpawned)
						{
							Pawn pawn = item;
							if (!pawn.IsQuestLodger() && pawn.genes != null && (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony || (pawn.IsColonyMutant && pawn.IsGhoul)))
							{
								list.Add(new FloatMenuOption(pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap, delegate
								{
									SetTargetPawnForXenoChanger(pawn);
								}, pawn, Color.white));
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
					Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x is Bill_Medical bill_Medical && bill_Medical.uniqueRequiredIngredients != null && bill_Medical.uniqueRequiredIngredients.Contains(this));
					if (bill != null)
					{
						targetPawn.BillStack.Delete(bill);
					}
					targetPawn = null;
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref targetPawn, "targetPawn");
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			if (stackCount > def.stackLimit)
			{
				for (int i = 0; i < stackCount; i++)
				{
					Thing thing = ThingMaker.MakeThing(def);
					thing.stackCount = 1;
					GenPlace.TryPlaceThing(thing, Position, map, ThingPlaceMode.Near, null, null, default);
				}
				Log.Warning(string.Concat("Spawned ", this, " with stackCount ", stackCount, " but stackLimit is ", def.stackLimit, ". Truncating."));
				stackCount = def.stackLimit;
			}
			base.SpawnSetup(map, respawningAfterLoad);
		}

	}

	public class Recipe_ImplantXenogermSerum : Recipe_AdministerUsableItem
	{

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (ingredients[0].TryGetComp(out CompUsable comp))
			{
				AcceptanceReport acceptanceReport = comp.CanBeUsedBy(pawn, forced: false, ignoreReserveAndReachable: true);
				if (acceptanceReport.Accepted)
				{
					comp.UsedBy(pawn);
					if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
					{
						ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
					}
					// if (ModsConfig.IdeologyActive)
					// {
						// Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
					// }
				}
				else if (!string.IsNullOrEmpty(acceptanceReport.Reason))
				{
					comp.SendCannotUseMessage(pawn, acceptanceReport.Reason);
				}
			}
		}

	}

}
