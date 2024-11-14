using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_HorrorPlating : Gene
	{

		public bool horrorSpawned = false;

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (ModsConfig.AnomalyActive && !horrorSpawned && pawn.Corpse?.Map != null)
			{
				SpawnMetalhorrorWithoutHediff(pawn);
				horrorSpawned = true;
				MiscUtility.SpawnItems(pawn, ThingDefOf.Bioferrite, (int)((pawn.GetStatValue(StatDefOf.MeatAmount) + pawn.GetStatValue(StatDefOf.LeatherAmount)) * 0.5), false);
				// pawn?.Corpse?.Destroy();
			}
		}

		public static void SpawnMetalhorrorWithoutHediff(Pawn infected)
		{
			if (!ModLister.CheckAnomaly("Metalhorror"))
			{
				return;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Metalhorror, Faction.OfEntities));
			if (!GenAdj.TryFindRandomAdjacentCell8WayWithRoom(infected.SpawnedParentOrMe, out var result))
			{
				result = infected.PositionHeld;
			}
			CompMetalhorror compMetalhorror = pawn.TryGetComp<CompMetalhorror>();
			compMetalhorror.emergedFrom = infected;
			compMetalhorror.implantSource = null;
			int index = 2;
			int num = (int)infected.ageTracker.AgeBiologicalTicks / 2500;
			if (num <= 24)
			{
				index = 0;
			}
			else if (num <= 72)
			{
				index = 1;
			}
			pawn.ageTracker.LockCurrentLifeStageIndex(index);
			pawn.ageTracker.AgeBiologicalTicks = infected.ageTracker.AgeBiologicalTicks;
			pawn.ageTracker.AgeChronologicalTicks = infected.ageTracker.AgeChronologicalTicks;
			Pawn pawn2 = (Pawn)GenSpawn.Spawn(pawn, result, infected.Corpse.MapHeld);
			// compMetalhorror.FindOrCreateEmergedLord();
			Find.BattleLog.Add(new BattleLogEntry_Event(infected, RulePackDefOf.Event_MetalhorrorEmerged, pawn2));
			pawn2.stances.stunner.StunFor(60, null, addBattleLog: false);
			// return pawn2;
			// GeneFeaturesUtility.TrySpawnBloodFilth(pawn2, new(3,4));
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn2, pawn2.Map).Trigger(pawn2, null);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref horrorSpawned, "horrorSpawned", false);
		}

	}

	public class Gene_MachineSenescent : Gene_OverOverridable
	{

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(75621))
			{
				return;
			}
			if (ModsConfig.AnomalyActive && pawn.Map != null)
			{
				MetalhorrorUtility.TryEmerge(pawn, "WVC_XaG_MetalhorrorReasonGeneRejection".Translate(pawn.Named("INFECTED")));
			}
		}

	}

	[Obsolete]
	public class Gene_EmergeMetalhorror : Gene_MachineSenescent
	{


	}

	public class Gene_Inhumanized : Gene, IGeneOverridden
	{

		private int nextTick = 45679;
		private bool inhumanizedBeforeGene = false;

		public HediffDef Inhumanized => HediffDefOf.Inhumanized;


		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.Inhumanized())
			{
				inhumanizedBeforeGene = true;
			}
			HediffUtility.TryAddHediff(Inhumanized, pawn, def, null);
		}

		public override void Tick()
		{
			// base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			if (ModsConfig.AnomalyActive && !pawn.Inhumanized())
			{
				pawn.mindState?.mentalBreaker?.TryDoMentalBreak("WVC_XaG_MentalBreakReason_Inhumanized".Translate(), MentalBreakDefOf.HumanityBreak);
			}
			nextTick = 157889;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			if (!inhumanizedBeforeGene)
			{
				HediffUtility.TryRemoveHediff(Inhumanized, pawn);
			}
		}

		public void Notify_Override()
		{
			HediffUtility.TryAddHediff(Inhumanized, pawn, def, null);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!inhumanizedBeforeGene)
			{
				HediffUtility.TryRemoveHediff(Inhumanized, pawn);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref inhumanizedBeforeGene, "inhumanizedBeforeGene", false);
		}

	}

	public class Gene_Duplicator : Gene
	{

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
                {
					defaultLabel = "DEV: TryDuplicate",
					action = delegate
                    {
                        TryDuplicate();
                    }
                };
			}
		}

		public bool? CanDuplicate()
		{
			return pawn.abilities?.GetAbility(def.abilities.First())?.OnCooldown;
		}

		public void TryDuplicate()
        {
            pawn.abilities?.GetAbility(def.abilities.First())?.Activate(pawn, pawn);
		}

		public void ResetAbility()
		{
			pawn.abilities?.GetAbility(def.abilities.First())?.ResetCooldown();
		}

	}

}
