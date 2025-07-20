using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class QuestNode_Root_MechakinJoin_WalkIn : RimWorld.QuestGen.QuestNode_Root_WandererJoin_WalkIn
	{

		public override Pawn GeneratePawn()
		{
			Slate slate = QuestGen.slate;
			Gender? gender = null;
			if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out var var))
			{
				PawnKindDef villager = PawnKindDefOf.Villager;
				Gender? fixedGender = gender;
				var = new PawnGenerationRequest(villager, null, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, forceRecruitable: true);
			}
			if (Find.Storyteller.difficulty.ChildrenAllowed)
			{
				var.AllowedDevelopmentalStages |= DevelopmentalStage.Child;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(var);
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, DefDatabase<XenotypeDef>.GetNamed("WVC_Meca"));
			if (!pawn.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn);
			}
			return pawn;
		}

	}

}
