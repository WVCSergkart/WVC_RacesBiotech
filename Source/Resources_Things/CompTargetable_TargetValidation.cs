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

	public class CompTargetable_HumanCanUseSerum : CompProperties_Targetable
	{
		public CompTargetable_HumanCanUseSerum()
		{
			compClass = typeof(CompHumanCanUseSerum);
		}
	}

	public class CompHumanCanUseSerum : CompTargetable
	{
		public new CompTargetable_HumanCanUseSerum Props => (CompTargetable_HumanCanUseSerum)props;

		protected override bool PlayerChoosesTarget => true;

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetItems = false,
				canTargetBuildings = false,
				validator = (TargetInfo x) => BaseTargetValidator(x.Thing) && x.Thing is Pawn pawn && SerumUtility.PawnCanUseSerums(pawn)
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

}
