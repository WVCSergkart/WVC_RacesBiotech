using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_FleshmassDuplicate : HediffWithComps
	{

		private HediffStage curStage;

		public override bool ShouldRemove => false;
		public override bool Visible => true;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			if (pawn.workSettings == null)
			{
				pawn.workSettings = new(pawn);
				pawn.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
				pawn.workSettings.Notify_DisabledWorkTypesChanged();
			}
			DuplicateUtility.NullifyBackstory(pawn);
			if (pawn.skills == null)
			{
				pawn.skills = new(pawn);
			}
			foreach (SkillRecord skillRecord in pawn.skills.skills)
			{
				skillRecord.xpSinceLastLevel = 500;
				skillRecord.Level = 8;
			}
			foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
			{
				if (pawn.WorkTypeIsDisabled(workTypeDef))
				{
					continue;
				}
				pawn.workSettings.SetPriority(workTypeDef, 3);
			}
			pawn.playerSettings = new(pawn);
			pawn.forceNoDeathNotification = true;
			if (pawn.genes != null && pawn.mutant?.Def is XaG_MutantDef xaG_MutantDef && xaG_MutantDef.geneDefs != null)
			{
				ReimplanterUtility.SetCustomGenes(pawn, xaG_MutantDef.geneDefs, null, null, true, false);
			}
		}

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
						curStage.regeneration = 50;
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

		public static bool fleshlingsChecked = false;
		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(600, delta))
			{
				HealingUtility.Regeneration(pawn, 50f, 600);
				//if (pawn.IsNestedHashIntervalTick(600, 6000))
				//{
				//	CheckFleshlings();
				//}
			}
			if (!fleshlingsChecked)
			{
				try
				{
					CheckFleshlings();
				}
				catch
				{
					Log.Warning("Failed check fleshling mutants.");
					fleshlingsChecked = true;
				}
			}
		}

		private bool CheckFleshlings()
		{
			if (pawn.mutant == null)
			{
				//pawn.health.RemoveHediff(this);
				return false;
			}
			int mutants = 0;
			foreach (Pawn mutant in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
			{
				if (mutant.mutant != null && mutant.Faction == pawn.Faction && mutant.mutant.Def == pawn.mutant.Def)
				{
					mutants++;
				}
			}
			if (mutants > (Gene_FleshlingCreator.MasterGenes.Count * 3))
			{
				pawn.Kill();
				fleshlingsChecked = false;
			}
			else
			{
				fleshlingsChecked = true;
			}
			return true;
		}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	yield return new Command_Action
		//	{
		//		defaultLabel = MainDefOf.WVC_HiddenEntitiesWorkButton.LabelCap,
		//		defaultDesc = MainDefOf.WVC_HiddenEntitiesWorkButton.description,
		//		icon = MainDefOf.WVC_HiddenEntitiesWorkButton.Icon,
		//		action = delegate
		//		{
		//			MainDefOf.WVC_HiddenEntitiesWorkButton.Worker.InterfaceTryActivate();
		//		}
		//	};
		//}

		//public override void Notify_PawnKilled()
		//{
		//	pawn.forceNoDeathNotification = true;
		//}

		//public override void Notify_PawnCorpseSpawned()
		//{
		//	if (!pawn.Destroyed)
		//	{
		//		pawn.Destroy();
		//	}
		//	Corpse corpse = pawn.Corpse;
		//	if (corpse != null)
		//	{
		//		IntVec3 postion = corpse.PositionHeld;
		//		Map map = corpse.MapHeld;
		//		corpse.Destroy();
		//		GasUtility.AddDeadifeGas(postion, map, pawn.Faction, 60);
		//	}
		//}

		//public override void Notify_Downed()
		//{
		//	pawn.Kill();
		//}

	}

}
