using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeDef_XaG : XenotypeDef
	{
		public List<FactionChance> factionChances;

		public override void ResolveReferences()
		{
			try
			{
				if (factionChances != null && WVC_Biotech.settings.enable_spawnXenotypesInFactions)
				{
					foreach (FactionChance faction in factionChances)
					{
						if (faction.factionDef.xenotypeSet == null)
						{
							faction.factionDef.xenotypeSet = new();
						}
						faction.factionDef.xenotypeSet.xenotypeChances.Add(new(this, faction.chance));
					}
				}
			}
			catch
			{
				Log.Error("Failed set xenotype chance for xenotype: " + defName);
			}
			base.ResolveReferences();
		}

		public class FactionChance
		{
			public FactionDef factionDef;

			public float chance;

			public FactionChance()
			{
			}
			public void LoadDataFromXmlCustom(XmlNode xmlRoot)
			{
				DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "factionDef", xmlRoot.Name);
				chance = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
			}
		}

	}

}
