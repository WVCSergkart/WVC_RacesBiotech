using RimWorld;
using Verse;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Undead : Gene
    {
        private readonly int penaltyYears = 5;

        private readonly float oneYear = 3600000f;

        private int Penalty => (int)(oneYear * penaltyYears);
        private long Limit => (long)(oneYear * MinAge);
        private float CurrentAge => pawn.ageTracker.AgeBiologicalTicks;
        private float MinAge => pawn.ageTracker.AdultMinAge;

        public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.childBackstoryDef;
        public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.adultBackstoryDef;

        // public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
        // public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

        public Gene_ResurgentCells Gene_ResurgentCells => pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
        public Gene_Dust Gene_Dust => pawn.genes?.GetFirstGeneOfType<Gene_Dust>();

        // public Gene_DustReincarnation gene_DustReincarnation;
        public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
        public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

        // public bool PawnCanResurrect => CanResurrect();
        // public bool PawnCanReincarnate => DustogenicCanReincarnate();

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (!CanResurrect())
            {
                return;
            }
            bool canResurrectBasic = true;
            if (DustogenicCanReincarnate())
            {
                Gene_DustReincarnation.Reincarnate(pawn, SummonQuest);
                canResurrectBasic = false;
            }
            if (EnoughResurgentCells())
            {
                Gene_ResurgentCells.Value -= def.resourceLossPerDay;
                UndeadUtility.Resurrect(pawn);
                canResurrectBasic = false;
            }
            if (CorrectAge() && canResurrectBasic)
            {
                UndeadUtility.ResurrectWithPenalties(pawn, Limit, Penalty, ChildBackstoryDef, AdultBackstoryDef, penaltyYears);
            }
        }

        public bool PawnCanResurrect()
        {
            if ((CanResurrect() && !DustogenicCanReincarnate()) || EnoughResurgentCells())
            {
                return true;
            }
            return false;
        }

        //For checks
        private bool CanResurrect()
        {
            if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless) || (!pawn.IsColonist && !WVC_Biotech.settings.canNonPlayerPawnResurrect))
            {
                return false;
            }
            if (PawnHasAnyResourceGene())
            {
                return true;
            }
            else if (CorrectAge())
            {
                return true;
            }
            return false;
        }

        private bool PawnHasAnyResourceGene()
        {
            if (EnoughResurgentCells() || DustogenicCanReincarnate())
            {
                return true;
            }
            return false;
        }

        private bool DustogenicCanReincarnate()
        {
            if (Gene_Dust != null && pawn.ageTracker.AgeChronologicalYears > MinChronoAge)
            {
                return true;
            }
            return false;
        }

        private bool EnoughResurgentCells()
        {
            // Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
            if (Gene_ResurgentCells != null)
            {
                if ((Gene_ResurgentCells.Value - def.resourceLossPerDay) >= 0f)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CorrectAge()
        {
            // int penalty = (int)(oneYear * penaltyYears);
            // long limit = (long)(oneYear * minAge);
            // float currentAge = pawn.ageTracker.AgeBiologicalTicks;
            if ((CurrentAge - Penalty) > Limit)
            {
                return true;
            }
            return false;
        }

    }

}
