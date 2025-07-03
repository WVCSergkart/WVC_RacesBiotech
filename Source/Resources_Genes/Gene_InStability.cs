using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AngelicStability : Gene_FoodEfficiency
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			// if (pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			// {
				// Gene_AddOrRemoveHediff.RemoveHediff(Props.hediffDefName, pawn);
			// }
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			// if (!pawn.IsHashIntervalTick(nextTick))
			// {
				// return;
			// }
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			ResetInterval();
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		// public override void PostRemove()
		// {
			// base.PostRemove();
			// if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			// {
				// pawn.health.AddHediff(Props.hediffDefName);
			// }
		// }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
		}

	}

	public class Gene_ResurgentStability : Gene_ResurgentOffset
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			// if (!pawn.IsHashIntervalTick(nextTick))
			// {
				// return;
			// }
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			ResetInterval();
			if (Resurgent?.ValuePercent >= 1f)
			{
				HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
			}
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

	public class Gene_AnomalyStability : Gene_ResurgentStability
	{

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			base.Notify_PawnDied(dinfo, culprit);
			if (ModsConfig.AnomalyActive && !pawn.IsMutant && MutantDefOf.Ghoul.allowedDevelopmentalStages == pawn.DevelopmentalStage)
			{
				if (GeneResourceUtility.TryResurrectWithSickness(pawn, true, 0.92f))
				{
					MutantUtility.SetPawnAsMutantInstantly(pawn, MutantDefOf.Ghoul);
					if (pawn.Map != null)
					{
						//WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
						MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
					}
					// Find.LetterStack.ReceiveLetter("WVC_XaG_LetterLabelThrallTransformedIntoGhoul".Translate(), "WVC_XaG_LetterDescThrallTransformedIntoGhoul".Translate(pawn), LetterDefOf.NegativeEvent, new LookTargets(pawn));
				}
			}
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

		public override void TickInterval(int delta)
        {
            //base.Tick();
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            Stabilize();
            ResetInterval();
        }

        public virtual void Stabilize()
        {
            HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
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
						Stabilize();
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

	public class Gene_GeneticInstability : Gene, IGeneInspectInfo, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(useStabilizerAuto);

		public virtual TaggedString RemoteActionDesc => "WVC_XaG_Gene_GeneticInstabilityDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			useStabilizerAuto = !useStabilizerAuto;
		}

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}


		//===========

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public int nextTick = 60000;

		public bool useStabilizerAuto = true;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
			useStabilizerAuto = Props.defaultBoolValue;
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			nextTick -= delta;
			if (nextTick > 0)
			{
				if (nextTick < 180000)
				{
					GetDelayJob(delta);
				}
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				ResetInterval();
				return;
			}
			GeneticStuff();
		}

		public virtual void GetDelayJob(int delta)
		{
			if (!useStabilizerAuto)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(12000, delta))
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				// Last Chance Mechanic
				//if (WVC_Biotech.settings.enableInstabilityLastChanceMechanic)
				//{
				//	nextTick += 9000;
				//}
				return;
			}
			// if (pawn.Faction != Faction.OfPlayer)
			// {
				// useStabilizerAuto = false;
				// ResetInterval();
				// return;
			// }
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			DelayJob();
		}

		public virtual void DelayJob()
		{
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

		public virtual void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Thing> things = caravan.AllThings.ToList();
			if (things.NullOrEmpty())
			{
				return;
			}
			for (int j = 0; j < things.Count; j++)
			{
				if (!Props.specialFoodDefs.Contains(things[j].def))
				{
					continue;
				}
				CompUseEffect_GeneticStabilizer stabilizer = things[j].TryGetComp<CompUseEffect_GeneticStabilizer>();
				if (stabilizer?.Props?.canBeUsedInCaravan != true)
				{
					continue;
				}
				nextTick += 60000 * stabilizer.Props.daysDelay;
				things[j].ReduceStack();
				break;
			}
		}

		public virtual void GeneticStuff()
		{
			if (!Props.hediffDefs.NullOrEmpty())
			{
				HediffUtility.AddHediffsFromList(pawn, Props.hediffDefs);
				if (Props.showMessageIfOwned && pawn.Faction == Faction.OfPlayer)
				{
					Messages.Message(Props.message.Translate(pawn.Name.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
				}
			}
			else
			{
				pawn.Kill(null, null);
			}
			ResetInterval();
		}

		private void ResetInterval()
		{
			nextTick = Props.intervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
            if (DebugSettings.ShowDevGizmos && !XaG_GeneUtility.SelectorActiveFaction(pawn, this))
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
			//foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes, enabled))
			//{
			//	yield return gizmo;
			//}
			//if (Props.jobDef == null)
			//{
			//	yield break;
			//}
			//yield return new Command_Action
			//{
			//	defaultLabel = "WVC_XaG_Gene_GeneticInstability".Translate() + ": " + XaG_UiUtility.OnOrOff(useStabilizerAuto),
			//	defaultDesc = "WVC_XaG_Gene_GeneticInstabilityDesc".Translate(),
			//	icon = ContentFinder<Texture2D>.Get(def.iconPath),
			//	action = delegate
			//	{
			//		useStabilizerAuto = !useStabilizerAuto;
			//		if (useStabilizerAuto)
			//		{
			//			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			//		}
			//		else
			//		{
			//			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			//		}
			//	}
			//};
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
				{
					yield return gizmo;
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextGenesRegrowUpdate", 60000);
			Scribe_Values.Look(ref useStabilizerAuto, "useStabilizerAutomatic", true);
		}

		public string GetInspectInfo
		{
			get
			{
				return "WVC_XaG_Gene_GeneticInstability_On_Info".Translate().Resolve() + ": " + nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
		}

	}

}
