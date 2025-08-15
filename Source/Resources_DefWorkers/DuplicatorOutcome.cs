using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DuplicatorOutcome
	{

		public DuplicatorOutcomeDef def;

		public virtual float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			return def.basicChance;
		}

		public virtual bool CanFireNow(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (GetChance(caster, source, duplicate) > 0f)
			{
				return true;
			}
			return false;
		}

		public virtual void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{

		}

	}

}
