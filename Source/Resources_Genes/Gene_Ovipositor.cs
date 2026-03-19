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
			if (!pawn.health.hediffSet.HasHediff<Hediff_Pregnant>())
			{
				shouldLayEgg = false;
				return;
			}
			LocalTargetInfo targetInfo = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 99, (Pawn pawn, IntVec3 loc, IntVec3 root) => WanderRoomUtility.IsValidWanderDest(pawn, loc, root) && !loc.IsForbidden(pawn) && pawn.CanReach(loc, PathEndMode.OnCell, Danger.Some), PawnUtility.ResolveMaxDanger(pawn, Danger.Some), false);
			XaG_Job job = new(Props.jobDef, targetInfo);
			pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, true);
		}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			LayEgg(pregnancy);
			return true;
		}

		public void LayEgg(Hediff_Pregnant pregnancy)
		{
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
				shouldLayEgg = pawn.health.hediffSet.HasHediff<Hediff_Pregnant>();
			}
			catch (Exception arg)
			{
				Log.Error("Failed create human egg. Reason: " + arg);
				shouldLayEgg = false;
			}
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{

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
						shouldLayEgg = true;
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref shouldLayEgg, "shouldLayEgg", false);
		}

	}

}
