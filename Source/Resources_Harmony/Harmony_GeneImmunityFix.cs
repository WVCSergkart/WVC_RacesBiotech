using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    namespace HarmonyPatches
    {

        // Replaces the vanilla check (without gene Active check) with a version with gene Active check
        [HarmonyPatch(typeof(Pawn_GeneTracker), "HediffGiversCanGive")]
		public static class WVC_Pawn_GeneTracker_HediffGiversCanGive_Patch
		{

			public static bool Prefix(ref bool __result, ref HediffDef hediff, Pawn_GeneTracker __instance)
			{
				if (WVC_Biotech.settings.fixVanillaGeneImmunityCheck)
				{
					if (!ModLister.BiotechInstalled)
					{
						__result = true;
						return false;
					}
					for (int i = 0; i < __instance.GenesListForReading.Count; i++)
					{
						Gene gene = __instance.GenesListForReading[i];
						if (gene.Active && gene.def.hediffGiversCannotGive != null && gene.def.hediffGiversCannotGive.Contains(hediff))
						{
							__result = false;
							return false;
						}
					}
					__result = true;
					return false;
				}
				return true;
			}

		}

		[HarmonyPatch(typeof(ImmunityHandler), "AnyGeneMakesFullyImmuneTo")]
		public static class WVC_ImmunityHandler_AnyGeneMakesFullyImmuneTo_Patch
		{

			public static bool Prefix(ref bool __result, ref HediffDef def, ImmunityHandler __instance)
			{
				if (WVC_Biotech.settings.fixVanillaGeneImmunityCheck)
				{
					if (!ModsConfig.BiotechActive || __instance.pawn.genes == null)
					{
						__result = false;
						return false;
					}
					for (int i = 0; i < __instance.pawn.genes.GenesListForReading.Count; i++)
					{
						Gene gene = __instance.pawn.genes.GenesListForReading[i];
						if (!gene.Active || gene.def.makeImmuneTo == null)
						{
							continue;
						}
						for (int j = 0; j < gene.def.makeImmuneTo.Count; j++)
						{
							if (gene.def.makeImmuneTo[j] == def)
							{
								__result = true;
								return false;
							}
						}
					}
					__result = false;
					return false;
				}
				return true;
			}

		}

	}

}
