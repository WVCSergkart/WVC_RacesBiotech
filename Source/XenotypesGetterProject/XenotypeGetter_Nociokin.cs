using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Nociokin : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (Faction.OfMechanoids?.deactivated != true)
			{
				return false;
			}
			return Find.Anomaly?.LevelDef == MonolithLevelDefOf.Embraced;
		}

	}

}
