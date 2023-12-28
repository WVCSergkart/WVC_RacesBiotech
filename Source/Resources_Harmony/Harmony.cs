using HarmonyLib;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class WVC_XenotypesAndGenes_Main : Mod
    {
        public WVC_XenotypesAndGenes_Main(ModContentPack content)
            : base(content)
        {
            new Harmony("wvc.sergkart.races.biotech").PatchAll();
        }
    }

}
