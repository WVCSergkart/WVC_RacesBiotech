using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffComp_GolemGestator : HediffComp
    {
        private readonly int ticksInday = 60000;
        // private readonly int ticksInday = 1500;

        private int ticksCounter = 0;

        private Pawn childOwner = null;

        public HediffCompProperties_GolemGestator Props => (HediffCompProperties_GolemGestator)props;

        protected int GestationIntervalDays => Props.gestationIntervalDays;

        // protected string customString => Props.customString;

        // protected bool produceEggs => Props.produceEggs;

        // protected string eggDef => Props.eggDef;

        public override string CompLabelInBracketsExtra => GetLabel();

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
            Scribe_References.Look(ref childOwner, "childOwner");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Pawn golem = parent.pawn;
            Pawn pawn = golem.GetOverseer();
            if (pawn == null)
            {
                base.Pawn.Kill(null, parent);
                return;
            }
            if (childOwner == null)
            {
                childOwner = pawn;
            }
            // if (pawn != childOwner)
            // {
            // base.Pawn.Kill(null, parent);
            // return;
            // }
            if (golem.Map == null)
            {
                return;
            }
            // || !pawn.ageTracker.CurLifeStage.reproductive
            if (golem.Faction != Faction.OfPlayer)
            {
                return;
            }
            ticksCounter++;
            if (ticksCounter < ticksInday * GestationIntervalDays)
            {
                return;
            }
            // if (Props.convertsIntoAnotherDef)
            // {
            // PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named(Props.newDef), pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: true, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: false, allowPregnant: true);
            // Pawn pawn2 = PawnGenerator.GeneratePawn(request);
            // PawnUtility.TrySpawnHatchedOrBornPawn(pawn2, pawn);
            // Messages.Message(Props.asexualHatchedMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
            // asexualFissionCounter = 0;
            // return;
            // }
            if (MechanoidizationUtility.HasActiveGene(Props.geneDef, childOwner))
            {
                GestationUtility.GenerateNewBornPawn(childOwner, Props.completeMessage, Props.endogeneTransfer, Props.xenogeneTransfer, spawnPawn: golem);
            }
            base.Pawn.Kill(null, parent);
            ticksCounter = 0;
        }

        public string GetLabel()
        {
            Pawn pawn = parent.pawn;
            // if (Props.isGreenGoo)
            // {
            // float f = (float)asexualFissionCounter / (float)(ticksInday * gestationIntervalDays);
            // return customString + f.ToStringPercent() + " (" + gestationIntervalDays + " days)";
            // }
            if (pawn.Faction == Faction.OfPlayer)
            {
                float percent = (float)ticksCounter / (float)(ticksInday * GestationIntervalDays);
                return percent.ToStringPercent(); // + " (" + GestationIntervalDays + " days)"
            }
            return "";
        }
    }

}
