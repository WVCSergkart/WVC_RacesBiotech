namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Fantasy : XenotypeGetter_Metalkin
	{

		public override bool CanFire()
		{
			if (base.CanFire())
			{
				return false;
			}
			return GeneResourceUtility.AnyUndeads && !GeneshiftUtility.AnyShapeshifters;
		}

	}

}
