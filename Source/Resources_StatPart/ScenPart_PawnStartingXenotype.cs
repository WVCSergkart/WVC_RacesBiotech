// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_XenotypesAndGenes : ScenPart_PawnModifier
	{

		public List<XenotypeChance> xenotypeChances = new();
        public List<XenotypeDef> allowedXenotypes = new();
        public List<GeneDef> geneDefs = new();
        public bool addMechlink = false;
        public bool nullifyBackstory = false;
        public bool nullifySkills = false;
        //public bool embraceTheVoid = false;
        public bool addSkipEffect = false;
        public List<GeneDef> chimeraGeneDefs = new();
        //public GeneDef chimeraEvolveGeneDef;
        public bool saveOldChimeraGeneSet = false;
        public int startingMutations = 0;
        public IntRange additionalChronoAge = new(0, 0);

        //public override void ExposeData()
        //{
        //    base.ExposeData();
        //    Scribe_Values.Look(ref addMechlink, "addMechlink", false);
        //    Scribe_Values.Look(ref nullifyBackstory, "nullifyBackstory", false);
        //    Scribe_Values.Look(ref context, "context", PawnGenerationContext.All);
        //    Scribe_Values.Look(ref hideOffMap, "hideOffMap", defaultValue: false);
        //}

        protected override void ModifyNewPawn(Pawn p)
        {
            SetXenotype(p);
            SetGenes(p);
            AddMechlink(p);
            NullifyBackstory(p);
            //ChimeraEvolve(p);
            //Void(p);
            ChimeraGenes(p);
            Mutations(p);
            AgeCorrection(p);
            Skills(p);
        }

        //private void Void(Pawn p)
        //{
        //    if (!embraceTheVoid && !ModsConfig.AnomalyActive)
        //    {
        //        return;
        //    }
        //    if (Find.Storyteller.difficulty.AnomalyPlaystyleDef.generateMonolith)
        //    {
        //        Find.Storyteller.difficulty.AnomalyPlaystyleDef = WVC_GenesDefOf.AmbientHorror;
        //    }
        //    p.health.AddHediff(HediffDefOf.VoidTouched);
        //    p.health.AddHediff(HediffDefOf.Inhumanized);
        //}

        public override void PostMapGenerate(Map map)
        {
            if (addSkipEffect && Find.GameInitData != null)
            {
                foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
                {
                    if (startingAndOptionalPawn.Spawned)
                    {
                        MiscUtility.DoSkipEffects(startingAndOptionalPawn.Position, startingAndOptionalPawn.Map);
                    }
                }
            }
        }

        private void Skills(Pawn p)
        {
            if (!nullifySkills)
            {
                return;
            }
            DuplicateUtility.NullifySkills(p, true);
        }

        private void Mutations(Pawn p)
        {
            if (startingMutations <= 0)
            {
                return;
            }
            int cycleTry = 0;
            int nextMutation = 0;
            while (cycleTry < 10 + startingMutations && nextMutation < startingMutations)
            {
                if (Gene_MorphMutations.TryGetBestMutation(p, out HediffDef mutation))
                {
                    if (HediffUtility.TryGiveFleshmassMutation(p, mutation))
                    {
                        nextMutation++;
                    }
                }
                cycleTry++;
            }
        }

        private void AgeCorrection(Pawn p)
        {
            if (additionalChronoAge.max > 0)
            {
                p.ageTracker.AgeChronologicalTicks += (long)(additionalChronoAge.RandomInRange * 3600000L);
            }
        }

        private void ChimeraGenes(Pawn p)
        {
            if (chimeraGeneDefs.NullOrEmpty())
            {
                return;
            }
            XaG_GeneUtility.AddGenesToChimera(p, chimeraGeneDefs);
        }

        //private void ChimeraEvolve(Pawn p)
        //{
        //    if (chimeraEvolveGeneDef == null)
        //    {
        //        return;
        //    }
        //    XaG_GeneUtility.ImplantChimeraEvolveGeneSet(p, chimeraEvolveGeneDef, saveOldChimeraGeneSet);
        //}

        private void SetGenes(Pawn p)
        {
            if (geneDefs.NullOrEmpty())
            {
                return;
            }
            foreach (GeneDef geneDef in geneDefs)
            {
                if (XaG_GeneUtility.HasGene(geneDef, p))
                {
                    continue;
                }
                p.genes.AddGene(geneDef, xenogene: !p.genes.Xenotype.inheritable);
            }
        }

        private void NullifyBackstory(Pawn p)
        {
            if (nullifyBackstory)
            {
                DuplicateUtility.NullifyBackstory(p);
            }
        }

        private void AddMechlink(Pawn p)
        {
            if (addMechlink && !p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
            {
                p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
            }
        }

        private void SetXenotype(Pawn p)
        {
            if (xenotypeChances.NullOrEmpty())
            {
                return;
            }
            List<XenotypeDef> xenotypes = new();
            foreach (XenotypeChance xenoChance in xenotypeChances)
            {
                xenotypes.Add(xenoChance.xenotype);
            }
            if (xenotypes.Contains(p.genes.Xenotype) || !allowedXenotypes.NullOrEmpty() && allowedXenotypes.Contains(p.genes.Xenotype))
            {
                return;
            }
            if (xenotypeChances.TryRandomElementByWeight((XenotypeChance xenoChance) => xenoChance.chance, out XenotypeChance xenotypeChance))
            {
                ReimplanterUtility.SetXenotype_DoubleXenotype(p, xenotypeChance.xenotype);
            }
        }

        private string cachedDesc = null; 

        public override string Summary(Scenario scen)
        {
            if (cachedDesc == null)
            {
                StringBuilder stringBuilder = new();
                if (!xenotypeChances.NullOrEmpty())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_AllowedXenotypes".Translate().CapitalizeFirst() + ":\n" + xenotypeChances.Select((XenotypeChance x) => x.xenotype.LabelCap.ToString()).ToLineList(" - ") + (!allowedXenotypes.NullOrEmpty() ? "\n" + allowedXenotypes.Select((XenotypeDef x) => x.LabelCap.ToString()).ToLineList(" - ") : ""));
                }
                if (addMechlink)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_AddMechlink".Translate().CapitalizeFirst());
                }
                if (nullifyBackstory)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_NullifyBackstory".Translate().CapitalizeFirst());
                }
                if (nullifySkills)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_NullifySkills".Translate().CapitalizeFirst());
                }
                //if (embraceTheVoid)
                //{
                //    stringBuilder.AppendLine();
                //    stringBuilder.AppendLine("WVC_XaG_ScenPart_EmbraceTheVoid".Translate().CapitalizeFirst());
                //}
                if (!geneDefs.NullOrEmpty())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_StartingGenes".Translate().CapitalizeFirst() + ":\n" + geneDefs.Select((GeneDef x) => x.LabelCap.ToString()).ToLineList(" - "));
                }
                if (!chimeraGeneDefs.NullOrEmpty())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_ChimeraStartingGenes".Translate().CapitalizeFirst() + ":\n" + chimeraGeneDefs.Select((GeneDef x) => x.LabelCap.ToString()).ToLineList(" - "));
                }
                //if (chimeraEvolveGeneDef != null)
                //{
                //    stringBuilder.AppendLine();
                //    stringBuilder.AppendLine("WVC_XaG_ScenPart_ChimeraStartingEvolution".Translate().CapitalizeFirst());
                //}
                if (startingMutations > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_StartingMutations".Translate(startingMutations).CapitalizeFirst());
                }
                if (additionalChronoAge.max > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("WVC_XaG_ScenPart_AddChronoAge".Translate(additionalChronoAge.min, additionalChronoAge.max).CapitalizeFirst());
                }
                cachedDesc = stringBuilder.ToString();
            }
            return cachedDesc;
        }

    }

    //public class ScenPart_ForcedMechanitor : ScenPart_PawnModifier
    //{

    //    protected override void ModifyNewPawn(Pawn p)
    //    {
    //        if (!p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
    //        {
    //            p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
    //        }
    //    }

    //}

}
