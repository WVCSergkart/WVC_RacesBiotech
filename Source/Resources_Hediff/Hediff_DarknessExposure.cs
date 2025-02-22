using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_DarknessExposure_Faulty : Hediff_DarknessExposure
	{

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_DarknessExposure cachedGene;

		public Gene_DarknessExposure Exposure
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_DarknessExposure>();
				}
				return cachedGene;
			}
		}

		private HediffStage curStage;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
				}
				return curStage;
			}
		}

		private int nextTick = 120;

		public override void Tick()
		{
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			nextTick = 345612;
			if (Exposure == null)
			{
				pawn?.health?.RemoveHediff(this);
			}
		}

	}

}
