using RimWorld;
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

		public XenotypeDef xenotypeDef = null;

		public Dictionary<NeedDef, float> savedPawnNeeds;

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

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

		public int AllGenesCount
		{
			get
			{
				return endogenes.Count + xenogenes.Count;
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
			Scribe_Collections.Look(ref savedPawnNeeds, "savedPawnNeeds", LookMode.Def, LookMode.Value);
			if (Scribe.mode == LoadSaveMode.LoadingVars && ((xenogenes != null && xenogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0) || (endogenes != null && endogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0)))
			{
				Log.Error("Removed null gene(s)");
			}
			if (Scribe.mode == LoadSaveMode.LoadingVars && savedPawnNeeds != null)
			{
				foreach (var need in savedPawnNeeds.ToList())
				{
					if (need.Key == null)
					{
						savedPawnNeeds.Remove(need.Key);
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
