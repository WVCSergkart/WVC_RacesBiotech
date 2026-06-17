using Verse;


namespace WVC_XenotypesAndGenes
{

	//public class HediffWithComps_DeadlifeDust : HediffWithComps
	//{

	//	private HediffStage curStage;

	//	public override bool ShouldRemove => false;
	//	public override bool Visible => true;

	//	public override void PostAdd(DamageInfo? dinfo)
	//	{
	//		base.PostAdd(dinfo);
	//		if (pawn.workSettings == null)
	//		{
	//			pawn.workSettings = new(pawn);
	//			pawn.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
	//		}
	//		foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
	//		{
	//			pawn.workSettings.SetPriority(workTypeDef, 3);
	//		}
	//	}

	//	public override HediffStage CurStage
	//	{
	//		get
	//		{
	//			if (curStage == null)
	//			{
	//				try
	//				{
	//					curStage = new();
	//					curStage.statOffsets = def.stages[CurStageIndex]?.statOffsets;
	//					curStage.statFactors = def.stages[CurStageIndex]?.statFactors;
	//					curStage.totalBleedFactor = 0f;
	//					//def.preventsDeath = true;
	//				}
	//				catch
	//				{
	//					try
	//					{
	//						curStage = def.stages[CurStageIndex];
	//						Log.Warning("Failed set curStage for HediffWithComps_DeadlifeDust. On def: " + def.defName);
	//					}
	//					catch
	//					{
	//						curStage = new();
	//					}
	//				}
	//			}
	//			return curStage;
	//		}
	//	}

	//	public override void TickInterval(int delta)
	//	{
	//		base.TickInterval(delta);
	//		if (pawn.IsHashIntervalTick(120, delta))
	//		{
	//			GasUtility.AddDeadifeGas(pawn.PositionHeld, pawn.MapHeld, pawn.Faction, 60);
	//		}
	//	}

	//	public override void Notify_PawnCorpseSpawned()
	//	{
	//		if (!pawn.Destroyed)
	//		{
	//			pawn.Destroy();
	//		}
	//		Corpse corpse = pawn.Corpse;
	//		if (corpse != null)
	//		{
	//			IntVec3 postion = corpse.PositionHeld;
	//			Map map = corpse.MapHeld;
	//			corpse.Destroy();
	//			GasUtility.AddDeadifeGas(postion, map, pawn.Faction, 60);
	//		}
	//	}

	//}

}
