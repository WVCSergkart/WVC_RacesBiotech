using RimWorld;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_SkeletonDuplicate : HediffWithComps_MutantDuplicate
	{

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			if (Source != null)
			{
				DuplicateUtility.CopySkills(Source, pawn);
			}
		}

		private HediffStage curStage;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					try
					{
						curStage = new();
						curStage.statOffsets = def.stages[CurStageIndex]?.statOffsets;
						curStage.statFactors = def.stages[CurStageIndex]?.statFactors;
						curStage.capMods = def.stages[CurStageIndex]?.capMods;
						curStage.totalBleedFactor = 0f;
						curStage.forgetMemoryThoughtMtbDays = 1;
						curStage.blocksInspirations = true;
						curStage.preventVacuumBurns = true;
						curStage.opinionOfOthersFactor = 0f;
						curStage.regeneration = 10;
						curStage.painFactor = 0f;
						//def.preventsDeath = true;
					}
					catch
					{
						try
						{
							curStage = def.stages[CurStageIndex];
							Log.Warning("Failed set curStage for HediffWithComps_FleshmassDuplicate. On def: " + def.defName);
						}
						catch
						{
							curStage = new();
						}
					}
				}
				return curStage;
			}
		}

		private Pawn cachedSource;
		private Pawn Source
		{
			get
			{
				if (cachedSource == null)
				{
					cachedSource = pawn.GetSourceCyclic();
				}
				return cachedSource;
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(600, delta))
			{
				HealingUtility.Regeneration(pawn, 10f, 600);
			}
			if (pawn.IsHashIntervalTick(55000, delta))
			{
				Pawn source = Source;
				if (source == null || source.Dead)
				{
					pawn.Kill();
				}
				else
				{
					DuplicateUtility.CopySkills(source, pawn);
				}
			}
		}

	}

}
