using RimWorld;
using RimWorld.Planet;
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

		public override bool IsOneTime
		{
			get
			{
				return false;

			}
		}

		public override TaggedString GizmoTootip => "WVC_XaG_ArchiverGizmoTip".Translate();

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
				if (target == null && !SavedGeneSets.NullOrEmpty())
				{
					phase = "trying random holder";
					target = SavedGeneSets.RandomElement() as PawnContainerHolder;
				}
				if (target != null)
				{
					phase = "trying drop pawn";
					if (target.innerContainer.TryDropAll(pawn.Position, pawn.Map, ThingPlaceMode.Direct))
					{
						RemoveSetHolder(target);
						nextPawn = target.holded;
						if (!nextPawn.Spawned)
						{
							GenSpawn.Spawn(nextPawn, pawn.Position, pawn.Map);
						}
					}
				}
				else if (SavedGeneSets.Count < CurrentLimit + 1)
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
				TransferHolders(this, archive);
				//phase = "upd pawn apparel";
				//TransferApparel(nextPawn);
				phase = "trying create new holder";
				//Log.Error("Create new holder");
				PawnContainerHolder newHolder = new();
				newHolder.formId = SavedGeneSets.Count + 1;
				if (!newHolder.TrySetContainer(nextPawn, pawn))
				{
					Log.Error("Failed set pawn container.");
					return false;
				}
				archive.AddSetHolder(newHolder);
				Find.Selector.ClearSelection();
				Find.Selector.Select(nextPawn);
				phase = "upd new pawn";
				UpdNewPawn(nextPawn, pawn);
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

        private void UpdNewPawn(Pawn newPawn, Pawn oldPawn)
        {
			if (oldPawn.ideo != null)
            {
				newPawn.ideo.SetIdeo(oldPawn.ideo.Ideo);
			}
			//if (oldPawn.needs?.mood?.thoughts?.memories != null)
			//{
   //             foreach (Thought_Memory memory in oldPawn.needs.mood.thoughts.memories.Memories)
   //             {
			//		if (!memory.permanent)
			//		{
			//			newPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(memory.def, memory.otherPawn);
			//		}
   //             }
   //         }
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

        private void DestroyHoldedPawns()
		{
			foreach (PawnGeneSetHolder holder in SavedGeneSets)
			{
				if (holder is not PawnContainerHolder pawnHolder)
                {
					continue;
				}
				//Log.Error("Try destroy duplicates");
				pawnHolder.holded?.Destroy();
				pawnHolder.innerContainer.Clear();
			}
			ResetAllSetHolders();
		}

		//public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		//{
		//	base.Notify_PawnDied(dinfo, culprit);
		//	DestroyHoldedPawns();
		//}

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
				if (SavedGeneSets != null && SavedGeneSets.RemoveAll((PawnGeneSetHolder saver) => saver is PawnContainerHolder holder && holder.IsNullContainer) > 0)
				{
					Log.Error("Removed null container(s). Holded pawn or owner is null.");
				}
			}
		}

	}

}
