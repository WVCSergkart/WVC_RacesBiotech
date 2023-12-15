using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace WVC_XenotypesAndGenes
{

	// [StaticConstructorOnStartup]
	public static class GestationUtility
	{

		public static void GenerateNewBornPawn(Pawn pawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true, bool xenogeneTransfer = true, Pawn spawnPawn = null)
		{
			Pawn pawnParent = pawn;
			Pawn pawnTarget = spawnPawn;
			if (pawnTarget == null)
			{
				pawnTarget = pawn;
			}
			int litterSize = ((pawnParent.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawnParent.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			PawnGenerationRequest generateNewBornPawn = new(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			for (int i = 0; i < litterSize; i++)
			{
				Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
				GeneTransfer(pawnNewBornChild, pawnParent, endogeneTransfer, xenogeneTransfer);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnTarget))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestriction = pawnParent.playerSettings.AreaRestriction;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (pawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnTarget.Position, pawnTarget.Map, ThingDefOf.Filth_AmnioticFluid, pawnTarget.LabelIndefinite(), 5);
				if (pawnTarget.caller != null)
				{
					pawnTarget.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
		}

		public static void GeneTransfer(Pawn pawnNewBornChild, Pawn pawnParent, bool endogeneTransfer = true, bool xenogeneTransfer = true)
		{
			if (endogeneTransfer)
			{
				for (int numEndogenes = pawnNewBornChild.genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawnNewBornChild.genes.RemoveGene(pawnNewBornChild.genes.Endogenes[numEndogenes]);
				}
				List<Gene> list = pawnParent.genes?.Endogenes;
				foreach (Gene item in list)
				{
					pawnNewBornChild.genes?.AddGene(item.def, xenogene: false);
				}
			}
			if (xenogeneTransfer)
			{
				for (int numXenogenes = pawnNewBornChild.genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawnNewBornChild.genes.RemoveGene(pawnNewBornChild.genes.Xenogenes[numXenogenes]);
				}
				List<Gene> list = pawnParent.genes?.Xenogenes;
				foreach (Gene item in list)
				{
					pawnNewBornChild.genes?.AddGene(item.def, xenogene: true);
				}
			}
			// if (pawnParent.genes?.Xenotype != null)
			// {
			// pawnNewBornChild.genes?.SetXenotypeDirect(pawnParent.genes?.Xenotype);
			// }
			pawnNewBornChild.genes?.SetXenotypeDirect(pawnParent.genes?.Xenotype);
			pawnNewBornChild.genes.xenotypeName = pawnParent.genes.xenotypeName;
			pawnNewBornChild.genes.iconDef = pawnParent.genes.iconDef;
		}

		public static void GenerateNewBornPawn_Thing(Pawn pawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true, bool xenogeneTransfer = true, Thing spawnTarget = null)
		{
			Pawn pawnParent = pawn;
			Thing pawnTarget = spawnTarget;
			if (pawnTarget == null)
			{
				// pawnTarget = pawn;
				return;
			}
			int litterSize = ((pawnParent.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawnParent.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			PawnGenerationRequest generateNewBornPawn = new(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			for (int i = 0; i < litterSize; i++)
			{
				Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
				GeneTransfer(pawnNewBornChild, pawnParent, endogeneTransfer, xenogeneTransfer);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnTarget))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestriction = pawnParent.playerSettings.AreaRestriction;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (pawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnTarget.Position, pawnTarget.Map, ThingDefOf.Filth_Slime, 5);
				// if (pawn.caller != null)
				// {
				// pawn.caller.DoCall();
				// }
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
		}

		// Xeno-Tree Spawner

		public static void GenerateNewBornPawn_WithChosenXenotype(Thing spawnTarget, XenotypeDef xenotypeDef, string completeLetterLabel, string completeLetterDesc)
		{
			if (spawnTarget == null || xenotypeDef == null)
			{
				return;
			}
			PawnGenerationRequest generateNewBornPawn = new(PawnKindDefOf.Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
			XenotypeUtility.SetXenotype_DoubleXenotype(pawnNewBornChild, xenotypeDef);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, spawnTarget))
			{
			}
			else
			{
				Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
			}
			if (spawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(spawnTarget.Position, spawnTarget.Map, ThingDefOf.Filth_Slime, 5);
				if (pawnNewBornChild.caller != null)
				{
					pawnNewBornChild.caller.DoCall();
				}
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawnNewBornChild))
			{
				Find.LetterStack.ReceiveLetter(completeLetterLabel.Translate(), completeLetterDesc.Translate(spawnTarget.def.label.CapitalizeFirst()), LetterDefOf.PositiveEvent, new LookTargets(pawnNewBornChild));
			}
		}

	}
}
