using System.Linq;
using UnityEngine.PlayerLoop;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Hivemind_Metabolism : Gene_Hivemind_Drone
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCache();
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			ResetCache();
		}

		public override void Notify_Override()
		{
			base.Notify_Override();
			ResetCache();
		}

		private static bool updated = false;
		public override void TickInterval(int delta)
		{
			if (updated)
			{
				return;
			}
			int? count = Hivemind?.Count;
			if (count.HasValue)
			{
				def.biostatMet = count.Value;
				def.cachedDescription = null;
				GeneResourceUtility.UpdMetabolism(pawn);
			}
			updated = true;
		}

		public static void ResetCache()
		{
			updated = false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				ResetCache();
			}
		}

	}

	public class Gene_SharedMetabolism : XaG_Gene, IGeneOverriddenBy
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCache();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCache();
		}

		public void Notify_Override()
		{
			ResetCache();
		}

		private static bool updated = false;

		public override void TickInterval(int delta)
		{
			if (updated)
			{
				return;
			}
			int? count = ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive?.Where((target) => target.genes?.GetFirstGeneOfType<Gene_SharedMetabolism>() != null)?.ToList()?.Count;
			if (count.HasValue)
			{
				def.biostatMet = count.Value;
				def.cachedDescription = null;
				GeneResourceUtility.UpdMetabolism(pawn);
			}
			updated = true;
		}

		private void ResetCache()
		{
			updated = false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				ResetCache();
			}
		}

	}

}
