using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Archive : Gene
	{

		public int DuplicatesCount => holders.Count;

		private List<GeneAsPawnHolder> holders = new();

		private int currentLimit = 1;

		public void AddLimit(int count = 1)
		{
			currentLimit += count;
		}

		public void ChangePawn(GeneAsPawnHolder target)
		{
			string phase = "";
			try
			{
				Pawn nextPawn = null;
				if (target == null && !holders.NullOrEmpty())
				{
					phase = "trying random holder";
					target = holders.RandomElement();
				}
				if (target != null)
				{
					phase = "trying drop pawn";
					target.innerContainer.TryDropAll(pawn.Position, pawn.Map, ThingPlaceMode.Direct);
					holders.Remove(target);
					nextPawn = target.holded;
					if (!nextPawn.Spawned)
					{
						GenSpawn.Spawn(nextPawn, pawn.Position, pawn.Map);
					}
				}
				else if (holders.Count < currentLimit + 1)
				{
					phase = "trying create new pawn";
					DuplicateUtility.TryDuplicatePawn(pawn, pawn, pawn.Position, pawn.Map, out nextPawn, out _, out _, doEffects: false);
				}
				if (nextPawn == null)
				{
					//Log.Error("Pawn is null");
					return;
				}
				phase = "get gene";
				Gene_Archive archive = nextPawn.genes?.GetFirstGeneOfType<Gene_Archive>();
				phase = "trying transfer holders";
				TransferHolders(holders, archive.holders);
				//phase = "upd pawn apparel";
				//TransferApparel(nextPawn);
				phase = "trying create new holder";
				//Log.Error("Create new holder");
				GeneAsPawnHolder newHolder = new();
				newHolder.Initial(nextPawn, pawn);
				archive.holders.Add(newHolder);
                if (nextPawn.Spawned)
                {
                    Find.Selector.ClearSelection();
                    Find.Selector.Select(nextPawn);
				}
				phase = "do effects";
				WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(nextPawn, nextPawn.Map).Trigger(nextPawn, null);
				//Log.Error("Spawned");
			}
			catch (Exception arg)
			{
				Log.Error("Failed replace pawn on phase: " + phase + ". Reason: " + arg);
			}
		}

		//private void TransferApparel(Pawn newPawn)
		//{
		//	pawn.apparel.DropAll(pawn.Position);
		//	foreach (Apparel item in pawn.apparel.WornApparel)
		//	{
		//		newPawn.apparel.Wear(item);
		//	}
		//}

		private void TransferHolders(List<GeneAsPawnHolder> oldHolders, List<GeneAsPawnHolder> newHolders)
		{
			foreach (GeneAsPawnHolder holder in oldHolders.ToList())
            {
				newHolders.Add(holder);
				oldHolders.Remove(holder);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryChangePawn",
					action = delegate
					{
						ChangePawn(null);
					}
				};
			}
		}

		private void KillHoldedPawns()
		{
			foreach (GeneAsPawnHolder holder in holders)
			{
				holder.holded?.Kill(null);
			}
			holders = new();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			KillHoldedPawns();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			KillHoldedPawns();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref holders, "holders", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (holders == null)
				{
					holders = new();
				}
			}
		}

	}

}
