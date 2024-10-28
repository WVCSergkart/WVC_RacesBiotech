using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_AbilityBloodeater : CompProperties_AbilityEffect
	{

		public CompProperties_AbilityBloodeater()
		{
			compClass = typeof(CompAbilityEffect_Bloodeater);
		}

	}

	[Obsolete]
	public class CompAbilityEffect_Bloodeater : CompAbilityEffect
	{

		public new CompProperties_AbilityBloodeater Props => (CompProperties_AbilityBloodeater)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Pawn pawn = parent.pawn;
			if (pawn?.genes == null)
			{
				return;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is IGeneBloodfeeder geneBloodfeeder && gene.Active)
				{
					geneBloodfeeder.Notify_Bloodfeed(target.Pawn);
				}
			}
		}

	}

	[Obsolete]
	public class CompAbilityEffect_Cellseater : CompAbilityEffect
	{

		public new CompProperties_AbilityBloodeater Props => (CompProperties_AbilityBloodeater)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Pawn pawn = parent.pawn;
			if (pawn?.genes == null)
			{
				return;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is IGeneCellsfeeder geneBloodfeeder && gene.Active)
				{
					geneBloodfeeder.Notify_Cellsfeed(target.Pawn);
				}
			}
		}

	}

}
