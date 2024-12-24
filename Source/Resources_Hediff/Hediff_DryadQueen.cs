using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_DryadQueen : HediffWithComps
	{

		//public int? cachedDryadsCount;
		public int refreshInterval = 41982;

		private HediffStage curStage;

		public override bool ShouldRemove => Gauranlen == null;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_DryadQueen cachedDryadsQueenGene;

		public Gene_DryadQueen Gauranlen
		{
			get
			{
				if (cachedDryadsQueenGene == null || !cachedDryadsQueenGene.Active)
				{
					cachedDryadsQueenGene = pawn?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedDryadsQueenGene;
			}
		}

		//public override void PostAdd(DamageInfo? dinfo)
		//{
		//	base.PostAdd(dinfo);
		//	Recache();
		//}

		public override HediffStage CurStage
		{
			get
			{
				//if (def is XaG_HediffDef newDef && newDef.statModifiers != null && curStage == null && cachedDryadsCount > 0)
				//{
				//	curStage = new HediffStage
				//	{
				//		statOffsets = new(),
				//		statFactors = new()
				//	};
				//	if (!newDef.statModifiers.statFactors.NullOrEmpty())
				//	{
				//		foreach (StatModifier item in newDef.statModifiers.statFactors)
				//		{
				//			StatModifier statModifier = new();
				//			statModifier.stat = item.stat;
				//			statModifier.value = 1f - (item.value * cachedDryadsCount);
				//			curStage.statFactors.Add(statModifier);
				//		}
				//	}
				//	if (!newDef.statModifiers.statOffsets.NullOrEmpty())
				//	{
				//		foreach (StatModifier item in newDef.statModifiers.statOffsets)
				//		{
				//			StatModifier statModifier = new();
				//			statModifier.stat = item.stat;
				//			statModifier.value = item.value * cachedDryadsCount;
				//			curStage.statOffsets.Add(statModifier);
				//		}
				//	}
				//}
				if (curStage == null)
				{
					curStage = new();
					if (Gauranlen != null && Gauranlen.DryadsListForReading.Count > 0)
					{
						StatModifier statModifier = new()
						{
							stat = StatDefOf.MaxNutrition,
							value = 0.1f * Gauranlen.DryadsListForReading.Count
						};
						curStage.statOffsets = new List<StatModifier> { statModifier };
						curStage.hungerRateFactor = 1f + (0.1f * Gauranlen.DryadsListForReading.Count);
					}
				}
				return curStage;
			}
		}

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(refreshInterval))
			{
				return;
			}
			Recache();
		}

		public void Recache()
		{
			if (Gauranlen == null)
			{
				pawn?.health?.RemoveHediff(this);
			}
			curStage = null;
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref cachedDryadsCount, "cachedDryadsCount", 0);
		//}

	}

	// public class HediffWithComps_Scarshield : HediffWithComps_Scars
	// {

		// public override HediffStage CurStage
		// {
			// get
			// {
				// if (curStage == null && cachedDryadsCount > 0)
				// {
					// StatModifier statModifier = new();
					// statModifier.stat = StatDefOf.IncomingDamageFactor;
					// statModifier.value = 1f - (0.15f * cachedDryadsCount);
					// curStage = new HediffStage
					// {
						// statFactors = new List<StatModifier> { statModifier }
					// };
				// }
				// return curStage;
			// }
		// }

	// }

	// public class HediffWithComps_Scarstomach : HediffWithComps_Scars
	// {

		// public override HediffStage CurStage
		// {
			// get
			// {
				// if (curStage == null && cachedDryadsCount > 0)
				// {
					// StatModifier statModifier = new();
					// statModifier.stat = StatDefOf.MaxNutrition;
					// statModifier.value = 0.2f * cachedDryadsCount;
					// curStage = new HediffStage
					// {
						// statOffsets = new List<StatModifier> { statModifier }
					// };
				// }
				// return curStage;
			// }
		// }

	// }

}
