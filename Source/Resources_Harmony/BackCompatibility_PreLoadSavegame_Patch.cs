using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    namespace HarmonyPatches
    {

		/// <summary>
		/// Inject BackCompatibilityConverter instances into vanilla code.
		/// </summary>
		[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.PreLoadSavegame))]
		public static class BackCompatibility_PreLoadSavegame_Patch
		{
			public static void Prefix(List<BackCompatibilityConverter> ___conversionChain)
			{
				___conversionChain.Add(new BackCompatibilityConverter_Any());
			}
		}

	}

}
