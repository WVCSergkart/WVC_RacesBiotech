// RimWorld.StatPart_Age
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_IceWorld : ScenPart_PawnModifier_Scarlands
	{

		public static Pawn lastDeadVoidPawn = null;

		public override void PostGameStart()
		{
			base.PostGameStart();
			SetGeneral();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGeneral();
			}
		}

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{

		}

		//protected override void ModifyNewPawn(Pawn p)
		//{

		//}

		public override void PostIdeoChosen()
		{
			if (lastDeadVoidPawn == null)
			{
				return;
			}
			foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
			{
				if (Rand.Chance(0.7f))
				{
					DuplicateUtility.CopyPawn(lastDeadVoidPawn, startingAndOptionalPawn);
					startingAndOptionalPawn.Name = lastDeadVoidPawn.Name;
					AgelessUtility.ChronoCorrection(startingAndOptionalPawn, lastDeadVoidPawn);
					break;
				}
			}
		}

		private void SetGeneral()
		{
			foreach (BiomeDef biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
			{
				try
				{
					if (biomeDef.constantOutdoorTemperature == null)
					{
						biomeDef.constantOutdoorTemperature = -51;
					}
				}
				catch
				{
					// Silent catcher.
				}
			}
		}

	}

}
