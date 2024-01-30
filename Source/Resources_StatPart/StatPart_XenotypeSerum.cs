// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_XenotypeSerum : StatPart
	{
		public override void TransformValue(StatRequest req, ref float val)
		{
			if (ModsConfig.BiotechActive && req.HasThing && req.Thing is XenotypeSerum xenotypeSerum)
			{
				float num = 0f;
				if (xenotypeSerum?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
				{
					XenotypeDef xenotypeDef = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotype;
					num = XaG_GeneUtility.XenotypeCost(xenotypeDef);
				}
				if (xenotypeSerum?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
				{
					XenotypeDef xenotypeDef = xenotypeSerum.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef;
					num = XaG_GeneUtility.XenotypeCost(xenotypeDef);
				}
				if (xenotypeSerum?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>() != null)
				{
					XenotypeDef xenotypeA = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().endotype;
					XenotypeDef xenotypeB = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().xenotype;
					num = XaG_GeneUtility.XenotypeCost(xenotypeA) + XaG_GeneUtility.XenotypeCost(xenotypeB);
				}
				val += (100 * num);
			}
			// Log.Error("StatPart_XenotypeSerum call");
		}

		public override string ExplanationPart(StatRequest req)
		{
			return null;
		}
	}

}
