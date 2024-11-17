using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class StaticCollectionsClass
	{

		public static int cachedPawnsCount = 0;
		public static int cachedXenotypesCount = 0;
		public static int cachedNonHumansCount = 0;

		//public static bool shapeshifterAppear = false;

	}

	public class GeneSetPresets : IExposable
	{

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

	public class PawnGeneSetHolder : IExposable
	{

		public int formId;

		public string name = null;

		public XenotypeIconDef iconDef = null;

		public List<Gene> endogenes = new();
		public List<Gene> xenogenes = new();

		public List<GeneDef> endogeneDefs = new();
		public List<GeneDef> xenogeneDefs = new();

		public XenotypeDef xenotypeDef = null;

		public Dictionary<NeedDef, float> savedPawnNeeds;
		public Dictionary<GeneDef, float> savedPawnResources;

		public bool SaveAndLoadGenes => WVC_Biotech.settings.enable_MorpherExperimentalMode;

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

		public void SaveGenes(Pawn pawn, Gene_Morpher morpher)
		{
			if (SaveAndLoadGenes)
			{
				xenogenes = new();
				endogenes = new();
				foreach (Gene gene in pawn.genes.Endogenes.ToList())
				{
					if (gene != morpher)
					{
						//gene.PostRemove();
						endogenes.Add(gene);
					}
				}
				foreach (Gene gene in pawn.genes.Endogenes.ToList())
				{
					morpher.RemoveGene(gene);
				}
				foreach (Gene gene in pawn.genes.Xenogenes.ToList())
				{
					if (gene != morpher)
					{
						//gene.PostRemove();
						xenogenes.Add(gene);
					}
				}
				foreach (Gene gene in pawn.genes.Xenogenes.ToList())
				{
					morpher.RemoveGene(gene);
				}
				foreach (Gene gene in endogenes)
				{
					gene.overriddenByGene = morpher;
				}
				foreach (Gene gene in xenogenes)
				{
					gene.overriddenByGene = morpher;
				}
			}
			else
			{
				xenogeneDefs = new();
				endogeneDefs = new();
				foreach (Gene gene in pawn.genes.Endogenes.ToList())
				{
					if (gene != morpher)
					{
						endogeneDefs.Add(gene.def);
					}
				}
				foreach (Gene gene in pawn.genes.Endogenes.ToList())
				{
					morpher.RemoveGene(gene);
				}
				foreach (Gene gene in pawn.genes.Xenogenes.ToList())
				{
					if (gene != morpher)
					{
						xenogeneDefs.Add(gene.def);
					}
				}
				foreach (Gene gene in pawn.genes.Xenogenes.ToList())
				{
					morpher.RemoveGene(gene);
				}
			}
		}

		public void LoadGenes(Pawn pawn, Gene_Morpher morpher)
		{
			if (SaveAndLoadGenes)
			{
				try
				{
					Dictionary<Gene, int> savedEndogenesIDs = new();
					Dictionary<Gene, Gene> savedEndogenesOverrides = new();
					foreach (Gene gene in endogenes)
					{
						morpher.AddGene(gene.def, true);
						Gene sourceGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == gene.def);
						savedEndogenesIDs[sourceGene] = sourceGene.loadID;
						savedEndogenesOverrides[sourceGene] = sourceGene.overriddenByGene;
						morpher.CopyGeneID(gene, sourceGene, pawn.genes.Endogenes);
					}
					Dictionary<Gene, int> savedXenogenesIDs = new();
					Dictionary<Gene, Gene> savedXenogenesOverrides = new();
					foreach (Gene gene in xenogenes)
					{
						morpher.AddGene(gene.def, false);
						Gene sourceGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == gene.def);
						savedXenogenesIDs[sourceGene] = sourceGene.loadID;
						savedXenogenesOverrides[sourceGene] = sourceGene.overriddenByGene;
						morpher.CopyGeneID(gene, sourceGene, pawn.genes.Xenogenes);
					}
					foreach (var item in savedEndogenesIDs)
					{
						Gene targetGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.loadID = item.Value;
					}
					foreach (var item in savedXenogenesIDs)
					{
						Gene targetGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.loadID = item.Value;
					}
					foreach (var item in savedEndogenesOverrides)
					{
						Gene targetGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.overriddenByGene = item.Value;
					}
					foreach (var item in savedXenogenesOverrides)
					{
						Gene targetGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.overriddenByGene = item.Value;
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed copy genes. Reason: " + arg);
				}
			}
			else
			{
				foreach (GeneDef gene in endogeneDefs)
				{
					morpher.AddGene(gene, true);
				}
				foreach (GeneDef gene in xenogeneDefs)
				{
					morpher.AddGene(gene, false);
				}
			}
		}

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					if (name.NullOrEmpty())
					{
						cachedLabel = xenotypeDef.label;
					}
					else
					{
						cachedLabel = name.CapitalizeFirst();
					}
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		private int? cachedGenesCount;

		public int AllGenesCount
		{
			get
			{
				if (!cachedGenesCount.HasValue)
                {
					if (SaveAndLoadGenes)
					{
						cachedGenesCount = endogenes.Count + xenogenes.Count;
					}
					else
					{
						cachedGenesCount = endogeneDefs.Count + xenogeneDefs.Count;
					}
				}
				return cachedGenesCount.Value;
			}
		}

        public void ExposeData()
		{
			Scribe_Values.Look(ref formId, "formId");
			Scribe_Values.Look(ref name, "name");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Collections.Look(ref endogenes, "endogenes", LookMode.Deep);
			Scribe_Collections.Look(ref xenogenes, "xenogenes", LookMode.Deep);
			Scribe_Collections.Look(ref endogeneDefs, "endogeneDefs", LookMode.Def);
			Scribe_Collections.Look(ref xenogeneDefs, "xenogeneDefs", LookMode.Def);
			Scribe_Collections.Look(ref savedPawnNeeds, "savedPawnNeeds", LookMode.Def, LookMode.Value);
			Scribe_Collections.Look(ref savedPawnResources, "savedPawnResources", LookMode.Def, LookMode.Value);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if ((xenogenes != null && xenogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0) || (endogenes != null && endogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0))
				{
					Log.Warning("Removed null gene(s)");
				}
				if ((endogeneDefs != null && endogeneDefs.RemoveAll((GeneDef x) => x == null) > 0) || (xenogeneDefs != null && xenogeneDefs.RemoveAll((GeneDef x) => x == null) > 0))
				{
					Log.Warning("Removed null geneDef(s)");
				}
				if (savedPawnNeeds != null)
				{
					foreach (var need in savedPawnNeeds.ToList())
					{
						if (need.Key == null)
						{
							savedPawnNeeds.Remove(need.Key);
						}
					}
				}
				if (savedPawnResources != null)
				{
					foreach (var geneDef in savedPawnResources.ToList())
					{
						if (geneDef.Key == null)
						{
							savedPawnResources.Remove(geneDef.Key);
						}
					}
				}
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (xenotypeDef == null)
				{
					xenotypeDef = XenotypeDefOf.Baseliner;
				}
				if (xenogenes == null)
				{
					xenogenes = new();
				}
				if (endogenes == null)
				{
					endogenes = new();
				}
				if (xenogeneDefs == null)
				{
					xenogeneDefs = new();
				}
				if (endogeneDefs == null)
				{
					endogeneDefs = new();
				}
			}
		}

	}

	public class SaveableXenotypeHolder : XenotypeHolder, IExposable
	{

		public void ExposeData()
		{
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Values.Look(ref name, "name");
			Scribe_Values.Look(ref inheritable, "inheritable");
			Scribe_Collections.Look(ref genes, "genes", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && genes != null && genes.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (xenotypeDef == null)
				{
					xenotypeDef = XenotypeDefOf.Baseliner;
				}
				if (genes == null)
				{
					genes = new();
				}
				if (xenotypeDef != XenotypeDefOf.Baseliner)
				{
					genes = xenotypeDef.genes;
					name = null;
					iconDef = null;
					inheritable = xenotypeDef.inheritable;
				}
			}
		}

		public SaveableXenotypeHolder()
		{

		}

		public SaveableXenotypeHolder(XenotypeHolder holder)
		{
			xenotypeDef = holder.xenotypeDef;
			name = holder.name;
			iconDef = holder.iconDef;
			genes = holder.genes;
			inheritable = holder.inheritable;
		}

	}

}
