using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Xenotypes : XenotypeGetter
	{

		public override bool CanFire()
		{
			return PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Any(xenos => xenos.genes != null && def.reqXenotypeDefs.Contains(xenos.genes.Xenotype.defName));
		}

	}

}
