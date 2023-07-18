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
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(FoodUtility), "IsAcceptablePreyFor")]
		public static class Patch_FoodUtility_IsAcceptablePreyFor
		{

			[HarmonyPrefix]
			// [HarmonyPostfix]
			public static bool Prefix(ref bool __result, ref Pawn prey)
			{
				if (prey.RaceProps.Humanlike && MechanoidizationUtility.IsNotAcceptablePrey(prey))
				{
					// Log.Error(prey.Name + " is not a prey.");
					__result = false;
					return false;
					// Log.Error(prey.Name + " is not a mechaskin.");
				}
				// Log.Error(prey.Name + " is a prey.");
				// WVC_BiotechSettings settings = WVC_Biotech.settings;
				// if (!settings.canMechaskinBePredatorPrey)
				// {
					// if (MechanoidizationUtility.PawnIsMechaskinned(prey))
					// {
						// __result = false;
						// return false;
					// }
				// }
				return true;
			}
		}

	}

}
