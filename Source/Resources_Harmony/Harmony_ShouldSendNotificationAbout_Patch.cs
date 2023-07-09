using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	[HarmonyPatch(typeof(PawnUtility), "ShouldSendNotificationAbout")]
	public static class PawnUtility_ShouldSendNotificationAbout_Patch
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			if (__result && MechanoidizationUtility.ShouldNotSendNotificationAbout(p))
			{
				__result = false;
			}
		}
	}

	// [HarmonyPatch(typeof(TaleUtility), "Notify_PawnDied")]
	// public static class TaleUtility_Notify_PawnDied_Patch
	// {
		// public static bool Prefix(ref Pawn victim)
		// {
			// if (MechanoidizationUtility.ShouldNotSendNotificationAbout(victim, true))
			// {
				// return false;
			// }
			// return true;
		// }
	// }

}
