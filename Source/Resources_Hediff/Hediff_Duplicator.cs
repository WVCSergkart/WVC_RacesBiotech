using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_Duplicator : Hediff
	{

		public override bool ShouldRemove => false;
		public override bool Visible => false;
		public virtual int TickRate => 76663;

		public int nextTick = 33;
		public override void TickInterval(int delta)
		{
			//if (!pawn.IsHashIntervalTick(TictRate, delta))
			//{
			//    return;
			//}
			if (!GeneResourceUtility.CanTick(ref nextTick, TickRate, delta))
			{
				return;
			}
			TryRemoveOrUpdate();
		}

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			if (pawn.IsDuplicate)
			{
				pawn.health.RemoveHediff(this);
			}
		}

		protected HediffStage curStage;
		public virtual void Recache()
		{
			curStage = null;
		}

		public virtual void TryRemoveOrUpdate()
		{

		}

		//public override void ExposeData()
		//{
		//    base.ExposeData();
		//    if (Scribe.mode == LoadSaveMode.PostLoadInit)
		//    {
		//        Recache();
		//    }
		//}

	}

	public class Hediff_DuplicatorBandwidth : Hediff_Duplicator
	{

		[Unsaved(false)]
		private Gene_Duplicator_Bandwidth cachedDuplicatorGene;
		public Gene_Duplicator_Bandwidth DuplicatorBandwidth
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator_Bandwidth>();
				}
				return cachedDuplicatorGene;
			}
		}
		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;
		public override int TickRate => 60613;
		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					StatModifier statMod = new();
					statMod.stat = StatDefOf.MechBandwidth;
					statMod.value = savedBandwidth;
					curStage.statOffsets = new();
					curStage.statOffsets.Add(statMod);
				}
				return curStage;
			}
		}

		public override void TryRemoveOrUpdate()
		{
			//Log.Error("0");
			if (DuplicatorBandwidth == null)
			{
				pawn.health.RemoveHediff(this);
			}
			else
			{
				Recache();
			}
		}

		public override void Recache()
		{
			if (DuplicatorBandwidth != null)
			{
				savedBandwidth = DuplicatorBandwidth.TotalBandwidth;
			}
			base.Recache();
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_Duplicator_Bandwidth && gene.Active))
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

		public float savedBandwidth = 0;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedBandwidth, "savedBandwidth", 0);
		}

	}

	public class Hediff_Duplicator_PsychicSensitivity : Hediff_Duplicator
	{

		[Unsaved(false)]
		private Gene_Duplicator_PsychicSensitivity cachedDuplicatorGene;
		public Gene_Duplicator_PsychicSensitivity Gene
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator_PsychicSensitivity>();
				}
				return cachedDuplicatorGene;
			}
		}

		public override bool Visible => false;
		public override int TickRate => 11013;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					if (Gene?.Duplicator != null)
					{
						float offset = 0f;
						foreach (Pawn dupe in Gene.Duplicator.PawnDuplicates)
						{
							if (dupe.psychicEntropy != null)
							{
								offset += dupe.psychicEntropy.PsychicSensitivity * 0.1f;
							}
							else
							{
								offset += dupe.GetStatValue(StatDefOf.PsychicSensitivity) * 0.1f;
							}
						}
						curStage.statOffsets = new();
						StatModifier statMod = new();
						statMod.stat = StatDefOf.PsychicSensitivity;
						statMod.value = offset;
						curStage.statOffsets.Add(statMod);
						StatModifier statFocusGainMod = new();
						statFocusGainMod.stat = StatDefOf.MeditationFocusGain;
						statFocusGainMod.value = offset / 2;
						curStage.statOffsets.Add(statFocusGainMod);
						StatModifier statEntropyRecoveryMod = new();
						statEntropyRecoveryMod.stat = StatDefOf.PsychicEntropyRecoveryRate;
						statEntropyRecoveryMod.value = offset / 2;
						curStage.statOffsets.Add(statEntropyRecoveryMod);
					}
				}
				return curStage;
			}
		}

		public override void TryRemoveOrUpdate()
		{
			if (Gene == null)
			{
				pawn.health.RemoveHediff(this);
			}
			else
			{
				Recache();
			}
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_Duplicator_PsychicSensitivity && gene.Active))
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
