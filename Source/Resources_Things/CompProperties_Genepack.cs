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

		public List<XenotypeDef> xenotypeDefs;

		public List<XaG_CountWithChance> genepacks;

		public string uniqueTag = "XaG_GeneSetter";

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

	public class CompHumanEmbryo : ThingComp
	{

		public CompProperties_Genepack Props => (CompProperties_Genepack)props;

		// public override void PostPostMake()
		// {
			// OverrideGeneSet(Props.xenotypeDefs.RandomElement());
		// }

		// public bool overrided = false;

		// public override string TransformLabel(string label)
		// {
			// if (overrided)
			// {
				// return label;
			// }
			// if (pawnSources.Count == 2)
			// {
				// return "ThingOfTwoSources".Translate(label, pawnSources[0].LabelShortCap, pawnSources[1].LabelShortCap);
			// }
			// return "ThingOfSource".Translate(label, pawnSources[0].LabelShortCap);
		// }

		public void OverrideGeneSet(XenotypeDef xenotypeDef = null)
		{
			if (parent is not HumanEmbryo embryo)
			{
				return;
			}
			if (!Props.xenotypeDefs.NullOrEmpty())
			{
				if (xenotypeDef == null)
				{
					xenotypeDef = Props.xenotypeDefs.RandomElement();
				}
				GeneSet geneSet = embryo.GeneSet;
				for (int i = 0; i < geneSet.GenesListForReading.Count; i++)
				{
					GeneDef geneDef = geneSet.GenesListForReading[i];
					geneSet.Debug_RemoveGene(geneDef);
				}
				foreach (GeneDef geneDef in xenotypeDef.genes)
				{
					geneSet.AddGene(geneDef);
				}
				geneSet.SortGenes();
				geneSet.SetNameDirect(parent.def.label + " " + xenotypeDef.label);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: OverrideGeneSet",
					action = delegate
					{
						OverrideGeneSet();
					}
				};
				yield return command_Action;
			}
		}

		// public override void PostExposeData()
		// {
			// base.PostExposeData();
			// Scribe_Values.Look(ref overrided, "overrided_" + Props.uniqueTag, false);
		// }

	}

}
