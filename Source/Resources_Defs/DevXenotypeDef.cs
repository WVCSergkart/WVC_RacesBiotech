using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class DevXenotypeDef : XenotypeDef
	{

		public bool isThrall = false;

		public bool isHybrid = false;

        public bool isRandom = false;

        public bool devDisable = false;

        public string xenotypeDesc = "WVC_XaG_DevXenotypeDef_XenotypeDesc";

        //public XenotypeIconDef iconDef;

        public List<ThrallDef> thrallDefs = new();

		public List<XenotypeDef> xenotypeDefs;

		public List<GeneDef> guaranteedGenes;

		public List<GeneDef> exceptedGenes;

		public override void ResolveReferences()
        {
            if (devDisable)
            {
                base.ResolveReferences();
                return;
            }
            if (genes == null)
            {
                genes = new();
            }
            GeneDef geneticShifter = MainDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
            if (genes.Contains(geneticShifter))
            {
                genes.Remove(geneticShifter);
            }
            Setup();
            if (!isHybrid || isRandom)
            {
                genes.Add(geneticShifter);
            }
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
            else if (isThrall)
            {
                SetupThrall();
            }
        }

        private void SetupThrall()
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
            foreach (XenotypeDef item in xenotypeDefs)
            {
                descriptionHyperlinks.Add(new DefHyperlink(item));
            }
            if (isRandom)
            {
                return;
            }
            if (xenotypeDefs.Count != 2)
            {
                Log.Warning(defName + " has more xenotypes than this type supports. Max xenotypeDefs is 2.");
            }
            XenotypeDef firstXenotypeDef = xenotypeDefs.First();
            XenotypeDef secondXenotypeDef = xenotypeDefs.Last();
            if (!SubXenotypeUtility.TryGetHybridGenes(null, firstXenotypeDef.genes, secondXenotypeDef.genes, out List<GeneDef> allNewGenes, exceptedGenes, new()))
            {
                description = "Failed create hybrid from: " + firstXenotypeDef.defName + " and " + secondXenotypeDef.defName + ". Because their total metabolism is not compatible. Please set other xenotypes in xenotypeDefs.";
                Log.Error(description);
                return;
            }
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
            foreach (GeneDef item in genes)
            {
                descriptionHyperlinks.Add(new DefHyperlink(item));
            }
        }

    }

}
