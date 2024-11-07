using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_EnterXenoculpterPod : JobDriver
	{

		public const int EnterPodDelay = 70;

		//private List<Thing> pickedUpIngredients = new();

		private CompXenosculpterPod XenosculpterPod => job.targetA.Thing.TryGetComp<CompXenosculpterPod>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed))
			{
				return false;
			}
			//List<LocalTargetInfo> targetQueue = job.GetTargetQueue(TargetIndex.B);
			//for (int i = 0; i < targetQueue.Count; i++)
			//{
			//	if (!pawn.Reserve(targetQueue[i], job, 1, -1, null, errorOnFailed))
			//	{
			//		return false;
			//	}
			//}
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			AddFinishAction(delegate
			{
				if (XenosculpterPod != null)
				{
					if (XenosculpterPod.queuedEnterJob == job)
					{
						XenosculpterPod.ClearQueuedInformation();
					}
					//if (XenosculpterPod.Occupant != GetActor())
					//{
					//	foreach (Thing pickedUpIngredient in pickedUpIngredients)
					//	{
					//		if (pawn.inventory.Contains(pickedUpIngredient))
					//		{
					//			pawn.inventory.innerContainer.TryDrop(pickedUpIngredient, ThingPlaceMode.Near, out var _);
					//		}
					//	}
					//}
				}
			});
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => job.biosculpterCycleKey == null || !XenosculpterPod.CanAcceptOnceCycleChosen(GetActor()));
			Toil goToPod = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			//yield return Toils_Jump.JumpIf(goToPod, () => job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
			//foreach (Toil item in CollectIngredientsToilsHelper(TargetIndex.B, pawn, pickedUpIngredients))
			//{
			//	yield return item;
			//}
			//yield return goToPod.FailOn(() => !XenosculpterPod.PawnCarryingExtraCycleIngredients(pawn, job.biosculpterCycleKey));
			yield return PrepareToEnterToil(TargetIndex.A);
			Toil enter = ToilMaker.MakeToil("MakeNewToils");
			enter.initAction = delegate
			{
				XenosculpterPod.TryAcceptPawn(enter.actor, job.biosculpterCycleKey);
			};
			enter.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return enter;
		}

		//public override string GetReport()
		//{
		//	if (!XenosculpterPod.PawnCarryingExtraCycleIngredients(pawn, job.biosculpterCycleKey))
		//	{
		//		return "BiosculpterJobReportCollectIngredients".Translate();
		//	}
		//	return base.GetReport();
		//}

		//public static IEnumerable<Toil> CollectIngredientsToilsHelper(TargetIndex ingredientIndex, Pawn carrier, List<Thing> pickedUpIngredients)
		//{
		//	Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(ingredientIndex);
		//	yield return extract;
		//	yield return Toils_Goto.GotoThing(ingredientIndex, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(ingredientIndex).FailOnSomeonePhysicallyInteracting(ingredientIndex);
		//	yield return Toils_Haul.StartCarryThing(ingredientIndex, putRemainderInQueue: true, subtractNumTakenFromJobCount: false, failIfStackCountLessThanJobCount: true);
		//	yield return HaulBiosculpterIngredientInInventory(carrier, pickedUpIngredients);
		//	yield return Toils_Jump.JumpIfHaveTargetInQueue(ingredientIndex, extract);
		//}

		//private static Toil HaulBiosculpterIngredientInInventory(Pawn carrier, List<Thing> pickedUpIngredients)
		//{
		//	Toil toil = ToilMaker.MakeToil("HaulBiosculpterIngredientInInventory");
		//	toil.initAction = delegate
		//	{
		//		Thing carriedThing = carrier.carryTracker.CarriedThing;
		//		carrier.carryTracker.innerContainer.TryTransferToContainer(carriedThing, carrier.inventory.innerContainer, carriedThing.stackCount, out var resultingTransferredItem);
		//		if (resultingTransferredItem != null)
		//		{
		//			pickedUpIngredients.Add(resultingTransferredItem);
		//		}
		//	};
		//	return toil;
		//}

		public static Toil PrepareToEnterToil(TargetIndex podIndex)
		{
			Toil prepare = Toils_General.Wait(70);
			prepare.FailOnCannotTouch(podIndex, PathEndMode.InteractionCell);
			prepare.WithProgressBarToilDelay(podIndex);
			prepare.PlaySustainerOrSound(() => prepare.actor.CurJob.GetTarget(podIndex).Thing?.TryGetComp<CompXenosculpterPod>()?.Props.enterSound);
			return prepare;
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Collections.Look(ref pickedUpIngredients, "pickedUpIngredients", LookMode.Reference);
		//}
	}

}
