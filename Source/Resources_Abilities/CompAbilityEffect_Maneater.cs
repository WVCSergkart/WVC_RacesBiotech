using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class CompAbilityEffect_Maneater : CompAbilityEffect_ChimeraDependant
    {

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn victim = target.Pawn;
            if (victim != null && ChimeraGene != null)
            {
                if (ChimeraGene.TryAddGenesFromList(victim.genes.GenesListForReading))
                {
                    //ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
                    //ReimplanterUtility.ExtractXenogerm(pawn);
                    if (!victim.Faction.IsPlayer)
                    {
                        victim.Faction.TryAffectGoodwillWith(parent.pawn.Faction, -20);
                    }
                    MiscUtility.DoSkipEffects(victim.Position, victim.Map);
                    victim.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: parent.pawn));
                    victim.Corpse?.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: parent.pawn));
                    if (victim.IsColonist)
                    {
                        if (Props.allOtherPawnsAboutMe != null)
                        {
                            foreach (Pawn otherPawn in parent.pawn.Map.mapPawns.AllHumanlikeSpawned)
                            {
                                otherPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.allOtherPawnsAboutMe, parent.pawn);
                            }
                        }
                        if (Props.allOtherPawns != null)
                        {
                            foreach (Pawn otherPawn in parent.pawn.Map.mapPawns.AllHumanlikeSpawned)
                            {
                                otherPawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.allOtherPawns);
                            }
                        }
                    }
                    if (Props.hediffDef != null)
                    {
                        parent.pawn.health.AddHediff(Props.hediffDef);
                    }
                    Messages.Message("WVC_XaG_GeneManeater_VictimEated".Translate(victim.NameShortColored), victim, MessageTypeDefOf.NeutralEvent, historical: false);
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
            if (!pawn.IsHuman())
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
            return Dialog_MessageBox.CreateConfirmation("WVC_XaG_WarningPawnWillDieFromHarvesting".Translate(target.Pawn.Named("PAWN")), confirmAction, destructive: true);
        }

    }

}
