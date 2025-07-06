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
            if (newVictim != null)
            {
                RemoveVictim();
                victim = newVictim;
                Hediff_VoidDrain hediff_Phylactery = (Hediff_VoidDrain)newVictim.health.GetOrAddHediff(Giver.hediffDef);
                if (hediff_Phylactery != null)
                {
                    hemogenDrain = 0.05f;
                    hediff_Phylactery.phylacteryOwner = pawn;
                }
                else
                {
                    Log.Error("Failed create phylactery on target: " + victim.Name);
                    victim = null;
                }
            }
        }

        public void RemoveVictim()
        {
            Victim?.health?.hediffSet?.GetFirstHediff<Hediff_VoidDrain>()?.Notify_VictimChanged();
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
            if (Victim == null)
            {
                hemogenDrain += 0.05f;
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            RemoveVictim();
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
            RemoveVictim();
        }

        public override void Reset()
        {
            RemoveVictim();
        }

        public void Notify_Override()
        {

        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            RemoveVictim();
        }

    }

}
