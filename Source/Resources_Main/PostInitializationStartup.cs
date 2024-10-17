using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class XaG_PostInitialization
	{

		static XaG_PostInitialization()
		{
			InitialUtility.InitializeModSettings();
			// Hediffs();
			InitialUtility.GenesAndMutants();
			InitialUtility.ThingDefs();
			InitialUtility.XenotypeDefs();
			HarmonyPatches.HarmonyUtility.PostInitialPatches();
		}

	}

}
