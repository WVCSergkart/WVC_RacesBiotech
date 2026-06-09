using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

	// Genes recache trigger
	public class HediffCompProperties_XenogermReplicating : HediffCompProperties
	{

		public HediffCompProperties_XenogermReplicating()
		{
			compClass = typeof(HediffComp_XenogermReplicating);
		}

	}

	public class HediffComp_XenogermReplicating : HediffComp
	{

		public override void CompPostPostRemoved()
		{
			ReimplanterUtility.NotifyGenesChanged(Pawn);
		}

	}

}
