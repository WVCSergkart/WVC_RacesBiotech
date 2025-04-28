using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Mainframe : Gene, IGeneMetabolism, IGeneOverridden
	{

		//private int? cahcedLimit;
		//public int GenesLimit
		//{
		//	get
		//	{
		//		if (!cahcedLimit.HasValue)
		//		{
		//			cahcedLimit = (int)Mathf.Clamp(DefDatabase<GeneDef>.AllDefsListForReading.Where((geneDef) => geneDef.IsGeneDefOfType<Gene_MainframeDependant>()).ToList().Count * 0.33f, 3, 14);
		//		}
		//		return cahcedLimit.Value;
		//	}
		//}

		//public int CurrentGenes => pawn.genes.GenesListForReading.Where((gene) => gene.def.IsGeneDefOfType<Gene_MainframeDependant>()).ToList().Count;

		private bool? metHediifDisabled;
		private HediffDef metHediffDef;
		public HediffDef MetHediffDef
		{
			get
			{
				if (!metHediifDisabled.HasValue)
				{
					metHediifDisabled = (metHediffDef = def?.GetModExtension<GeneExtension_Giver>()?.metHediffDef) != null;
				}
				if (!metHediifDisabled.Value)
				{
					return null;
				}
				return metHediffDef;
			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			HediffUtility.TryRemoveHediff(MetHediffDef, pawn);
		}

		public void Notify_Override()
        {
			UpdateMetabolism();
		}

		public override void PostAdd()
		{
			base.PostAdd();
			UpdateMetabolism();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(MetHediffDef, pawn);
		}

		public void UpdateMetabolism()
		{
			HediffUtility.TryAddOrUpdMetabolism(MetHediffDef, pawn, this);
		}

		private int resource = 0;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref resource, "resource", 0);
		}

	}

}
