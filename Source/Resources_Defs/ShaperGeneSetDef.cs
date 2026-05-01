using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ShaperGeneSetDef : Def
	{

		public List<GeneDef> allowedShifterDefs;
		public List<GeneDef> requiredGeneDefs;
		public List<GeneralHolder> geneSets;
		public List<GeneCategoryDef> geneCategoryDefs;

		//private List<GeneralHolder> cachedGeneSets;
		//public List<GeneralHolder> GeneSets
		//{
		//	get
		//	{
		//		if (cachedGeneSets == null)
		//		{
		//			cachedGeneSets = geneSets.ConvertToDefs<GeneDef>();
		//		}
		//		return cachedGeneSets;
		//	}
		//}

		public Type workerClass = typeof(ShaperGeneSet);

		[Unsaved(false)]
		private ShaperGeneSet workerInt;

		public ShaperGeneSet Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (ShaperGeneSet)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

		public bool Allowed(Pawn pawn, Gene gene)
		{
			if (geneSets == null)
			{
				return false;
			}
			if (allowedShifterDefs != null && !allowedShifterDefs.Contains(gene.def))
			{
				return false;
			}
			if (requiredGeneDefs != null && !XaG_GeneUtility.HasAllGenes(requiredGeneDefs, pawn))
			{
				return false;
			}
			if (geneCategoryDefs != null && !pawn.genes.Endogenes.Any(gene => geneCategoryDefs.Contains(gene.def.displayCategory)))
			{
				return false;
			}
			return Worker.Allowed(pawn);
		}

	}

}
