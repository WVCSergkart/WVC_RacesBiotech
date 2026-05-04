using System;
using System.Collections.Generic;
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
				//GeneResourceUtility.UpdMetabolism(pawn);
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

		public override void Notify_GenesRecache(Gene changedGene)
		{
			ResetCache();
		}

	}

	public class Gene_SharedMetabolism : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
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
			try
			{
				def.biostatMet = 0;
				SetMetabolism();
				def.cachedDescription = null;
			}
			catch (Exception arg)
			{
				Log.Error("Failed set metabolism: " + def.defName + ". Reason: " + arg.Message);
			}
			updated = true;
		}

		public virtual void SetMetabolism()
		{
			int metabol = 0;
			IEnumerable<Pawn> allPlayerPawns_MapsOrCaravans_Alive = ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive;
			List<Pawn> sharedPawns = new();
			foreach (Pawn item in allPlayerPawns_MapsOrCaravans_Alive)
			{
				if (item?.genes?.GetFirstGeneOfType<Gene_SharedMetabolism>() == null)
				{
					continue;
				}
				foreach (Gene gene in item.genes.GenesListForReading)
				{
					if (gene.Overridden)
					{
						continue;
					}
					metabol += gene.def.biostatMet;
				}
				sharedPawns.Add(item);
			}
			def.biostatMet = metabol / sharedPawns.Count;
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

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			ResetCache();
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			ResetCache();
		}

	}

}
