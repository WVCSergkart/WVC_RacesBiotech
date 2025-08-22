using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Voidlink : Gene_Mechlink, IGeneOverridden, IGeneNotifyOnKilled, IGeneNotifyGenesChanged
    {

        public override void PostAdd()
        {
            base.PostAdd();
            HediffUtility.TryAddOrRemoveHediff(Spawner.mechanitorHediff, pawn, this, null);
            if (!MiscUtility.GameNotStarted())
            {
                return;
            }
            FloatRange range = new(0.01f, 0.17f);
            OffsetResource(range.RandomInRange);
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            KillMechs();
            HediffUtility.TryRemoveHediff(Spawner.mechanitorHediff, pawn);
        }

        public void Notify_Override()
        {
            HediffUtility.TryAddOrRemoveHediff(Spawner.mechanitorHediff, pawn, this, null);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            KillMechs();
            HediffUtility.TryRemoveHediff(Spawner.mechanitorHediff, pawn);
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            if (!Active)
            {
                return;
            }
            if (!MiscUtility.TryGetAndDestroyCorpse_WithPosition(pawn, out Map mapHeld, out IntVec3 positionHeld))
            {
                return;
            }
            if (mapHeld == null)
            {
                return;
            }
            MiscUtility.DoSkipEffects(positionHeld, mapHeld);
        }

        public void Notify_PawnKilled()
        {
            KillMechs();
            //StaticCollectionsClass.voidLinkNewGamePlusPawn = pawn;
        }

        public void KillMechs(bool offsetResource = false)
        {
            if (pawn.mechanitor != null)
            {
                List<Pawn> mechs = pawn.mechanitor.ControlledPawns;
                foreach (Pawn mech in mechs)
                {
                    if (!mech.Dead && mech.health.hediffSet.HasHediff(Spawner.mechHediff))
                    {
                        KillSelectedMech(offsetResource, mech);
                    }
                }
            }
        }

        public void KillSelectedMech(bool offsetResource, Pawn mech)
        {
            if (offsetResource)
            {
                OffsetResource(MechanoidHolder.GetVoidMechCost(mech.kindDef, 25f) * 0.0025f * mech.health.summaryHealth.SummaryHealthPercent);
            }
            mech.forceNoDeathNotification = true;
            bool cachedGearDrop = mech.kindDef.destroyGearOnDrop;
            mech.kindDef.destroyGearOnDrop = true;
            mech.Kill(null);
            mech.forceNoDeathNotification = false;
            mech.kindDef.destroyGearOnDrop = cachedGearDrop;
        }

        private Gizmo gizmo;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (XaG_GeneUtility.SelectorActiveFactionMapMechanitor(pawn, this))
            {
                yield break;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: SkipMechs",
                    action = delegate
                    {
                        List<PawnKindDef> pawnKindDefs = MechKindDefs;
                        selectedMechs = new();
                        selectedMechs.Add(pawnKindDefs.RandomElement());
                        selectedMechs.Add(pawnKindDefs.RandomElement());
                        selectedMechs.Add(pawnKindDefs.RandomElement());
                        timeForNextSummon = 60;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEV: AddResource 10",
                    action = delegate
                    {
                        OffsetResource(0.10f);
                    }
                };
            }
            if (gizmo == null)
            {
                gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
            }
            yield return gizmo;
        }

        public List<PawnKindDef> MechKindDefs => DefDatabase<PawnKindDef>.AllDefsListForReading.Where((mechkind) => MechanoidsUtility.MechanoidIsPlayerMechanoid(mechkind)).ToList();

        private float geneResource = 0;

        public float ResourceGain => def.resourceLossPerDay / 60000;

        private float? cachedTotalResourceGain;
        public float TotalResourceGain
        {
            get
            {
                if (!cachedTotalResourceGain.HasValue)
                {
                    float resourceGain = def.resourceLossPerDay;
                    foreach (Gene gene in pawn.genes.GenesListForReading)
                    {
                        if (gene is Gene_VoidlinkOffset offset)
                        {
                            resourceGain += offset.def.resourceLossPerDay;
                        }
                    }
                    cachedTotalResourceGain = (float)Math.Round(resourceGain, 2);
                }
                return cachedTotalResourceGain.Value;
            }
        }

        private float? cachedMaxResource;
        public float MaxResource
        {
            get
            {
                if (!cachedMaxResource.HasValue)
                {
                    //cachedMaxResource = 1f;
                    //foreach (Gene gene in pawn.genes.GenesListForReading)
                    //{
                    //	if (gene is Gene_VoidlinkMaxResource resource && resource.Giver != null)
                    //	{
                    //		cachedMaxResource += resource.Giver.maxVoidEnergyOffset;
                    //	}
                    //}
                    cachedMaxResource = pawn.GetStatValue(Spawner.voidMaxResource_StatDef);
                }
                return cachedMaxResource.Value;
            }
        }

        public void Notify_GenesChanged(Gene changedGene)
        {
            CacheReset();
        }

        public void CacheReset(bool notifyBandwidth = false)
        {
            //if (pawn.mechanitor == null)
            //{
            //	return;
            //}
            if (notifyBandwidth)
            {
                pawn.mechanitor?.Notify_BandwidthChanged();
            }
            cachedMaxResource = null;
            allMechsCount = null;
            cachedMaxMechs = null;
            sphereChance = null;
        }

        public float CurrentResource => (float)Math.Round(geneResource, 2);

        private float? cachedMaxMechs;
        public float MaxMechs
        {
            get
            {
                if (!cachedMaxMechs.HasValue)
                {
                    cachedMaxMechs = pawn.GetStatValue(Spawner.voidMechsLimit_StatDef);
                }
                return cachedMaxMechs.Value;
            }
        }

        public float ResourcePercent => geneResource / MaxResource;

        public float ResourceForDisplay => (float)Math.Round(geneResource * 100f, 2);

        public TaggedString ResourcePerDay => "WVC_XaG_PerDay".Translate((TotalResourceGain < 0 ? "-" : "+").ToString() + TotalResourceGain * 100);

        //public override void Tick()
        //{
        //}

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn.IsHashIntervalTick(2500, delta))
            {
                OffsetResource(ResourceGain * 2500);
            }
            if (timeForNextSummon > 0)
            {
                timeForNextSummon -= delta;
                if (timeForNextSummon == 0)
                {
                    if (TrySummonMechs())
                    {
                        timeForNextSummon = 45;
                    }
                    else
                    {
                        CacheReset(true);
                    }
                }
            }
        }

        //public void PostSummon()
        //{
        //	foreach (Pawn mech in pawn.mechanitor.ControlledPawns.ToList())
        //	{
        //		if (mech.Dead)
        //		{
        //			continue;
        //		}
        //		MechanitorControlGroup savedGroup = pawn.mechanitor.GetControlGroup(mech);
        //		MechanoidsUtility.SetOverseer(pawn, mech);
        //		savedGroup.Assign(mech);
        //	}
        //}

        private int? allMechsCount;
        public int AllMechsCount
        {
            get
            {
                if (!allMechsCount.HasValue)
                {
                    allMechsCount = pawn.mechanitor.ControlledPawns.Where((mech) => !mech.Dead).ToList().Count;
                }
                return allMechsCount.Value;
            }
        }

        private float? sphereChance;
        public float SphereChance
        {
            get
            {
                if (!sphereChance.HasValue)
                {
                    cachedMaxMechs = null;
                    float chance = (AllMechsCount - MaxMechs) > 0 ? (AllMechsCount - MaxMechs) * 0.01f : 0f;
                    sphereChance = chance > 0.5f ? 0.5f : chance;
                }
                return sphereChance.Value;
            }
        }

        public void OffsetResource(float value)
        {
            //Log.Error("Resource gain: " + value);
            geneResource = Mathf.Clamp((geneResource + value), 0f, MaxResource);
        }

        public List<PawnKindDef> selectedMechs = new();

        private bool TrySummonMechs()
        {
            string phase = "start";
            try
            {
                if (pawn.MapHeld == null || pawn.mechanitor == null)
                {
                    return false;
                }
                if (selectedMechs.NullOrEmpty())
                {
                    return false;
                }
                phase = "check nociosphere presence";
                if (NociospherePresence())
                {
                    Messages.Message("WVC_XaG_Gene_Voidlink_NociospherePresence".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    return false;
                }
                phase = "get mech cost";
                PawnKindDef mechKind = selectedMechs.RandomElement();
                float consume = MechanoidHolder.GetVoidMechCost(mechKind, WVC_Biotech.settings.voidlink_mechCostLimit) * 0.01f;
                if (consume > geneResource)
                {
                    //selectedMechs = new();
                    Messages.Message("WVC_XaG_Gene_Voidlink_CannotSummon".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    return false;
                }
                phase = "try find spawn spot near mechanitor";
                if (!CellFinder.TryFindRandomCellNear(pawn.PositionHeld, pawn.MapHeld, Mathf.FloorToInt(4.9f), pos => pos.Standable(pawn.MapHeld) && pos.Walkable(pawn.MapHeld) && !pos.Fogged(pawn.MapHeld), out var spawnCell, 100))
                {
                    //selectedMechs = new();
                    Messages.Message("WVC_XaG_Gene_Voidlink_CannotSummon".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    return false;
                }
                phase = "get mechs count and sphere chance";
                if (Rand.Chance(SphereChance))
                {
                    phase = "try summon sphere and activate it";
                    Pawn sphere = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Nociosphere, Faction.OfEntities, PawnGenerationContext.NonPlayer, pawn.MapHeld.Tile));
                    NociosphereUtility.SkipTo((Pawn)GenSpawn.Spawn(sphere, spawnCell, pawn.MapHeld), spawnCell);
                    CompActivity activity = sphere.TryGetComp<CompActivity>();
                    if (activity != null)
                    {
                        activity.AdjustActivity(1f);
                    }
                    return false;
                }
                phase = "generate mech";
                selectedMechs.Remove(mechKind);
                Pawn mech = TryGetNewMech(mechKind, ref phase);
                phase = "effects and spawn";
                MiscUtility.DoSkipEffects(spawnCell, pawn.MapHeld);
                //if (mech.Spawned)
                //{
                //    MiscUtility.DoSkipEffects(mech.Position, pawn.MapHeld);
                //    mech.Position = spawnCell;
                //}
                //else
                //{
                //}
                GenSpawn.Spawn(mech, spawnCell, pawn.MapHeld);
                MechanoidsUtility.SetOverseer(pawn, mech);
                phase = "hediffs and resource offset";
                HediffUtility.TryAddOrRemoveHediff(Spawner.mechHediff, mech, this, null);
                UpdHediff();
                OffsetResource(-1f * consume);
                return true;
            }
            catch (Exception arg)
            {
                Log.Error("Voidlink failed summon. On phase: " + phase + ". Reason: " + arg);
            }
            return false;
        }

        private Pawn TryGetNewMech(PawnKindDef mechKind, ref string phase)
        {
            Pawn mech = null;
            foreach (Pawn item in PawnsFinder.All_AliveOrDead)
            {
                if (item.kindDef != mechKind)
                {
                    continue;
                }
                if (!item.Dead)
                {
                    continue;
                }
                if (item.health.hediffSet.HasHediff<HediffWithComps_VoidMechanoid>())
                {
                    mech = item;
                    break;
                }
            }
            if (mech == null)
            {
                PawnGenerationRequest request = new(mechKind, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
                mech = PawnGenerator.GeneratePawn(request);
                phase = "set mech age";
                AgelessUtility.SetAge(mech, 3600000 * new IntRange(5, 44).RandomInRange);
            }
            float chance = pawn.GetStatValue(Spawner.voidDamageChance_StatDef);
            if (mech.Dead)
            {
                chance *= 5;
                //if (mech.Corpse != null)
                //{
                //    MiscUtility.DoSkipEffects(mech.Corpse.PositionHeld, mech.Corpse.MapHeld);
                //}
                GeneResourceUtility.TryResurrectWithSickness(mech, false, 0f);
            }
            if (Rand.Chance(chance))
            {
                phase = "set random damage";
                FloatRange healthRange = new(0.65f, 0.95f);
                float minHealth = healthRange.RandomInRange;
                while (mech.health.summaryHealth.SummaryHealthPercent > minHealth)
                {
                    BodyPartRecord part = mech.health.hediffSet.GetNotMissingParts().Where((part) => !mech.health.hediffSet.GetInjuredParts().Contains(part)).RandomElement();
                    IntRange damageRange = new(1, 20);
                    int num = (int)pawn.health.hediffSet.GetPartHealth(part) - 1;
                    int randomInRange = damageRange.RandomInRange;
                    HealingUtility.TakeDamage(mech, part, Rand.Chance(0.5f) ? DamageDefOf.Blunt : DamageDefOf.Crush, randomInRange > num ? num : randomInRange);
                    AgelessUtility.AddAgeInYears(mech, 1);
                }
            }
            return mech;
        }

        private bool NociospherePresence()
        {
            foreach (Pawn pawn in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.kindDef == PawnKindDefOf.Nociosphere)
                {
                    return true;
                }
            }
            return false;
        }

        public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref geneResource, "geneResource", 0);
            Scribe_Collections.Look(ref selectedMechs, "selectedMechs", LookMode.Def);
            Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
        }

        public void UpdHediff()
        {
            if (pawn.health.hediffSet.TryGetHediff(out HediffWithComps_VoidMechanitor hediff))
            {
                hediff.Recache();
            }
            else
            {
                HediffUtility.TryAddOrRemoveHediff(Spawner.mechanitorHediff, pawn, this, null);
            }
        }
    }

}
