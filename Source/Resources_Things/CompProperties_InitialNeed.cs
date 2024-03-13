using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_InitialEnergyNeed : CompProperties
	{

		// public List<HediffDef> hediffDefs;

		public int refreshHours = 2;

		public float shutdownEnergyReplenish = 1.0f;

		public CompProperties_InitialEnergyNeed()
		{
			compClass = typeof(CompEnergyNeed);
		}
	}

	public class CompEnergyNeed : ThingComp
	{

		private CompProperties_InitialEnergyNeed Props => (CompProperties_InitialEnergyNeed)props;

		// public override void Initialize(CompProperties props)
		// {
		// base.Initialize(props);
		// AddHediff();
		// }

		// private readonly int ticksInHour = 1500;

		public override void CompTick()
		{
			base.CompTick();
			Pawn pawn = parent as Pawn;
			if (!pawn.IsHashIntervalTick(Props.refreshHours * 1500))
			{
				return;
			}
			// Log.Error("1");
			MechanoidsUtility.OffsetNeedEnergy(pawn, Props.shutdownEnergyReplenish, Props.refreshHours);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				Pawn pawn = parent as Pawn;
				if (pawn.needs != null && pawn.needs.energy != null)
				{
					pawn.needs.energy.CurLevel = pawn.needs.energy.MaxLevel;
				}
			}
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
