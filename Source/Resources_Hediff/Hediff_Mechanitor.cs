using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_VoidMechanitor : HediffWithComps
	{

		public int refreshInterval = 56282;

		private HediffStage curStage;

		public override bool ShouldRemove => pawn.mechanitor == null;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_Voidlink cachedVoidlinkGene;

		public Gene_Voidlink Voidlink
		{
			get
			{
				if (cachedVoidlinkGene == null || !cachedVoidlinkGene.Active)
				{
					cachedVoidlinkGene = pawn?.genes?.GetFirstGeneOfType<Gene_Voidlink>();
				}
				return cachedVoidlinkGene;
			}
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					if (Voidlink != null && pawn.mechanitor != null)
					{
						int allMechsCount = pawn.mechanitor.UsedBandwidth;
						curStage.statOffsets = new List<StatModifier>
						{
							new()
							{
								stat = StatDefOf.MechBandwidth,
								value = allMechsCount + 1f
							}
						};
						curStage.statFactors = new List<StatModifier>
						{
							//new()
							//{
							//	stat = WVC_GenesDefOf.MechFormingSpeed,
							//	value = 0.1f
							//},
							new()
							{
								stat = StatDefOf.SocialImpact,
								value = 1f - (0.02f * allMechsCount)
							},
							new()
							{
								stat = StatDefOf.MentalBreakThreshold,
								value = 1f - (0.005f * allMechsCount)
							}
						};
						float talkCap = 1f / (allMechsCount + 1);
						curStage.capMods = new()
						{
							new()
							{
								capacity = PawnCapacityDefOf.Talking,
								setMax = (talkCap > 0.2f ? talkCap : 0.2f)
							}
                        };
                    }
                }
				return curStage;
			}
		}

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(refreshInterval))
			{
				return;
			}
			Recache();
		}

		public void Recache()
		{
			if (Voidlink == null)
			{
				pawn?.health?.RemoveHediff(this);
			}
			curStage = null;
		}

	}

	public class HediffWithComps_VoidMechanoid : HediffWithComps
	{

		public override bool ShouldRemove => Voidlink == null;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_Voidlink cachedVoidlinkGene;

		public Gene_Voidlink Voidlink
		{
			get
			{
				if (cachedVoidlinkGene == null || !cachedVoidlinkGene.Active)
				{
					cachedVoidlinkGene = pawn?.GetOverseer()?.genes?.GetFirstGeneOfType<Gene_Voidlink>();
				}
				return cachedVoidlinkGene;
			}
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo);
			Voidlink.UpdHediff();
			if (pawn.Corpse?.Map != null)
			{
				MiscUtility.DoSkipEffects(pawn.Corpse.Position, pawn.Corpse.Map);
				pawn.Corpse.Destroy();
			}
		}

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(600))
			{
				return;
			}
			MechRepairUtility.RepairTick(pawn);
		}

	}

}
