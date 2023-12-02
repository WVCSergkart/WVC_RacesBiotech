// RimWorld.CompToxifier
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class CompToxifier : ThingComp
    {
        private IntVec3 nextPollutionCell = IntVec3.Invalid;

        private int pollutingProgressTicks;

        // private int pollutionIntervalTicks;

        private CompProperties_Toxifier Props => (CompProperties_Toxifier)props;

        public bool CanPolluteNow
        {
            get
            {
                if (!parent.Spawned)
                {
                    return false;
                }
                if (!nextPollutionCell.CanPollute(parent.Map))
                {
                    UpdateNextPolluteCell();
                }
                return nextPollutionCell.CanPollute(parent.Map);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            // if (!ModLister.CheckBiotech("Toxifier"))
            // {
            // parent.Destroy();
            // return;
            // }
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                ResetInterval();
                // pollutingProgressTicks = pollutionIntervalTicks;
            }
        }

        public override string CompInspectStringExtra()
        {
            TaggedString taggedString = base.CompInspectStringExtra();
            if (CanPolluteNow)
            {
                if (!taggedString.NullOrEmpty())
                {
                    taggedString += "\n";
                }
                taggedString += (string)("PollutingTerrainProgress".Translate() + ": " + (pollutingProgressTicks).ToStringTicksToPeriod() + " (") + Props.cellsToPollute + " " + "TilesLower".Translate() + ")";
            }
            return taggedString.Resolve();
        }

        private void UpdateNextPolluteCell()
        {
            if (nextPollutionCell.CanPollute(parent.Map))
            {
                return;
            }
            nextPollutionCell = IntVec3.Invalid;
            int num = GenRadial.NumCellsInRadius(Props.radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c2 = parent.Position + GenRadial.RadialPattern[i];
                if (NextPolluteCellValidator(c2))
                {
                    nextPollutionCell = c2;
                    break;
                }
            }
            bool NextPolluteCellValidator(IntVec3 c)
            {
                if (!c.InBounds(parent.Map))
                {
                    return false;
                }
                if (!c.CanPollute(parent.Map))
                {
                    return false;
                }
                return true;
            }
        }

        public void PolluteNextCell(bool silent = false)
        {
            if (!CanPolluteNow)
            {
                return;
            }
            int num = GenRadial.NumCellsInRadius(Props.radius);
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = parent.Position + GenRadial.RadialPattern[i];
                if (c.InBounds(parent.Map) && c.CanPollute(parent.Map))
                {
                    c.Pollute(parent.Map, silent);
                    num2++;
                    if (num2 >= Props.cellsToPollute)
                    {
                        break;
                    }
                }
            }
            if (!silent)
            {
                DoEffects();
            }
        }

        private void DoEffects()
        {
            FleckMaker.Static(parent.TrueCenter(), parent.Map, FleckDefOf.Fleck_ToxifierPollutionSource);
            SoundDefOf.Toxifier_Pollute.PlayOneShot(parent);
        }

        public override void CompTick()
        {
            base.CompTick();
            Pollute(1);
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            Pollute(250);
        }

        public override void CompTickLong()
        {
            base.CompTickRare();
            Pollute(2000);
        }

        public void Pollute(int tick)
        {
            if (CanPolluteNow)
            {
                pollutingProgressTicks -= tick;
                if (pollutingProgressTicks <= 0)
                {
                    PolluteNextCell();
                    ResetInterval();
                }
            }
        }

        public void ResetInterval()
        {
            pollutingProgressTicks = Props.pollutionIntervalTicks.RandomInRange;
            // pollutingProgressTicks = 0;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action = new()
                {
                    defaultLabel = "DEV: Pollute",
                    action = delegate
                    {
                        ResetInterval();
                        PolluteNextCell();
                    }
                };
                yield return command_Action;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref nextPollutionCell, "nextPollutionCell", IntVec3.Invalid);
            Scribe_Values.Look(ref pollutingProgressTicks, "pollutingProgressTicks", 0);
            // Scribe_Values.Look(ref pollutionIntervalTicks, "pollutionIntervalTicks", 0);
        }
    }

}
