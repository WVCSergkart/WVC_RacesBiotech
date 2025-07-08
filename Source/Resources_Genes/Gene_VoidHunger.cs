using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Gene_VoidHunger : Gene_HemogenOffset, IGeneOverridden
    {

        public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

        public GeneExtension_Opinion Opinion => def?.GetModExtension<GeneExtension_Opinion>();

        public override float ResourceLossPerDay
        {
            get
            {
                if (Victim == null)
                {
                    return hemogenDrain;
                }
                return -1f;
            }
        }

        private Pawn victim;

        public Pawn Victim
        {
            get
            {
                if (victim != null && (victim.Dead || victim.Destroyed))
                {
                    victim = null;
                }
                return victim;
            }
        }

        public void SetVictim(Pawn newVictim)
        {
            try
            {
                if (newVictim != null)
                {
                    ResetVictim();
                    victim = newVictim;
                    Hediff_VoidDrain hediff_Phylactery = (Hediff_VoidDrain)newVictim.health.GetOrAddHediff(Giver.hediffDef);
                    if (hediff_Phylactery != null)
                    {
                        newVictim.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.AboutMeThoughtDef, pawn);
                        newVictim.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.myTargetInGeneralThought);
                        hemogenDrain = 0.05f;
                        hediff_Phylactery.SetOwner(pawn);
                    }
                    else
                    {
                        Log.Error("Failed create drain on target: " + newVictim.Name);
                        ResetVictim(true);
                    }
                }
            }
            catch
            {
                Log.Error("Failed create drain on target: " + newVictim.Name);
                ResetVictim(true);
            }
        }

        public void ResetVictim(bool removeHediff = true)
        {
            if (removeHediff)
            {
                Victim?.health?.hediffSet?.GetFirstHediff<Hediff_VoidDrain>()?.Notify_VictimChanged();
            }
            victim = null;
        }

        public override void PostAdd()
        {
            base.PostAdd();
        }

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!pawn.IsHashIntervalTick(43299, delta))
            {
                return;
            }
            if (pawn.Faction != Faction.OfPlayer)
            {
                if (victim != null)
                {
                    ResetVictim();
                }
                hemogenDrain = 0.05f;
                return;
            }
            if (Victim == null)
            {
                hemogenDrain += 0.05f;
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            ResetVictim();
        }

        private float hemogenDrain = 0.05f;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hemogenDrain, "hemogenDrain", 0.05f);
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            hemogenDrain = 0.05f;
            ResetVictim();
        }

        public override void Reset()
        {
            ResetVictim();
        }

        public void Notify_Override()
        {

        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            ResetVictim();
        }

    }

}
