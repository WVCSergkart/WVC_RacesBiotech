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
		public static bool haveAssignedWork = false;
		//public static bool leaderIsUndead = false;
		//public static bool leaderIsShapeshifter = false;
		//public static bool presentUndead = false;
		//public static bool presentShapeshifter = false;

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
