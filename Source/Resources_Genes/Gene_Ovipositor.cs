using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using static UnityEngine.Scripting.GarbageCollector;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Ovipositor : XaG_Gene, IGenePregnantHuman
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
			if (pawn.Drafted)
			{
				return;
			}
			if (pawn.Map == null)
			{
				//Caravan();
				return;
			}
			if (Gene_Rechargeable.PawnHaveThisJob(pawn, Props.jobDef))
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff<Hediff_Pregnant>() || pawn.Faction != Faction.OfPlayer)
			{
				StopLayEgg();
				return;
			}
			LayEgg_Job();
			if (Find.TickManager.TicksGame > startTick + (60000 * 5))
			{
				StopLayEgg();
			}
		}

		private void LayEgg_Job()
		{
			PathEndMode peMode = PathEndMode.OnCell;
			TraverseParms traverseParms = TraverseParms.For(pawn, Danger.Some);
			XaG_Job job = null;
			if (pawn.Faction == Faction.OfPlayer)
			{
				Thing bestEggBox = GestationUtility.GetBestEggBox(pawn, Props.thingDefToSpawn, peMode, traverseParms);
				if (bestEggBox != null)
				{
					job = new(Props.jobDef, bestEggBox)
					{
						gene = this
					};
					pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
					return;
				}
			}
			//LocalTargetInfo targetInfo = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 9, (Pawn pawn, IntVec3 loc, IntVec3 root) => WanderRoomUtility.IsValidWanderDest(pawn, loc, root) && !loc.IsForbidden(pawn) && pawn.CanReach(loc, PathEndMode.OnCell, Danger.Some), PawnUtility.ResolveMaxDanger(pawn, Danger.Some), false);
			//XaG_Job job = new(Props.jobDef, targetInfo);
			//job.gene = this;
			//pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
			Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(Props.thingDefToSpawn), peMode, traverseParms, 30f, (Thing x) => pawn.GetRoom() == null || x.GetRoom() == pawn.GetRoom());
			job = new(Props.jobDef, thing?.Position ?? RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 5f, null, Danger.Some))
			{
				gene = this
			};
			pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
		}

		//private void Caravan()
		//{

		//}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			//LayEgg(pregnancy);
			ShouldLayEgg();
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_CanLayEggLabel".Translate(), "WVC_XaG_CanLayEggDesc".Translate(pawn), MainDefOf.WVC_XaG_GestationEvent, new LookTargets(pawn));
			}
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
				if (Props.cooldownHediffDef != null)
				{
					pawn.health.AddHediff(Props.cooldownHediffDef);
				}
				// Multy preg support
				if (pawn.health.hediffSet.HasHediff<Hediff_Pregnant>())
				{
					ShouldLayEgg();
				}
				else
				{
					StopLayEgg();
					startTick = -1;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed create human egg. Reason: " + arg);
				StopLayEgg();
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
			//Alert_CanLayEgg.Disabled = false;
		}

		private void StopLayEgg()
		{
			shouldLayEgg = false;
			//Alert_CanLayEgg.Disabled = true;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref shouldLayEgg, "shouldLayEgg", false);
			Scribe_Values.Look(ref startTick, "startTick", -1);
		}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	StopLayEgg();
		//}

		//public void Notify_Override()
		//{

		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	StopLayEgg();
		//}

	}

}
