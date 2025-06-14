using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_OnlyOneOverseer : HediffCompProperties
	{

		public HediffCompProperties_OnlyOneOverseer()
		{
			compClass = typeof(HediffComp_OnlyOneOverseer);
		}

	}

	public class HediffComp_OnlyOneOverseer : HediffComp
	{

		// public HediffCompProperties_OnlyOneOverseer Props => (HediffCompProperties_OnlyOneOverseer)props;

		private Pawn firstOverseer = null;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(2200, delta))
			{
				return;
			}
			if (firstOverseer == null)
			{
				firstOverseer = Pawn.GetOverseer();
			}
			if (!Pawn.IsHashIntervalTick(22000, delta))
			{
				return;
			}
			if (firstOverseer != Pawn.GetOverseer())
			{
				base.Pawn.Kill(null, parent);
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_References.Look(ref firstOverseer, "firstOverseer");
		}

		public string GetLabel()
		{
			if (firstOverseer != null)
			{
				return firstOverseer.NameShortColored;
			}
			return "";
		}

	}

}
