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

		public virtual List<SubXenotypeDef> SubXenotypeDefs
		{
			get
			{
				return def.SubXenotypeDefs;
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

		public virtual XenotypeHolder GetXenotype()
		{
			if (XenotypeDefs.TryRandomElement(out XenotypeDef xenotypeDef))
			{
				return new(xenotypeDef);
			}
			if (SubXenotypeDefs.TryRandomElement(out SubXenotypeDef subXenotypeDef))
			{
				return new(subXenotypeDef);
			}
			return null;
		}

		public virtual float Chance()
		{
			return def.chance;
		}

	}

}
