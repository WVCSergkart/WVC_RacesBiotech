using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PatchOperationOptional : PatchOperation
	{
		public string settingName;
		public PatchOperation caseTrue;
		public PatchOperation caseFalse;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) != true && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}

}
