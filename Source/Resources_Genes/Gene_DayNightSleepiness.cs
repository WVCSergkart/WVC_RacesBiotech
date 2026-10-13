using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{


	public class Gene_DaySleep : Gene_AddOrRemoveHediff
	{

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(2671, delta))
			{
				SetRest();
			}
		}

		public SimpleCurve curve = new()
		{
			new CurvePoint(0, 1f),
			new CurvePoint(8, 0.6f),
			new CurvePoint(11, 0.0f),
			new CurvePoint(18, 0.0f),
			new CurvePoint(19, 1f),
			new CurvePoint(24f, 1f)
		};

		public void SetRest()
		{
			Map mapHeld = pawn.MapHeld;
			if (mapHeld == null)
			{
				return;
			}
			Need_Rest needRest = pawn.needs?.rest;
			if (needRest != null)
			{
				needRest.CurLevelPercentage = curve.Evaluate(GenLocalDate.HourOfDay(mapHeld.Tile));
			}
		}

	}

}
