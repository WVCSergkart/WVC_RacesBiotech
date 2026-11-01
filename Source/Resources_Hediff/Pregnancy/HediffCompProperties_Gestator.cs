using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_Gestator : HediffCompProperties
	{

		public int gestationIntervalDays = -1;
		public XenotypeDef xenotypeDef;

		public string completeLetterLabel = "WVC_XaG_XenoTreeBirthLabel";

		public string completeLetterDesc = "WVC_XaG_XenoTreeBirthDesc";

		public bool endogeneTransfer = true;
		public bool xenogeneTransfer = true;

		public bool xenogerminationComa = false;

		public bool shouldBeAdult = true;

		public HediffDef hediffDef;

		public string uniqueTag = "HediffGestator";

		public string completeMessage = "WVC_RB_Gene_MechaGestator";

		public GeneDef geneDef;

		// public HediffCompProperties_Gestator()
		// {
		// compClass = typeof(HediffComp_Gestator);
		// }

	}

}
