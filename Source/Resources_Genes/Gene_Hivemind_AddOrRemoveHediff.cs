using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Hivemind_Hediff : Gene_AddOrRemoveHediff, IGeneHivemind
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		public void ResetCollection()
		{
			Gene_Hivemind.ResetCollection();
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
