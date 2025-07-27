using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_ChimeraDependant : HediffWithComps
	{

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_Chimera cachedChimeraGene;

		public Gene_Chimera Chimera
		{
			get
			{
				if (cachedChimeraGene == null || !cachedChimeraGene.Active)
				{
					cachedChimeraGene = pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>();
				}
				return cachedChimeraGene;
			}
		}

		public virtual void Recache()
		{

		}

	}

	public class HediffWithComps_ChimeraLimitFromBandwidth : HediffWithComps_ChimeraDependant
	{

		public int nextTick = 4;

		private HediffStage curStage;

		public override bool ShouldRemove => pawn.mechanitor == null;

		public override bool Visible => false;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					if (Chimera != null && pawn.mechanitor != null)
					{
						float newLimit = pawn.mechanitor.TotalBandwidth - pawn.mechanitor.UsedBandwidth;
                        curStage.statOffsets = new();
						StatModifier statMod = new();
						statMod.stat = Chimera.ChimeraLimitStatDef;
						statMod.value = newLimit;
						curStage.statOffsets.Add(statMod);
                    }
                }
				return curStage;
			}
		}

		public override void PostTickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 43151, delta))
			{
				return;
			}
			Recache();
		}

		public override void Recache()
		{
			//if (Chimera == null)
			//{
			//	pawn?.health?.RemoveHediff(this);
			//}
			curStage = null;
		}

	}

}
