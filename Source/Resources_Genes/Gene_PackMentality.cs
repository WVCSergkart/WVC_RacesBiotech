using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_PackMentality : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		private GeneExtension_Opinion cachedExtension;
		public GeneExtension_Opinion Props
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

		private static List<Pawn> cachedPackPawns;
		public static List<Pawn> ThePack
		{
			get
			{
				if (cachedPackPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
					{
						Gene_PackMentality packGene = pawn.genes?.GetFirstGeneOfType<Gene_PackMentality>();
						if (packGene != null)
						{
							list.Add(pawn);
						}
					}
					cachedPackPawns = list;
				}
				return cachedPackPawns;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCache();
		}

		//public static int lastRecacheTick = -1;

		private static int lastRecacheTick = -1;
		public static bool ShouldUpdate => shouldUpdate || Find.TickManager.TicksGame > lastRecacheTick;

		private static bool shouldUpdate = true;

		public override void TickInterval(int delta)
		{
			if (ShouldUpdate)
			{
				UpdThoughts();
			}
		}

		public void UpdThoughts()
		{
			//if (pawn?.Faction != Faction.OfPlayer)
			//{
			//	return;
			//}
			//Notify_GenesRecache(null);
			try
			{
				foreach (Pawn pawn in ThePack)
				{
					foreach (Pawn member in ThePack)
					{
						if (member != pawn)
						{
							pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, member);
							member.needs?.mood?.thoughts?.memories.TryGainMemory(Props.AboutMeThoughtDef, pawn);
						}
					}
					pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.thoughtDef, null);
				}
			}
			catch (Exception arg)
			{
				Log.Warning("Failed update pack thoughts. Reason: " + arg.Message);
			}
			shouldUpdate = false;
			lastRecacheTick = Find.TickManager.TicksGame + 60000;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCache();
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			ResetCache();
		}

		public static void ResetCache()
		{
			cachedPackPawns = null;
			shouldUpdate = true;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCache();
		}

		public void Notify_Override()
		{
			ResetCache();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			//ResetCache();
			foreach (Pawn member in ThePack)
			{
				if (member != pawn)
				{
					member.needs?.mood?.thoughts?.memories.TryGainMemory(Props.packMemberLost, pawn);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: UpdPackThoughts",
					action = delegate
					{
						UpdThoughts();
					}
				};
			}
		}

	}

	public class Gene_ColdBlooded : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		private static List<Pawn> cachedColdBloodedPawns;
		public static List<Pawn> ColdBloodedPawns
		{
			get
			{
				if (cachedColdBloodedPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_ColdBlooded>() != null)
						{
							list.Add(pawn);
						}
					}
					cachedColdBloodedPawns = list;
				}
				return cachedColdBloodedPawns;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Notify_GenesRecache(null);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Notify_GenesRecache(null);
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			cachedColdBloodedPawns = null;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Notify_GenesRecache(null);
		}

		public void Notify_Override()
		{
			Notify_GenesRecache(null);
		}

	}

	public class Gene_Pheromones : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		private static List<Pawn> cachedPheromonesPawns;
		public static List<Pawn> PheromonesPawns
		{
			get
			{
				if (cachedPheromonesPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_Pheromones>() != null)
						{
							list.Add(pawn);
						}
					}
					cachedPheromonesPawns = list;
				}
				return cachedPheromonesPawns;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Notify_GenesRecache(null);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Notify_GenesRecache(null);
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			cachedPheromonesPawns = null;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Notify_GenesRecache(null);
		}

		public void Notify_Override()
		{
			Notify_GenesRecache(null);
		}

	}

}
