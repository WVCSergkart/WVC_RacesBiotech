using Verse;


namespace WVC_XenotypesAndGenes
{
    public class Hediff_FleshmassImmunity : Hediff
	{

		//public int cachedGenes = 0;
		public int nextTick = 182;

		private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;

		//public override string LabelInBrackets
		//{
		//	get
		//	{
		//		return (cachedGenes > 0 ? "+" : "") + cachedGenes.ToString();
		//	}
		//}

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			Reset();
		}

		[Unsaved(false)]
		private Gene_FleshmassImmunity cachedGene;

		public Gene_FleshmassImmunity Immunity
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_FleshmassImmunity>();
				}
				return cachedGene;
			}
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
                    curStage.makeImmuneTo = Immunity?.ImmunizedHediffs;
                }
                return curStage;
			}
		}

		public void Reset()
		{
			curStage = null;
			if (Immunity == null)
			{
				pawn.health.RemoveHediff(this);
			}
		}

		public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 114568, delta))
			{
				return;
			}
			Reset();
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_FleshmassImmunity && gene.Active))
			{
				if (HediffUtility.TryAddHediff(def, pawn, null, null))
				{
					if (DebugSettings.ShowDevGizmos)
					{
						Log.Warning("Trying to remove " + def.label + " hediff, but " + pawn.Name.ToString() + " has the required gene. Hediff is added back.");
					}
				}
			}
		}

	}
}
