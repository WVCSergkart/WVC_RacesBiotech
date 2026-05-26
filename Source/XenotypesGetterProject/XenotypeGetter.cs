using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter
	{

		public XenotypeGetterDef def;

		public virtual bool Disabled => def.disabled || XenotypeDefs.Empty();

		public virtual List<XenotypeDef> XenotypeDefs
		{
			get
			{
				return def.XenotypeDefs;
			}
		}

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
			if (XenotypeDefs.TryRandomElement(out XenotypeDef xenotypeDef))
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
