using Verse;
using WVC;
using WVC_XenotypesAndGenes;


namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_GolemGestator : HediffCompProperties
	{
		public int gestationIntervalDays = 1;

		// public string customString = "";

		// public bool produceEggs = false;

		// public string eggDef = "";

		// public bool isGreenGoo = false;

		// public int GreenGooLimit = 0;

		// public string GreenGooTarget = "";

		public string completeMessage = "WVC_RB_Gene_MechaGestator";

		// public string asexualCloningMessage = "VEF_AsexualCloning";

		// public string asexualEggMessage = "VEF_AsexualHatchedEgg";

		// public bool convertsIntoAnotherDef = false;

		// public string newDef = "";

		public bool endogeneTransfer = false;

		public GeneDef geneDef;

		public HediffCompProperties_GolemGestator()
		{
			compClass = typeof(HediffComp_GolemGestator);
		}
	}

}
