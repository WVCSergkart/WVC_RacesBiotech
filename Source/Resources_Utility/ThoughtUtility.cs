using System;
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

		// ==================

		public static void ThoughtFromThing(Thing parent, ThoughtDef thoughtDef, bool showEffect = true, int radius = 5)
		{
			foreach (Thing item in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, radius, useCenter: true))
			{
				if (item is not Pawn pawn || pawn.AnimalOrWildMan() || !pawn.RaceProps.IsFlesh || pawn == parent || pawn.Dead || pawn.Downed || !pawn.IsPsychicSensitive())
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
			if (shouldBePsySensitive && !other.IsPsychicSensitive())
			{
				return false;
			}
			if (onlySameXenotype && !GeneUtility.SameXenotype(pawn, other))
			{
				return false;
			}
			return true;
		}

		// Mute voice

		public static bool TryInteractRandomly(Pawn pawn, bool psychicInteraction, bool ignoreTalking, bool closeTarget, out Pawn otherPawn, Gene shouldHaveGeneOfType = null)
		{
			// if (Pawn_InteractionsTracker.InteractedTooRecentlyToInteract())
			// {
			// return false;
			// }
			otherPawn = null;
			if (pawn?.Map == null || pawn.Downed)
            {
                return false;
            }
            if (!Telepath_CanInitiateRandomInteraction(pawn, ignoreTalking))
			{
				return false;
			}
			List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			workingList.Shuffle();
			//List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
			List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
			for (int i = 0; i < workingList.Count; i++)
            {
                Pawn targetPawn = workingList[i];
                if (!targetPawn.RaceProps.Humanlike)
                {
                    continue;
                }
                if (psychicInteraction && !targetPawn.IsPsychicSensitive())
                {
                    continue;
                }
                if (closeTarget && !InteractionUtility.IsGoodPositionForInteraction(pawn, targetPawn))
                {
                    continue;
                }
                if (shouldHaveGeneOfType != null && !XaG_GeneUtility.HasGeneOfType(shouldHaveGeneOfType, targetPawn))
                {
                    continue;
                }
                if (targetPawn != pawn && Telepath_CanInteractNowWith(pawn, targetPawn, ignoreTalking: ignoreTalking) && InteractionUtility.CanReceiveRandomInteraction(targetPawn) && !pawn.HostileTo(targetPawn) && TryGetInteractionDef(pawn, ignoreTalking, allDefsListForReading, targetPawn, out InteractionDef result))
                {
                    if (TryInteractWith(pawn, targetPawn, result, psychicInteraction))
                    {
                        otherPawn = targetPawn;
                        return true;
                    }
                    Log.Error(string.Concat(pawn, " failed to interact with ", targetPawn));
                }
            }
            return false;
		}

        public static bool TryGetInteractionDef(Pawn pawn, bool ignoreTalking, List<InteractionDef> allDefsListForReading, Pawn targetPawn, out InteractionDef result)
		{
			if (pawn.story.IsDisturbing)
			{
				result = InteractionDefOf.DisturbingChat;
				return true;
			}
			return allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => (!Telepath_CanInteractNowWith(pawn, targetPawn, ignoreTalking: ignoreTalking, x)) ? 0f : x.Worker.RandomSelectionWeight(pawn, targetPawn), out result);
        }

        public static bool TryInteractWith(Pawn pawn, Pawn recipient, InteractionDef intDef, bool psychicInteraction)
		{
			if (pawn == recipient)
			{
				return false;
			}
			//if (!Telepath_CanInteractNowWith(pawn, recipient, intDef, ignoreTalking))
			//{
			//	return false;
			//}
			// List<RulePackDef> list = new();
			if (intDef.initiatorThought != null)
			{
				Pawn_InteractionsTracker.AddInteractionThought(pawn, recipient, intDef.initiatorThought);
			}
			if (intDef.recipientThought != null && recipient.needs?.mood != null)
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
			intDef.Worker.Interacted(pawn, recipient, new List<RulePackDef>(), out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets);
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
			if (psychicInteraction)
			{
				FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			}
			return true;
		}

		private static bool Telepath_CanInitiateRandomInteraction(Pawn pawn, bool ignoreTalking)
		{
			if (!ignoreTalking)
			{
				return InteractionUtility.CanInitiateRandomInteraction(pawn);
			}
			if (!Telepath_CanInitiateInteraction(pawn, ignoreTalking: ignoreTalking))
			{
				return false;
			}
			return true;
		}

		private static bool Telepath_CanInteractNowWith(Pawn pawn, Pawn recipient, bool ignoreTalking, InteractionDef interactionDef = null)
		{
			if (!pawn.IsCarryingPawn(recipient) && !recipient.Spawned)
			{
				return false;
			}
			if (!Telepath_CanInitiateInteraction(pawn, ignoreTalking, interactionDef) || !InteractionUtility.CanReceiveInteraction(recipient, interactionDef))
			{
				return false;
			}
			return true;
		}

		private static bool Telepath_CanInitiateInteraction(Pawn pawn, bool ignoreTalking, InteractionDef interactionDef = null)
		{
			if (!ignoreTalking)
			{
				return InteractionUtility.CanInitiateInteraction(pawn);
			}
			if (!CanInteract_BasicValidation(pawn))
			{
				return false;
			}
			if (pawn.IsInteractionBlocked(interactionDef, isInitiator: true, isRandom: false))
			{
				return false;
			}
			return true;
		}

		public static bool CanInteract_BasicValidation(Pawn pawn)
		{
			if (pawn.interactions == null)
			{
				return false;
			}
			if (!pawn.RaceProps.Humanlike || pawn.InAggroMentalState || pawn.IsInteractionBlocked(null, isInitiator: true, isRandom: true))
			{
				return false;
			}
			if (!pawn.ageTracker.CurLifeStage.canInitiateSocialInteraction)
			{
				return false;
			}
			if (pawn.Faction == null)
			{
				return false;
			}
			if (pawn.Inhumanized())
			{
				return false;
			}
			if (pawn.IsMutant)
			{
				return false;
			}
			if (!pawn.Awake())
			{
				return false;
			}
			if (pawn.IsBurning())
			{
				return false;
			}
			return true;
		}

	}
}
