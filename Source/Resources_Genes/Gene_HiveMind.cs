using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Gene_HiveMind : Gene, IGeneOverridden
    {

        public override string LabelCap => base.LabelCap + " connected with " + bondedPawns.Count + " pawn(s).";

        private List<Pawn> bondedPawns = new();

        public override void PostAdd()
        {
            base.PostAdd();
            ResetTicker();
        }

        private int nextTick = 6000;

        public override void TickInterval(int delta)
        {
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            SyncHive();
            ResetTicker();
        }

        private void ResetTicker()
        {
            nextTick = new IntRange(30000, 60000).RandomInRange;
        }

        public void TickDelay()
        {
            nextTick += 60000;
        }

        public void SyncWith(Pawn target)
        {
            Gene_HiveMind hiveMind = target.genes.GetFirstGeneOfType<Gene_HiveMind>();
            if (hiveMind == null)
            {
                return;
            }
            List<Pawn> newList = new();
            newList.Add(target);
            newList.Add(pawn);
            foreach (Pawn item in hiveMind.bondedPawns)
            {
                if (!newList.Contains(item))
                {
                    newList.Add(item);
                }
            }
            foreach (Pawn item in bondedPawns)
            {
                if (!newList.Contains(item))
                {
                    newList.Add(item);
                }
            }
            hiveMind.bondedPawns = newList;
            bondedPawns = newList;
            SyncHive();
            ResetTicker();
        }

        public void SyncHive()
        {
            string phase = "start";
            try
            {
                Dictionary<SkillDef, float> sumSkillsExp = new();
                foreach (Pawn otherPawn in bondedPawns)
                {
                    Gene_HiveMind hiveMind = otherPawn.genes.GetFirstGeneOfType<Gene_HiveMind>();
                    if (hiveMind == null)
                    {
                        continue;
                    }
                    phase = "delay others ticker";
                    hiveMind.TickDelay();
                    phase = "get skills";
                    GetSkills(sumSkillsExp, otherPawn);
                }
                foreach (Pawn otherPawn in bondedPawns)
                {
                    phase = "set skills";
                    SetSkills(sumSkillsExp, otherPawn);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive. On phase: " + phase + ". Reason: " + arg);
            }
        }

        private static void SetSkills(Dictionary<SkillDef, float> sumSkillsExp, Pawn otherPawn)
        {
            foreach (SkillRecord skillRecord in otherPawn.skills.skills.ToList())
            {
                if (skillRecord.TotallyDisabled)
                {
                    continue;
                }
                if (sumSkillsExp.TryGetValue(skillRecord.def, out float value))
                {
                    otherPawn.skills.skills.Remove(skillRecord);
                    SkillRecord item = new(otherPawn, skillRecord.def)
                    {
                        levelInt = 0,
                        passion = skillRecord.passion,
                        xpSinceLastLevel = value,
                        xpSinceMidnight = 0
                    };
                    otherPawn.skills.skills.Add(item);
                    item.Learn(1, true, true);
                }
            }
        }

        private static void GetSkills(Dictionary<SkillDef, float> sumSkillsExp, Pawn otherPawn)
        {
            foreach (SkillRecord skillRecord in otherPawn.skills.skills)
            {
                if (skillRecord.TotallyDisabled)
                {
                    continue;
                }
                if (sumSkillsExp.TryGetValue(skillRecord.def, out float value))
                {
                    if (value < skillRecord.XpTotalEarned)
                    {
                        sumSkillsExp.Remove(skillRecord.def);
                        sumSkillsExp[skillRecord.def] = skillRecord.XpTotalEarned;
                    }
                }
                else
                {
                    sumSkillsExp[skillRecord.def] = skillRecord.XpTotalEarned;
                }
            }
        }

        public void DeSyncMe()
        {
            foreach (Pawn otherPawn in bondedPawns)
            {
                Gene_HiveMind hiveMind = otherPawn.genes.GetFirstGeneOfType<Gene_HiveMind>();
                if (hiveMind == null)
                {
                    continue;
                }
                hiveMind.bondedPawns.Remove(pawn);
            }
            bondedPawns = new();
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            DeSyncMe();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: CreateHiveMind",
                    action = delegate
                    {
                        foreach (Pawn otherPawn in pawn.Map.mapPawns.FreeAdultColonistsSpawned)
                        {
                            SyncWith(otherPawn);
                        }
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEV: UpdSync",
                    action = delegate
                    {
                        nextTick = 0;
                    }
                };
            }
        }

        public void Notify_Override()
        {

        }

        public override void PostRemove()
        {
            base.PostRemove();
            DeSyncMe();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref bondedPawns, "bondedPawns", LookMode.Reference);
            Scribe_Values.Look(ref nextTick, "nextTick", 0);
            if (Scribe.mode == LoadSaveMode.LoadingVars && bondedPawns != null && bondedPawns.RemoveAll((Pawn x) => x == null) > 0)
            {
                Log.Warning("Removed null Pawn(s)");
            }
        }

    }

}
