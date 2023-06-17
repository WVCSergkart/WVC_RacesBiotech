using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeSerum : ThingWithComps
	{
		// public override void PostMake()
		// {
			// base.PostMake();
			// InitializeComps();
			// if (comps != null)
			// {
				// for (int i = 0; i < comps.Count; i++)
				// {
					// comps[i].PostPostMake();
				// }
			// }
		// }

		public override IEnumerable<DefHyperlink> DescriptionHyperlinks
		{
			get
			{
				if (this?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotype);
				}
				if (this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef);
				}
				if (def.descriptionHyperlinks != null)
				{
					for (int i = 0; i < def.descriptionHyperlinks.Count; i++)
					{
						yield return def.descriptionHyperlinks[i];
					}
				}
			}
		}

		// public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		// {
			// if (this?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null || this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
			// {
				// IEnumerable<Dialog_InfoCard.Hyperlink> enumerable = Dialog_InfoCard.DefsToHyperlinks(GeneDefHyperlinks);
				// yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XenotypeSerumThing_XenotypeLabel".Translate().CapitalizeFirst(), XenotypeLabel().CapitalizeFirst(), "WVC_XenotypeSerumThing_XenotypeDesc".Translate().CapitalizeFirst() + ":", 999, null, enumerable);
			// }
		// }

		// private string XenotypeLabel()
		// {
			// if (this?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
			// {
				// return this.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotype.label;
			// }
			// else if (this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
			// {
				// return this.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef.label;
			// }
			// else
			// {
				// return "ERROR";
			// }
		// }

		// private IEnumerable<DefHyperlink> GeneDefHyperlinks
		// {
			// get
			// {
				// if (this?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
				// {
					// yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotype);
				// }
				// if (this?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
				// {
					// yield return new DefHyperlink(this.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef);
				// }
			// }
		// }
	}

}
