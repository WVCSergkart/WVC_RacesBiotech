using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
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
                newHolder.currentLevel = skillRecord.levelInt;
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

        public override void UpdGeneSync()
        {
            base.UpdGeneSync();
            SyncSkills(HivemindUtility.HivemindPawns);
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
                    if (skillRecord.levelInt < value)
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
                    if (value < skillRecord.levelInt)
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

}
