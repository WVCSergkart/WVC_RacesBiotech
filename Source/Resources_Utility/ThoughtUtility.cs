using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ThoughtUtility
	{

		public static bool TryInteractRandomly(Pawn pawn)
		{
			// if (Pawn_InteractionsTracker.InteractedTooRecentlyToInteract())
			// {
				// return false;
			// }
			if (pawn?.Map == null || pawn.Downed)
			{
				return false;
			}
			if (!InteractionUtility.CanInitiateRandomInteraction(pawn))
			{
				return false;
			}
			List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			workingList.Shuffle();
			List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
			for (int i = 0; i < workingList.Count; i++)
			{
				Pawn p = workingList[i];
				if (!p.RaceProps.Humanlike)
				{
					continue;
				}
				if (!p.PawnPsychicSensitive())
				{
					continue;
				}
				if (p != pawn && CanInteractNowWith(pawn, p) && InteractionUtility.CanReceiveRandomInteraction(p) && !pawn.HostileTo(p) && allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => (!CanInteractNowWith(pawn, p, x)) ? 0f : x.Worker.RandomSelectionWeight(pawn, p), out var result))
				{
					if (TryInteractWith(pawn, p, result))
					{
						return true;
					}
					Log.Error(string.Concat(pawn, " failed to interact with ", p));
				}
			}
			return false;
		}

		public static bool CanInteractNowWith(Pawn pawn, Pawn recipient, InteractionDef interactionDef = null)
		{
			if (!pawn.IsCarryingPawn(recipient))
			{
				if (!recipient.Spawned)
				{
					return false;
				}
			}
			if (!InteractionUtility.CanInitiateInteraction(pawn, interactionDef) || !InteractionUtility.CanReceiveInteraction(recipient, interactionDef))
			{
				return false;
			}
			return true;
		}

		public static bool TryInteractWith(Pawn pawn, Pawn recipient, InteractionDef intDef)
		{
			if (pawn == recipient)
			{
				return false;
			}
			if (!CanInteractNowWith(pawn, recipient, intDef))
			{
				return false;
			}
			// List<RulePackDef> list = new();
			if (intDef.initiatorThought != null)
			{
				Pawn_InteractionsTracker.AddInteractionThought(pawn, recipient, intDef.initiatorThought);
			}
			if (intDef.recipientThought != null && recipient.needs.mood != null)
			{
				Pawn_InteractionsTracker.AddInteractionThought(recipient, pawn, intDef.recipientThought);
			}
			if (intDef.initiatorXpGainSkill != null)
			{
				pawn.skills.Learn(intDef.initiatorXpGainSkill, intDef.initiatorXpGainAmount);
			}
			if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike)
			{
				recipient.skills.Learn(intDef.recipientXpGainSkill, intDef.recipientXpGainAmount);
			}
			recipient.ideo?.IncreaseIdeoExposureIfBaby(pawn.Ideo, 0.5f);
			intDef.Worker.Interacted(pawn, recipient, null, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets);
			MoteMaker.MakeInteractionBubble(pawn, recipient, intDef.interactionMote, intDef.GetSymbol(pawn.Faction, pawn.Ideo), intDef.GetSymbolColor(pawn.Faction));
			PlayLogEntry_Interaction playLogEntry_Interaction = new(intDef, pawn, recipient, null);
			Find.PlayLog.Add(playLogEntry_Interaction);
			if (letterDef != null)
			{
				string text = playLogEntry_Interaction.ToGameStringFromPOV(pawn);
				if (!letterText.NullOrEmpty())
				{
					text = text + "\n\n" + letterText;
				}
				Find.LetterStack.ReceiveLetter(letterLabel, text, letterDef, lookTargets ?? ((LookTargets)pawn));
			}
			FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			return true;
		}

		// ==================

		public static void ThoughtFromThing(Thing parent, ThoughtDef thoughtDef, bool showEffect = true, int radius = 5)
		{
			foreach (Thing item in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, radius, useCenter: true))
			{
				if (item is not Pawn pawn || pawn.AnimalOrWildMan() || !pawn.RaceProps.IsFlesh || pawn == parent || pawn.Dead || pawn.Downed || !pawn.PawnPsychicSensitive())
				{
					continue;
				}
				if (showEffect)
				{
					Find.TickManager.slower.SignalForceNormalSpeedShort();
					SoundDefOf.PsychicPulseGlobal.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
					FleckMaker.AttachedOverlay(parent, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
				}
				pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
				// pawn.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(thoughtDef);
			}
		}

		// ============================= GENE OPINION =============================

		public static void MyOpinionAboutPawnMap(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				// Log.Error(pawn.Name.ToString() + " hate " + pawns[i].Name.ToString());
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawns[i]);
			}
		}

		public static void PawnMapOpinionAboutMe(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				pawns[i].needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawn);
			}
			if (shouldBePsySensitive && pawn.Map != null)
			{
				FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			}
		}

		public static bool CanSetOpinion(Pawn pawn, Pawn other, Gene gene, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (!other.RaceProps.Humanlike)
			{
				return false;
			}
			if (other == pawn)
			{
				return false;
			}
			if (shouldBeFamily && !pawn.relations.FamilyByBlood.Contains(other))
			{
				return false;
			}
			if (ignoreIfHasGene && XaG_GeneUtility.HasActiveGene(gene.def, other))
			{
				return false;
			}
			if (shouldBePsySensitive && !other.PawnPsychicSensitive())
			{
				return false;
			}
			if (onlySameXenotype && !GeneUtility.SameXenotype(pawn, other))
			{
				return false;
			}
			return true;
		}

	}
}
