using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityObsolete : CompProperties_AbilityEffect
	{


	}

	public class CompAbilityEffect_Obsolete : CompAbilityEffect
	{
		public override void CompTick()
		{
			parent.pawn?.abilities?.RemoveAbility(parent.def);
		}

	}

}
