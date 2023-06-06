using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_TEST : Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			// pawn.ageTracker.CurLifeStage.bodySizeFactor = 0.9f;
			// pawn.ageTracker.CurLifeStage.bodyWidth = 0.9f;
			// pawn.ageTracker.CurLifeStage.headSizeFactor = 0.9f;
			// pawn.ageTracker.CurLifeStage.eyeSizeFactor = 0.9f;
			// pawn.RaceProps.leatherDef = ThingDefOf.Plasteel;
		}

	}

}
