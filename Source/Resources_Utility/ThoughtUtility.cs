using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ThoughtUtility
	{

		public static void ThoughtFromThing(Thing parent, ThoughtDef thoughtDef, bool showEffect = true, int radius = 5)
		{
			foreach (Thing item in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, radius, useCenter: true))
			{
				if (item is not Pawn pawn || pawn.AnimalOrWildMan() || !pawn.RaceProps.IsFlesh || pawn == parent || pawn.Dead || pawn.Downed || !(pawn.GetStatValue(StatDefOf.PsychicSensitivity) > 0f))
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

	}
}
