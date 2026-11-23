using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Holder_SkipGate : Gene_Holder, IGeneChargeable
	{

		public override bool Active
		{
			get
			{
				if (MechanoidsUtility.CerebrexCoreDefeated)
				{
					return false;
				}
				return base.Active;
			}
		}

		public static XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;

		public override List<PawnContainerHolder> Containers => GameComponent.HolderContainers;

		public void Notify_Charging(float chargePerTick, int tick, float factor)
		{
			foreach (PawnContainerHolder pawnContainer in Containers)
			{
				if (pawnContainer.holded.TryGetNeedFood(out Need_Food food))
				{
					food.CurLevel += chargePerTick * 0.2f * factor;
				}
			}
		}

		public override void DoEffects(Pawn pawn)
		{
			EffectsUtility.DoSkipEffects(pawn.PositionHeld, pawn.MapHeld);
		}

	}

}
