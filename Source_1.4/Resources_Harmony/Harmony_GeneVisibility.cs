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

		[HarmonyPatch(typeof(Dialog_CreateXenotype), "DrawGene")]
		public static class Patch_Dialog_CreateXenotype_DrawGene
		{
			public static bool Prefix(GeneDef geneDef, ref bool __result)
			{
				if (!WVC_Biotech.settings.hideXaGGenes)
				{
					return true;
				}
				if (geneDef.modContentPack != null && geneDef.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech"))
				{
					__result = false;
					return false;
				}
				return true;
			}
		}

	}

}
