// RimWorld.CompProperties_AbilityGiveHediff
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityGiveHediff : CompProperties_AbilityEffectWithDuration
	{
		public HediffDef hediffDef;

		public bool onlyBrain;

		public bool applyToSelf;

		public bool onlyApplyToSelf;

		public bool applyToTarget = true;

		public bool humanityCheck = false;

		public bool replaceExisting;

		public bool onlyReproductive;

		public float severity = -1f;
	}

}
