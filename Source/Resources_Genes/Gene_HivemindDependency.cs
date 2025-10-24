using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_HivemindDependency : Gene_Hivemind_Denier, IGeneInspectInfo
    {

        public int nextTick = 0;

        public IntRange Range => new(522000, 880000);

        public override void PostAdd()
        {
            base.PostAdd();
            if (MiscUtility.GameNotStarted())
            {
                nextTick = Range.TrueMax;
            }
            else
            {
                nextTick = Range.TrueMin;
            }
        }

        public override void TickInterval(int delta)
        {
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            nextTick = Range.RandomInRange;
            DoAction();
        }

        public void DoAction()
        {
            ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 2500);
        }

        public string GetInspectInfo
        {
            get
            {
                return "WVC_XaG_NextXenogermComa_Info".Translate().Resolve() + ": " + nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            }
        }

    }

}
