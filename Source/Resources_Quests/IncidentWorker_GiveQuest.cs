using RimWorld;

namespace WVC_XenotypesAndGenes
{

    public class IncidentWorker_GiveQuest_UniqueXenotype : RimWorld.IncidentWorker_GiveQuest
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return QuestNode_Root_WandererJoin_WalkIn.GetXenotype() != null;
        }

        public override float ChanceFactorNow(IIncidentTarget target)
        {
            return 1f - (StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.15f);
        }

    }

    //public class IncidentWorker_MechakinQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedPlayerPawnsCount <= 3)
    //           {
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_ChimerkinQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedPlayerPawnsCount >= StaticCollectionsClass.cachedNonDeathrestingColonistsCount / 2)
    //           {
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_DuplicatorQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedPlayerPawnsCount <= 1)
    //           {
    //               if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 3)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_BloodeaterQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount >= 2 || StaticCollectionsClass.cachedNonDeathrestingColonistsCount <= 6)
    //           {
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_UndeadQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1)
    //           {
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_BlankindQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (!StaticCollectionsClass.haveAssignedWork)
    //           {
    //               if (Faction.OfPlayer.def.techLevel != TechLevel.Spacer)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_GolemkindQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedColonyMechsCount <= 0)
    //           {
    //               if (Faction.OfPlayer.def.techLevel != TechLevel.Neolithic && Faction.OfPlayer.def.techLevel != TechLevel.Medieval && Faction.OfPlayer.def.techLevel != TechLevel.Animal)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_LilithQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedPlayerPawnsCount == StaticCollectionsClass.cachedNonDeathrestingColonistsCount)
    //           {
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_FeatherdustQuest : IncidentWorker_GiveQuest
    //{
    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (StaticCollectionsClass.cachedPlayerPawnsCount <= 2)
    //           {
    //               if (StaticCollectionsClass.cachedColonyMechsCount > 0)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }
    //   }

    //public class IncidentWorker_InitiateHivemindQuest : IncidentWorker_GiveQuest
    //{

    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (HivemindUtility.HivemindPawns.Count <= 0)
    //           {
    //               if (StaticCollectionsClass.cachedPlayerPawnsCount > 7)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }

    //   }

    //public class IncidentWorker_ExpandHivemindQuest : IncidentWorker_GiveQuest
    //{

    //	protected override bool CanFireNowSub(IncidentParms parms)
    //       {
    //           if (HivemindUtility.HivemindPawns.Count >= 1)
    //           {
    //               if (StaticCollectionsClass.cachedPlayerPawnsCount > 4)
    //               {
    //                   return false;
    //               }
    //               return base.CanFireNowSub(parms);
    //           }
    //           return false;
    //       }

    //   }

}
