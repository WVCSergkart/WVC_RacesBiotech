using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_StorageImplanter : Gene
    {

        private List<GeneDef> genes = new();
        public List<GeneDef> StoredGenes
        {
            get
            {
                if (genes == null)
                {
                    genes = new();
                }
                return genes;
            }
        }

        public bool TryAddGene(GeneDef geneDef)
		{
			if (!genes.Contains(geneDef))
			{
                genes.Add(geneDef);
				return true;
			}
			return false;
		}

		public bool TryRemoveGene(GeneDef geneDef)
		{
			if (genes.Contains(geneDef))
			{
                genes.Remove(geneDef);
                return true;
			}
            return false;
        }

        public void ResetGenes()
        {
            genes = new();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref genes, "storedGenes", LookMode.Def);
            if (Scribe.mode == LoadSaveMode.LoadingVars && genes != null && genes.RemoveAll((GeneDef x) => x == null) > 0)
            {
                Log.Warning("Removed null geneDef(s)");
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (genes == null)
                {
                    genes = new();
                }
            }
        }

    }

}
