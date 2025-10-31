using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_RestStomach : Gene_AddOrRemoveHediff
	{

		public override bool Active
		{
			get
			{
				if (disabled)
				{
					return false;
				}
				return base.Active;
			}
		}

		public bool disabled = false;

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(3329, delta))
			{
				return;
			}
			base.TickInterval(3329);
			SyncFoodWithRest();
		}

		public void SyncFoodWithRest()
		{
			Need_Rest need_Rest = pawn.needs?.rest;
			if (need_Rest == null)
			{
				DisableGene();
				return;
			}
			if (!pawn.TryGetNeedFood(out Need_Food need_Food))
			{
				DisableGene();
				return;
			}
			need_Food.CurLevelPercentage = need_Rest.CurLevelPercentage;
		}

		public void EnableGene()
		{
			disabled = false;
			Local_AddOrRemoveHediff();
		}

		public void DisableGene()
		{
			disabled = true;
			RemoveHediff();
		}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//    base.Notify_OverriddenBy(overriddenBy);
		//}

		public override void Notify_Override()
		{
			EnableGene();
			base.Notify_Override();
		}

	}

}
