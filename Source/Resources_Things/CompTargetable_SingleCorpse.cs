using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompTargetable_HumanCorpse : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = false,
				canTargetBuildings = false,
				canTargetItems = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = (TargetInfo x) => x.Thing is Corpse corpse && ValidateTarget(x.Thing) && !corpse.IsUnnaturalCorpse() && ReimplanterUtility.IsHuman(corpse.InnerPawn)
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

}
