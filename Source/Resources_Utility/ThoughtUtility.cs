using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ThoughtUtility
	{

		// ==================

		[Obsolete]
		public static void ThoughtFromThing(Thing parent, ThoughtDef thoughtDef, bool showEffect = true, int radius = 5)
		{
			foreach (Thing item in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, radius, useCenter: true))
			{
				if (item is not Pawn pawn || pawn.AnimalOrWildMan() || !pawn.RaceProps.IsFlesh || pawn == parent || pawn.Dead || pawn.Downed || !pawn.IsPsychicSensitive())
				{
					continue;
				}
				if (showEffect)
				{
					Find.TickManager.slower.SignalForceNormalSpeedShort();
					SoundDefOf.PsychicPulseGlobal.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
					FleckMaker.AttachedOverlay(parent, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
				}
				pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
				// pawn.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(thoughtDef);
			}
		}

		// ============================= GENE OPINION =============================

		public static void MyOpinionAboutPawnMap(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				// Log.Error(pawn.Name.ToString() + " hate " + pawns[i].Name.ToString());
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawns[i]);
			}
		}

		public static void PawnMapOpinionAboutMe(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				pawns[i].needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawn);
			}
			if (shouldBePsySensitive && pawn.Map != null)
			{
				FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			}
		}

		public static bool CanSetOpinion(Pawn pawn, Pawn other, Gene gene, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (!other.RaceProps.Humanlike)
			{
				return false;
			}
			if (other == pawn)
			{
				return false;
			}
			if (shouldBeFamily && !pawn.relations.FamilyByBlood.Contains(other))
			{
				return false;
			}
			if (ignoreIfHasGene && XaG_GeneUtility.HasActiveGene(gene.def, other))
			{
				return false;
			}
			if (shouldBePsySensitive && !other.IsPsychicSensitive())
			{
				return false;
			}
			if (onlySameXenotype && !GeneUtility.SameXenotype(pawn, other))
			{
				return false;
			}
			return true;
		}

	}
}
