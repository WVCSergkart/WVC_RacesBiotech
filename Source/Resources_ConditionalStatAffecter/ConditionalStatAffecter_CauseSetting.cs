// using System.Xml;
using System.Linq;
// using System.Reflection;
// using System.Collections.Generic;
using RimWorld;
using Verse;
// using UnityEngine;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
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
