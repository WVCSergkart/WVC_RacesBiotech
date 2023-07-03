using System.Collections.Generic;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompNearSpawner : ThingComp
	{
		private int ticksUntilSpawn;

		public CompProperties_NearSpawner PropsSpawner => (CompProperties_NearSpawner)props;

		private bool PowerOn => parent.GetComp<CompPowerTrader>()?.PowerOn ?? false;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				ResetCountdown();
			}
		}

		public override void CompTick()
		{
			TickInterval(1);
		}

		public override void CompTickRare()
		{
			TickInterval(250);
		}

		private void TickInterval(int interval)
		{
			if (!parent.Spawned)
			{
				return;
			}
			CompCanBeDormant comp = parent.GetComp<CompCanBeDormant>();
			if (comp != null)
			{
				if (!comp.Awake)
				{
					return;
				}
			}
			else if (parent.Position.Fogged(parent.Map))
			{
				return;
			}
			if (!PropsSpawner.requiresPower || PowerOn)
			{
				ticksUntilSpawn -= interval;
				CheckShouldSpawn();
			}
		}

		private void CheckShouldSpawn()
		{
			if (ticksUntilSpawn <= 0)
			{
				ResetCountdown();
				TryDoSpawn();
			}
		}

		public void TryDoSpawn()
		{
			Thing thing = ThingMaker.MakeThing(PropsSpawner.thingToSpawn);
			thing.stackCount = PropsSpawner.spawnCount;
			GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near, null, null, default);
			// if (PropsSpawner.spawnForbidden)
			// {
				// lastResultingThing.SetForbidden(value: true);
			// }
			if (PropsSpawner.showMessageIfOwned && parent.Faction == Faction.OfPlayer)
			{
				Messages.Message("MessageCompSpawnerSpawnedItem".Translate(PropsSpawner.thingToSpawn.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
			}
		}

		private void ResetCountdown()
		{
			ticksUntilSpawn = PropsSpawner.spawnIntervalRange.RandomInRange;
		}

		public override void PostExposeData()
		{
			string text = (PropsSpawner.saveKeysPrefix.NullOrEmpty() ? null : (PropsSpawner.saveKeysPrefix + "_"));
			Scribe_Values.Look(ref ticksUntilSpawn, text + "ticksUntilSpawn", 0);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "DEV: Spawn " + PropsSpawner.thingToSpawn.label,
                    icon = TexCommand.DesirePower,
                    action = delegate
                    {
                        ResetCountdown();
                        TryDoSpawn();
                    }
                };
                yield return command_Action;
			}
		}

		public override string CompInspectStringExtra()
		{
			if (PropsSpawner.writeTimeLeftToSpawn && (!PropsSpawner.requiresPower || PowerOn))
			{
				return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(PropsSpawner.thingToSpawn, null, PropsSpawner.spawnCount)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return null;
		}
	}

}
