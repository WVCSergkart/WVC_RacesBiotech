using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_InitialHediff : CompProperties
	{

		public List<HediffDef> hediffDefs;

		public CompProperties_InitialHediff()
		{
			compClass = typeof(CompInitialHediff);
		}
	}

	public class CompInitialHediff : ThingComp
	{

		private CompProperties_InitialHediff Props => (CompProperties_InitialHediff)props;

		// public override void Initialize(CompProperties props)
		// {
			// base.Initialize(props);
			// AddHediff();
		// }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			AddHediff();
		}

		public void AddHediff()
		{
			Pawn pawn = parent as Pawn;
			foreach (HediffDef hediff in Props.hediffDefs)
			{
				if (!pawn.health.hediffSet.HasHediff(hediff))
				{
					pawn.health.AddHediff(hediff);
				}
			}
		}
	}

}