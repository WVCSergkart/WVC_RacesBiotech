// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ScenPart_PawnModifier_Duplicators : ScenPart_PawnModifier
	{

		public override void PostGameStart()
		{
			startingPawn = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists?.First();
			//SetGeneral();
			DuplicateUtility.HarmonyPatch();
		}

		private int nextTick;
		private Pawn startingPawn;

		public override void Tick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 59999, 1))
			{
				return;
			}
			try
			{
				TryDoLeave();
			}
			catch (Exception arg)
			{
				Log.Warning("Failed trigger colony leave event. Reason: " + arg.Message);
			}
		}

		private void TryDoLeave()
		{
			//if (startingPawn == null)
			//{
			//	return;
			//}
			foreach (Pawn colonist in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
			{
				if (Rand.Chance(0.66f))
				{
					continue;
				}
				if (colonist == startingPawn)
				{
					continue;
				}
				if (colonist.IsDuplicate)
				{
					continue;
				}
				if (ScenPart_PawnModifier_LongRunningJoke.CanLeaveColony(colonist))
				{
					ScenPart_PawnModifier_LongRunningJoke.DoLeave(colonist);
					return;
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref startingPawn, "mainHero", saveDestroyedThings: true);
			Scribe_Values.Look(ref nextTick, "nextTick", defaultValue: 60000);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				//SetGeneral();
				DuplicateUtility.HarmonyPatch();
			}
		}

	}

}
