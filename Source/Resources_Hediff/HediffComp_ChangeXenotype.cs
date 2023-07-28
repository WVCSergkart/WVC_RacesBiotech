using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_ChangeXenotype : HediffCompProperties
    {
        public float requiredSeverity = 1.0f;

        // public bool endogeneTransfer = true;

        // public bool xenogeneTransfer = true;

        public HediffCompProperties_ChangeXenotype()
        {
            compClass = typeof(HediffComp_ChangeXenotype);
        }
    }

    public class HediffComp_ChangeXenotype : HediffComp
    {
        public HediffCompProperties_ChangeXenotype Props => (HediffCompProperties_ChangeXenotype)props;

        // protected int GestationIntervalDays => Props.gestationIntervalDays;

        public override string CompLabelInBracketsExtra => GetLabel();

        public Pawn genesOwner = null;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (!Pawn.IsHashIntervalTick(1500))
            {
                return;
            }
            Pawn pawn = parent.pawn;
            if (pawn.Map == null)
            {
                return;
            }
            if (genesOwner == null)
            {
                base.Pawn.health.RemoveHediff(parent);
                return;
            }
            if (parent.Severity >= Props.requiredSeverity)
            {
                DustUtility.ReimplantGenes(genesOwner, parent.pawn);
                base.Pawn.health.RemoveHediff(parent);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref genesOwner, "genesOwner");
        }

        public string GetLabel()
        {
            // Pawn mechanoid = parent.pawn;
            if (genesOwner != null)
            {
                return genesOwner.NameShortColored;
            }
            return "ERR";
        }
    }

}
