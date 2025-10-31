using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{
	public class Hediff_PsychicSensitivity : Hediff
	{

		public int cachedGenes = 0;
		public int nextTick = 114;

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
		private Gene_Overrider cachedGene;

		public Gene_Overrider Overrider
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Overrider>();
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
					foreach (Gene item in pawn.genes.GenesListForReading)
					{
						if (item is Gene_OverriderDependant overrided && overrided.Active)
						{
							cachedGenes++;
						}
					}
					curStage.statOffsets = new();
					StatModifier newStatMod = new();
					newStatMod.value = 0;
					newStatMod.stat = StatDefOf.PsychicSensitivity;
					if (cachedGenes > 0)
					{
						float offset = 0;
						float genesFactor = 1f / Gene_Overrider.SubGenesCount;
						if (Overrider.addPsychicSensitivity)
						{
							offset = cachedGenes * (genesFactor * 1.5f);
						}
						else
						{
							offset = -1f * cachedGenes * (genesFactor * 3f);
						}
						newStatMod.value = offset;
					}
					curStage.statOffsets.Add(newStatMod);
					// curStage.hungerRateFactor = newDef.curve.Evaluate(cachedMetabolism);
				}
				return curStage;
			}
		}

		public void Reset()
		{
			cachedGenes = 0;
			curStage = null;
			if (Overrider == null)
			{
				pawn.health.RemoveHediff(this);
			}
		}

		public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 60189, delta))
			{
				return;
			}
			Reset();
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_Overrider && gene.Active))
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
