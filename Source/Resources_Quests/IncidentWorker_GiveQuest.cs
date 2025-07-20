using RimWorld;

namespace WVC_XenotypesAndGenes
{

    public class IncidentWorker_GiveQuest : RimWorld.IncidentWorker_GiveQuest
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedColonistsCount > 5)
			{
				return false;
			}
			return base.CanFireNowSub(parms);
		}
	}

}
