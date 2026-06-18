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
			foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
			{
				if (!pawn.workSettings.WorkIsActive(workTypeDef))
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
						curStage.totalBleedFactor = 0f;
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

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(60, delta))
			{
				GasUtility.AddDeadifeGas(pawn.PositionHeld, pawn.MapHeld, pawn.Faction, 60);
			}
		}

		public override void Notify_PawnKilled()
		{
			pawn.forceNoDeathNotification = true;
		}

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

		public override void Notify_Downed()
		{
			pawn.Kill();
		}

	}

}
