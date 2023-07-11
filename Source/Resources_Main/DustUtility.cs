using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public static class DustUtility
	{

		public static void OffsetDust(Pawn pawn, float offset)
		{
			if (!ModsConfig.BiotechActive)
			{
				return;
			}
			Gene_Dust gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (gene_Hemogen != null)
			{
				gene_Hemogen.Value += offset;
			}
		}

	}
}
