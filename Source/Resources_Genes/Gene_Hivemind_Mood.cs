using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_Mood : Gene_Hivemind_Drone
	{

		private GeneExtension_Opinion cachedExtension;
		public GeneExtension_Opinion Opinion
		{
			get
			{
				if (cachedExtension == null)
				{
					cachedExtension = def?.GetModExtension<GeneExtension_Opinion>();
				}
				return cachedExtension;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			AddMemory();
		}

		private void AddMemory()
		{
			try
			{
				ThoughtUtility.AddPermanentMemory(pawn, Opinion.thoughtDef);
			}
			catch (Exception arg)
			{
				Log.Error("Failed add memory. Reason: " + arg);
			}
		}

		private void RemoveMemory()
		{
			ThoughtUtility.RemoveMemory(pawn, Opinion.thoughtDef);
		}


		private bool updated = false;
		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!updated)
			{
				updated = true;
				AddMemory();
			}
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			RemoveMemory();
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			AddMemory();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveMemory();
		}

		//public override void SyncHive()
		//{
		//	pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.thoughtDef);
		//}

	}

}
