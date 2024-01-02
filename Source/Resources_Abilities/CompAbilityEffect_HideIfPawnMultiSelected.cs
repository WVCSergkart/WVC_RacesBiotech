using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityHideIfPawnMultiSelected : CompProperties_AbilityEffect
	{

		// public bool prenancyPrevent = true;

		// public int recacheFrequency = 9000;

		public CompProperties_AbilityHideIfPawnMultiSelected()
		{
			compClass = typeof(CompAbilityEffect_HideIfPawnMultiSelected);
		}
	}

	public class CompAbilityEffect_HideIfPawnMultiSelected : CompAbilityEffect
	{
		// public new CompProperties_AbilityHideIfPawnMultiSelected Props => (CompProperties_AbilityHideIfPawnMultiSelected)props;

		public override bool ShouldHideGizmo => Find.Selector.SelectedPawns.Count > 1;

	}

}
