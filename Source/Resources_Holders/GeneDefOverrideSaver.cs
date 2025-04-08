using Verse;

namespace WVC_XenotypesAndGenes
{
    public class GeneDefOverrideSaver : IExposable
	{

		public bool overriderIsXenogene = false;
		public bool overridedIsXenogene = false;

		public GeneDef overrider = null;
		public GeneDef overrided = null;

		public bool IsNull => overrided == null || overrider == null;

		public void ExposeData()
		{
			Scribe_Values.Look(ref overriderIsXenogene, "overriderIsXenogene");
			Scribe_Values.Look(ref overridedIsXenogene, "overridedIsXenogene");
			Scribe_Defs.Look(ref overrider, "overrider");
			Scribe_Defs.Look(ref overrided, "overrided");
		}

	}

}
