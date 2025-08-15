using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class DuplicatorOutcomeDef : Def
	{

		public Type workerClass = typeof(DuplicatorOutcome);

		public float basicChance = 1f;

		public List<GeneDef> geneDefs;

		[Unsaved(false)]
		private DuplicatorOutcome workerInt;

		public DuplicatorOutcome Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (DuplicatorOutcome)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

	}

}
