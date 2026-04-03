using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter
	{

		public XenotypeGetterDef def;

		public virtual bool CanFire(Pawn pawn)
		{
			return CanFire();
		}

		public virtual bool CanFire()
		{
			return true;
		}

		public virtual XenotypeDef GetXenotype()
		{
			return def.XenotypeDefs.RandomElement();
		}

		public virtual float Chance()
		{
			return def.chance;
		}

	}

}
