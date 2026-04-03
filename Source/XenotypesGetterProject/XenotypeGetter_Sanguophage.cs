using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Sanguophage : XenotypeGetter
	{

		public override bool CanFire()
		{
			return PawnsFinder.AllMaps?.Any(pawn => pawn?.genes?.Xenotype == XenotypeDefOf.Sanguophage) == true;
		}

	}

	public class XenotypeGetter_Sandycat : XenotypeGetter_Sanguophage
	{

		public override bool CanFire()
		{
			if (Find.CurrentMap?.mapTemperature?.OutdoorTemp < 20)
			{
				return false;
			}
			return base.CanFire();
		}

	}

}
