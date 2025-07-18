using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

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
			Pawn horror = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Metalhorror, Faction.OfEntities));
			if (!GenAdj.TryFindRandomAdjacentCell8WayWithRoom(infected.SpawnedParentOrMe, out var result))
			{
				result = infected.PositionHeld;
			}
			CompMetalhorror compMetalhorror = horror.TryGetComp<CompMetalhorror>();
			compMetalhorror.emergedFrom = infected;
			compMetalhorror.implantSource = null;
			int index = 2;
			int num = (int)(infected.ageTracker.AgeBiologicalTicks / (60 * 60000));
			if (num <= 24)
			{
				index = 0;
			}
			else if (num <= 72)
			{
				index = 1;
			}
			horror.ageTracker.LockCurrentLifeStageIndex(index);
			horror.ageTracker.AgeBiologicalTicks = infected.ageTracker.AgeBiologicalTicks;
			horror.ageTracker.AgeChronologicalTicks = infected.ageTracker.AgeChronologicalTicks;
			GenSpawn.Spawn(horror, result, infected.Corpse.MapHeld);
			// compMetalhorror.FindOrCreateEmergedLord();
			Find.BattleLog.Add(new BattleLogEntry_Event(infected, RulePackDefOf.Event_MetalhorrorEmerged, horror));
			horror.stances.stunner.StunFor(60, null, addBattleLog: false);
			// return pawn2;
			// GeneFeaturesUtility.TrySpawnBloodFilth(pawn2, new(3,4));
			//WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(horror, horror.Map).Trigger(horror, null);
			MiscUtility.DoShapeshiftEffects_OnPawn(horror);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref horrorSpawned, "horrorSpawned", false);
		}

	}

	public class Gene_MachineSenescent : Gene_OverOverridable
	{

		public override void TickInterval(int delta)
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(75621, delta))
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

		public override void TickInterval(int delta)
        {
            // base.Tick();
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            Inhumanize(pawn);
            nextTick = 157889;
        }

        public static void Inhumanize(Pawn pawn)
        {
            if (ModsConfig.AnomalyActive && !pawn.Inhumanized() && Find.Anomaly.LevelDef != MonolithLevelDefOf.Disrupted)
            {
                pawn.mindState?.mentalBreaker?.TryDoMentalBreak("WVC_XaG_MentalBreakReason_Inhumanized".Translate(), MentalBreakDefOf.HumanityBreak);
            }
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

	public class Gene_Deadlife : Gene
	{

		public int nextTick = 6000;

		public int nextDeathRefusal = 2;

		public override void TickInterval(int delta)
		{
			nextTick -= delta;
			if (nextTick > 0f)
			{
				return;
			}
			AddDeadifeGas(60);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: CreateShamblers",
					action = delegate
					{
						AddDeadifeGas(60);
					}
				};
			}
		}

		public void AddDeadifeGas(int tick)
        {
            if (!pawn.Spawned)
            {
                nextTick = 60000;
                return;
            }
            GasUtility.AddDeadifeGas(pawn.PositionHeld, pawn.MapHeld, pawn.Faction, 30);
            bool pause = true;
            try
            {
                if (pawn.Map.listerThings.AllThings.TryRandomElement((Thing thing) => thing is Corpse && !thing.IsUnnaturalCorpse() && pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly), out Thing target))
                {
                    GasUtility.AddDeadifeGas(target.PositionHeld, target.MapHeld, pawn.Faction, 30);
                    pause = false;
                }
            }
            catch (Exception arg)
            {
                nextTick = 180000;
                Log.Error("Failed create shambler. Reason: " + arg);
            }
            float penaltyTick = 60 * StaticCollectionsClass.cachedColonistsCount;
            if (pause)
            {
                nextTick = 60000 + (int)penaltyTick;
                nextDeathRefusal--;
            }
            else
            {
                nextTick = tick + (int)penaltyTick;
            }
            if (nextDeathRefusal <= 0)
            {
                Gene_DeathRefusal.AddDeathRefusal(pawn, false, this);
                nextDeathRefusal = 2 + StaticCollectionsClass.cachedColonistsCount;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick");
			Scribe_Values.Look(ref nextDeathRefusal, "nextDeathRefusal");
		}

	}

	public class Gene_DarknessExposure : Gene, IGeneOverridden
	{
		//private int updTry = 0;

		//public override void Tick()
		//{
		//    nextTick--;
		//    if (updTry > 0)
		//    {
		//        updTry--;
		//    }
		//    if (nextTick > 0)
		//    {
		//        return;
		//    }
		//    if (pawn.health.hediffSet.TryGetHediff(HediffDefOf.DarknessExposure, out Hediff hediff))
		//    {
		//        pawn.health.RemoveHediff(hediff);
		//        updTry = 60000;
		//    }
		//    if (updTry > 0)
		//    {
		//        nextTick = 30;
		//    }
		//    else
		//    {
		//        nextTick = 3333;
		//    }
		//}

		//public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		//{
		//    yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_DarknessExposureImmunity".Translate().CapitalizeFirst(), (updTry > 0).ToStringYesNo(), "WVC_XaG_DarknessExposureImmunityDesc".Translate(), 100);
		//}

		//public override void ExposeData()
		//{
		//    base.ExposeData();
		//    Scribe_Values.Look(ref updTry, "updTry", defaultValue: 0);
		//}

		private int nextTick = 320;

		public override void PostAdd()
		{
			base.PostAdd();
			UpdExposureHediff();
		}

		public HediffDef DarknessExposure => HediffDefOf.DarknessExposure;

		public override void TickInterval(int delta)
        {
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            if (!pawn.health.hediffSet.TryGetHediff(DarknessExposure, out Hediff hediff) || hediff is not Hediff_DarknessExposure_Faulty)
            {
				UpdExposureHediff();
			}
            nextTick = 133133;
		}

		public void UpdExposureHediff()
		{
			HediffUtility.TryRemoveHediff(DarknessExposure, pawn);
			if (!TryMakeHediff(DarknessExposure, pawn, out Hediff_DarknessExposure_Faulty hediff, null))
			{
				return;
			}
			pawn.health.AddHediff(hediff);
		}

		public static bool TryMakeHediff(HediffDef def, Pawn pawn, out Hediff_DarknessExposure_Faulty hediff, BodyPartRecord partRecord = null)
		{
			hediff = null;
			if (pawn == null)
			{
				return false;
			}
			hediff = (Hediff_DarknessExposure_Faulty)Activator.CreateInstance(typeof(Hediff_DarknessExposure_Faulty));
			hediff.def = def;
			hediff.pawn = pawn;
			hediff.Part = partRecord;
			hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
			hediff.PostMake();
			return true;
		}

        public void Notify_OverriddenBy(Gene overriddenBy)
		{
			HediffUtility.TryRemoveHediff(DarknessExposure, pawn);
		}

        public void Notify_Override()
		{
			UpdExposureHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(DarknessExposure, pawn);
		}

	}

	public class Gene_DeathRefusal : Gene
	{

		public int nextDeathRefusal = -1;

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameNotStarted() && Rand.Chance(0.2f))
			{
				AddDeathRefusal(pawn);
			}
			nextDeathRefusal = 296774;
		}

		public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextDeathRefusal, 360000, delta))
			{
				return;
			}
			AddDeathRefusal(pawn, false, this);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextDeathRefusal, "nextDeathRefusal", -1);
		}

		public static void AddDeathRefusal(Pawn pawn, bool ignoreLimit = false, Gene gene = null)
		{
			Hediff_DeathRefusal firstHediff = pawn.health.hediffSet.GetFirstHediff<Hediff_DeathRefusal>();
			if (firstHediff != null)
			{
				firstHediff.SetUseAmountDirect(firstHediff.UsesLeft + 1, ignoreLimit);
			}
			else
			{
				Hediff_DeathRefusal hediff_DeathRefusal = (Hediff_DeathRefusal)HediffMaker.MakeHediff(HediffDefOf.DeathRefusal, pawn);
				hediff_DeathRefusal.SetUseAmountDirect(1);
				pawn.health.AddHediff(hediff_DeathRefusal);
			}
			if (gene != null && PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message("WVC_XaG_AddedDeathRefusalAttempt".Translate(pawn.NameShortColored, gene.Label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

	}

}
