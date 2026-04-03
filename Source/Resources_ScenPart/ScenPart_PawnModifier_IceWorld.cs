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
