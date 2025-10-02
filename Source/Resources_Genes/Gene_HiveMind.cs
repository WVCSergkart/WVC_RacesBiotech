using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Hivemind_Drone : Gene, IGeneOverridden, IGeneHivemind
    {

        public override void PostAdd()
        {
            base.PostAdd();
            ResetCollection();
        }

        public void ResetCollection()
        {
            Gene_Hivemind.ResetCollection();
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            ResetCollection();
        }

        public void Notify_Override()
        {
            ResetCollection();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            ResetCollection();
        }

    }

    public class Gene_Hivemind : Gene, IGeneOverridden, IGeneHivemind
    {

        private static List<Pawn> cachedPawns;
        public static List<Pawn> HivemindPawns
        {
            get
            {
                if (cachedPawns == null)
                {
                    cachedPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Where((target) => target.IsPsychicSensitive() && target.genes != null && target.genes.GenesListForReading.Any((gene) => gene is IGeneHivemind)).ToList();
                }
                return cachedPawns;
            }
        }

        public static void ResetCollection()
        {
            cachedPawns = null;
            cachedRefreshRate = null;
            Gene_Chimera_HiveGeneline.cachedGenelineGenes = null;
        }

        private static int? cachedRefreshRate;
        public static int TickRefresh
        {
            get
            {
                if (!cachedRefreshRate.HasValue)
                {
                    cachedRefreshRate = (int)(11992 * ((HivemindPawns.Count > 1 ? HivemindPawns.Count : 5) * 0.4f));
                }
                return cachedRefreshRate.Value;
            }
        }

        public override void PostAdd()
        {
            base.PostAdd();
            UpdHive(true);
        }

        public override void Notify_NewColony()
        {
            base.Notify_NewColony();
            UpdHive(false);
        }

        public int nextTick = 2000;
        public override void TickInterval(int delta)
        {
            //if (!pawn.IsHashIntervalTick(TickRefresh, delta))
            //{
            //    return;
            //}
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            nextTick = new IntRange((int)(TickRefresh * 0.9f), (int)(TickRefresh * 1.2f)).RandomInRange;
            if (pawn.Faction != Faction.OfPlayer)
            {
                return;
            }
            SyncHive();
        }

        public virtual void SyncHive()
        {
            //if (pawn.SpawnedOrAnyParentSpawned)
            //{
            //    FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
            //}
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: SyncHive",
                    action = delegate
                    {
                        SyncHive();
                    }
                };
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            UpdHive(true);
        }

        private void UpdHive(bool syncHive)
        {
            if (!pawn.IsColonist)
            {
                return;
            }
            ResetCollection();
            if (syncHive && MiscUtility.GameStarted())
            {
                SyncHive();
            }
        }

        //public void Notify_GenesChanged(Gene changedGene)
        //{
        //    UpdHive(false);
        //}

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            UpdHive(false);
        }

        public void Notify_Override()
        {
            UpdHive(false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 1200);
        }

    }

    public class Gene_HiveMind_Opinion : Gene_Hivemind
    {

        private GeneExtension_Opinion cachedExtension;
        public GeneExtension_Opinion Opinion
        {
            get
            {
                if (cachedExtension == null)
                {
                    cachedExtension = def?.GetModExtension<GeneExtension_Opinion>();
                }
                return cachedExtension;
            }
        }

        public override void SyncHive()
        {
            base.SyncHive();
            List<Pawn> bondedPawns = HivemindPawns;
            //string phase = "start";
            try
            {
                foreach (Pawn otherPawn in bondedPawns)
                {
                    if (otherPawn == pawn)
                    {
                        continue;
                    }
                    //phase = otherPawn.NameShortColored.ToString();
                    otherPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, pawn);
                    //phase = pawn.NameShortColored.ToString();
                    pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, otherPawn);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive opinions. Reason: " + arg);
            }
        }

    }

    public class Gene_HiveMind_Skills : Gene_Hivemind
    {


        private class SkillRecordHolderList
        {

            public List<SkillRecordHolder> skillRecords = new();

            public bool TryGetLevel(SkillDef skillDef, out int level)
            {
                level = 0;
                foreach (SkillRecordHolder holder in skillRecords)
                {
                    if (skillDef == holder.skillDef)
                    {
                        level = holder.currentLevel;
                        return true;
                    }
                }
                return false;
            }

            public void AddSkill(SkillRecord skillRecord)
            {
                foreach (SkillRecordHolder holder in skillRecords.ToList())
                {
                    if (skillRecord.def == holder.skillDef)
                    {
                        skillRecords.Remove(holder);
                    }
                }
                SkillRecordHolder newHolder = new();
                newHolder.skillDef = skillRecord.def;
                //newHolder.currentExp = skillRecord.xpSinceLastLevel;
                newHolder.currentLevel = skillRecord.Level;
                skillRecords.Add(newHolder);
            }

        }

        private struct SkillRecordHolder
        {

            public SkillDef skillDef;

            //public float currentExp;

            public int currentLevel;

            //public Passion passion;

        }

        public override void SyncHive()
        {
            base.SyncHive();
            SyncSkills(HivemindPawns);
        }

        public static void SyncSkills(List<Pawn> bondedPawns)
        {
            if (bondedPawns == null)
            {
                return;
            }
            string phase = "start";
            try
            {
                SkillRecordHolderList sumSkillsExp = new();
                foreach (Pawn otherPawn in bondedPawns)
                {
                    phase = "get skills";
                    GetSkills(ref sumSkillsExp, otherPawn);
                }
                foreach (Pawn otherPawn in bondedPawns)
                {
                    phase = "set skills";
                    SetSkills(ref sumSkillsExp, otherPawn);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive skills. On phase: " + phase + ". Reason: " + arg);
            }
        }

        private static void SetSkills(ref SkillRecordHolderList sumSkillsExp, Pawn otherPawn)
        {
            foreach (SkillRecord skillRecord in otherPawn.skills.skills.ToList())
            {
                if (skillRecord.TotallyDisabled)
                {
                    continue;
                }
                if (sumSkillsExp.TryGetLevel(skillRecord.def, out int value))
                {
                    if (skillRecord.Level < value)
                    {
                        skillRecord.Level = value;
                        //skillRecord.Learn(0.01f, true, true);
                    }
                }
            }
        }

        private static void GetSkills(ref SkillRecordHolderList sumSkillsExp, Pawn otherPawn)
        {
            foreach (SkillRecord skillRecord in otherPawn.skills.skills)
            {
                if (skillRecord.TotallyDisabled)
                {
                    continue;
                }
                if (sumSkillsExp.TryGetLevel(skillRecord.def, out int value))
                {
                    if (value < skillRecord.Level)
                    {
                        sumSkillsExp.AddSkill(skillRecord);
                    }
                }
                else
                {
                    sumSkillsExp.AddSkill(skillRecord);
                }
            }
        }

    }

    public class Gene_HiveMind_Thoughts : Gene_Hivemind
    {

        private class ThoughtHolder
        {

            public Thought_Memory memory;

            //public float offset;

            public int expireTime;

            public float moodPowerFactor = 1f;

            public int moodOffset;

            public Pawn otherPawn;

            public int age;

        }

        private class ThoughtsHolder
        {

            public List<ThoughtHolder> memoryDefs = new();

            public bool GetThisMemory(Thought_Memory thought_Memory, out ThoughtHolder thoughtHolder)
            {
                thoughtHolder = null;
                foreach (ThoughtHolder holder in memoryDefs)
                {
                    if (holder.memory.def == thought_Memory.def)
                    {
                        thoughtHolder = holder;
                        return true;
                    }
                }
                return false;
            }

            public void AddMemory(Thought_Memory thought_Memory)
            {
                ThoughtHolder newHolder = new();
                newHolder.memory = thought_Memory;
                newHolder.expireTime = thought_Memory.DurationTicks;
                newHolder.moodOffset = thought_Memory.moodOffset;
                newHolder.moodPowerFactor = thought_Memory.moodPowerFactor;
                newHolder.otherPawn = thought_Memory.otherPawn;
                newHolder.age = thought_Memory.age;
                memoryDefs.Add(newHolder);
            }

        }

        public override void SyncHive()
        {
            base.SyncHive();
            List<Pawn> bondedPawns = HivemindPawns;
            string phase = "start";
            try
            {
                ThoughtsHolder sumSkillsExp = new();
                foreach (Pawn otherPawn in bondedPawns)
                {
                    //Gene_HiveMind hiveMind = otherPawn.genes.GetFirstGeneOfType<Gene_HiveMind>();
                    //if (hiveMind == null)
                    //{
                    //    continue;
                    //}
                    phase = "remove wrong memos";
                    RemoveWrongMemos(otherPawn, bondedPawns);
                    phase = "get memories";
                    GetMemos(otherPawn, sumSkillsExp);
                }
                foreach (Pawn otherPawn in bondedPawns)
                {
                    phase = "set memories";
                    SetMemos(otherPawn, sumSkillsExp);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive memories. On phase: " + phase + ". Reason: " + arg);
            }
        }

        private void RemoveWrongMemos(Pawn target, List<Pawn> hive)
        {
            MemoryThoughtHandler thoughtHandler = target.needs?.mood?.thoughts?.memories;
            if (thoughtHandler == null)
            {
                return;
            }
            foreach (Thought_Memory memory in thoughtHandler.Memories.ToList())
            {
                if (!hive.Contains(memory.otherPawn))
                {
                    continue;
                }
                thoughtHandler.RemoveMemory(memory);
            }
        }

        private void GetMemos(Pawn target, ThoughtsHolder holders)
        {
            MemoryThoughtHandler thoughtHandler = target.needs?.mood?.thoughts?.memories;
            if (thoughtHandler == null)
            {
                return;
            }
            foreach (Thought_Memory memory in thoughtHandler.Memories)
            {
                if (memory.permanent)
                {
                    continue;
                }
                if (holders.GetThisMemory(memory, out ThoughtHolder thoughtHolder) && memory.otherPawn == thoughtHolder.otherPawn)
                {
                    if (memory.durationTicksOverride > thoughtHolder.expireTime)
                    {
                        thoughtHolder.expireTime = memory.durationTicksOverride;
                    }
                    if (memory.age > thoughtHolder.age)
                    {
                        thoughtHolder.age = memory.age;
                    }
                    if (memory.moodPowerFactor > thoughtHolder.moodPowerFactor)
                    {
                        thoughtHolder.moodPowerFactor = memory.moodPowerFactor;
                    }
                    if (memory.moodOffset > thoughtHolder.moodOffset)
                    {
                        thoughtHolder.moodOffset = memory.moodOffset;
                    }
                }
                else
                {
                    holders.AddMemory(memory);
                }
            }
        }

        private void SetMemos(Pawn target, ThoughtsHolder holders)
        {
            MemoryThoughtHandler thoughtHandler = target.needs?.mood?.thoughts?.memories;
            if (thoughtHandler == null)
            {
                return;
            }
            foreach (ThoughtHolder holder in holders.memoryDefs)
            {
                bool shouldAdd = false;
                foreach (Thought_Memory memory in thoughtHandler.Memories)
                {
                    if (memory.permanent)
                    {
                        continue;
                    }
                    if (holder.memory.def == memory.def && memory.otherPawn == holder.otherPawn)
                    {
                        memory.durationTicksOverride = holder.expireTime;
                        memory.moodOffset = holder.moodOffset;
                        memory.moodPowerFactor = holder.moodPowerFactor;
                        memory.age = holder.age;
                    }
                    else
                    {
                        shouldAdd = true;
                    }
                }
                if (shouldAdd)
                {
                    holder.memory.durationTicksOverride = holder.expireTime;
                    holder.memory.moodOffset = holder.moodOffset;
                    holder.memory.moodPowerFactor = holder.moodPowerFactor;
                    holder.memory.age = holder.age;
                    thoughtHandler.TryGainMemory(holder.memory, holder.otherPawn);
                }
            }
        }

    }

    public class Gene_HiveMind_Needs : Gene_Hivemind
    {

        public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

        private class NeedHolder
        {

            public NeedDef needDef;

            public float needValue;

        }

        private class NeedsHolder
        {

            public List<NeedHolder> holders = new();

            public bool TryGetNeed(NeedDef needDef, out NeedHolder needHolder)
            {
                needHolder = null;
                foreach (NeedHolder holder in holders)
                {
                    if (holder.needDef == needDef)
                    {
                        needHolder = holder;
                        return true;
                    }
                }
                return false;
            }

            public void AddNeed(Need need)
            {
                NeedHolder needHolder = new();
                needHolder.needDef = need.def;
                needHolder.needValue = need.CurLevel;
                holders.Add(needHolder);
            }

        }

        public override void SyncHive()
        {
            base.SyncHive();
            List<Pawn> bondedPawns = HivemindPawns;
            string phase = "start";
            try
            {
                NeedsHolder sumSkillsExp = new();
                foreach (Pawn otherPawn in bondedPawns)
                {
                    //Gene_HiveMind hiveMind = otherPawn.genes.GetFirstGeneOfType<Gene_HiveMind>();
                    //if (hiveMind == null)
                    //{
                    //    continue;
                    //}
                    phase = "get needs";
                    GetNeeds(otherPawn, sumSkillsExp);
                }
                foreach (Pawn otherPawn in bondedPawns)
                {
                    phase = "set needs";
                    SetNeeds(otherPawn, sumSkillsExp);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive needs. On phase: " + phase + ". Reason: " + arg);
            }
        }

        private void GetNeeds(Pawn target, NeedsHolder needsHolder)
        {
            if (target.needs == null)
            {
                return;
            }
            foreach (Need need in target.needs.AllNeeds)
            {
                if (!Giver.needDefs.Contains(need.def))
                {
                    continue;
                }
                if (needsHolder.TryGetNeed(need.def, out NeedHolder needHolder))
                {
                    if (needHolder.needValue < need.CurLevel)
                    {
                        needHolder.needValue = need.CurLevel;
                    }
                }
                else
                {
                    needsHolder.AddNeed(need);
                }
            }
        }

        private void SetNeeds(Pawn target, NeedsHolder needsHolder)
        {
            //Log.Error("0");
            if (target.needs == null)
            {
                //Log.Error("Fail");
                return;
            }
            //Log.Error("1");
            foreach (Need need in target.needs.AllNeeds)
            {
                //Log.Error("2");
                if (!Giver.needDefs.Contains(need.def))
                {
                    //Log.Error("WTF?");
                    continue;
                }
                //Log.Error("3");
                if (needsHolder.TryGetNeed(need.def, out NeedHolder needHolder))
                {
                    need.CurLevel = needHolder.needValue;
                }
            }
        }

    }

    public class Gene_Hivemind_Mood : Gene_HiveMind_Opinion
    {

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(59998, delta))
            {
                return;
            }
            SyncHive();
        }

        public override void SyncHive()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.thoughtDef);
        }

    }

}
