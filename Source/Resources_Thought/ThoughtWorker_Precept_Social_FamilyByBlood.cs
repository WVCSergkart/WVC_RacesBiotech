using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ThoughtWorker_Precept_Social_FamilyByBlood : ThoughtWorker_Precept_Social
    {


        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (p.relations == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.relations.FamilyByBlood.Contains(otherPawn))
            {
                if (p.ideo.Ideo?.GetRole(otherPawn)?.def == PreceptDefOf.IdeoRole_Leader)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
                return ThoughtState.ActiveAtStage(0);
            }
            if (otherPawn.IsSlave)
            {
                return ThoughtState.ActiveAtStage(2);
            }
            return ThoughtState.ActiveAtStage(3);
        }

	}

	public abstract class ThoughtWorker_Precept_Family : ThoughtWorker_Precept
    {

        public static List<Family> families = new();

        public class Family
        {

            public List<Pawn> pawns = new();

            public void AddToFamily(Pawn pawn)
            {
                if (!InFamily(pawn))
                {
                    pawns.Add(pawn);
                }
            }

            //public void RemoveFromFamily(Pawn pawn)
            //{
            //    if (!InFamily(pawn))
            //    {
            //        pawns.Remove(pawn);
            //    }
            //}

            public bool InFamily(Pawn pawn)
            {
                return pawns.Contains(pawn);
            }

            public void AddRange(List<Pawn> newPawns)
            {
                foreach (Pawn pawn in newPawns)
                {
                    AddToFamily(pawn);
                }
            }

            public void AddRange(IEnumerable<Pawn> newPawns)
            {
                foreach (Pawn pawn in newPawns)
                {
                    AddToFamily(pawn);
                }
            }

            public bool HasBloodRelationWithAny(Pawn pawn)
            {
                foreach (Pawn item in pawns)
                {
                    if (item.relations.FamilyByBlood.Contains(pawn))
                    {
                        return true;
                    }
                }
                return false;
            }

        }

        //public void RemoveFromAllFamilies(Pawn pawn)
        //{
        //    foreach (Family family in families)
        //    {
        //        family.RemoveFromFamily(pawn);
        //    }
        //}

        public Family GetFamilyOf(Pawn pawn)
        {
            foreach (Family family in families)
            {
                if (family.InFamily(pawn))
                {
                    return family;
                }
                else if (family.HasBloodRelationWithAny(pawn))
                {
                    //RemoveFromAllFamilies(pawn);
                    family.AddToFamily(pawn);
                    return family;
                }
            }
            return null;
        }

        public void AddFamilyOf(Pawn pawn)
        {
            Family family = GetFamilyOf(pawn);
            if (family == null)
            {
                Family newFamily = new();
                newFamily.pawns = new() { pawn };
                newFamily.AddRange(pawn.relations.FamilyByBlood);
                families.Add(newFamily);
            }
            else
            {
                family.AddToFamily(pawn);
            }
        }

        public void UpdPlayerFamilies()
        {
            families = new();
            List<Pawn> playerPawn = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Where((pawn) => pawn.relations != null && pawn.RaceProps.Humanlike && !pawn.IsSlave && !pawn.IsPrisoner && !pawn.IsQuestLodger() && !pawn.Suspended).ToList();
            foreach (Pawn pawn in playerPawn)
            {
                AddFamilyOf(pawn);
            }
            cachedFamiliesCount = families.Count();
        }

        public static int lastRecacheTick = -1;
        public static int cachedFamiliesCount = 0;

        public void UpdCollection()
        {
            if (lastRecacheTick < Find.TickManager.TicksGame)
            {
                UpdPlayerFamilies();
                lastRecacheTick = Find.TickManager.TicksGame + 6000;
            }
        }

    }

	public class ThoughtWorker_Precept_FamilyByBlood : ThoughtWorker_Precept_Family
    {

        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.relations == null)
            {
                return ThoughtState.Inactive;
            }
            UpdCollection();
            if (cachedFamiliesCount > 1)
            {
                return ThoughtState.ActiveDefault;
            }
            return ThoughtState.Inactive;
        }

        public override float MoodMultiplier(Pawn p)
        {
            return cachedFamiliesCount - 1;
        }

    }

}
