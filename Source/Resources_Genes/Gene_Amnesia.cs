using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	// public class Gene_BackstoryChanger : Gene
	public class Gene_BackstoryChanger : Gene
	{

		public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>().childBackstoryDef;
		public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>().adultBackstoryDef;

		// public bool backstoryChanged = false;

		public override void PostAdd()
		{
			base.PostAdd();
			BackstoryChanger(pawn, ChildBackstoryDef, AdultBackstoryDef);
			// if (pawn?.story != null && ChildBackstoryDef != null && pawn.ageTracker.AgeBiologicalTicks >= 46800000f)
			// {
				// pawn.story.Childhood = ChildBackstoryDef;
			// }
			// if (pawn?.story != null && AdultBackstoryDef != null && pawn.ageTracker.AgeBiologicalTicks >= 46800000f)
			// {
				// pawn.story.Adulthood = AdultBackstoryDef;
			// }
			// if (pawn?.story != null && pawn.ageTracker.AgeBiologicalTicks >= 46800000f)
			// {
				// pawn.story.Childhood = WVC_GenesDefOf.WVC_RacesBiotech_Amnesia_Child;
			// }
			// if (pawn?.story != null && pawn.ageTracker.AgeBiologicalTicks >= 46800000f)
			// {
				// pawn.story.Adulthood = WVC_GenesDefOf.WVC_RacesBiotech_Amnesia_Adult;
			// }
		}

		public static void BackstoryChanger(Pawn pawn, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null)
		{
			if (pawn?.story != null)
			{
				if (pawn.story.Childhood != null)
				{
					List<BackstoryDef> blackListedBackstoryForChanger = XenotypeFilterUtility.BlackListedBackstoryForChanger();
					if (childBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Childhood))
					{
						pawn.story.Childhood = childBackstoryDef;
					}
					if (pawn.story.Adulthood != null && adultBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Adulthood))
					{
						pawn.story.Adulthood = adultBackstoryDef;
					}
				}
			}
		}
	}

	// public class Gene_Amnesia : Gene_BackstoryChanger
	// {

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// if (pawn?.story != null && !pawn.story.Childhood.IsPlayerColonyChildBackstory && pawn.ageTracker.AgeBiologicalTicks >= 46800000f)
			// {
				// pawn.story.Childhood = ChildBackstoryDef;
				// pawn.story.Adulthood = null;
			// }
		// }
	// }

}
