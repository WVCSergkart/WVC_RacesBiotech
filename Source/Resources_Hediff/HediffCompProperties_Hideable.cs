using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_Hideable : HediffCompProperties
	{

		public HediffCompProperties_Hideable()
		{
			compClass = typeof(HediffComp_Hideable);
		}
	}

	public class HediffComp_Hideable : HediffComp
	{

		public override bool CompDisallowVisible()
		{
			return WVC_Biotech.settings.hideGeneHediffs;
		}

	}

}
