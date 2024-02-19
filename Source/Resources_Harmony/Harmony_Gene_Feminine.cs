using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	namespace HarmonyPatches
	{

		// Useless
		[HarmonyPatch(typeof(PawnGenerator), "GetBodyTypeFor")]
		public static class Patch_PawnGenerator_GetBodyTypeFor
		{

			[HarmonyPostfix]
			public static void Postfix(ref BodyTypeDef __result, ref Pawn pawn)
			{
				// Log.Error("0");
				if (__result != BodyTypeDefOf.Male)
				{
					return;
				}
				// Log.Error("1");
				if (pawn?.genes?.GetFirstGeneOfType<Gene_Feminine>() == null)
				{
					return;
				}
				pawn.story.bodyType = BodyTypeDefOf.Female;
				// Log.Error("2");
			}

		}

	}

}
