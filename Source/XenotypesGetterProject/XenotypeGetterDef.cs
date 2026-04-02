using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetterDef : Def
	{

		public List<XenotypeDef> xenotypeDefs;

		public float chance = 0.5f;

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
