using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_StaticCollection : Gene, IGeneOverridden
    {

        public override void PostAdd()
        {
            if (!Active)
            {
                return;
            }
            base.PostAdd();
            UpdCollection();
        }

        public virtual void Notify_Override()
        {
            UpdCollection();
        }

        public virtual void Notify_OverriddenBy(Gene overriddenBy)
        {
            UpdCollection();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            UpdCollection();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (!Active)
                {
                    return;
                }
                UpdCollection();
            }
        }

        public virtual void UpdCollection()
        {

        }

    }

    public class Gene_PerfectMemory : Gene_StaticCollection
    {

        public override void UpdCollection()
        {
            HarmonyPatches.HarmonyUtility.perfectMemoryPawns = new();
            HarmonyPatches.HarmonyUtility.ignoredMemoryPawns = new();
        }

    }

}
