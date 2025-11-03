using System.Collections.Generic;
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
			Gene_Hivemind_EfficiencyNode.cachedEfficiencyOffset = (int)(efficiency * -1f);
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
			SetEfficiency();
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedEfficiencyOffset, "efficiencyOffset", 0);
		}

	}

}
