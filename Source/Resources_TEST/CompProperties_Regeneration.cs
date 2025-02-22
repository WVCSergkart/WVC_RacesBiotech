using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_Regeneration : CompProperties
	{

		public float regenAmount = 10f;

		public CompProperties_Regeneration()
		{
			compClass = typeof(CompRegeneration);
		}

	}

	public class CompRegeneration : ThingComp
	{

		public CompProperties_Regeneration Props => (CompProperties_Regeneration)props;

		private int nextTick = 1500;

		public override void CompTick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 1500))
			{
				return;
			}
			HealingUtility.Regeneration(parent as Pawn, Props.regenAmount, false, 1500);
		}
	}

}
