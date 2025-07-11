using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class GeneSetPresets : IExposable
	{

		[MustTranslate]
		public string name;

		public List<GeneDef> geneDefs = new();

		public void ExposeData()
		{
			Scribe_Values.Look(ref name, "name");
			Scribe_Collections.Look(ref geneDefs, "geneDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && geneDefs != null && geneDefs.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
		}

	}

}
