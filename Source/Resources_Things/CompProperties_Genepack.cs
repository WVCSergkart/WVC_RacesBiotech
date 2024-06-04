using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_Genepack : Genepack
	{

		public override void PostMake()
		{
			base.PostMake();
			CompProperties_Genepack xag_genepack = this?.TryGetComp<CompGenepack>()?.Props;
			if (xag_genepack != null)
			{
				ThingDef thingDef = this.def;
				GeneSet newGeneSet = new();
				XaG_CountWithChance geneCount = xag_genepack.genesCountProbabilities.RandomElementByWeight((XaG_CountWithChance x) => x.chance);
				XaG_GeneUtility.SetGenesInPack(geneCount, newGeneSet);
				newGeneSet.SortGenes();
				XaG_GeneUtility.GenerateName(newGeneSet, xag_genepack.genepackNamer);
				if (!newGeneSet.Empty)
				{
					geneSet = newGeneSet;
				}
				else
				{
					Log.Warning(thingDef.LabelCap + " generated with null geneSet. Vanilla geneSet will be used instead.");
				}
				if (xag_genepack.styleDef != null)
				{
					this.SetStyleDef(xag_genepack.styleDef);
				}
				this.def = ThingDefOf.Genepack;
			}
		}

		// public void SetGeneset(GeneSet geneset)
		// {
			// geneSet = geneset;
		// }

	}

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
			// SetTrueParentGenes();
		// }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				SetTrueParentGenes();
			}
		}

		public void SetTrueParentGenes()
		{
			if (parent is not HumanEmbryo embryo)
			{
				return;
			}
			GeneSet geneSet = embryo.GeneSet;
			if (geneSet == null)
			{
				return;
			}
			// Log.Error("0");
			for (int i = 0; i < geneSet.GenesListForReading.Count; i++)
			{
				GeneDef geneDef = geneSet.GenesListForReading[i];
				geneSet.Debug_RemoveGene(geneDef);
			}
			// Log.Error("1");
			HediffComp_TrueParentGenes.AddParentGenes(embryo.Mother, geneSet);
			HediffComp_TrueParentGenes.AddParentGenes(embryo.Father, geneSet);
			// Log.Error("2");
			geneSet.SortGenes();
		}

		// public void OverrideGeneSet(XenotypeDef xenotypeDef = null)
		// {
			// if (parent is not HumanEmbryo embryo)
			// {
				// return;
			// }
			// if (!Props.xenotypeDefs.NullOrEmpty())
			// {
				// if (xenotypeDef == null)
				// {
					// xenotypeDef = Props.xenotypeDefs.RandomElement();
				// }
				// GeneSet geneSet = embryo.GeneSet;
				// for (int i = 0; i < geneSet.GenesListForReading.Count; i++)
				// {
					// GeneDef geneDef = geneSet.GenesListForReading[i];
					// geneSet.Debug_RemoveGene(geneDef);
				// }
				// foreach (GeneDef geneDef in xenotypeDef.genes)
				// {
					// geneSet.AddGene(geneDef);
				// }
				// geneSet.SortGenes();
			// }
		// }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: OverrideGeneSet",
					action = delegate
					{
						SetTrueParentGenes();
					}
				};
				yield return command_Action;
			}
		}

		// public override void PostExposeData()
		// {
			// base.PostExposeData();
			// Scribe_Values.Look(ref newLabel, "label_" + Props.uniqueTag);
		// }

	}

}
