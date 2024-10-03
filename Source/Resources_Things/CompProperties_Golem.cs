using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_Golem : CompProperties
	{

		// public Type geneGizmoType = typeOf(GeneGizmo_Golems);

		// public GeneDef reqGeneDef;

		// public List<GeneDef> anyGeneDefs;

		[Obsolete]
		public int golemIndex = -1;

		public int refreshHours = 2;

		[Obsolete]
		public float shutdownEnergyReplenish = 1.0f;

		[Obsolete]
		public IntRange checkOverseerInterval = new(45000, 85000);

		[Obsolete]
		public string uniqueTag = "XaG_Golems";

		public CompProperties_Golem()
		{
			compClass = typeof(CompGolem);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!parentDef.description.NullOrEmpty())
			{
				parentDef.description = parentDef.description + "\n\n" + "WVC_XaG_GolemnoidsGeneralDesc".Translate();
			}
		}

	}

	public class CompGolem : ThingComp
	{

		public CompProperties_Golem Props => (CompProperties_Golem)props;

		private int nextEnergyTick = 1500;

		public override void CompTick()
		{
			nextEnergyTick--;
			if (nextEnergyTick <= 0f)
			{
				Pawn pawn = parent as Pawn;
				MechanoidsUtility.OffsetNeedEnergy(pawn, WVC_Biotech.settings.golemnoids_ShutdownRechargePerTick, Props.refreshHours);
				nextEnergyTick = Props.refreshHours * 1500;
			}
		}

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
			// base.PostSpawnSetup(respawningAfterLoad);
			// if (!respawningAfterLoad)
			// {
				// Pawn pawn = parent as Pawn;
				// if (pawn?.needs?.energy != null)
				// {
					// pawn.needs.energy.CurLevel = pawn.needs.energy.MaxLevel;
				// }
			// }
		// }

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
					return "WVC_XaG_GolemEnergyRecovery_Info".Translate((energy.CurLevelPercentage).ToStringPercent(), MechanoidsUtility.GolemsEnergyPerDayInPercent(energy.MaxLevel));
				}
			}
			return null;
		}
    }

}
