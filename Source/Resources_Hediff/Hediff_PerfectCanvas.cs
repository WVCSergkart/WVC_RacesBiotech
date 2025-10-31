using System.Collections.Generic;
using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_PerfectCanvas : Hediff
	{

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		public int? savedImplantsCount;

		private HediffStage curStage;
		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					GetImplantCount();
					curStage.statOffsets = new();
					curStage.statFactors = new();
					//// MoveSpeed
					//StatModifier newStat1 = new();
					//newStat1.stat = StatDefOf.MoveSpeed;
					//newStat1.value = 0.01f * savedImplantsCount.Value;
					//curStage.statOffsets.Add(newStat1);
					//// MoveSpeed
					//StatModifier newStat2 = new();
					//newStat1.stat = StatDefOf.MoveSpeed;
					//newStat1.value = 0.01f * savedImplantsCount.Value;
					//curStage.statOffsets.Add(newStat2);
					//// MoveSpeed
					//StatModifier newStat3 = new();
					//newStat1.stat = StatDefOf.MoveSpeed;
					//newStat1.value = 0.01f * savedImplantsCount.Value;
					//curStage.statOffsets.Add(newStat3);
					//// MoveSpeed
					//StatModifier newStat4 = new();
					//newStat1.stat = StatDefOf.MoveSpeed;
					//newStat1.value = 0.01f * savedImplantsCount.Value;
					//curStage.statOffsets.Add(newStat4);
					if (savedImplantsCount != null && def is XaG_HediffDef newDef && newDef.statModifiers != null)
					{
						if (!newDef.statModifiers.statFactors.NullOrEmpty())
						{
							foreach (StatModifier item in newDef.statModifiers.statFactors)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								float factor = 1f + (item.value * savedImplantsCount.Value);
								statModifier.value = factor > 0f ? factor : 0f;
								curStage.statFactors.Add(statModifier);
							}
						}
						if (!newDef.statModifiers.statOffsets.NullOrEmpty())
						{
							foreach (StatModifier item in newDef.statModifiers.statOffsets)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								statModifier.value = item.value * savedImplantsCount.Value;
								curStage.statOffsets.Add(statModifier);
							}
						}
					}
				}
				return curStage;
			}
		}

		[Unsaved(false)]
		private Gene_PerfectCanvas cachedGene;
		public Gene_PerfectCanvas Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_PerfectCanvas>();
				}
				return cachedGene;
			}
		}

		public void UpdCount()
		{
			curStage = null;
			savedImplantsCount = null;
			if (Gene == null)
			{
				pawn.health.RemoveHediff(this);
			}
		}

		public override bool TryMergeWith(Hediff other)
		{
			if (other?.def?.addedPartProps?.solid == true)
			{
				UpdCount();
			}
			return base.TryMergeWith(other);
		}

		private void GetImplantCount()
		{
			if (savedImplantsCount.HasValue)
			{
				return;
			}
			List<HediffDef> pawnImplants = pawn.health?.hediffSet?.hediffs?.ConvertToDef();
			if (pawnImplants.NullOrEmpty())
			{
				savedImplantsCount = 0;
				return;
			}
			savedImplantsCount = 0;
			foreach (HediffDef hediffDef in pawnImplants)
			{
				if (hediffDef.countsAsAddedPartOrImplant && hediffDef.addedPartProps?.solid == true)
				{
					savedImplantsCount++;
				}
			}
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_PerfectCanvas && gene.Active))
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref savedImplantsCount, "savedImplantsCount", defaultValue: 0);
		}

	}

}
