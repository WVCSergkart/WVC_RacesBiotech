using Verse;

namespace WVC_XenotypesAndGenes
{

	/// <summary>
	/// Dormant hivemind gene. Dormant hivemind gene - do not cause recache and synchronization, but are still considered hivemind genes.
	/// </summary>
	public class Gene_Hivemind_Hediff : Gene_AddOrRemoveHediff, IGeneHivemind, IGeneNonSync
	{

		//public override void PostAdd()
		//{
		//	ResetCollection();
		//	base.PostAdd();
		//}

		public override void Local_AddOrRemoveHediff()
		{
			if (!HivemindUtility.InHivemind(pawn))
			{
				RemoveHediff();
				return;
			}
			base.Local_AddOrRemoveHediff();
		}

		//public void ResetCollection()
		//{
		//	if (!HivemindUtility.SuitableForHivemind(pawn))
		//	{
		//		return;
		//	}
		//	HivemindUtility.ResetCollection();
		//}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	base.Notify_OverriddenBy(overriddenBy);
		//	ResetCollection();
		//}

		//public override void Notify_Override()
		//{
		//	ResetCollection();
		//	base.Notify_Override();
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	ResetCollection();
		//}

	}

}
