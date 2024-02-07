using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(CompRottable), "RotProgress", MethodType.Setter)]
		public static class CompRottable_RotProgress_Patch
		{
			public static bool Prefix(CompRottable __instance)
			{
				if (__instance.parent is Pawn pawn && pawn.IsNotRottable())
				{
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(CompRottable), "CompInspectStringExtra")]
		public static class CompRottable_CompInspectStringExtra_Patch
		{
			public static bool Prefix(CompRottable __instance, ref string __result)
			{
				if (__instance.parent is Corpse corpse && corpse.InnerPawn.IsNotRottable())
				{
					__result = null;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(CompRottable), "Active", MethodType.Getter)]
		public static class CompRottable_Active_Patch
		{
			public static bool Prefix(CompRottable __instance, ref bool __result)
			{
				if (__instance.parent is Corpse corpse && corpse.InnerPawn.IsNotRottable())
				{
					__result = false;
					return false;
				}
				return true;
			}
		}

	}

}
