using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffAddedPart_FleshmassNucleus : Hediff_AddedPart
	{

		private int mutationLevel = 0;

		//public int maxMutationLevel = 5;

		public static List<StatDef> IgnoredStatDefs = new()
		{
			StatDefOf.PawnBeauty
		};

        public override float PainOffset => 0f;

        public override float PainFactor => 1f;

        //public override Color LabelColor => ColorLibrary.LightPink;
        //public override float SummaryHealthPercentImpact => 5f * mutationLevel;

        private HediffStage curStage;

        public override string LabelInBrackets => "WVC_Level".Translate(CurrentLevel);

        public override bool Visible => !WVC_Biotech.settings.fleshmass_HideBodypartHediffs;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
                {
                    GetHediffStage(def, ref curStage, def.stages[CurStageIndex], mutationLevel, IgnoredStatDefs);
                }
                return curStage;
			}
		}

        public static void GetHediffStage(HediffDef def, ref HediffStage curStage, HediffStage defStage, int mutationLevel, List<StatDef> ignoredStatDefs)
        {
            curStage = new();
            //curStage = def.stages[CurStageIndex];
            //HediffStage defStage = defStage;
            curStage.foodPoisoningChanceFactor = defStage.foodPoisoningChanceFactor;
            curStage.partEfficiencyOffset = defStage.partEfficiencyOffset;
            curStage.hungerRateFactor = defStage.hungerRateFactor;
            curStage.hungerRateFactorOffset = defStage.hungerRateFactorOffset;
            curStage.totalBleedFactor = defStage.totalBleedFactor;
            curStage.fertilityFactor = defStage.fertilityFactor;
            curStage.naturalHealingFactor = defStage.naturalHealingFactor;
            curStage.partIgnoreMissingHP = defStage.partIgnoreMissingHP;
            curStage.regeneration = defStage.regeneration;
            curStage.restFallFactor = defStage.restFallFactor;
            curStage.restFallFactorOffset = defStage.restFallFactorOffset;
            if (!defStage.makeImmuneTo.NullOrEmpty())
            {
                curStage.makeImmuneTo = new();
                foreach (HediffDef hediffDef in defStage.makeImmuneTo)
                {
                    curStage.makeImmuneTo.Add(hediffDef);
                }
            }
            if (!defStage.capMods.NullOrEmpty())
            {
                curStage.capMods = new();
                foreach (PawnCapacityModifier pawnCapacityModifier in defStage.capMods)
                {
                    //if (pawnCapacityModifier.offset < 0f || pawnCapacityModifier.postFactor < 1f || pawnCapacityModifier.setMax < 2f)
                    //{
                    //	continue;
                    //}
                    PawnCapacityModifier newPawnCapacityModifier = new();
                    newPawnCapacityModifier.statFactorMod = pawnCapacityModifier.statFactorMod;
                    newPawnCapacityModifier.setMaxCurveOverride = pawnCapacityModifier.setMaxCurveOverride;
                    newPawnCapacityModifier.setMaxCurveEvaluateStat = pawnCapacityModifier.setMaxCurveEvaluateStat;
                    newPawnCapacityModifier.capacity = pawnCapacityModifier.capacity;
                    newPawnCapacityModifier.offset = pawnCapacityModifier.offset;
                    newPawnCapacityModifier.postFactor = pawnCapacityModifier.postFactor;
                    //newPawnCapacityModifier.setMax = pawnCapacityModifier.setMax;
                    curStage.capMods.Add(newPawnCapacityModifier);
                }
            }
            if (!defStage.statFactors.NullOrEmpty())
            {
                curStage.statFactors = new();
                foreach (StatModifier statModifier in defStage.statFactors)
                {
                    if (ignoredStatDefs.Contains(statModifier.stat))
                    {
                        continue;
                    }
                    StatModifier newStatMod = MiscUtility.CopyStatModifier(statModifier);
                    curStage.statFactors.Add(newStatMod);
                }
            }
            if (!defStage.statOffsets.NullOrEmpty())
            {
                curStage.statOffsets = new();
                foreach (StatModifier statModifier in defStage.statOffsets)
                {
                    if (ignoredStatDefs.Contains(statModifier.stat))
                    {
                        continue;
                    }
                    StatModifier newStatMod = MiscUtility.CopyStatModifier(statModifier);
                    curStage.statOffsets.Add(newStatMod);
                }
            }
            if (mutationLevel > 0)
            {
                if (def.addedPartProps.partEfficiency <= 1f)
                {
                    if (curStage.partEfficiencyOffset > 0)
                        curStage.partEfficiencyOffset += mutationLevel * 0.02f;
                    else
                        curStage.partEfficiencyOffset = mutationLevel * 0.04f;
                }
                //if (curStage.regeneration > 0f)
                //    curStage.regeneration += mutationLevel;
                //else
                //    curStage.regeneration = mutationLevel * 2f;
                if (mutationLevel >= 5)
                    curStage.partIgnoreMissingHP = true;
            }
        }

        //public bool CanLevelUp => mutationLevel < maxMutationLevel;

        public int CurrentLevel => mutationLevel;

        public void LevelUp()
		{
			mutationLevel++;
			//Log.Error("Current level: " + mutationLevel);
			curStage = null;
			HediffUtility.MutationMeatSplatter(pawn);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref mutationLevel, "mutationLevel", 0);
			//Scribe_Values.Look(ref maxMutationLevel, "maxMutationLevel", 5);
		}

    }

    //public class HediffImplant_FleshmassNucleus : Hediff_Implant
    //{

    //    private int mutationLevel = 0;

    //    public override float PainOffset => 0f;

    //    public override float PainFactor => 1f;

    //    private HediffStage curStage;

    //    public override bool Visible => true;

    //    public override HediffStage CurStage
    //    {
    //        get
    //        {
    //            if (curStage == null)
    //            {
    //                HediffAddedPart_FleshmassNucleus.GetHediffStage(def, ref curStage, def.stages[CurStageIndex], mutationLevel, HediffAddedPart_FleshmassNucleus.IgnoredStatDefs);
    //            }
    //            return curStage;
    //        }
    //    }

    //    public int CurrentLevel => mutationLevel;

    //    public void LevelUp()
    //    {
    //        mutationLevel++;
    //        curStage = null;
    //        HediffUtility.MutationMeatSplatter(pawn);
    //    }

    //    public override void ExposeData()
    //    {
    //        base.ExposeData();
    //        Scribe_Values.Look(ref mutationLevel, "mutationLevel", 0);
    //    }

    //}

}
