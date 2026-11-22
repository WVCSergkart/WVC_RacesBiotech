using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class SacrificerModeDef : Def
	{

		public Type workerClass = typeof(SacrificerMode);

		public bool canBeScenarioOption = false;
		public float displayOrder;
		public bool initial = false;
		public bool canBeSwitchedManually = true;
		public List<ThingDefCountWithChance> rewards;
		public int chosenOnesLimit = 5;

		public class ThingDefCountWithChance
		{

			public string LabelCap => thingDef.LabelCap;

			public ThingDef thingDef;
			public int count = 1;
			public float chance = 1f;

			public ThingDefCountWithChance()
			{
			}

			//public void LoadDataFromXmlCustom(XmlNode xmlRoot)
			//{
			//	DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
			//	count = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
			//	chance = ParseHelper.FromString<float>(xmlRoot.);
			//}

		}

		[Unsaved(false)]
		private SacrificerMode workerInt;
		public SacrificerMode Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (SacrificerMode)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

	}

}
