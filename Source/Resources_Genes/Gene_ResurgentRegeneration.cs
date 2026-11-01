// Verse.Gene_Healing
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentRegeneration : Gene_ResurgentOffset
	{

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(1500, delta))
			{
				HealingUtility.Regeneration(pawn, Resurgent.Value, 1500);
			}
		}

	}

}
