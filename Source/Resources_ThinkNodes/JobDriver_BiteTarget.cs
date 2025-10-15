using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_CastBloodfeedMelee : JobDriver_CastAbility, IJobCustomEater
	{

		public virtual bool Finalize => true;

		public bool ShouldFinalize => Finalize;

        public Pawn Victim
        {
            get
            {
				Thing target = job.GetTarget(TargetIndex.A).Thing;
				if (target is Corpse corpse)
                {
					return corpse.InnerPawn;
                }
                return (Pawn)target;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.GetTarget(TargetIndex.A).Thing, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(() => !job.ability.CanCast && !job.ability.Casting || !Victim.Dead && Victim.health.hediffSet.HasHediff(HediffDefOf.BloodLoss));
			Ability ability = ((Verb_CastAbility)job.verbToUse).ability;
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOn(() => !ability.CanApplyOn(job.targetA));
			yield return Toils_Combat.CastVerb(TargetIndex.A, TargetIndex.B, canHitNonTargetPawns: false);
		}

		public override void Notify_Starting()
		{
			base.Notify_Starting();
			job.ability?.Notify_StartedCasting();
		}

	}

	//public class JobDriver_CastCellsfeedMelee : JobDriver_CastBloodfeedMelee
	//{

	//	public override bool Finalize => false;

	//	protected override IEnumerable<Toil> MakeNewToils()
	//	{
	//		this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
	//		this.FailOn(() => !job.ability.CanCast && !job.ability.Casting);
	//		Ability ability = ((Verb_CastAbility)job.verbToUse).ability;
	//		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOn(() => !ability.CanApplyOn(job.targetA));
	//		yield return Toils_Combat.CastVerb(TargetIndex.A, TargetIndex.B, canHitNonTargetPawns: false);
	//	}

	//}

}
