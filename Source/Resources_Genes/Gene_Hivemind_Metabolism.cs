using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	// In Dev
	public class Gene_Hivemind_Metabolism : Gene_Hivemind_Drone
	{

		private int savedMetCount = 0;
		public int LastMetCount
		{
			get
			{
				if (MiscUtility.GameNotStarted())
				{
					return savedMetCount;
				}
				savedMetCount = Hivemind.Count;
				return savedMetCount;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (Active && MiscUtility.GameStarted())
			{
				UpdMet();
			}
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59889, delta))
			{
				return;
			}
			if (HivemindUtility.InHivemind(pawn))
			{
				UpdMet();
			}
		}

		private void UpdMet()
		{
			def.biostatMet = LastMetCount;
			def.cachedDescription = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedMetCount, "savedMetCount", 0);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				def.biostatMet = savedMetCount;
				def.cachedDescription = null;
			}
		}

	}

	public class Gene_SharedMetabolism : Gene
	{

		private int savedMetCount = 0;
		public int LastMetCount
		{
			get
			{
				if (MiscUtility.GameNotStarted())
				{
					return savedMetCount;
				}
				savedMetCount = ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive.Where((target) => target.genes?.GetFirstGeneOfType<Gene_SharedMetabolism>() != null).ToList().Count;
				return savedMetCount;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (Active && MiscUtility.GameStarted())
			{
				UpdMet();
			}
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59879, delta))
			{
				return;
			}
			UpdMet();
		}

		private void UpdMet()
		{
			def.biostatMet = LastMetCount;
			def.cachedDescription = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedMetCount, "savedMetCount", 0);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				def.biostatMet = savedMetCount;
				def.cachedDescription = null;
			}
		}

	}

}
