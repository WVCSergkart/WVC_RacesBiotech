using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_VoidMechanitor : HediffWithComps
	{

		public int nextTick = 4;

		private HediffStage curStage;

		private int? allMechsCount;

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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref allMechsCount, "allMechsCount");
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
						if (!allMechsCount.HasValue)
						{
							allMechsCount = pawn.mechanitor.UsedBandwidth;
						}
						curStage.statOffsets = new List<StatModifier>
						{
							new()
							{
								stat = StatDefOf.MechBandwidth,
								value = allMechsCount.Value + 1f
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
								value = 1f - (0.02f * allMechsCount.Value)
							},
							new()
							{
								stat = StatDefOf.MentalBreakThreshold,
								value = 1f - (0.005f * allMechsCount.Value)
							}
						};
						float talkCap = 1f - (allMechsCount.Value * 0.02f);
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

		//public override void PostTickInterval(int delta)
		//{
		//	if (!pawn.IsHashIntervalTick(nextTick, delta))
		//	{
		//		return;
		//	}
		//	Recache();
		//}

		public override void PostTickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 56282, delta))
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
			allMechsCount = null;
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
			Voidlink.CacheReset(false);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_Gene_VoidlinkKillVoidMechLabel".Translate(),
				defaultDesc = "WVC_XaG_Gene_VoidlinkKillVoidMechDesc".Translate(),
				icon = GeneGizmo_Voidlink.KillMechsIcon.Texture,
				action = delegate
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Gene_VoidlinkKillVoidMechDesc".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
					{
						Voidlink.KillSelectedMech(true, pawn);
					});
					Find.WindowStack.Add(window);
				}
			};
		}

		public override void PostTickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(600, delta))
			{
				return;
			}
			MechRepairUtility.RepairTick(pawn, delta);
		}

	}

}
