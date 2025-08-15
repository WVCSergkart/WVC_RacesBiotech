using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DuplicatorOutcome_RandomBackstory : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.RandomizeBackstory(duplicate, ref customLetter);
		}

	}

}
