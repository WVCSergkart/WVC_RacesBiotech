using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityPawnPregnant : CompProperties_AbilityEffect
	{

		// public bool prenancyPrevent = true;

		public int recacheFrequency = 9000;

		public CompProperties_AbilityPawnPregnant()
		{
			compClass = typeof(CompAbilityEffect_PawnPregnant);
		}
	}

	public class CompAbilityEffect_PawnPregnant : CompAbilityEffect
	{
		public new CompProperties_AbilityPawnPregnant Props => (CompProperties_AbilityPawnPregnant)props;

		private int nextRecache = -1;
		private string cachedReason = null;
		private bool cachedResult = false;

		private void Cache(string reason)
		{
			cachedReason = reason;
			cachedResult = reason != null;
		}

		public override bool GizmoDisabled(out string reason)
		{
			if (Find.TickManager.TicksGame < nextRecache)
			{
				reason = cachedReason;
				return cachedResult;
			}
			nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
			Pawn pawn = parent.pawn;
			Hediff pregnancy = HediffUtility.GetFirstHediffPreventsPregnancy(pawn.health.hediffSet.hediffs);
			if (pregnancy != null)
			{
				reason = "WVC_XaG_GeneXenoGestator_Disabled".Translate(pregnancy.def.label);
				Cache(reason);
				return true;
			}
			reason = null;
			Cache(reason);
			return false;
		}
	}

}
