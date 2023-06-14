using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class ThingWithComps_XenotypeForcer : ThingWithComps
	{
		public override void PostMake()
		{
			base.PostMake();
			InitializeComps();
			if (comps != null)
			{
				for (int i = 0; i < comps.Count; i++)
				{
					comps[i].PostPostMake();
				}
			}
		}
	}

}
