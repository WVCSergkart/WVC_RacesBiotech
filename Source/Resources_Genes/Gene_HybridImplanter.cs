using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_HybridImplanter : Gene
	{

		private bool? cachedInheritability;
		public bool IsEndogene
		{
			get
			{
				if (!cachedInheritability.HasValue)
				{
					cachedInheritability = !pawn.genes.IsXenogene(this);
				}
				return cachedInheritability.Value;
			}
		}

		//public XenotypeDef firstXenotypeDef;
		//public XenotypeDef secondXenotypeDef;

		//public void SetXenotypes(XenotypeDef firstXenos, XenotypeDef secondXenos)
		//{
		//    firstXenotypeDef = firstXenos;
		//    secondXenotypeDef = secondXenos;
		//}

		//public override void ExposeData()
		//{
		//    base.ExposeData();
		//    Scribe_Defs.Look(ref firstXenotypeDef, "firstXenotypeDef");
		//    Scribe_Defs.Look(ref secondXenotypeDef, "secondXenotypeDef");
		//}

	}

	public class Gene_MergeImplanter : Gene_HybridImplanter
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameNotStarted())
			{
				SubXenotypeUtility.XenotypeShapeShift(pawn, null);
			}
		}

	}

}
