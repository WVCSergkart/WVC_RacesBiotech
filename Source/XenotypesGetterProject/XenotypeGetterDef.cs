using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// Non-def. But Def in name
	public class XenotypeGetterDef
	{

		public FactionDef factionDef;

		public List<string> reqXenotypeDefs;

		public List<string> xenotypeDefs;

		private List<XenotypeDef> cachedXenotypeDefs;
		public List<XenotypeDef> XenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					cachedXenotypeDefs = xenotypeDefs.ConvertToDefs<XenotypeDef>();
					if (cachedXenotypeDefs.NullOrEmpty() && cachedSubXenotypeDefs.NullOrEmpty())
					{
						disabled = true;
					}
				}
				return cachedXenotypeDefs;
			}
		}

		private List<SubXenotypeDef> cachedSubXenotypeDefs;
		public List<SubXenotypeDef> SubXenotypeDefs
		{
			get
			{
				if (cachedSubXenotypeDefs == null)
				{
					cachedSubXenotypeDefs = xenotypeDefs.ConvertToDefs<SubXenotypeDef>();
					if (cachedSubXenotypeDefs.NullOrEmpty() && cachedXenotypeDefs.NullOrEmpty())
					{
						disabled = true;
					}
				}
				return cachedSubXenotypeDefs;
			}
		}

		public bool disabled = false;

		public float chance = 1.0f;

		public Type workerClass = typeof(XenotypeGetter);

		[Unsaved(false)]
		private XenotypeGetter workerInt;
		public XenotypeGetter Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (XenotypeGetter)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}


	}

}
