using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Photosynthesis : Gene_FoodEfficiency
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(541))
			{
				return;
			}
			if (pawn.Map == null)
			{
				InCaravan();
				return;
			}
			if (!pawn.Position.InSunlight(pawn.Map))
			{
				return;
			}
			if (pawn.apparel.AnyClothing)
			{
				return;
			}
			ReplenishHunger();
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan?.NightResting != false)
			{
				return;
			}
			ReplenishHunger();
		}

		public void ReplenishHunger()
		{
			UndeadUtility.OffsetNeedFood(pawn, Giver.passivelyReplenishedNutrition);
		}

	}

}
