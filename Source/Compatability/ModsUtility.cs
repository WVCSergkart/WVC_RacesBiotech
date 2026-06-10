using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class ModsUtility
	{

		public static bool DevMode => WVC_Biotech.settings.devMode && DebugSettings.ShowDevGizmos;

		public static bool GameStarted()
		{
			return !GameNotStarted();
		}

		public static bool GameNotStarted()
		{
			return Current.ProgramState != ProgramState.Playing;
		}

		// ======================== OTHER_MODS ========================
		// ======================== OTHER_MODS ========================
		// ======================== OTHER_MODS ========================

		private static bool? cachedVanillaExpandedFrameworkActive;
		public static bool VanillaExpandedFrameworkActive
		{
			get
			{
				if (cachedVanillaExpandedFrameworkActive == null)
				{
					cachedVanillaExpandedFrameworkActive = LoadedModManager.RunningModsListForReading.Any(mod => mod.PackageId == "oskarpotocki.vanillafactionsexpanded.core");
					//GetMods();
				}
				return cachedVanillaExpandedFrameworkActive.Value;
			}
		}

		//private static void GetMods()
		//{
		//	cachedVanillaExpandedFrameworkActive = false;
		//	cachedFungoidForkedActive = false;
		//	cachedLycanthropeForkedActive = false;
		//	cachedSanguophageForkedActive = false;
		//	cachedSauridForkedActive = false;
		//	foreach (ModContentPack modContentPack in LoadedModManager.RunningModsListForReading)
		//	{
		//		switch (modContentPack.PackageId)
		//		{
		//			case "oskarpotocki.vanillafactionsexpanded.core":
		//				cachedVanillaExpandedFrameworkActive = true;
		//				break;
		//			case "vanillaracesexpanded.fungoid.forked":
		//				cachedFungoidForkedActive = true;
		//				break;
		//			case "vanillaracesexpanded.lycanthrope.forked":
		//				cachedLycanthropeForkedActive = true;
		//				break;
		//			case "vanillaracesexpanded.sanguophage.forked":
		//				cachedSanguophageForkedActive = true;
		//				break;
		//			case "vanillaracesexpanded.saurid.forked":
		//				cachedSauridForkedActive = true;
		//				break;
		//		}
		//		//if (modContentPack.PackageId == "oskarpotocki.vanillafactionsexpanded.core")
		//		//{
		//		//	cachedVanillaExpandedFrameworkActive = true;
		//		//}
		//		//else if (modContentPack.PackageId == "vanillaracesexpanded.fungoid.forked")
		//		//{
		//		//	cachedFungoidForkedActive = true;
		//		//}
		//		//else if (modContentPack.PackageId == "vanillaracesexpanded.lycanthrope.forked")
		//		//{
		//		//	cachedLycanthropeForkedActive = true;
		//		//}
		//		//else if (modContentPack.PackageId == "vanillaracesexpanded.sanguophage.forked")
		//		//{
		//		//	cachedSanguophageForkedActive = true;
		//		//}
		//		//else if (modContentPack.PackageId == "vanillaracesexpanded.saurid.forked")
		//		//{
		//		//	cachedSauridForkedActive = true;
		//		//}
		//	}
		//}

		//private static bool? cachedFungoidForkedActive;
		//public static bool FungoidForkedActive
		//{
		//	get
		//	{
		//		if (cachedFungoidForkedActive == null)
		//		{
		//			GetMods();
		//		}
		//		return cachedFungoidForkedActive.Value;
		//	}
		//}

		//private static bool? cachedLycanthropeForkedActive;
		//public static bool LycanthropeForkedActive
		//{
		//	get
		//	{
		//		if (cachedLycanthropeForkedActive == null)
		//		{
		//			GetMods();
		//		}
		//		return cachedLycanthropeForkedActive.Value;
		//	}
		//}

		//private static bool? cachedSanguophageForkedActive;
		//public static bool SanguophageForkedActive
		//{
		//	get
		//	{
		//		if (cachedSanguophageForkedActive == null)
		//		{
		//			GetMods();
		//		}
		//		return cachedSanguophageForkedActive.Value;
		//	}
		//}

		//private static bool? cachedSauridForkedActive;
		//public static bool SauridForkedActive
		//{
		//	get
		//	{
		//		if (cachedSauridForkedActive == null)
		//		{
		//			GetMods();
		//		}
		//		return cachedSauridForkedActive.Value;
		//	}
		//}

	}

}