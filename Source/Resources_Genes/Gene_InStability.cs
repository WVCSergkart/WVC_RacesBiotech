using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AngelicStability : Gene_DustDrain
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(Props.hediffDefName, pawn);
			}
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(nextTick))
			// {
				// return;
			// }
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
			ResetInterval();
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			{
				pawn.health.AddHediff(Props.hediffDefName);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
		}

	}

	public class Gene_ResurgentStability : Gene_ResurgentCellsGain
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(nextTick))
			// {
				// return;
			// }
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
			ResetInterval();
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
		}

	}

	public class Gene_GeneticStability : Gene
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
			ResetInterval();
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GeneticStability",
					action = delegate
					{
						HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
		}

	}

	public class Gene_GeneticInstability : Gene
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			GeneticStuff();
		}

		public void GeneticStuff()
		{
			HediffUtility.AddHediffsFromList(pawn, Props.hediffDefs);
			if (Props.showMessageIfOwned && pawn.Faction == Faction.OfPlayer)
			{
				Messages.Message(Props.message.Translate(pawn.Name.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
			}
			ResetInterval();
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GeneticInstability",
					action = delegate
					{
						GeneticStuff();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
		}

	}

}
