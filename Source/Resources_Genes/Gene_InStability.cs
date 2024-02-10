using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

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

		public bool useStabilizerAuto = true;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
			useStabilizerAuto = Props.defaultBoolValue;
		}

		public override void Tick()
		{
			base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				if (nextTick < 180000)
				{
					GetSerumJob();
				}
				return;
			}
			GeneticStuff();
		}

		private void GetSerumJob()
		{
			if (!pawn.IsHashIntervalTick(12000))
			{
				return;
			}
			if (!useStabilizerAuto)
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				useStabilizerAuto = false;
				return;
			}
			if (Props.jobDef == null || Props.specialFoodDefs.NullOrEmpty())
			{
				return;
			}
			for (int j = 0; j < Props.specialFoodDefs.Count; j++)
			{
				Thing serum = MiscUtility.GetSpecialFood(pawn, Props.specialFoodDefs[j]);
				if (serum == null)
				{
					continue;
				}
				pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(Props.jobDef, serum), JobTag.Misc, true);
			}
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
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
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
				yield return new Command_Action
				{
					defaultLabel = "DEV: Reduce instability ticker",
					action = delegate
					{
						nextTick -= 30000;
					}
				};
			}
			if (Props.jobDef == null)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_Gene_GeneticInstability".Translate() + ": " + GeneUiUtility.OnOrOff(useStabilizerAuto),
				defaultDesc = "WVC_XaG_Gene_GeneticInstabilityDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					useStabilizerAuto = !useStabilizerAuto;
					if (useStabilizerAuto)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
			Scribe_Values.Look(ref useStabilizerAuto, "useStabilizerAutomatic", true);
		}

	}

}
