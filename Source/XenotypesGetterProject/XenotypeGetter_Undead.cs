namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter_Undead : XenotypeGetter
	{

		public override bool CanFire()
		{
			//if (base.CanFire())
			//{
			//	return false;
			//}
			return GeneResourceUtility.AnyUndeads;
		}

	}

}
