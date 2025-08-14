using RimWorld;

namespace WVC_XenotypesAndGenes
{

    public class IncidentWorker_GiveQuest : RimWorld.IncidentWorker_GiveQuest
	{

		public override float ChanceFactorNow(IIncidentTarget target)
		{
			return 1f - (StaticCollectionsClass.cachedColonistsCount * 0.15f);
		}

		//protected override bool CanFireNowSub(IncidentParms parms)
		//{
		//	if (StaticCollectionsClass.cachedColonistsCount > 7)
		//	{
		//		return false;
		//	}
		//	return base.CanFireNowSub(parms);
		//}

	}

	public class IncidentWorker_MechakinQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedXenotypesCount > 3)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_ChimerkinQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedXenotypesCount < StaticCollectionsClass.cachedColonistsCount / 2)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_DuplicatorQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedXenotypesCount > 1)
			{
				return false;
			}
			if (StaticCollectionsClass.cachedColonistsCount > 3)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_BloodeaterQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedColonistsCount < 2 && StaticCollectionsClass.cachedColonistsCount > 6)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_UndeadQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedColonistsCount <= 1)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_BlankindQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.haveAssignedWork)
			{
				return false;
			}
			if (Faction.OfPlayer.def.techLevel != TechLevel.Spacer)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_GolemkindQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedColonyMechsCount > 0)
			{
				return false;
			}
			if (Faction.OfPlayer.def.techLevel != TechLevel.Neolithic && Faction.OfPlayer.def.techLevel != TechLevel.Medieval && Faction.OfPlayer.def.techLevel != TechLevel.Animal)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_LilithQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedXenotypesCount != StaticCollectionsClass.cachedColonistsCount)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

	public class IncidentWorker_FeatherdustQuest : IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedXenotypesCount > 1)
			{
				return false;
			}
			if (StaticCollectionsClass.cachedColonyMechsCount > 0)
			{
				return false;
			}
			if (StaticCollectionsClass.cachedColonistsCount > 2)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

}
