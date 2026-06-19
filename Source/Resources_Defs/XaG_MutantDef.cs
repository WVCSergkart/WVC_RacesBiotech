using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XaG_MutantDef : MutantDef
	{


		public List<GeneDef> geneDefs;


		public static bool initialized = false;

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			//if (initialized)
			//{
			//	return;
			//}
			//initialized = true;
			//PawnTableDef workTable = MainDefOf.WVC_EntitiesWork;
			//foreach (PawnColumnDef item in DefDatabase<PawnColumnDef>.AllDefsListForReading)
			//{
			//	if (item.Worker is PawnColumnWorker_WorkPriority)
			//	{
			//		workTable.columns.Insert(workTable.columns.FindIndex((PawnColumnDef x) => x.Worker is PawnColumnWorker_CopyPasteWorkPriorities) + 1, item);
			//	}
			//}
		}

	}

}
