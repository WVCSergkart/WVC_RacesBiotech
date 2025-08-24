using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Mechlink : Gene
	{

		//public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

        private GeneExtension_Spawner cachedGeneExtension_Spawner;
        public GeneExtension_Spawner Spawner
        {
            get
            {
                if (cachedGeneExtension_Spawner == null)
                {
                    cachedGeneExtension_Spawner = def.GetModExtension<GeneExtension_Spawner>();
                }
                return cachedGeneExtension_Spawner;
            }
        }

        public int timeForNextSummon = -1;
		public bool summonMechanoids = false;

		//public bool pawnHadMechlinkBefore = false;

		public override void PostAdd()
        {
            base.PostAdd();
            if (!WVC_Biotech.settings.link_addedMechlinkWithGene)
            {
                return;
            }
			GeneResourceUtility.TryAddMechlink(pawn);
            //if (Active)
            //{
            //    StaticCollectionsClass.AddMechanitor(pawn);
            //}
            //else
            //{
            //	pawnHadMechlinkBefore = true;
            //}
        }

        public override void TickInterval(int delta)
		{
			//base.Tick();
			GeneResourceUtility.TryAddMechlinkRandomly(pawn, delta, WVC_Biotech.settings.mechlink_HediffFromGeneChance);
		}

		public bool CanDoOrbitalSummon()
        {
            if (CanDoOrbitalSummon(pawn))
            {
                return true;
            }
            //summonMechanoids = false;
            return false;
        }

        public static bool CanDoOrbitalSummon(Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer)
            {
                return false;
            }
            if (pawn.Map == null || pawn.Map.IsUnderground())
            {
                return false;
            }
            if (!MechanitorUtility.IsMechanitor(pawn))
            {
                return false;
            }
            return true;
        }

        //public virtual void Notify_OverriddenBy(Gene overriddenBy)
        //{
        //    StaticCollectionsClass.RemoveMechanitor(pawn);
        //}

        //public virtual void Notify_Override()
        //{
        //    StaticCollectionsClass.AddMechanitor(pawn);
        //}

        //public override void PostRemove()
        //{
        //    base.PostRemove();
        //    StaticCollectionsClass.RemoveMechanitor(pawn);
        //}

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
            //if (pawn != null && PawnGenerator.IsBeingGenerated(pawn) is false && Active)
            //{
            //    StaticCollectionsClass.AddMechanitor(pawn);
            //}
            //Scribe_Values.Look(ref pawnHadMechlinkBefore, "pawnHadMechlinkBefore", false);
        }

	}

}
