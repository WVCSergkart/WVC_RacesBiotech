using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_TargetEffect_DoJobOnTarget : CompProperties
	{
		public JobDef jobDef;

		public ThingDef moteDef;

		public XenotypeDef xenotypeDef = null;

		public CompProperties_TargetEffect_DoJobOnTarget()
		{
			compClass = typeof(CompTargetEffect_DoJobOnTarget);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			Log.Warning(parentDef.defName + " uses an outdated serum component.");
		}

	}

}
