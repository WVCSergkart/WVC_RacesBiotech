using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class DuplicatorOutcomeDef : Def
	{

		public Type workerClass = typeof(DuplicatorOutcome);

		public float basicChance = 1f;

		[Unsaved(false)]
		private DuplicatorOutcome workerInt;

		public DuplicatorOutcome Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (DuplicatorOutcome)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

	}

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

	public class DuplicatorOutcome_DuplicateSickness : DuplicatorOutcome
	{

		public override float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			float factor = 1f;
			if (StaticCollectionsClass.cachedNonHumansCount < 10 && StaticCollectionsClass.cachedXenotypesCount < 5)
			{
				factor *= 0.05f;
			}
			return base.GetChance(caster, source, duplicate) * factor;
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.AddDuplicateSickness(source, duplicate);
		}

	}

	public class DuplicatorOutcome_OrganDecay : DuplicatorOutcome
	{

		public override float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (duplicate.health?.immunity?.AnyGeneMakesFullyImmuneTo(HediffDefOf.OrganDecayUndiagnosedDuplicaton) == true)
			{
				return base.GetChance(caster, source, duplicate) * 0.01f;
			}
			return base.GetChance(caster, source, duplicate);
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

	public class DuplicatorOutcome_CrumblingMind : DuplicatorOutcome
	{

		public override float GetChance(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (duplicate.health?.immunity?.AnyGeneMakesFullyImmuneTo(HediffDefOf.CrumblingMind) == true)
			{
				return base.GetChance(caster, source, duplicate) * 0.01f;
			}
			return base.GetChance(caster, source, duplicate);
		}

		public override bool CanFireNow(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (duplicate.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.CrumblingMind))
			{
				return false;
			}
			return base.CanFireNow(caster, source, duplicate);
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			duplicate.health.AddHediff(HediffDefOf.CrumblingMind);
		}

	}

	public class DuplicatorOutcome_Mutation : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			if (HediffUtility.TryGetBestMutation(duplicate, out HediffDef mutation))
			{
				DuplicateUtility.TryMutate(duplicate, ref customLetter, ref letterDef, mutation);
			}
			else
			{
				letterDef = LetterDefOf.PositiveEvent;
			}
		}

	}

	public class DuplicatorOutcome_RandomTrait : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.RandomizeTraits(duplicate, ref customLetter);
		}

	}

	public class DuplicatorOutcome_RandomBackstory : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.RandomizeBackstory(duplicate, ref customLetter);
		}

	}

	public class DuplicatorOutcome_Berserk : DuplicatorOutcome
	{

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			DuplicateUtility.MakeHostile(caster, source, duplicate, ref letterDef, ref customLetter);
		}

	}

}
