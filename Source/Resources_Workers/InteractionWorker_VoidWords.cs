using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class InteractionWorker_VoidWords : InteractionWorker
	{

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (Rand.Chance(0.7f))
			{
				return 0f;
			}
			if (!recipient.IsPsychicSensitive())
			{
				return 0f;
			}
			if (initiator?.genes?.GetFirstGeneOfType<Gene_VoidVoice>() != null)
			{
				return 999f;
			}
			return 0f;
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
			initiator.genes?.GetFirstGeneOfType<Gene_VoidVoice>()?.VoidInteraction(recipient);
		}

	}

}
