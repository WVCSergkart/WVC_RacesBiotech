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
			if (__result && MechanoidizationUtility.ShouldSendNotificationAbout(p))
			{
				__result = false;
			}
		}
	}

}
