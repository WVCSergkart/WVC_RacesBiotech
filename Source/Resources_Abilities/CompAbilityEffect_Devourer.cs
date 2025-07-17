using RimWorld;
using System;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class CompAbilityEffect_Devourer : CompAbilityEffect_ChimeraDependant
    {

        //[Unsaved(false)]
        //private Gene_FleshmassNucleus cachedFleshGene;

        //public Gene_FleshmassNucleus FleshmassNucleusGene
        //{
        //    get
        //    {
        //        if (cachedFleshGene == null || !cachedFleshGene.Active)
        //        {
        //            cachedFleshGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_FleshmassNucleus>();
        //        }
        //        return cachedFleshGene;
        //    }
        //}

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn victim = target.Pawn;
            if (victim != null && ChimeraGene != null)
            {
                DevourTarget(victim);
            }
        }

        private void DevourTarget(Pawn victim)
        {
            string phase = "start";
            try
            {
                if (!ChimeraGene.TryAddGenesFromList(victim.genes.GenesListForReading))
                {
                    return;
                }
                Pawn caster = parent.pawn;
                phase = "change goodwill";
                if (victim.HomeFaction != null && !victim.HomeFaction.IsPlayer && !victim.HostileTo(caster.Faction) || victim.IsQuestLodger())
                {
                    int goodwillChange = (victim.RaceProps.Humanlike ? (-29) : (-21)) * (victim.guilt.IsGuilty ? 1 : 2);
                    if (victim.kindDef.factionHostileOnDeath || victim.kindDef.factionHostileOnKill && !victim.guilt.IsGuilty)
                    {
                        goodwillChange = caster.Faction.GoodwillToMakeHostile(victim.HomeFaction);
                    }
                    victim.HomeFaction.TryAffectGoodwillWith(caster.Faction, goodwillChange, canSendMessage: true, true, reason: RimWorld.HistoryEventDefOf.MemberKilled);
                }
                phase = "reset xenotype";
                float genesFactor = victim.genes.GenesListForReading.Count * 0.01f;
                ReimplanterUtility.SetXenotype(victim, XenotypeDefOf.Baseliner);
                phase = "try copy chimera genes";
                ChimeraGene.TryAddGenesFromList(victim.genes?.GetFirstGeneOfType<Gene_Chimera>()?.CollectedGenes);
                phase = "drop apparel";
                victim.apparel?.DropAll(victim.Position, true, false);
                phase = "get food";
                if (victim.TryGetNeedFood(out Need_Food victimFood) && caster.TryGetNeedFood(out Need_Food casterFood))
                {
                    casterFood.CurLevel += victimFood.CurLevel;
                    for (int i = 0; i < victimFood.CurLevel; i++)
                    {
                        ChimeraGene.GetRandomGene();
                    }
                }
                phase = "copy skills";
                CopySkillsExp(caster, victim, genesFactor);
                phase = "inhumanize";
                if (Rand.Chance(0.25f + (caster.relations.OpinionOf(victim) * 0.01f)))
                {
                    Gene_Inhumanized.Inhumanize(caster);
                }
                phase = "meat boom";
                MiscUtility.MeatSplatter(victim, FleshbeastUtility.MeatExplosionSize.Large, 7);
                phase = "kill and destroy";
                victim.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: caster));
                victim.Corpse?.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: caster));
                phase = "add hediff";
                if (Props.hediffDef != null)
                {
                    Hediff hediff = caster.health.GetOrAddHediff(Props.hediffDef);
                    HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (hediffComp_Disappears != null)
                    {
                        hediffComp_Disappears.SetDuration(hediffComp_Disappears.ticksToDisappear += (int)(60000 * victim.BodySize));
                    }
                }
                phase = "try fleshmass overgrow";
                caster.genes?.GetFirstGeneOfType<Gene_FleshmassNucleus>()?.TryGiveMutation();
                phase = "message";
                Messages.Message("WVC_XaG_GeneManeater_VictimEated".Translate(victim.NameShortColored), victim, MessageTypeDefOf.NeutralEvent, historical: false);
            }
            catch (Exception arg)
            {
                Log.Error("Failed devour target: " + victim.Name + ". On phase: " + phase + ". Reason: " + arg);
            }
        }

        private void CopySkillsExp(Pawn casterPawn, Pawn victimPawn, float factor)
        {
            foreach (SkillRecord victimSkillRecord in victimPawn.skills.skills.ToList())
            {
                if (victimSkillRecord.TotallyDisabled)
                {
                    continue;
                }
                foreach (SkillRecord casterSkillRecord in casterPawn.skills.skills.ToList())
                {
                    if (casterSkillRecord.TotallyDisabled)
                    {
                        continue;
                    }
                    casterSkillRecord.Learn(victimSkillRecord.XpTotalEarned * factor);
                }
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (parent.pawn.IsQuestLodger())
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_PawnIsQuestLodgerMessage".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (ChimeraGene == null || !pawn.IsHuman())
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        {
            if (target.Pawn.HostileTo(parent.pawn))
            {
                return null;
            }
            return Dialog_MessageBox.CreateConfirmation("WVC_XaG_WarningPawnWillDieFromDevourer".Translate(target.Pawn.Named("PAWN")), confirmAction, destructive: true);
        }

    }

}
