using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DevXenotypeDef : XenotypeDef
	{

		public bool isThrall = false;

		public bool isHybrid = false;

		public List<ThrallDef> thrallDefs = new();

		public List<XenotypeDef> xenotypeDefs;

		public List<GeneDef> guaranteedGenes;

		public List<GeneDef> exceptedGenes;

		public override void ResolveReferences()
        {
            if (genes == null)
            {
                genes = new();
            }
            GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
			if (genes.Contains(geneticShifter))
			{
				genes.Remove(geneticShifter);
			}
            Setup();
			genes.Add(geneticShifter);
		}

        private void Setup()
		{
			if (descriptionHyperlinks == null)
			{
				descriptionHyperlinks = new List<DefHyperlink>();
			}
			if (isHybrid)
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
					xenotypeDefs = new();
				}
				foreach (XenotypeDef item in xenotypeDefs)
				{
					descriptionHyperlinks.Add(new DefHyperlink(item));
				}
			}
            else if (isThrall)
            {
                if (guaranteedGenes.NullOrEmpty())
                {
                    guaranteedGenes = genes;
                }
                foreach (GeneDef gene in guaranteedGenes)
                {
                    descriptionHyperlinks.Add(new DefHyperlink(gene));
                }
                foreach (ThrallDef thrallDef in DefDatabase<ThrallDef>.AllDefsListForReading)
                {
                    descriptionHyperlinks.Add(new DefHyperlink(thrallDef));
                }
                inheritable = true;
                doubleXenotypeChances = null;
            }
        }
    }

}
