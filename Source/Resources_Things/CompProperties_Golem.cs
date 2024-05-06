using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Golem : CompProperties
	{

		// public Type geneGizmoType = typeOf(GeneGizmo_Golems);

		// public GeneDef reqGeneDef;

		// public List<GeneDef> anyGeneDefs;

		public int golemIndex = -1;

		public int refreshHours = 2;

		public float shutdownEnergyReplenish = 1.0f;

		public IntRange checkOverseerInterval = new(45000, 85000);

		public string uniqueTag = "XaG_Golems";

		public CompProperties_Golem()
		{
			compClass = typeof(CompGolem);
		}

	}

	public class CompGolem : ThingComp
	{

		public CompProperties_Golem Props => (CompProperties_Golem)props;

		private int nextEnergyTick = 1500;
		private int nextOverseerTick = 27371;

		public override void CompTick()
		{
			Pawn pawn = parent as Pawn;
			if (pawn.IsHashIntervalTick(nextEnergyTick))
			{
				MechanoidsUtility.OffsetNeedEnergy(pawn, Props.shutdownEnergyReplenish, Props.refreshHours);
				nextEnergyTick = Props.refreshHours * 1500;
			}
			if (pawn.IsHashIntervalTick(nextOverseerTick))
			{
				Pawn currentOverseer = pawn.GetOverseer();
				HasEnoughGolembond(pawn, currentOverseer);
				ResetOverseerTick();
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				Pawn pawn = parent as Pawn;
				if (pawn?.needs?.energy != null)
				{
					pawn.needs.energy.CurLevel = pawn.needs.energy.MaxLevel;
				}
				ResetOverseerTick();
			}
		}

		private void HasEnoughGolembond(Pawn golem, Pawn overseer)
		{
			if (overseer == null)
			{
				golem.Kill(null, null);
				return;
			}
			// if (golem.Map == null || overseer.Map == null)
			// {
				// return;
			// }
			if (!overseer.IsGolemistOfIndex(Props.golemIndex) || !MechanoidsUtility.HasEnoughGolembond(overseer))
			{
				golem.Kill(null, null);
			}
		}

		private void ResetOverseerTick()
		{
			nextOverseerTick = Props.checkOverseerInterval.RandomInRange;
		}

		public override string CompInspectStringExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (parent is Pawn pawn)
			{
				Need_MechEnergy energy = pawn?.needs?.energy;
				if (energy?.IsSelfShutdown == true)
				{
					return "WVC_XaG_GolemEnergyRecovery_Info".Translate((energy.CurLevelPercentage).ToStringPercent(), ((24f * Props.shutdownEnergyReplenish) / 100f / (energy.MaxLevel / 100)).ToStringPercent());
				}
			}
			return null;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref nextOverseerTick, "nextOverseerTick_" + Props.uniqueTag, 0);
			// Scribe_Refernces.Look(ref currentGolemOverseer, "currentGolemOverseer_" + Props.uniqueTag);
		}

	}

}
