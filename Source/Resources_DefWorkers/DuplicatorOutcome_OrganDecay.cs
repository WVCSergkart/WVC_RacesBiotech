using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DuplicatorOutcome_OrganDecay : DuplicatorOutcome
	{

		public override float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			float factor = 1f;
			if (Find.Storyteller?.difficulty != null)
			{
				factor *= Find.Storyteller.difficulty.threatScale;
			}
			if (duplicate.health?.immunity?.AnyGeneMakesFullyImmuneTo(HediffDefOf.OrganDecayUndiagnosedDuplicaton) == true)
			{
				factor *= 0.01f;
			}
			return base.GetChance(caster, source, duplicate) * factor;
		}

		public override bool CanFireNow(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (duplicate.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecay) || duplicate.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecayCreepjoiner))
			{
				return false;
			}
			return base.CanFireNow(caster, source, duplicate);
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			duplicate.health.AddHediff(HediffDefOf.OrganDecayUndiagnosedDuplicaton);
		}

	}

}
