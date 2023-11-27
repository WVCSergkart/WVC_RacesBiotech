using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_InitialEnergyNeed : CompProperties
	{

		// public List<HediffDef> hediffDefs;

		public CompProperties_InitialEnergyNeed()
		{
			compClass = typeof(CompEnergyNeed);
		}
	}

	public class CompEnergyNeed : ThingComp
	{

		// private CompProperties_InitialEnergyNeed Props => (CompProperties_InitialEnergyNeed)props;

		// public override void Initialize(CompProperties props)
		// {
		// base.Initialize(props);
		// AddHediff();
		// }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			Pawn pawn = parent as Pawn;
			pawn.needs.energy.CurLevel = pawn.needs.energy.MaxLevel;
			// AddHediff();
		}

		// public void AddHediff()
		// {
			// Pawn pawn = parent as Pawn;
			// foreach (HediffDef hediff in Props.hediffDefs)
			// {
				// if (!pawn.health.hediffSet.HasHediff(hediff))
				// {
					// pawn.health.AddHediff(hediff);
				// }
			// }
		// }
	}

}
