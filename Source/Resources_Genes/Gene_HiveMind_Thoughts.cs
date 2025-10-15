using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
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
            List<Pawn> bondedPawns = HivemindUtility.HivemindPawns;
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

}
