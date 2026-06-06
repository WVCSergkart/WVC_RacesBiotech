using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Dryads : XenotypeGetter
	{

		public override bool CanFire()
		{
			return Find.Maps.Any(map => map.mapPawns.AllPawnsSpawned.Any(p => p.RaceProps.Dryad));
		}

	}

}
