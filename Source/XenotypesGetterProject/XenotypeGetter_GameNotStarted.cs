namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_GameNotStarted : XenotypeGetter
	{

		public override bool CanFire()
		{
			return MiscUtility.GameNotStarted();
		}

	}

}
