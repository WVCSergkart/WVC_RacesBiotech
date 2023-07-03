using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityGeneIsActive : CompProperties_AbilityEffect
	{
		public List<GeneDef> anyOfGenes;

		public List<GeneDef> eachOfGenes;

		public CompProperties_AbilityGeneIsActive()
		{
			compClass = typeof(CompAbilityEffect_GeneIsActive);
		}
	}

}
