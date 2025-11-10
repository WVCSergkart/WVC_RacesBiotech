using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompTargetable_HumanCanUseSerum : CompProperties_Targetable
	{
		public CompTargetable_HumanCanUseSerum()
		{
			compClass = typeof(CompHumanCanUseSerum);
		}
	}

	[Obsolete]
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
				validator = (TargetInfo x) => ValidateTarget(x.Thing) && x.Thing is Pawn pawn && ReimplanterUtility.IsHuman(pawn)
			};
		}

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

}
