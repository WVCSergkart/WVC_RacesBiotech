using RimWorld;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class IngestionOutcomeDoer_HumanEgg : IngestionOutcomeDoer
	{

		public ThoughtDef thoughtDef;
		public TaleDef taleDef;

		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
		{
			try
			{
				//TaleRecorder.RecordTale(TaleDefOf.KilledChild, pawn, ingested);
				//Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RimWorld.HistoryEventDefOf.MemberKilled, pawn.Named(HistoryEventArgsNames.Doer)), canApplySelfTookThoughts: false);
				//Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RimWorld.HistoryEventDefOf.AteHumanMeat, pawn.Named(HistoryEventArgsNames.Doer)), canApplySelfTookThoughts: false);
				//CompHumanEgg compHumanEgg = ingested.TryGetComp<CompHumanEgg>();
				//if (compHumanEgg != null)
				//{
				//	compHumanEgg.hatcheeParent?.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef, pawn);
				//	compHumanEgg.otherParent?.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef, pawn);
				//}
				TaleRecorder.RecordTale(taleDef, pawn);
				pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
			}
			catch (Exception arg)
			{
				Log.Warning("Error in IngestionOutcomeDoer_HumanEgg in " + ingested.def + ". Reason: " + arg.Message);
			}
		}

	}

}
