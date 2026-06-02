using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Holder : Gene_RemoteController, IGeneFloatMenuOptions
	{

		private GeneExtension_Giver cachedGeneExtension_Giver;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension_Giver == null)
				{
					cachedGeneExtension_Giver = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension_Giver;
			}
		}

		private List<PawnContainerHolder> savedContainers;
		public List<PawnContainerHolder> Containers
		{
			get
			{
				if (savedContainers == null)
				{
					savedContainers = new();
				}
				return savedContainers;
			}
		}

		public override string RemoteActionName => "Release".Translate().CapitalizeFirst();
		public override TaggedString RemoteActionDesc => "WVC_GeneHolder_RCDesc".Translate();

		private bool autoRelease = true;

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			List<FloatMenuOption> list = new();
			for (int i = 0; i < Containers.Count; i++)
			{
				PawnContainerHolder holder = Containers[i];
				list.Add(new FloatMenuOption("Release".Translate().CapitalizeFirst() + ": " + holder.LabelCap, delegate
				{
					TryDropPawn(holder);
				}));
			}
			list.Add(new FloatMenuOption("WVC_AutoRelease".Translate() + ": " + XaG_UiUtility.OnOrOff(autoRelease), delegate
			{
				autoRelease = !autoRelease;
			}, orderInPriority: 999));
			Find.WindowStack.Add(new FloatMenu(list));
		}

		public bool TryHoldCaller(Pawn caller)
		{
			PawnContainerHolder newHolder = new();
			if (newHolder.TrySetContainer(pawn, caller))
			{
				Containers.Add(newHolder);
				HediffUtility.TryAddHediff(Giver.hediffDef, caller, def);
				EffectsUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
				return true;
			}
			Log.Error("Failed set pawn container.");
			return false;
		}

		private void TryDropPawn(PawnContainerHolder target)
		{
			if (target.innerContainer != null && target.innerContainer.TryDropAll(pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Direct, playDropSound: false))
			{
				Pawn nextPawn = target.holded;
				if (!nextPawn.Spawned)
				{
					GenSpawn.Spawn(nextPawn, pawn.PositionHeld, pawn.MapHeld, pawn.Rotation);
				}
			}
			RemoveSetHolder(target);
			HediffUtility.TryRemoveHediff(Giver.hediffDef, target.holded);
			EffectsUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
		}

		private void RemoveSetHolder(PawnContainerHolder oldSet)
		{
			if (Containers.Contains(oldSet))
			{
				Containers.Remove(oldSet);
			}
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				TickHoldedPawns();
				if (pawn.IsNestedHashIntervalTick(2500, 5000))
				{
					TryRelease();
				}
			}
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			DropContainer();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			DropContainer();
		}

		public void DropContainer()
		{
			if (pawn.MapHeld == null)
			{
				return;
			}
			foreach (PawnContainerHolder pawnContainerHolder in Containers.ToList())
			{
				TryDropPawn(pawnContainerHolder);
			}
		}

		private void TickHoldedPawns()
		{
			foreach (PawnContainerHolder container in Containers)
			{
				//container.innerContainer.DoTick();
				//if (container.holded.needs != null)
				//{
				//	foreach (Need need in container.holded.needs.AllNeeds)
				//	{
				//		need.CurLevelPercentage++;
				//	}
				//}
				//foreach (Gene gene in container.holded.genes.GenesListForReading)
				//{
				//	gene.TickInterval(5000);
				//}
				container.DoTick(5000);
			}
		}

		private void TryRelease()
		{
			if (pawn.Drafted || pawn.Downed)
			{
				return;
			}
			if (pawn.Map == null || pawn.Map.mapPawns.FreeColonistsSpawned.Contains(pawn))
			{
				return;
			}
			if (Containers.TryRandomElement(out PawnContainerHolder holded))
			{
				TryDropPawn(holded);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref autoRelease, "autoRelease", true);
			Scribe_Collections.Look(ref savedContainers, "HolderContainers", LookMode.Deep);
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
			if (selPawn.genes?.GetFirstGeneOfType<Gene_Holded>() == null)
			{
				yield break;
			}
			if (pawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_GeneHolder_HoldMe".Translate(), delegate
			{
				Job job = JobMaker.MakeJob(Giver.jobDef, pawn);
				selPawn.TryTakeOrderedJob(job, JobTag.Misc, false);
			}), selPawn, pawn);
		}

	}

	public class Gene_Holded : Gene_RemoteController, IGeneFloatMenuOptions
	{

		private GeneExtension_Giver cachedGeneExtension_Giver;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension_Giver == null)
				{
					cachedGeneExtension_Giver = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension_Giver;
			}
		}

		private bool autoHold = true;

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
			if (pawn.IsHashIntervalTick(15555, delta))
			{
				TryCallHolder();
			}
		}

		private void TryCallHolder()
		{
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
			if (!pawn.Map.mapPawns.FreeColonistsSpawned.Contains(pawn))
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
				if (target.genes.GenesListForReading.Any(gene => gene is Gene_Holder && gene.Active))
				{
					Job job = JobMaker.MakeJob(Giver.jobDef, target);
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
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_GeneHolder_HoldTarget".Translate(), delegate
			{
				Job job = JobMaker.MakeJob(Giver.tryHoldJobDef, pawn);
				selPawn.TryTakeOrderedJob(job, JobTag.Misc, false);
			}), selPawn, pawn);
		}

	}

}
