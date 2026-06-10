using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Shapeshifter : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (ModsUtility.GameNotStarted())
			{
				return false;
			}
			return GeneshiftUtility.AnyShapeshifters;
		}

	}

}
