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

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

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
            summonMechanoids = false;
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

        //public void Notify_OverriddenBy(Gene overriddenBy)
        //{
        //	if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
        //	{
        //		HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
        //	}
        //}

        //public void Notify_Override()
        //{
        //	if (WVC_Biotech.settings.link_removeMechlinkWithGene && WVC_Biotech.settings.link_addedMechlinkWithGene)
        //	{
        //		if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
        //		{
        //			pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
        //		}
        //	}
        //}

        //public override void PostRemove()
        //{
        //	base.PostRemove();
        //	if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
        //	{
        //		HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
        //	}
        //}

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
			//Scribe_Values.Look(ref pawnHadMechlinkBefore, "pawnHadMechlinkBefore", false);
		}

	}

}
