using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class ModsUtility
	{

		private static bool? cachedVanillaExpandedFrameworkActive;
		public static bool VanillaExpandedFrameworkActive
		{
			get
			{
				if (cachedVanillaExpandedFrameworkActive == null)
				{
					cachedVanillaExpandedFrameworkActive = LoadedModManager.RunningModsListForReading.Any(mod => mod.PackageId == "oskarpotocki.vanillafactionsexpanded.core");
				}
				return cachedVanillaExpandedFrameworkActive.Value;
			}
		}

	}

}