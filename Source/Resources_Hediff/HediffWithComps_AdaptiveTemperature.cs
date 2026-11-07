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

		private int maxTemp;
		private int minTemp;

		private HediffStage curStage;
		//public override HediffStage CurStage
		//{
		//	get
		//	{
		//		if (curStage == null)
		//		{
		//			curStage = new();
		//			try
		//			{
		//				curStage.statOffsets = new();
		//				StatModifier statModifier = new StatModifier();
		//				statModifier.stat = StatDefOf.ComfyTemperatureMax;
		//				statModifier.value = 0;
		//				Caravan caravan = pawn.GetCaravan();
		//				if (pawn.MapHeld != null)
		//				{
		//					float currentTemp = pawn.PositionHeld.GetTemperature(pawn.MapHeld);
		//					SetStatMod(ref statModifier, currentTemp);
		//					if (currentTemp > 50)
		//					{
		//						StatModifier statFlammability = new StatModifier();
		//						statFlammability.stat = StatDefOf.Flammability;
		//						statFlammability.value = (float)Math.Round(flammabilityCurve.Evaluate(currentTemp), 2);
		//						curStage.statFactors = new();
		//						curStage.statFactors.Add(statFlammability);
		//					}
		//					else if (currentTemp < -50 && ModsConfig.OdysseyActive)
		//					{
		//						curStage.preventVacuumBurns = true;
		//						StatModifier statVacuumResistance = new StatModifier();
		//						statVacuumResistance.stat = StatDefOf.VacuumResistance;
		//						statVacuumResistance.value = (float)Math.Round(vacResCurve.Evaluate(currentTemp), 2);
		//						curStage.statOffsets.Add(statVacuumResistance);
		//					}
		//				}
		//				else if (caravan != null)
		//				{
		//					SetStatMod(ref statModifier, caravan.Tile.Tile.temperature);
		//				}
		//				curStage.statOffsets.Add(statModifier);
		//			}
		//			catch (Exception arg)
		//			{
		//				Log.Warning("Failed dynamically change temp offset. Reason: " + arg.Message);
		//			}
		//		}
		//		return curStage;

		//		void SetStatMod(ref StatModifier statModifier, float currentTemp)
		//		{
		//			float positionTemp = currentTemp - 20;
		//			statModifier.stat = positionTemp > 0 ? StatDefOf.ComfyTemperatureMax : StatDefOf.ComfyTemperatureMin;
		//			statModifier.value = positionTemp;
		//		}
		//	}
		//}
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
						curStage.statFactors = new();
						StatModifier statModifierMax = new StatModifier();
						statModifierMax.stat = StatDefOf.ComfyTemperatureMax;
						statModifierMax.value = maxTemp;
						StatModifier statModifierMin = new StatModifier();
						statModifierMin.stat = StatDefOf.ComfyTemperatureMin;
						statModifierMin.value = minTemp;
						curStage.statOffsets.Add(statModifierMax);
						curStage.statOffsets.Add(statModifierMin);
						if (maxTemp > 50)
						{
							StatModifier statFlammability = new StatModifier();
							statFlammability.stat = StatDefOf.Flammability;
							statFlammability.value = (float)Math.Round(flammabilityCurve.Evaluate(maxTemp), 2);
							curStage.statFactors.Add(statFlammability);
						}
						if (minTemp < -50 && ModsConfig.OdysseyActive)
						{
							curStage.preventVacuumBurns = true;
							StatModifier statVacuumResistance = new StatModifier();
							statVacuumResistance.stat = StatDefOf.VacuumResistance;
							statVacuumResistance.value = (float)Math.Round(vacResCurve.Evaluate(minTemp), 2);
							curStage.statOffsets.Add(statVacuumResistance);
						}
					}
					catch (Exception arg)
					{
						Log.Warning("Failed dynamically change temp offset. Reason: " + arg.Message);
					}
				}
				return curStage;
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(5000, delta))
			{
				if (minTemp > -100 && pawn.health.hediffSet.HasHediff(HediffDefOf.Hypothermia))
				{
					minTemp -= 2;
				}
				else if (maxTemp < 100 && pawn.health.hediffSet.HasHediff(HediffDefOf.Heatstroke))
				{
					maxTemp += 2;
				}
				if (pawn.IsNestedHashIntervalTick(2500, 12500))
				{
					curStage = null;
				}
			}
			//if (pawn.IsHashIntervalTick(12500, delta))
			//{
			//	curStage = null;
			//}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref maxTemp, "maxTemp");
			Scribe_Values.Look(ref minTemp, "minTemp");
		}

	}

}
