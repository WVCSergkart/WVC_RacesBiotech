using RimWorld;
using RimWorld.Planet;
using System;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_AdaptiveTemperature : HediffWithComps
	{

		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;

		public SimpleCurve flammabilityCurve = new()
		{
			new CurvePoint(50, 1.0f),
			new CurvePoint(60, 0.9f),
			new CurvePoint(70, 0.6f),
			new CurvePoint(100, 0.20f),
			new CurvePoint(120, 0f)
		};

		public SimpleCurve vacResCurve = new()
		{
			new CurvePoint(-50, 0.0f),
			new CurvePoint(-60, 0.2f),
			new CurvePoint(-70, 0.6f),
			new CurvePoint(-100, 0.90f),
			new CurvePoint(-150, 1.0f)
		};

		private HediffStage curStage;
		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					try
					{
						curStage.statOffsets = new();
						StatModifier statModifier = new StatModifier();
						statModifier.stat = StatDefOf.ComfyTemperatureMax;
						statModifier.value = 0;
						Caravan caravan = pawn.GetCaravan();
						if (pawn.MapHeld != null)
						{
							float currentTemp = pawn.PositionHeld.GetTemperature(pawn.MapHeld);
							SetStatMod(ref statModifier, currentTemp);
							if (currentTemp > 50)
							{
								StatModifier statFlammability = new StatModifier();
								statFlammability.stat = StatDefOf.Flammability;
								statFlammability.value = (float)Math.Round(flammabilityCurve.Evaluate(currentTemp), 2);
								curStage.statFactors = new();
								curStage.statFactors.Add(statFlammability);
							}
							else if (currentTemp < -50 && ModsConfig.OdysseyActive)
							{
								curStage.preventVacuumBurns = true;
								StatModifier statVacuumResistance = new StatModifier();
								statVacuumResistance.stat = StatDefOf.VacuumResistance;
								statVacuumResistance.value = (float)Math.Round(vacResCurve.Evaluate(currentTemp), 2);
								curStage.statOffsets.Add(statVacuumResistance);
							}
						}
						else if (caravan != null)
						{
							SetStatMod(ref statModifier, caravan.Tile.Tile.temperature);
						}
						curStage.statOffsets.Add(statModifier);
					}
					catch (Exception arg)
					{
						Log.Warning("Failed dynamically change temp offset. Reason: " + arg.Message);
					}
				}
				return curStage;

				void SetStatMod(ref StatModifier statModifier, float currentTemp)
				{
					float positionTemp = currentTemp - 20;
					statModifier.stat = positionTemp > 0 ? StatDefOf.ComfyTemperatureMax : StatDefOf.ComfyTemperatureMin;
					statModifier.value = positionTemp;
				}
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(12500, delta))
			{
				curStage = null;
			}
		}

	}

}
