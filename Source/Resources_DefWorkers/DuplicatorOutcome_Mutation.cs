using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class DuplicatorOutcome_Mutation : DuplicatorOutcome
	{

		private HediffDef mutation;

		public override bool CanFireNow(Pawn caster, Pawn source, Pawn duplicate)
		{
			return HediffUtility.TryGetBestMutation(duplicate, out mutation);
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			if (mutation != null)
			{
				DuplicateUtility.TryMutate(duplicate, ref customLetter, ref letterDef, mutation);
			}
			else
			{
				letterDef = LetterDefOf.PositiveEvent;
			}
		}

	}

}
