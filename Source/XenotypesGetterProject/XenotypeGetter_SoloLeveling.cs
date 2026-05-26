using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter_SoloLeveling : XenotypeGetter
	{

		private static List<XenotypeDef> cachedXenotypeDefs;
		public override List<XenotypeDef> XenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					cachedXenotypeDefs = ListsUtility.GetAllXenotypesExceptAndroids().Where(xenos => xenos.IsXenoGenesDef()).ToList();
				}
				return cachedXenotypeDefs;
			}
		}

		public override bool CanFire()
		{
			return StaticCollectionsClass.oneManArmyMode;
		}

	}

}
