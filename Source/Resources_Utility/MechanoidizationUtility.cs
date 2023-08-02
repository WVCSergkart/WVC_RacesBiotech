using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    // [StaticConstructorOnStartup]
    public static class MechanoidizationUtility
    {

        public static bool ShouldNotSendNotificationAbout(Pawn pawn)
        {
            GeneExtension_General modExtension = pawn.def.GetModExtension<GeneExtension_General>();
            if (modExtension != null)
            {
                return !modExtension.shouldSendNotificationAbout;
            }
            return false;
        }

        public static bool PawnIsAndroid(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PawnIsGolem(Pawn pawn)
        {
            if (pawn.RaceProps.IsMechanoid)
            {
                return pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_SelfPopulationRegulation_Golems);
            }
            return false;
        }

        public static bool PawnCannotUseSerums(Pawn pawn)
        {
            if (!pawn.RaceProps.Humanlike)
            {
                return true;
            }
            List<Def> blackListedThings = new();
            foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
            {
                blackListedThings.AddRange(item.blackListedDefsForSerums);
            }
            if (blackListedThings.Contains(pawn.def))
            {
                return true;
            }
            if (pawn?.genes == null)
            {
                return false;
            }
            List<GeneDef> nonCandidates = new();
            foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
            {
                nonCandidates.AddRange(item.nonCandidatesForSerums);
            }
            for (int i = 0; i < nonCandidates.Count; i++)
            {
                if (HasActiveGene(nonCandidates[i], pawn))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DelayedReimplanterIsActive(Pawn pawn)
        {
            if (pawn.health != null && pawn.health.hediffSet != null)
            {
                List<HediffDef> hediffDefs = new();
                foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
                {
                    hediffDefs.AddRange(item.blackListedHediffDefForReimplanter);
                }
                for (int i = 0; i < hediffDefs.Count; i++)
                {
                    if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool PawnIsExoskinned(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            List<GeneDef> whiteListedGenes = new();
            foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
            {
                whiteListedGenes.AddRange(item.whiteListedExoskinGenes);
            }
            for (int i = 0; i < whiteListedGenes.Count; i++)
            {
                if (HasActiveGene(whiteListedGenes[i], pawn))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotAcceptablePrey(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].Active == true)
                {
                    GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
                    if (modExtension != null)
                    {
                        if (!modExtension.canBePredatorPrey)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool PawnSkillsNotDecay(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].Active == true)
                {
                    GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
                    if (modExtension != null)
                    {
                        if (modExtension.noSkillDecay)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsAngelBeauty(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].Active == true)
                {
                    GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
                    if (modExtension != null)
                    {
                        if (modExtension.geneIsAngelBeauty)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasActiveGene(GeneDef geneDef, Pawn pawn)
        {
            if (geneDef == null)
            {
                return false;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].Active == true && genesListForReading[i].def == geneDef)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EyesShouldBeInvisble(Pawn pawn)
        {
            if (pawn?.genes == null || pawn?.story == null || pawn?.story?.headType == null)
            {
                return false;
            }
            if (pawn.story.headType.defName.Contains("WVC_Faceless"))
            {
                return true;
            }
            return false;
        }

        // ===============================================================

        public static void ReimplantEndogerm(Pawn caster, Pawn recipient)
        {
            if (!ModLister.CheckBiotech("xenogerm reimplantation"))
            {
                return;
            }
            QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
            ReimplanterUtility.ReimplantGenesBase(caster, recipient);
            GeneUtility.ExtractXenogerm(caster);
        }

        // ===============================================================

        public static void MechSummonQuest(Pawn pawn, QuestScriptDef quest)
        {
            Slate slate = new();
            slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(pawn.Map));
            slate.Set("asker", pawn);
            _ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
            // QuestUtility.SendLetterQuestAvailable(quest);
        }

    }
}
