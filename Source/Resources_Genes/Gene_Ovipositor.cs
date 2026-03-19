using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Ovipositor : Gene, IGenePregnantHuman
	{


		private GeneExtension_Spawner cachedGeneExtension;
		public GeneExtension_Spawner Props
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Spawner>();
				}
				return cachedGeneExtension;
			}
		}

		public bool shouldLayEgg = false;
		private int startTick = -1;

		public override void TickInterval(int delta)
		{
			if (!shouldLayEgg)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(2500, delta))
			{
				return;
			}
			if (Gene_Rechargeable.PawnHaveThisJob(pawn, Props.jobDef))
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff<Hediff_Pregnant>() || pawn.Faction != Faction.OfPlayer)
			{
				shouldLayEgg = false;
				return;
			}
			if (pawn.Map == null)
			{
				//Caravan();
				return;
			}
			LocalTargetInfo targetInfo = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 9, (Pawn pawn, IntVec3 loc, IntVec3 root) => WanderRoomUtility.IsValidWanderDest(pawn, loc, root) && !loc.IsForbidden(pawn) && pawn.CanReach(loc, PathEndMode.OnCell, Danger.Some), PawnUtility.ResolveMaxDanger(pawn, Danger.Some), false);
			XaG_Job job = new(Props.jobDef, targetInfo);
			job.gene = this;
			pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
			if (Find.TickManager.TicksGame > startTick + (60000 * 5))
			{
				shouldLayEgg = false;
			}
		}

		//private void Caravan()
		//{

		//}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			//LayEgg(pregnancy);
			ShouldLayEgg();
			return true;
		}

		public void LayEgg(Hediff_Pregnant pregnancy)
		{
			if (pregnancy == null)
			{
				return;
			}
			try
			{
				Thing thing = ThingMaker.MakeThing(Props.thingDefToSpawn);
				CompHumanEgg compHumanEgg = thing.TryGetComp<CompHumanEgg>();
				compHumanEgg.SetupEgg(pregnancy);
				int litterSize = GestationUtility.BabiesCount(pawn);
				MiscUtility.SpawnItems(pawn, thing, litterSize, Props.showMessageIfOwned, Props.spawnMessage);
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.MorningSickness);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
				Hediff firstHediffOfDef2 = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PregnancyMood);
				if (firstHediffOfDef2 != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef2);
				}
				pawn.health.RemoveHediff(pregnancy);
				// Multy preg support
				if (pawn.health.hediffSet.HasHediff<Hediff_Pregnant>())
				{
					ShouldLayEgg();
				}
				else
				{
					startTick = -1;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed create human egg. Reason: " + arg);
				shouldLayEgg = false;
			}
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			//Skip
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			//foreach (Gizmo item in base.GetGizmos())
			//{
			//	yield return item;
			//}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: ShouldLayEggs",
					action = delegate
					{
						ShouldLayEgg();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: LayEgg",
					action = delegate
					{
						LayEgg(pawn.health.hediffSet.GetFirstHediff<Hediff_Pregnant>());
					}
				};
			}
		}

		private void ShouldLayEgg()
		{
			shouldLayEgg = true;
			startTick = Find.TickManager.TicksGame;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref shouldLayEgg, "shouldLayEgg", false);
			Scribe_Values.Look(ref startTick, "startTick", -1);
		}

	}

}
