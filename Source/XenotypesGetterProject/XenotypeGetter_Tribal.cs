namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Tribal : XenotypeGetter_Metalkin
	{

		public override bool CanFire()
		{
			return !base.CanFire();
		}

		//public override float Chance()
		//{
		//	return def.chance;
		//}

	}

}
