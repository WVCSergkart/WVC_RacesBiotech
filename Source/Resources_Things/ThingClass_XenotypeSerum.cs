using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeSerum : ThingWithComps
	{

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
				if (this?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>() != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().endotype);
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().xenotype);
				}
				if (this?.TryGetComp<CompUseEffect_GeneGiver>()?.geneDef != null)
				{
					yield return new DefHyperlink(this.TryGetComp<CompUseEffect_GeneGiver>().geneDef);
				}
				// if (this?.TryGetComp<CompUseEffect_GeneRestoration>()?.Props?.hediffsToRemove != null)
				// {
					// foreach (HediffDef item in this.TryGetComp<CompUseEffect_GeneRestoration>().Props.hediffsToRemove)
					// {
						// yield return new DefHyperlink(item);
					// }
				// }
				if (def.descriptionHyperlinks != null)
				{
					for (int i = 0; i < def.descriptionHyperlinks.Count; i++)
					{
						yield return def.descriptionHyperlinks[i];
					}
				}
			}
		}

	}

}
