using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter
	{

		public XenotypeGetterDef def;

		public virtual bool CanFire()
		{
			return true;
		}

		public virtual XenotypeDef GetXenotype()
		{
			return def.xenotypeDefs.RandomElement();
		}

	}

}
