using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Holded : Gene_RemoteController, IGeneFloatMenuOptions
	{

		//[Unsaved(false)]
		//private GeneExtension_Giver cachedGeneExtension_Giver;
		//public GeneExtension_Giver Giver
		//{
		//	get
		//	{
		//		if (cachedGeneExtension_Giver == null)
		//		{
		//			cachedGeneExtension_Giver = def.GetModExtension<GeneExtension_Giver>();
		//		}
		//		return cachedGeneExtension_Giver;
		//	}
		//}

		private bool autoHold = true;
		private int lastHoldingTicks = 0;

		public override string RemoteActionName => XaG_UiUtility.OnOrOff(autoHold);
		public override TaggedString RemoteActionDesc => "WVC_GeneHolded_RCDesc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			autoHold = !autoHold;
		}

		public override void TickInterval(int delta)
		{
			if (!autoHold)
			{
				return;
			}
			if (pawn.IsHashIntervalTick(5555, delta))
			{
				TryCallHolder();
			}
		}

		public void SetDelay(int delay = 5)
		{
			lastHoldingTicks = delay;
		}

		private void TryCallHolder()
		{
			if (lastHoldingTicks > 0)
			{
				lastHoldingTicks--;
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.HomeFaction != Faction.OfPlayer)
			{
				return;
			}
			if (pawn.Drafted || pawn.Downed)
			{
				return;
			}
			if (!pawn.mindState.IsIdle)
			{
				return;
			}
			List<Pawn> allHumanlikeSpawned = pawn.Map.mapPawns.AllHumanlikeSpawned;
			allHumanlikeSpawned.Shuffle();
			foreach (Pawn target in allHumanlikeSpawned)
			{
				if (target.HomeFaction != Faction.OfPlayer)
				{
					continue;
				}
				if (target.genes == null)
				{
					continue;
				}
				if (target.Drafted || target.Downed)
				{
					continue;
				}
				if (target.genes.GenesListForReading.Any(gene => gene is Gene_Holder && gene.Active))
				{
					Job job = JobMaker.MakeJob(Extension_Giver.jobDef, target);
					pawn.TryTakeOrderedJob(job, JobTag.Misc, false);
					break;
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref autoHold, "autoHold", true);
		}

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!Active)
			{
				yield break;
			}
			if (!selPawn.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Deadly))
			{
				yield break;
			}
			if (selPawn.genes?.GetFirstGeneOfType<Gene_Holder>() == null)
			{
				yield break;
			}
			if (pawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			if (selPawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(selPawn.Named("PAWN")), null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_GeneHolder_HoldTarget".Translate(), delegate
			{
				Job job = JobMaker.MakeJob(Extension_Giver.tryHoldJobDef, pawn);
				selPawn.TryTakeOrderedJob(job, JobTag.Misc, false);
			}), selPawn, pawn);
		}

		public IEnumerable<FloatMenuOption> CompMultiSelectFloatMenuOptions(IEnumerable<Pawn> selPawns)
		{
			yield break;
		}

		//public IEnumerable<FloatMenuOption> CompMultiSelectFloatMenuOptions(IEnumerable<Pawn> selPawns)
		//{
		//	yield break;
		//}

	}

}
