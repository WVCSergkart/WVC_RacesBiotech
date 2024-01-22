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

		[HarmonyPatch(typeof(InteractionUtility), "IsGoodPositionForInteraction", new Type[] {typeof(Pawn), typeof(Pawn)} )]
		public static class Patch_InteractionUtility_IsGoodPositionForInteraction
		{

			[HarmonyPostfix]
			public static void Postfix(ref bool __result, Pawn p, Pawn recipient)
			{
				if (__result)
				{
					return;
				}
				if (recipient.PawnPsychicSensitive() && p?.genes?.GetFirstGeneOfType<Gene_Telepathy>() != null)
				{
					__result = true;
				}
			}

		}

	}

}
