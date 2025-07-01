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
				CompUseEffect_XenogermSerum xenoHolderSerum = this?.TryGetComp<CompUseEffect_XenogermSerum>();
				if (xenoHolderSerum?.xenotype != null)
				{
					yield return new DefHyperlink(xenoHolderSerum.xenotype);
				}
				if (xenoHolderSerum?.xenotypeHolder != null)
				{
					yield return new DefHyperlink(xenoHolderSerum.xenotypeHolder.xenotypeDef);
				}
				if (this?.TryGetComp<CompTargetEffect_DoJobOnTarget_XenogermSerum>()?.xenotypeDef != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompTargetEffect_DoJobOnTarget_XenogermSerum>().xenotypeDef);
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
			TaggedString text = "WVC_XaG_AdministerXenogermWarningDesc".Translate(newTarget.Named("PAWN"), trueMax.ToStringTicksToPeriod().Named("COMADURATION"));
			text += "\n\n" + "WouldYouLikeToContinue".Translate();
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
			{
				Bill bill = targetPawn?.BillStack?.Bills?.FirstOrDefault((Bill x) => x is Bill_Medical bill_Medical && bill_Medical.uniqueRequiredIngredients != null && bill_Medical.uniqueRequiredIngredients.Contains(this));
				if (bill != null)
				{
					targetPawn.BillStack.Delete(bill);
				}
				HealthCardUtility.CreateSurgeryBill(newTarget, MainDefOf.WVC_ImplantXenogermSerum, null, new() { this });
				targetPawn = newTarget;
			}, destructive: true));
		}

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
							if (!pawn.IsQuestLodger() && pawn.IsHuman() && (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony || (pawn.IsColonySubhuman && pawn.IsGhoul)))
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

		public override void Notify_RecipeProduced(Pawn pawn)
        {
            base.Notify_RecipeProduced(pawn);
			if (pawn != null)
			{
				this?.TryGetComp<CompUseEffect_XenogermSerum>()?.Notify_SerumCrafted(pawn);
				//this?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>()?.Notify_SerumCrafted(pawn);
				//this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.Notify_SerumCrafted(pawn);
			}
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
				}
				else if (!string.IsNullOrEmpty(acceptanceReport.Reason))
				{
					comp.SendCannotUseMessage(pawn, acceptanceReport.Reason);
				}
			}
		}

	}

}
