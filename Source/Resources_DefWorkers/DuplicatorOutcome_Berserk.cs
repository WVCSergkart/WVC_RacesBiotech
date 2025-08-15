using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DuplicatorOutcome_Berserk : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.MakeHostile(caster, source, duplicate, ref letterDef, ref customLetter);
		}

	}

}
