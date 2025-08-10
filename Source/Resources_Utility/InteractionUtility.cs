using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class GeneInteractionsUtility
	{

		public static bool TryInteractRandomly(Pawn pawn, bool psychicInteraction, bool ignoreTalking, bool closeTarget, out Pawn otherPawn, Gene shouldHaveGeneOfType = null, InteractionDef interactionDef = null)
		{
			return TryInteractRandomly(pawn, pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where((Pawn colonist) => colonist.RaceProps.Humanlike && colonist != pawn && colonist.Spawned).ToList(), psychicInteraction, ignoreTalking, closeTarget, out otherPawn, shouldHaveGeneOfType, interactionDef);
		}

		public static bool TryInteractRandomly(Pawn pawn, List<Pawn> workingList, bool psychicInteraction, bool ignoreTalking, bool closeTarget, out Pawn otherPawn, Gene shouldHaveGeneOfType = null, InteractionDef interactionDef = null)
		{
			otherPawn = null;
			if (pawn?.Map == null || pawn.Downed)
			{
				return false;
			}
			if (!Telepath_CanInitiateRandomInteraction(pawn, ignoreTalking))
			{
				return false;
			}
			workingList.Shuffle();
			List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
			for (int i = 0; i < workingList.Count; i++)
			{
				Pawn targetPawn = workingList[i];
				if (psychicInteraction && !targetPawn.IsPsychicSensitive())
				{
					continue;
				}
				if (closeTarget && !SocialInteractionUtility.IsGoodPositionForInteraction(pawn, targetPawn))
				{
					continue;
				}
				if (shouldHaveGeneOfType != null && !XaG_GeneUtility.HasGeneOfType(shouldHaveGeneOfType, targetPawn))
				{
					continue;
				}
				if (pawn.IsCarryingPawn(targetPawn))
				{
					continue;
				}
				if (SocialInteractionUtility.CanReceiveRandomInteraction(targetPawn) && TryGetInteractionDef(pawn, allDefsListForReading, targetPawn, out InteractionDef result, interactionDef))
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

		public static bool TryGetInteractionDef(Pawn pawn, List<InteractionDef> allDefsListForReading, Pawn targetPawn, out InteractionDef result, InteractionDef interactionDef = null)
		{
			InteractionDef resultInteraction = null;
			if (interactionDef != null)
			{
				resultInteraction = interactionDef;
			}
			if (Rand.Chance(0.3f) && pawn.story.IsDisturbing)
			{
				resultInteraction = InteractionDefOf.DisturbingChat;
			}
			if (resultInteraction != null && SocialInteractionUtility.CanReceiveInteraction(targetPawn, interactionDef))
			{
				result = resultInteraction;
				return true;
			}
			return allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => !SocialInteractionUtility.CanReceiveInteraction(targetPawn, interactionDef) ? 0f : x.Worker.RandomSelectionWeight(pawn, targetPawn), out result);
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
			if (intDef.recipientXpGainSkill != null)
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
				return SocialInteractionUtility.CanInitiateRandomInteraction(pawn);
			}
			if (!Telepath_CanInitiateInteraction(pawn))
			{
				return false;
			}
			return true;
		}

		private static bool Telepath_CanInitiateInteraction(Pawn pawn)
		{
			if (!CanInteract_BasicValidation(pawn))
			{
				return false;
			}
			if (pawn.IsInteractionBlocked(null, isInitiator: true, isRandom: false))
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
			if (pawn.InAggroMentalState || pawn.IsInteractionBlocked(null, isInitiator: true, isRandom: true))
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
			if (pawn.IsMutant && pawn.mutant.Def.incapableOfSocialInteractions)
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
