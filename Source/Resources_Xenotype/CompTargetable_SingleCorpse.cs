using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

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
				validator = (TargetInfo x) => x.Thing is Corpse corpse && BaseTargetValidator(x.Thing) && HumanityCheck(corpse.InnerPawn)
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}

		public static bool HumanityCheck(Pawn pawn)
		{
			if (MechanoidizationUtility.PawnIsAndroid(pawn) || !pawn.RaceProps.Humanlike || MechanoidizationUtility.PawnCannotUseSerums(pawn))
			{
				return false;
			}
			return true;
		}
	}

}