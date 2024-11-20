using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Archiver : Gene_Morpher
	{

		//public int DuplicatesCount => holders.Count;

		//private List<PawnContainerHolder> holders = new();

		//private int currentLimit = 1;

		//public void AddLimit(int count = 1)
		//{
		//	currentLimit += count;
		//}

		public override bool TryMorph(PawnGeneSetHolder nextGeneSet, bool shouldMorph = false, bool removeMorpher = false)
		{
			string phase = "";
			try
			{
				if (!shouldMorph || (nextGeneSet != null && nextGeneSet is not PawnContainerHolder))
				{
					return false;
				}
				PawnContainerHolder target = nextGeneSet as PawnContainerHolder;
				Pawn nextPawn = null;
				if (target == null && !savedGeneSets.NullOrEmpty())
				{
					phase = "trying random holder";
					target = savedGeneSets.RandomElement() as PawnContainerHolder;
				}
				if (target != null)
				{
					phase = "trying drop pawn";
					target.innerContainer.TryDropAll(pawn.Position, pawn.Map, ThingPlaceMode.Direct);
					savedGeneSets.Remove(target);
					nextPawn = target.holded;
					if (!nextPawn.Spawned)
					{
						GenSpawn.Spawn(nextPawn, pawn.Position, pawn.Map);
					}
				}
				else if (savedGeneSets.Count < currentLimit + 1)
				{
					phase = "trying create new pawn";
					DuplicateUtility.TryDuplicatePawn(pawn, pawn, pawn.Position, pawn.Map, out nextPawn, out _, out _, doEffects: false);
					//Reimplant(nextPawn, pawn.genes.GetFirstGeneOfType<Gene_ArchiverXenotypeChanger>() != null);
				}
				if (nextPawn == null)
				{
					//Log.Error("Pawn is null");
					return false;
				}
				phase = "get gene";
				Gene_Archiver archive = nextPawn.genes?.GetFirstGeneOfType<Gene_Archiver>();
				if (archive == null)
				{
					Log.Error("Failed get Gene_Archiver.");
					return false;
				}
				//archive.UpdToolGenes(true);
				phase = "trying transfer holders";
				TransferHolders(savedGeneSets, archive.savedGeneSets);
				//phase = "upd pawn apparel";
				//TransferApparel(nextPawn);
				phase = "trying create new holder";
				//Log.Error("Create new holder");
				PawnContainerHolder newHolder = new();
				newHolder.formId = savedGeneSets.Count + 1;
				if (!newHolder.TrySetContainer(nextPawn, pawn))
				{
					Log.Error("Failed set pawn container.");
					return false;
				}
				archive.savedGeneSets.Add(newHolder);
				Find.Selector.ClearSelection();
				Find.Selector.Select(nextPawn);
				phase = "do effects";
				DoEffects(nextPawn);
				//Log.Error("Spawned");
				return true;
			}
			catch (Exception arg)
			{
				Log.Error("Failed replace pawn on phase: " + phase + ". Reason: " + arg);
			}
			return false;
		}

		//private void Reimplant(Pawn pawn, bool shouldUpdXenotype = false)
		//{
		//	if (!shouldUpdXenotype)
		//	{
		//		return;
		//	}
		//	XenotypeHolder xenotypeDef = GetBestNewForm(this);
		//	if (xenotypeDef == null)
  //          {
		//		Log.Error("Failed get best form.");
		//		return;
		//	}
		//	foreach (Gene gene in pawn.genes.GenesListForReading)
		//	{
		//		RemoveGene(gene);
		//	}
		//	ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef.xenotypeDef, true);
		//	foreach (GeneDef geneDef in xenotypeDef.genes)
		//	{
		//		AddGene(geneDef, xenotypeDef.inheritable);
		//	}
		//	if (xenotypeDef.CustomXenotype)
		//	{
		//		pawn.genes.xenotypeName = xenotypeDef.Label;
		//		pawn.genes.iconDef = xenotypeDef.iconDef;
		//	}
		//}

		//private void TransferApparel(Pawn newPawn)
		//{
		//	pawn.apparel.DropAll(pawn.Position);
		//	foreach (Apparel item in pawn.apparel.WornApparel)
		//	{
		//		newPawn.apparel.Wear(item);
		//	}
		//}

		private void TransferHolders(List<PawnGeneSetHolder> oldHolders, List<PawnGeneSetHolder> newHolders)
		{
			foreach (PawnGeneSetHolder holder in oldHolders.ToList())
            {
				newHolders.Add(holder);
				oldHolders.Remove(holder);
			}
		}

		private void DestroyHoldedPawns()
		{
			foreach (PawnContainerHolder holder in savedGeneSets)
			{
				holder.innerContainer.Clear();
				holder.holded?.Destroy();
			}
			savedGeneSets = new();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			DestroyHoldedPawns();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			DestroyHoldedPawns();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if (savedGeneSets != null && savedGeneSets.RemoveAll((PawnGeneSetHolder saver) => saver is PawnContainerHolder holder && holder.IsNullContainer) > 0)
				{
					Log.Error("Removed null container(s). Holded pawn or owner is null.");
				}
			}
		}

	}

}
