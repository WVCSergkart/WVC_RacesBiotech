using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_WithInspectString : HediffWithComps
	{

		public override string GetInspectString()
		{
			HediffComp_Spawner spawner = this.GetComp<HediffComp_Spawner>();
			if (spawner != null)
			{
				return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(spawner.Props.thingDef, null, spawner.Props.stackCount)).Resolve() + ": " + spawner.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return null;
		}

	}

	public class HediffCompProperties_Spawner : HediffCompProperties
	{

		public ThingDef thingDef;

		public int stackCount = 1;

		public IntRange intervals = new(120000, 300000);

		public bool showMessage = false;

		public string message = "MessageCompSpawnerSpawnedItem";

		public HediffCompProperties_Spawner()
		{
			compClass = typeof(HediffComp_Spawner);
		}
	}

	public class HediffComp_Spawner : HediffComp
	{

		public HediffCompProperties_Spawner Props => (HediffCompProperties_Spawner)props;

		public int nextTick = 60000;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			ResetInterval();
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			if (Pawn.Map != null)
			{
				Spawn();
			}
			ResetInterval();
		}

		private void Spawn()
		{
			MiscUtility.SpawnItems(Pawn, Props.thingDef, Props.stackCount, Props.showMessage, Props.message);
		}

		private void ResetInterval()
		{
			nextTick = Props.intervals.RandomInRange;
		}

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn " + Props.thingDef.label,
					action = delegate
					{
						Spawn();
					}
				};
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

}
