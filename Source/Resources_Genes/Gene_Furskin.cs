using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// public class Gene_LifeStageStarted : Gene
	// {

		// public virtual void Notify_LifeStageStarted()
		// {
		// }

	// }

	// WIP

	public class Gene_Exoskin : Gene
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

	}

	public class Gene_GauntSkin : Gene_Exoskin, IGeneLifeStageStarted
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeBodyType();
		}

		public void ChangeBodyType()
		{
			if (!Active)
			{
				return;
			}
			if (pawn?.gender == Gender.Female)
			{
				if (pawn?.story?.bodyType == BodyTypeDefOf.Hulk || pawn?.story?.bodyType == BodyTypeDefOf.Fat)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
			}
			else if (pawn?.gender == Gender.Male)
			{
				if (pawn?.story?.bodyType == BodyTypeDefOf.Hulk || pawn?.story?.bodyType == BodyTypeDefOf.Fat)
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
			}
		}

		public void Notify_LifeStageStarted()
		{
			ChangeBodyType();
		}

	}

	// WIP
	// [Obsolete]
	public class Gene_BodySize : Gene
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

	}

}
