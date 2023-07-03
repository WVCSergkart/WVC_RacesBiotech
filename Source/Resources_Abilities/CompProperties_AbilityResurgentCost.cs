using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityResurgentCost : CompProperties_AbilityEffect
	{
		public float resurgentCost = 0.2f;

		public bool skipIfNotResurgent = false;

		public CompProperties_AbilityResurgentCost()
		{
			compClass = typeof(CompAbilityEffect_ResurgentCost);
		}

		public override IEnumerable<string> ExtraStatSummary()
		{
			yield return (string)("WVC_XaG_AbilityResurgentCost".Translate() + ": ") + Mathf.RoundToInt(resurgentCost * 100f);
		}
	}

}
