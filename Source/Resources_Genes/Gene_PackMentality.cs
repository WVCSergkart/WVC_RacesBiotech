using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_PackMentality : Gene, IGeneOverridden, IGeneNotifyGenesChanged
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
			Notify_GenesChanged(null);
			UpdThoughts();
		}

		private static int? cachedRefreshRate;
		public static int TickRefresh
		{
			get
			{
				if (!cachedRefreshRate.HasValue)
				{
					cachedRefreshRate = (int)(57835 * ((ThePack.Count > 1 ? ThePack.Count : 1) * 0.5f));
				}
				return cachedRefreshRate.Value;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(TickRefresh, delta))
			{
				return;
			}
			UpdThoughts();
		}

		public void UpdThoughts()
        {
			if (pawn?.Faction != Faction.OfPlayer)
            {
				return;
			}
			Notify_GenesChanged(null);
			//if (!ThePack.Contains(pawn))
			//{
			//	Notify_GenesChanged(null);
			//}
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

		public override void PostRemove()
		{
			base.PostRemove();
			Notify_GenesChanged(null);
			UpdThoughts();
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedPackPawns = null;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Notify_GenesChanged(null);
		}

		public void Notify_Override()
		{
			Notify_GenesChanged(null);
			UpdThoughts();
		}

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
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

}
