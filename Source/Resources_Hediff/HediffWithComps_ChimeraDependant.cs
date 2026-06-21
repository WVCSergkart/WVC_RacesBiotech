using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_ChimeraDependant : HediffWithComps
	{

		public override bool Visible => false;

		protected HediffStage curStage;

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
			curStage = null;
		}

	}

	public class HediffWithComps_DisabledGenes : HediffWithComps_ChimeraDependant
	{

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					if (ModsUtility.GameNotStarted())
					{
						return new();
					}
					curStage = new();
					if (Chimera != null && def is XaG_HediffDef newDef && newDef.statModifiers != null)
					{
						if (!newDef.statModifiers.statFactors.NullOrEmpty())
						{
							curStage.statFactors = new();
							foreach (StatModifier item in newDef.statModifiers.statFactors)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								float factor = 1f - (item.value * Chimera.DisabledGenes.Count);
								statModifier.value = factor > 0f ? factor : 0f;
								curStage.statFactors.Add(statModifier);
							}
						}
						if (!newDef.statModifiers.statOffsets.NullOrEmpty())
						{
							curStage.statOffsets = new();
							foreach (StatModifier item in newDef.statModifiers.statOffsets)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								statModifier.value = item.value * Chimera.DisabledGenes.Count;
								curStage.statOffsets.Add(statModifier);
							}
						}
					}
				}
				return curStage;
			}
		}

	}

	[Obsolete]
	public class HediffWithComps_ChimeraLimitFromBandwidth : HediffWithComps_ChimeraDependant
	{

		public int nextTick = 4;

		//private HediffStage curStage;

		public override bool ShouldRemove => false;

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

	[Obsolete]
	public class HediffWithComps_ChimeraLimitFromHiveMind : HediffWithComps_ChimeraDependant
	{

		public int nextTick = 22;

		private new static HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					if (Chimera != null)
					{
						float newLimit = HivemindUtility.HivemindPawns.Count;
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
			if (!GeneResourceUtility.CanTick(ref nextTick, 143151, delta))
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
