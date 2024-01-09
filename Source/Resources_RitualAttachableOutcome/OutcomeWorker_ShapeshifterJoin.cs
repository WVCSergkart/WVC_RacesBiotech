using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class RitualAttachableOutcomeEffectWorker_XenotypeRecruit : RitualAttachableOutcomeEffectWorker
	{

		private GeneExtension_General extension;

		public float recruitChance = 0.5f;
		public PawnKindDef pawnKindDef;

		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			extension = def?.GetModExtension<GeneExtension_General>();
			if (extension != null)
			{
				pawnKindDef = extension.pawnKindDef;
				recruitChance = extension.recruitChance;
			}
			if (Rand.Chance(recruitChance))
			{
				Slate slate = new();
				slate.Set("map", jobRitual.Map);
				slate.Set("overridePawnGenParams", new PawnGenerationRequest(pawnKindDef ?? PawnKindDefOf.Villager, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, jobRitual.Ritual.ideo));
				QuestUtility.GenerateQuestAndMakeAvailable(QuestScriptDefOf.WandererJoins, slate);
				extraOutcomeDesc = def.letterInfoText;
			}
			else
			{
				extraOutcomeDesc = null;
			}
		}
	}

}
