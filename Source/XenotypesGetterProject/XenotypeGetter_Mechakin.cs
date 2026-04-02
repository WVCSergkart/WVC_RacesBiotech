using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Mechakin : XenotypeGetter_Metalkin
	{

		public override bool CanFire()
		{
			if (Find.CurrentMap?.mapTemperature?.OutdoorTemp < 0)
			{
				return base.CanFire();
			}
			return false;
		}

	}

}
