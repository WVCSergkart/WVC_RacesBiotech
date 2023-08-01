using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
