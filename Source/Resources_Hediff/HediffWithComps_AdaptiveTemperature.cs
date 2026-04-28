using RimWorld;
using RimWorld.Planet;
using System;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_AdaptiveTemperature : HediffWithComps
	{

		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;

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
							SetStatMod(ref statModifier, pawn.PositionHeld.GetTemperature(pawn.MapHeld));
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
