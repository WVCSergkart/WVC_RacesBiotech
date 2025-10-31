using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

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

		public List<Pawn> ArchivedPawns
		{
			get
			{
				List<Pawn> archivedPawns = new();
				foreach (PawnGeneSetHolder holder in SavedGeneSets)
				{
					if (holder is PawnContainerHolder container && !archivedPawns.Contains(container.holded))
					{
						archivedPawns.Add(container.holded);
					}
				}
				return archivedPawns;
			}
		}

		public override bool CanMorphNow
		{
			get
			{
				if (pawn.Spawned)
				{
					return base.CanMorphNow;
				}
				return false;
			}
		}

		public override bool IsOneTime
		{
			get
			{
				return false;
			}
		}

		public override TaggedString GizmoTootip => "WVC_XaG_ArchiverGizmoTip".Translate();

		public override TaggedString WarningDesc => "WVC_XaG_GeneAbilityMorphWarning_Archiver".Translate();

		public override bool TryMorph(PawnGeneSetHolder nextGeneSet, bool shouldMorph = false, bool removeMorpher = false)
		{
			string phase = "";
			Pawn nextPawn = null;
			try
			{
				if (!shouldMorph || (nextGeneSet != null && nextGeneSet is not PawnContainerHolder))
				{
					return false;
				}
				Gene_Archiver archive = null;
				if (nextGeneSet is PawnContainerHolder target)
				{
					phase = "try drop pawn";
					BeforeDrop(target);
					TryDropNextPawn(ref nextPawn, ref archive, target);
				}
				else if (nextGeneSet == null && CanAddNewForm)
				{
					phase = "trying create new pawn";
					DuplicateUtility.TryDuplicatePawn(pawn, out nextPawn);
					phase = "trying get gene and update xenotype";
					//if (XaG_GeneUtility.TryRemoveAllConflicts(nextPawn, def))
					//{
					//	nextPawn.TryAddGene(def, pawn.genes.IsXenogene(this));
					//}
					archive = nextPawn.genes?.GetFirstGeneOfType<Gene_Archiver>();
					archive.ClearGenes();
					archive.Reimplant(archive.GetBestNewFormForMorpher());
					UpdSkinAndHair(nextPawn);
				}
				else if (!SavedGeneSets.NullOrEmpty())
				{
					phase = "try drop pawn";
					TryDropNextPawn(ref nextPawn, ref archive, SavedGeneSets.RandomElement() as PawnContainerHolder);
				}
				if (nextPawn == null)
				{
					Log.Error("Failed morph into next pawn, cause nextPawn is null.");
					return false;
				}
				if (archive == null)
				{
					Log.Warning("Failed get Gene_Archiver. Trying added.");
					nextPawn.genes?.AddGene(def, !nextPawn.genes.Xenotype.inheritable);
					archive = nextPawn.genes?.GetFirstGeneOfType<Gene_Archiver>();
					if (archive == null)
					{
						Log.Error("Failed get Gene_Archiver.");
						nextPawn?.Destroy();
						return false;
					}
				}
				phase = "abjust rotation and upd tools";
				archive.UpdToolGenes();
				bool isSelected = Find.Selector.IsSelected(pawn) && Find.Selector.SelectedObjectsListForReading.Count == 1;
				nextPawn.Rotation = pawn.Rotation;
				nextPawn.stances.stunner.StunFor(60, null, addBattleLog: false);
				phase = "trying upd equipment";
				ApparelUpd(pawn, nextPawn);
				WeaponUpd(pawn, nextPawn);
				InventoryUpd(pawn, nextPawn);
				phase = "trying transfer holders";
				TransferHolders(this, archive, nextPawn);
				phase = "trying create new holder";
				if (!TryArchiveSelectedPawn(pawn, nextPawn, archive))
				{
					return false;
				}
				if (isSelected)
				{
					Find.Selector.ClearSelection();
					Find.Selector.Select(nextPawn);
				}
				phase = "upd new pawn";
				PostMorph(nextPawn, pawn);
				phase = "do effects";
				DoEffects(nextPawn);
				//Log.Error("Spawned");
				return true;
			}
			catch (Exception arg)
			{
				Log.Error("Failed replace pawn on phase: " + phase + ". Reason: " + arg);
				nextPawn?.Destroy();
			}
			return false;
		}

		public bool TryArchiveSelectedPawn(Pawn toHold, Pawn nextOwner, Gene_Archiver archive)
		{
			PawnContainerHolder newHolder = new();
			if (toHold.genes?.GetFirstGeneOfType<Gene_Archiver>() == null)
			{
				toHold.genes.AddGene(archive.def, !toHold.genes.Xenotype.inheritable);
			}
			archive.SaveFormID(newHolder);
			//newHolder.formId = SavedGeneSets.Count + 1;
			if (!newHolder.TrySetContainer(nextOwner, toHold))
			{
				Log.Error("Failed set pawn container.");
				nextOwner?.Destroy();
				return false;
			}
			archive.AddSetHolder(newHolder);
			return true;
		}


		private void ApparelUpd(Pawn oldPawn, Pawn newPawn)
		{
			if (!WVC_Biotech.settings.archiver_transferWornApparel)
			{
				return;
			}
			if (oldPawn.apparel.AnyApparelLocked || newPawn.apparel.AnyApparelLocked)
			{
				return;
			}
			foreach (var item in oldPawn.apparel.WornApparel.ToList())
			{
				newPawn.apparel.WornApparel.Add(item);
				oldPawn.apparel.WornApparel.Remove(item);
			}
		}

		private void WeaponUpd(Pawn oldPawn, Pawn newPawn)
		{
			if (!WVC_Biotech.settings.archiver_transferEquipedWeapon)
			{
				return;
			}
			foreach (var item in oldPawn.equipment.AllEquipmentListForReading.ToList())
			{
				newPawn.equipment.AllEquipmentListForReading.Add(item);
				oldPawn.equipment.AllEquipmentListForReading.Remove(item);
			}
		}

		private void InventoryUpd(Pawn oldPawn, Pawn newPawn)
		{
			if (!WVC_Biotech.settings.archiver_transferEquipedWeapon || !WVC_Biotech.settings.archiver_transferWornApparel)
			{
				return;
			}
			oldPawn.inventory.GetDirectlyHeldThings().TryTransferAllToContainer(newPawn.inventory.GetDirectlyHeldThings());
		}

		private void BeforeDrop(PawnContainerHolder holder)
		{
			int ticks = Find.TickManager.TicksGame - holder.lastTimeSeenByPlayer;
			if (ticks > 0)
			{
				HealingUtility.Regeneration(holder.holded, regeneration: 50, tick: ticks);
			}
		}

		public override void TransferHolders(Gene_Morpher oldMorpher, Gene_Morpher newMorpher, Pawn newOwner)
		{
			base.TransferHolders(oldMorpher, newMorpher, newOwner);
			foreach (PawnGeneSetHolder holder in newMorpher.SavedGeneSets)
			{
				if (holder is PawnContainerHolder container)
				{
					container.owner = newOwner;
				}
			}
		}

		private void TryDropNextPawn(ref Pawn nextPawn, ref Gene_Archiver archive, PawnContainerHolder target)
		{
			if (target.innerContainer != null && target.innerContainer.TryDropAll(pawn.Position, pawn.Map, ThingPlaceMode.Direct, playDropSound: false))
			{
				nextPawn = target.holded;
				if (!nextPawn.Spawned)
				{
					GenSpawn.Spawn(nextPawn, pawn.Position, pawn.Map, pawn.Rotation);
				}
				archive = nextPawn.genes?.GetFirstGeneOfType<Gene_Archiver>();
			}
			RemoveSetHolder(target);
		}

		private void PostMorph(Pawn newPawn, Pawn oldPawn)
		{
			newPawn.ideo?.SetIdeo(oldPawn.ideo.Ideo);
			//if (newPawn.ideo != null)
			//{
			//	newPawn.ideo?.SetIdeo(oldPawn.ideo.Ideo);
			//	newPawn.ideo.Debug_ReduceCertainty(Mathf.Clamp01(newPawn.ideo.Certainty - oldPawn.ideo.Certainty));
			//}
			foreach (Need newNeed in newPawn.needs.AllNeeds)
			{
				if (newNeed.def.onlyIfCausedByGene || newNeed.def.onlyIfCausedByHediff || newNeed.def.onlyIfCausedByTrait)
				{
					continue;
				}
				newNeed.CurLevel = newNeed.MaxLevel;
				foreach (Need oldNeed in oldPawn.needs.AllNeeds)
				{
					if (newNeed.def != oldNeed.def)
					{
						continue;
					}
					newNeed.CurLevel = Mathf.Clamp(oldNeed.CurLevel, 0f, newNeed.MaxLevel);
				}
			}
			//if (oldPawn.needs?.mood?.thoughts?.memories != null)
			//{
			//	foreach (Thought_Memory memory in oldPawn.needs.mood.thoughts.memories.Memories)
			//	{
			//		if (!memory.permanent)
			//		{
			//			newPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(memory.def, memory.otherPawn);
			//		}
			//	}
			//}
			foreach (Gene gene in newPawn.genes.GenesListForReading)
			{
				if (gene is not Gene_MorpherDependant postMorphGene)
				{
					continue;
				}
				postMorphGene.PostMorph(newPawn);
			}
			MiscUtility.Notify_DebugPawn(pawn);
			//pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		private void UpdSkinAndHair(Pawn nextPawn)
		{
			ReimplanterUtility.FindSkinAndHairGenes(nextPawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor);
			if (!xenotypeHasSkinColor)
			{
				GeneDef skinDef = GetSkinDef();
				if (skinDef != null)
				{
					recipientGenes?.AddGene(skinDef, false);
				}
			}
			if (!xenotypeHasHairColor)
			{
				GeneDef hairDef = GetHairDef();
				if (hairDef != null)
				{
					recipientGenes?.AddGene(hairDef, false);
				}
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		}

		private GeneDef GetHairDef()
		{
			if (!pawn.genes.Endogenes.NullOrEmpty())
			{
				List<Gene> geneDefs = pawn.genes.Endogenes?.Where((Gene gene) => gene.def.hairColorOverride != null)?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First().def;
			}
			Log.Warning("Failed get hair color for pawn. Color endogenes is null.");
			return null;
		}

		private GeneDef GetSkinDef()
		{
			if (!pawn.genes.Endogenes.NullOrEmpty())
			{
				List<Gene> geneDefs = pawn.genes.Endogenes?.Where((Gene gene) => gene.def.skinColorBase != null || gene.def.skinColorOverride != null)?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First().def;
			}
			Log.Warning("Failed get skin color for pawn. Color endogenes is null.");
			return null;
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
				//pawnHolder.holded?.Discard(true);
				//pawnHolder.holded?.Kill(null);
				pawnHolder.holded?.duplicate?.Notify_PawnKilled();
				pawnHolder.holded?.Destroy();
				pawnHolder.innerContainer.Clear();
			}
			ResetAllSetHolders();
		}

		//public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		//{
		//    base.Notify_PawnDied(dinfo, culprit);
		//    DestroyHoldedPawns();
		//}

		public override void PostRemove()
		{
			base.PostRemove();
			DestroyHoldedPawns();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (SavedGeneSets != null && SavedGeneSets.RemoveAll((PawnGeneSetHolder saver) => saver is PawnContainerHolder holder && holder.IsNullContainer) > 0)
				{
					Log.Error("Removed null container(s). Holded pawn or owner is null.");
				}
			}
		}

		public override void StoreGeneSet(PawnGeneSetHolder geneSet, Gene_StorageImplanter storage)
		{
			if (geneSet is PawnContainerHolder pawnHolder)
			{
				Pawn_GeneTracker geneTracker = pawnHolder.holded.genes;
				List<GeneDef> selectedGenes = new();
				if (!geneTracker.Xenogenes.NullOrEmpty())
				{
					selectedGenes.AddRange(XaG_GeneUtility.ConvertToDefs(geneTracker.Xenogenes));
				}
				if (!geneTracker.Endogenes.NullOrEmpty())
				{
					selectedGenes.AddRange(XaG_GeneUtility.ConvertToDefs(geneTracker.Endogenes));
				}
				if (!selectedGenes.Contains(def))
				{
					selectedGenes.Add(def);
				}
				storage.SetupHolder(XenotypeDefOf.Baseliner, selectedGenes, geneTracker.Xenotype.inheritable, geneTracker.iconDef, null);
			}
		}

	}

	public class Gene_ArchiverMindsConnection : Gene_Telepathy
	{


		[Unsaved(false)]
		private Gene_Archiver cachedArchiverGene;
		public Gene_Archiver Archiver
		{
			get
			{
				if (cachedArchiverGene == null || !cachedArchiverGene.Active)
				{
					cachedArchiverGene = pawn?.genes?.GetFirstGeneOfType<Gene_Archiver>();
				}
				return cachedArchiverGene;
			}
		}

		public override void TryInteract()
		{
			if (GeneInteractionsUtility.TryInteractRandomly(pawn, Archiver.ArchivedPawns, true, true, false, out _) && pawn.needs?.joy != null)
			{
				pawn.needs.joy.CurLevelPercentage += 0.02f;
				if (Rand.Chance(0.05f) && pawn.skills.skills.Where((SkillRecord skill) => !skill.TotallyDisabled && skill.Level < 20).TryRandomElement(out SkillRecord resultSkill))
				{
					resultSkill.Learn(resultSkill.XpRequiredForLevelUp * 0.2f);
				}
			}
			ResetInterval(new(6600, 22000));
		}

	}

}
