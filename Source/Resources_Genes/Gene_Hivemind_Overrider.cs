using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Hivemind_Overrider : Gene_SelfOverrider, IGeneHivemind
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		public void ResetCollection()
		{
			if (!HivemindUtility.SuitableForHivemind(pawn))
			{
				return;
			}
			HivemindUtility.ResetCollection();
		}

		public override void ResetCooldown()
		{
			lastTick = Find.TickManager.TicksGame + (60000 * 12);
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			ResetCollection();
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			ResetCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

	}

}
