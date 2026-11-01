using Verse;


namespace WVC_XenotypesAndGenes
{
	public class HediffWithComps_MutantDuplicate : HediffWithComps
	{

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
				if (pawn.WorkTypeIsDisabled(workTypeDef))
				{
					continue;
				}
				pawn.workSettings.SetPriority(workTypeDef, 3);
			}
			pawn.playerSettings = new(pawn);
			if (pawn.genes != null && pawn.mutant?.Def is XaG_MutantDef xaG_MutantDef && xaG_MutantDef.geneDefs != null)
			{
				ReimplanterUtility.SetCustomGenes(pawn, xaG_MutantDef.geneDefs, null, null, true, false);
			}
			MainButtonWorker_AutoInitTab.visible = true;
		}

		public override void Notify_PawnKilled()
		{
			base.Notify_PawnKilled();
			MainButtonWorker_AutoInitTab.visible = null;
		}

	}

}
