using HarmonyLib;
using RimWorld;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	public class WVC_XenotypesAndGenes_Main : Mod
	{
		public WVC_XenotypesAndGenes_Main(ModContentPack content)
			: base(content)
		{
			new Harmony("wvc.sergkart.races.biotech").PatchAll();
			// var harmony = new Harmony("wvc.sergkart.races.biotech");
			// harmony.PatchAll();
		}
	}

	// namespace HarmonyPatches
	// {

		// public static class OptionalHarmonyPatches
		// {

			// public static bool HideXaGGenes(GeneDef geneDef, ref bool __result)
			// {
				// if (geneDef.modContentPack != null && geneDef.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech"))
				// {
					// __result = false;
					// return false;
				// }
				// return true;
			// }

		// }

	// }

}
