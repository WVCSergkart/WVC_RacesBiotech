using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class GeneSetPreset : IExposable
	{

		[MustTranslate]
		public string name;

		public List<GeneDef> geneDefs = new();
		public float selectionWeight = 1f;

		public GeneSetPreset()
		{

		}

		public GeneSetPreset(XenotypeHolder holder)
		{
			name = holder.Label;
			geneDefs = holder.genes;
		}

		public static void SetupGeneSetPresets(IGeneXenogenesEditor geneXenogenesEditor)
		{
			List<GeneSetPreset> newList = new();
			foreach (XenotypeHolder xenotypeHolder in ListsUtility.GetAllXenotypesHolders())
			{
				newList.Add(new(xenotypeHolder));
			}
			geneXenogenesEditor.GeneSetPresets = newList;
		}

		public void ExposeData()
		{
			Scribe_Values.Look(ref name, "name");
			Scribe_Values.Look(ref selectionWeight, "selectionWeight", 1f);
			Scribe_Collections.Look(ref geneDefs, "geneDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && geneDefs != null && geneDefs.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
		}

	}

}
