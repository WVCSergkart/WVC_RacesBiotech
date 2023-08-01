using System.Collections.Generic;
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
            // if (!CanResurrect())
            // {
                // return;
            // }
            // bool canResurrectBasic = true;
			if (DustogenicCanReincarnate())
			{
				Gene_DustReincarnation.Reincarnate(pawn, SummonQuest);
				// canResurrectBasic = false;
			}
            if (PawnCanResurrect())
            {
				if (EnoughResurgentCells())
				{
					Gene_ResurgentCells.Value -= def.resourceLossPerDay;
					UndeadUtility.Resurrect(pawn);
					// canResurrectBasic = false;
				}
				else if (CorrectAge())
				{
					UndeadUtility.ResurrectWithPenalties(pawn, Limit, Penalty, ChildBackstoryDef, AdultBackstoryDef, penaltyYears);
				}
            }
        }

        // public bool PawnCanResurrect()
        // {
            // if ((CanResurrect() && !DustogenicCanReincarnate()) || EnoughResurgentCells())
            // {
                // return true;
            // }
            // return false;
        // }

        //For checks
        private bool GeneIsActive()
        {
            if (!Active || Overridden || (!pawn.IsColonist && !WVC_Biotech.settings.canNonPlayerPawnResurrect) || pawn.genes.HasGene(GeneDefOf.Deathless))
            {
                return false;
            }
            return true;
        }

        public bool PawnCanResurrect()
        {
            if (GeneIsActive())
            {
				if (EnoughResurgentCells())
				{
					return true;
				}
				else if (CorrectAge())
				{
					return true;
				}
            }
            return false;
        }

        private bool DustogenicCanReincarnate()
        {
            if (Gene_Dust != null)
            {
                return Gene_DustReincarnation.CanReincarnate(pawn, this, MinChronoAge);
            }
            return false;
        }

        private bool EnoughResurgentCells()
        {
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
			if (Gene_ResurgentCells != null || Gene_Dust != null)
			{
				return false;
			}
			if ((CurrentAge - Penalty) > Limit)
			{
				return true;
			}
			return false;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanResurrect".Translate().CapitalizeFirst(), PawnCanResurrect().ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanResurrect_Desc".Translate(), 1100);
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), DustogenicCanReincarnate().ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(), 1090);
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanShapeshift".Translate().CapitalizeFirst(), SubXenotypeUtility.TestXenotype(pawn).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanShapeshift_Desc".Translate(), 150);
		}

    }

}
