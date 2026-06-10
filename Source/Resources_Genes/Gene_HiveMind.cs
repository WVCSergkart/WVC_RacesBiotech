using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Hivemind_Drone : XaG_Gene, IGeneOverriddenBy, IGeneHivemind, IGeneRecacheable
	{

		public List<Pawn> Hivemind => HivemindUtility.HivemindPawns;
		public float PsyFactor => HivemindUtility.HivemindPsychicSensitivity;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		public virtual void ResetCollection()
		{
			if (!HivemindUtility.SuitableForHivemind(pawn))
			{
				return;
			}
			HivemindUtility.ResetCollection();
		}

		public override void TickInterval(int delta)
		{

		}

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public virtual void Notify_Override()
		{
			ResetCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

		public virtual void Notify_GenesRecache(Gene changedGene)
		{
			ResetCollection();
		}

	}

	public class Gene_Hivemind : Gene_Hivemind_Drone
	{

		public int nextTick = 2000;

		//public float PsyFactor => pawn.GetStatValue(StatDefOf.PsychicSensitivity);

		public override void PostAdd()
		{
			base.PostAdd();
			if (ModsUtility.GameStarted())
			{
				HivemindUtility.ResetTick(ref nextTick);
			}
		}

		public override void TickInterval(int delta)
		{
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			HivemindUtility.ResetTick(ref nextTick);
			if (!HivemindUtility.SuitableForHivemind(pawn))
			{
				return;
			}
			UpdGeneSync();
		}

		public virtual void UpdGeneSync()
		{

		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SyncHive",
					action = delegate
					{
						UpdGeneSync();
					}
				};
			}
		}

		//public override void ResetCollection()
		//{
		//    base.ResetCollection();
		//    if (MiscUtility.GameStarted() && pawn.InHivemind())
		//    {
		//        SyncHive();
		//    }
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 1200);
		}

	}

	public class Gene_Hivemind_Telepathy : Gene_Hivemind_Drone
	{

		private static int tickBeforeNextRandomHivemindInteraction = -1;

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(30901, delta))
			{
				return;
			}
			try
			{
				RandomInteraction();
			}
			catch (Exception arg)
			{
				Log.Error($"Failed hivemind interaction. Reason: {arg.Message}");
				tickBeforeNextRandomHivemindInteraction = Find.TickManager.TicksGame + 60000;
			}
		}

		public void RandomInteraction()
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (Find.TickManager.TicksGame < tickBeforeNextRandomHivemindInteraction)
			{
				return;
			}
			if (!HivemindUtility.InHivemind(pawn))
			{
				return;
			}
			foreach (Pawn drone in Hivemind)
			{
				if (GeneInteractionsUtility.TryInteractRandomly(drone, Hivemind.Where((hiver) => hiver != drone && hiver.Spawned).ToList(), psychicInteraction: false, ignoreTalking: true, closeTarget: false, out Pawn otherPawn))
				{
					tickBeforeNextRandomHivemindInteraction = Find.TickManager.TicksGame + (int)(10000 / PsyFactor);
					EffectsUtility.PulseEffect(drone);
					EffectsUtility.PulseEffect(otherPawn);
					if (Rand.Chance(0.75f))
					{
						break;
					}
				}
			}
		}

	}

	public class Gene_Hivemind_DeathChain : Gene_Hivemind_Drone
	{

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (pawn.Faction == Faction.OfPlayer)
			{
				Player_Trigger();
			}
			else
			{
				NPC_Trigger();
			}
		}

		private void Player_Trigger()
		{
			ResetCollection();
			foreach (Pawn hiveMember in Hivemind.ToList())
			{
				hiveMember.Kill();
			}
		}

		private void NPC_Trigger()
		{
			if (pawn.Corpse?.Map == null)
			{
				return;
			}
			foreach (Pawn hiveMember in pawn.Corpse.Map.mapPawns.AllHumanlikeSpawned.Where((p) => p.genes != null && p.Faction == pawn.Faction && p.genes.GenesListForReading.Any((g) => HivemindUtility.IsHivemindGene(g))).ToList())
			{
				hiveMember.Kill();
			}
		}

	}

}
