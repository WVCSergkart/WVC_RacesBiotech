using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_PostShapeshift_Recovery : Gene, IGeneShapeshift
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			if (Props == null)
			{
				return;
			}
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffDefs);
		}

	}

	public class Gene_PostShapeshift_Regeneration : Gene, IGeneShapeshift
	{

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				// HealingUtility.TryHealRandomPermanentWound(pawn, this, true, true);
				HealthUtility.FixWorstHealthCondition(pawn, HediffDefOf.Scarification);
			}
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
		}

	}

	public class Gene_PostShapeshift_GiveHediff : Gene, IGeneShapeshift
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Hediff hediff = HediffMaker.MakeHediff(Props.hediffDefName, pawn);
			HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = Props.intervalRange.RandomInRange;
			}
			pawn.health.AddHediff(hediff);
		}

	}

}
