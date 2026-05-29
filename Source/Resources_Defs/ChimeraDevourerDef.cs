using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ChimeraDevourerDef : Def
	{

		public List<PawnKindDef> targetPawnKindDefs;
		public List<string> rewardXenotypeDefs;

		private List<XenotypeDef> cachedXenotypeDefs;
		public List<XenotypeDef> XenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					cachedXenotypeDefs = rewardXenotypeDefs.ConvertToDefs<XenotypeDef>();
				}
				return cachedXenotypeDefs;
			}
		}

		public Type workerClass = typeof(ChimeraDevourer);

		[Unsaved(false)]
		private ChimeraDevourer workerInt;

		public ChimeraDevourer Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (ChimeraDevourer)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

	}

}
