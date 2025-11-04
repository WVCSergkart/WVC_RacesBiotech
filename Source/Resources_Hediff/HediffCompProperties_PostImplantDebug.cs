using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_PostImplantDebug : HediffCompProperties
	{

		public HediffCompProperties_PostImplantDebug()
		{
			compClass = typeof(HediffComp_PostImplantDebug);
		}

	}

	public class HediffComp_PostImplantDebug : HediffComp
	{

		public override void CompPostPostRemoved()
		{
			ReimplanterUtility.PostImplantDebug(Pawn);
		}

	}

}
