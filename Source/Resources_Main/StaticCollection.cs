using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class StaticCollectionsClass
	{

		public static int cachedPawnsCount = 0;
		public static int cachedXenotypesCount = 0;
		public static int cachedNonHumansCount = 0;

	}

	public class GeneSetPresets : IExposable
	{

		public string name;

		public List<GeneDef> geneDefs = new();

		public void ExposeData()
		{
			Scribe_Values.Look(ref name, "name");
			Scribe_Collections.Look(ref geneDefs, "geneDefs", LookMode.Def);
		}

	}

	public class PawnGeneSetHolder : IExposable
	{

		public int formId;

		public string name;

		public XenotypeIconDef iconDef;

		public List<Gene> endogenes = new();
		public List<Gene> xenogenes = new();

		public XenotypeDef xenotypeDef;

		public void ExposeData()
		{
			Scribe_Values.Look(ref formId, "formId");
			Scribe_Values.Look(ref name, "name");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Collections.Look(ref endogenes, "endogenes", LookMode.Deep);
			Scribe_Collections.Look(ref xenogenes, "xenogenes", LookMode.Deep);
		}

	}

}
