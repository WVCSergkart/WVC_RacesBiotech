using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// public class XaG_Genepack : Genepack
	// {

		// public void SetGeneset(GeneSet geneset)
		// {
			// geneSet = geneset;
		// }

	// }

	public class CompProperties_Genepack : CompProperties
	{

		public List<GeneDef> geneDefs;

		public List<XaG_CountWithChance> genepacks;

		public string uniqueTag = "Backup";

		public List<XaG_CountWithChance> genesCountProbabilities;
		public RulePackDef genepackNamer;
		public ThingStyleDef styleDef;

		public CompProperties_Genepack()
		{
			compClass = typeof(CompGenepack);
		}

	}

	public class CompGenepack : ThingComp
	{

		public CompProperties_Genepack Props => (CompProperties_Genepack)props;

		// public List<GeneDef> genes = new();

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			// if (parent is Genepack genepack && !respawningAfterLoad)
			// {
				// GeneSet newGeneSet = new();
				// if (!Props.genesCountProbabilities.NullOrEmpty())
				// {
					// XaG_CountWithChance geneCount = Props.genesCountProbabilities.RandomElementByWeight((XaG_CountWithChance x) => x.chance);
					// MiscUtility.SetGenesInPack(geneCount, newGeneSet);
					// newGeneSet.SortGenes();
					// MiscUtility.GenerateName(newGeneSet, Props.genepackNamer);
					// if (!newGeneSet.Empty)
					// {
						// genepack.Initialize(newGeneSet.GenesListForReading);
						// genepack.GeneSet.SetNameDirect(newGeneSet.Label);
					// }
					// else
					// {
						// Log.Warning(genepack.LabelCap + " generated with null geneSet. Vanilla geneSet will be used instead.");
					// }
				// }
			// }
			if (Props.styleDef != null)
			{
				parent.SetStyleDef(Props.styleDef);
			}
			if (parent.Map != null)
			{
				parent.Map.listerThings.Remove(parent);
				RegionListersUpdater.DeregisterInRegions(parent, parent.Map);
			}
			parent.def = ThingDefOf.Genepack;
			if (parent.Map != null)
			{
				parent.Map.listerThings.Add(parent);
				RegionListersUpdater.RegisterInRegions(parent, parent.Map);
			}
		}

	}

	// public class CompReplaceWithGenepack : ThingComp
	// {

		// public CompProperties_Genepack Props => (CompProperties_Genepack)props;

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
			// parent.def = ThingDefOf.Genepack;
		// }

	// }

}
