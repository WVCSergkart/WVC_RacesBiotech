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
			if (def.XenotypeDefs.TryRandomElement(out XenotypeDef xenotypeDef))
			{
				return xenotypeDef;
			}
			return null;
		}

		public virtual float Chance()
		{
			return def.chance;
		}

	}

}
