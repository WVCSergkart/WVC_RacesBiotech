using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public interface IGeneInspectInfo
	{

		string GetInspectInfo { get; }

	}

	// public interface IGeneAddOrRemoveHediff
	// {

		// void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene, List<BodyPartDef> bodyparts = null);

		// void RemoveHediff(HediffDef hediffDef, Pawn pawn);

	// }

	public interface IGeneBloodfeeder
	{

		void Notify_Bloodfeed(Pawn victim);

	}

}
