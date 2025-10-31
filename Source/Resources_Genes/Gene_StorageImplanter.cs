using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_StorageImplanter : Gene
	{

		//      private List<GeneDef> genes = new();
		//      public List<GeneDef> StoredGenes
		//      {
		//          get
		//          {
		//              if (genes == null)
		//              {
		//                  genes = new();
		//              }
		//              return genes;
		//          }
		//      }

		//      public bool TryAddGene(GeneDef geneDef)
		//{
		//	if (!genes.Contains(geneDef))
		//	{
		//              genes.Add(geneDef);
		//		return true;
		//	}
		//	return false;
		//}

		//public bool TryRemoveGene(GeneDef geneDef)
		//{
		//	if (genes.Contains(geneDef))
		//	{
		//              genes.Remove(geneDef);
		//              return true;
		//	}
		//          return false;
		//      }

		//public void SetupHolder()
		//{
		//    xenotypeHolder = new();
		//}

		public static bool CanStoreGenes(Pawn pawn, out Gene_StorageImplanter implanter)
		{
			implanter = pawn.genes?.GetFirstGeneOfType<Gene_StorageImplanter>();
			if (implanter != null)
			{
				Messages.Message("WVC_XaG_StorageImplanter_Message".Translate(), null, MessageTypeDefOf.PositiveEvent, historical: false);
				return true;
			}
			Messages.Message("WVC_XaG_StorageImplanter_ErrorMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}

		public void SetupHolder(XenotypeDef xenotypeDef = null, List<GeneDef> genes = null, bool inheritable = false, XenotypeIconDef icon = null, string name = null)
		{
			if (xenotypeDef == XenotypeDefOf.Baseliner)
			{
				if (name.NullOrEmpty())
				{
					name = GeneUtility.GenerateXenotypeNameFromGenes(genes);
				}
				if (icon == null)
				{
					icon = DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
				}
			}
			this.xenotypeHolder = new SaveableXenotypeHolder(xenotypeDef, genes, inheritable, icon, name);
			this.xenotypeHolder.PostSetup();
			//GeneUtility.UpdateXenogermReplication(pawn);
		}

		public void SetupHolder(XenotypeHolder xenotypeHolder)
		{
			this.xenotypeHolder = new SaveableXenotypeHolder(xenotypeHolder);
			this.xenotypeHolder.PostSetup();
			//GeneUtility.UpdateXenogermReplication(pawn);
		}

		public bool TryAddGene(GeneDef geneDef)
		{
			if (xenotypeHolder == null)
			{
				return false;
			}
			if (!xenotypeHolder.genes.Contains(geneDef))
			{
				xenotypeHolder.genes.Add(geneDef);
				return true;
			}
			return false;
		}

		public bool TryRemoveGene(GeneDef geneDef)
		{
			if (xenotypeHolder == null)
			{
				return false;
			}
			if (xenotypeHolder.genes.Contains(geneDef))
			{
				xenotypeHolder.genes.Remove(geneDef);
				return true;
			}
			return false;
		}

		private SaveableXenotypeHolder xenotypeHolder = null;
		public SaveableXenotypeHolder XenotypeHolder => xenotypeHolder;

		public void ResetHolder()
		{
			//genes = new();
			xenotypeHolder = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref xenotypeHolder, "xenotypeHolder");
			//Scribe_Collections.Look(ref genes, "storedGenes", LookMode.Def);
			//if (Scribe.mode == LoadSaveMode.LoadingVars && genes != null && genes.RemoveAll((GeneDef x) => x == null) > 0)
			//{
			//    Log.Warning("Removed null geneDef(s)");
			//}
			//if (Scribe.mode == LoadSaveMode.PostLoadInit)
			//{
			//    if (genes == null)
			//    {
			//        genes = new();
			//    }
			//}
		}

	}

}
