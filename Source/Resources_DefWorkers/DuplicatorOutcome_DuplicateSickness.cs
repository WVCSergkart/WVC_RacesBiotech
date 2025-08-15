using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DuplicatorOutcome_DuplicateSickness : DuplicatorOutcome
	{

		public override float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			float factor = 1f;
			if (StaticCollectionsClass.cachedPlayerPawnsCount < 8 || StaticCollectionsClass.cachedDuplicatesCount < 2)
			{
				factor *= 0.10f;
			}
			if (Find.Storyteller?.difficulty != null)
			{
				factor *= Find.Storyteller.difficulty.threatScale;
			}
			return base.GetChance(caster, source, duplicate) * factor;
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.AddDuplicateSickness(source, duplicate);
		}

	}

}
