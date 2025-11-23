using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Hivemind_EfficiencyNode : Gene_Hivemind
	{

		public static int cachedEfficiencyOffset = 0;

		private void SetEfficiency()
		{
			MiscUtility.UpdateStaticCollection();
			float efficiency = 0;
			List<Pawn> hivemindPawns = Hivemind;
			if (hivemindPawns.Count <= 0)
			{
				HivemindUtility.ResetCollection();
				hivemindPawns = Hivemind;
				Log.Warning("Hivemind pawns count is 0, but the efficiency node was triggered. Trying recache hivemind.");
			}
			foreach (Pawn hiver in hivemindPawns)
			{
				efficiency += (hiver.GetStatValue(StatDefOf.PsychicSensitivity) - 1f) * 100f;
				if (StaticCollectionsClass.cachedPlayerPawnsCount == hivemindPawns.Count)
				{
					efficiency += 10000;
				}
				else
				{
					efficiency += (hivemindPawns.Count - StaticCollectionsClass.cachedPlayerPawnsCount) * 100f;
				}
				efficiency += (StaticCollectionsClass.cachedDuplicatesCount / hivemindPawns.Count) * 100f;
				efficiency += SubSetEfficiency(hiver);
			}
			// Colony size buff
			SimpleCurve colonyCurve = new()
			{
				new CurvePoint(1, 50000),
				new CurvePoint(3, 30000),
				new CurvePoint(5, 15000),
				new CurvePoint(10, 0)
			};
			efficiency += colonyCurve.Evaluate(StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount);
			if (pawn.Spawned)
			{
				// Performance check
				SimpleCurve pawnUpdateRateTicks = new()
				{
					new CurvePoint(500, 30000),
					new CurvePoint(60, 5000),
					new CurvePoint(0, 0)
				};
				efficiency += pawnUpdateRateTicks.Evaluate(pawn.UpdateRateTicks);
			}
			Gene_Hivemind_EfficiencyNode.cachedEfficiencyOffset = (int)(efficiency);
		}


#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
		/// <summary>
		/// Modders hook.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static float SubSetEfficiency(Pawn target)
		{
			return 0;
		}
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression

		/// <summary>
		/// Efficiency offset affects this gene.
		/// </summary>
		public override void UpdGeneSync()
		{
			try
			{
				SetEfficiency();
			}
			catch (Exception arg)
			{
				Log.Error("Failed SetEfficiency(). Reason: " + arg.Message);
			}
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			if (overriddenBy is not Gene_Hivemind_EfficiencyNode)
			{
				cachedEfficiencyOffset = 0;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			cachedEfficiencyOffset = 0;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_HivemindTick_Label".Translate(), HivemindUtility.TickRefresh.ToStringTicksToDays(), "WVC_XaG_HivemindTick_Desc".Translate(), 120);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedEfficiencyOffset, "efficiencyOffset", 0);
		}

	}

}
