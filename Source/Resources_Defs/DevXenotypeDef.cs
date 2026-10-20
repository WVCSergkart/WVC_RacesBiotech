using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class DevXenotypeDef : XenotypeDef_XaG
	{

		//public bool isThrall = false;

		public bool isHybrid = false;

		public bool isRandom = false;

		//public bool implemented = true;

		//public GeneDef devGeneDef;

		public string xenotypeDesc = "WVC_XaG_DevXenotypeDef_XenotypeDesc";

		//public XenotypeIconDef iconDef;

		//public List<ThrallDef> thrallDefs;

		public List<string> xenotypeDefs;

		//public List<GeneDef> guaranteedGenes;

		public List<GeneDef> exceptedGenes;

		public override void ResolveReferences()
		{
			if (isRandom)
			{
				base.ResolveReferences();
				return;
			}
			if (genes == null)
			{
				genes = new();
			}
			//GeneDef geneticShifter = devGeneDef;
			//if (genes.Contains(geneticShifter))
			//{
			//	genes.Remove(geneticShifter);
			//}
			Setup();
			//if (!isHybrid || isRandom)
			//{
			//	genes.Add(geneticShifter);
			//}
			base.ResolveReferences();
		}

		private void Setup()
		{
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (isHybrid)
			{
				SetupHybrid();
			}
			//else if (isThrall)
			//{
			//    SetupThrall();
			//}
		}

		//private void SetupThrall()
		//{
		//    if (guaranteedGenes.NullOrEmpty())
		//    {
		//        guaranteedGenes = genes;
		//    }
		//    foreach (GeneDef gene in guaranteedGenes)
		//    {
		//        descriptionHyperlinks.Add(new DefHyperlink(gene));
		//    }
		//    foreach (ThrallDef thrallDef in DefDatabase<ThrallDef>.AllDefsListForReading)
		//    {
		//        descriptionHyperlinks.Add(new DefHyperlink(thrallDef));
		//    }
		//    inheritable = true;
		//    doubleXenotypeChances = null;
		//}

		private List<XenotypeDef> cachedXenotypeDefs;
		public List<XenotypeDef> XenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					List<XenotypeDef> list = new();
					foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
					{
						if (xenotypeDefs.Contains(xenotypeDef.defName) && !list.Contains(xenotypeDef))
						{
							list.Add(xenotypeDef);
						}
					}
					cachedXenotypeDefs = list;
				}
				return cachedXenotypeDefs;
			}
		}

		private void SetupHybrid()
		{
			//List<XenotypeDef> devXenotypeDefs = ListsUtility.GetDevXenotypeDefs();
			//foreach (XenotypeDef item in devXenotypeDefs)
			//{
			//	if (xenotypeDefs.Contains(item))
			//	{
			//		xenotypeDefs.Remove(item);
			//	}
			//}
			if (exceptedGenes == null)
			{
				exceptedGenes = new();
			}
			if (xenotypeDefs == null)
			{
				Log.Warning(defName + " has no xenotypes in xenotypeDefs. Min xenotypeDefs is 2.");
				xenotypeDefs = new();
			}
			foreach (XenotypeDef item in XenotypeDefs)
			{
				descriptionHyperlinks.Add(new DefHyperlink(item));
			}
			if (XenotypeDefs.Count < 2)
			{
				Log.Error(defName + " has less xenotypes than this type supports. Min xenotypeDefs is 2.");
				return;
			}
			XenotypeDef firstXenotypeDef = XenotypeDefs.First();
			XenotypeDef secondXenotypeDef = XenotypeDefs.Last();
			//if (!SubXenotypeUtility.TryGetHybridGenes(null, firstXenotypeDef.genes, secondXenotypeDef.genes, out List<GeneDef> allNewGenes, exceptedGenes, new(), ModsUtility.DevMode))
			//{
			//	description = "Failed create hybrid from: " + firstXenotypeDef.defName + " and " + secondXenotypeDef.defName + ". Because their total metabolism is not compatible. Please set other xenotypes in xenotypeDefs.";
			//	Log.Error(description);
			//	return;
			//}
			List<GeneDef> allNewGenes = GetGenes();
			if (!xenotypeDesc.NullOrEmpty())
			{
				description = xenotypeDesc.Translate(firstXenotypeDef.label, secondXenotypeDef.label);
			}
			label = firstXenotypeDef.label.TrimmedToLength(3) + secondXenotypeDef.label.Reverse().TrimmedToLength(4).Reverse();
			foreach (GeneDef item in allNewGenes)
			{
				if (!genes.Contains(item))
				{
					genes.Add(item);
				}
			}

			List<GeneDef> GetGenes()
			{
				List<GeneDef> allNewGenes = new();
				int totalGenesCount = 0;
				foreach (XenotypeDef xenotype in XenotypeDefs)
				{
					totalGenesCount += xenotype.genes.Count;
					foreach (GeneDef geneDef in xenotype.genes)
					{
						if (exceptedGenes.Contains(geneDef) || allNewGenes.Contains(geneDef) || XaG_GeneUtility.ConflictWith(geneDef, allNewGenes))
						{
							continue;
						}
						allNewGenes.Add(geneDef);
					}
				}
				totalGenesCount /= XenotypeDefs.Count;
				allNewGenes.SortBy(gene => gene.prerequisite != null);
				foreach (GeneDef geneDef in allNewGenes.ToList())
				{
					if (totalGenesCount < allNewGenes.Count)
					{
						allNewGenes.Remove(geneDef);
					}
				}
				foreach (GeneDef geneDef in allNewGenes.ToList())
				{
					if (geneDef.prerequisite != null)
					{
						allNewGenes.Add(geneDef.prerequisite);
					}
				}
				foreach (GeneDef geneDef in allNewGenes.ToList())
				{
					if (geneDef.prerequisite != null && !allNewGenes.Contains(geneDef.prerequisite))
					{
						allNewGenes.Remove(geneDef);
					}
				}

				return allNewGenes;
			}
			//foreach (GeneDef item in genes)
			//{
			//	descriptionHyperlinks.Add(new DefHyperlink(item));
			//}
		}

	}

}
