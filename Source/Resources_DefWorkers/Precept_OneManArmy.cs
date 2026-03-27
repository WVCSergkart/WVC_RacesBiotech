using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Precept_OneManArmy : Precept
	{

		private int nextTick = 60000;
		public override void Tick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 59981, 1))
			{
				return;
			}
			MiscUtility.ForeverAloneDevelopmentPoints();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 60000);
		}

	}

}
