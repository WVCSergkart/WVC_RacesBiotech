using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Recipe_AdministerSerum : Recipe_AdministerUsableItem
	{

		// public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		// {
			// if (!base.AvailableOnNow(thing, part))
			// {
				// return false;
			// }
			// if (!ModsConfig.BiotechActive)
			// {
				// return false;
			// }
			// if (thing is not Pawn pawn || !pawn.Spawned)
			// {
				// return false;
			// }
			// List<XenotypeSerum> list = new();
			// List<Thing> listerThings = pawn.Map.listerThings.AllThings;
			// foreach (Thing item in listerThings)
			// {
				// if (item is XenotypeSerum xenotypeSerum)
				// {
					// list.Add(xenotypeSerum);
				// }
			// }
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
			// return false;
		// }

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			ingredients[0].TryGetComp<CompUsable>().UsedBy(pawn);
			if (ModLister.CheckBiotech("xenogerm implanting"))
			{
				if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
				{
					ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
				}
				if (ModsConfig.IdeologyActive)
				{
					Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
				}
			}
		}

		// public override string LabelFromUniqueIngredients(Bill bill)
		// {
			// if (bill.ingredientFilter.AnyAllowedDef?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
			// {
				// CompUseEffect_XenotypeForcer_II xeno = bill.ingredientFilter.AnyAllowedDef.TryGetComp<CompUseEffect_XenotypeForcer_II>();
				// return bill.ingredientFilter.AnyAllowedDef.label + " (" + xeno.xenotype.label + ")";
			// }
			// if (bill.ingredientFilter.AnyAllowedDef?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>() != null)
			// {
				// CompUseEffect_XenotypeForcer_Hybrid hybrid = bill.ingredientFilter.AnyAllowedDef.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>();
				// return bill.ingredientFilter.AnyAllowedDef.label + " (" + hybrid.endotype.label + " + " + hybrid.xenotype.label + ")";
			// }
			// return null;
		// }

	}

}
