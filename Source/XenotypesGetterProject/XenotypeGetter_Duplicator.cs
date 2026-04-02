using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Duplicator : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (!ModsConfig.AnomalyActive)
			{
				return false;
			}
			if (Find.Anomaly.LevelDef == MonolithLevelDefOf.Inactive || Find.Anomaly.LevelDef == MonolithLevelDefOf.Disrupted)
			{
				if (Find.Maps?.Any(map => map?.listerThings.AllThings?.Any(thing => thing.TryGetComp<CompObelisk_Duplicator>() != null) == true) == true)
				{
					return true;
				}
				return false;
			}
			return true;
		}

	}

}
