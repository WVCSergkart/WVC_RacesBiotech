// using System.Xml;
// using System.Reflection;
// using System.Collections.Generic;
using RimWorld;
using System;
using System.Linq;
using Verse;
// using UnityEngine;

namespace WVC_XenotypesAndGenes
{

    [Obsolete]
    public class ConditionalStatAffecter_CauseSetting : ConditionalStatAffecter
    {
        public string settingName;

        public override string Label => Invert();

        public override bool Applies(StatRequest req)
        {
            if (!ModsConfig.BiotechActive)
            {
                return false;
            }
            if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) == true)
            {
                return true;
            }
            return false;
        }

        private string Invert()
        {
            if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) == true)
            {
                return "WVC_StatsReport_CauseSetting_On".Translate();
            }
            return "WVC_StatsReport_CauseSetting_Off".Translate();
        }
    }
}
