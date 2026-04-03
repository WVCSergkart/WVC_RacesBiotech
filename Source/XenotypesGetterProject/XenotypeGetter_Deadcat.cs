using RimWorld;
using System.Linq;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Deadcat : XenotypeGetter
	{

		public override bool CanFire()
		{
			return PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive?.Where(pawn => pawn.def == MainDefOf.Cat)?.Count() > 14;
		}

	}

}
