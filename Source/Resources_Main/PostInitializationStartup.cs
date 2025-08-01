using RimWorld;
using System;
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
            string phase = "";
            try
            {
                phase = "mod settings";
                InitialUtility.InitializeModSettings();
                // Hediffs();
                phase = "genes setup";
                InitialUtility.GenesAndMutants();
                //InitialUtility.ThingDefs();
                //InitialUtility.XenotypeDefs();
                //phase = "harmony";
                //HarmonyPatches.HarmonyUtility.HarmonyPatches();
            }
            catch (Exception arg)
            {
                Log.Error("Initial error on phase: " + phase + ". Reason: " + arg);
            }
        }

    }

}
