using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ShapeshifterDependant : Gene
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		[Unsaved(false)]
		private Gene_Shapeshifter cachedShapeshifterGene;

		public Gene_Shapeshifter Shapeshifter
		{
			get
			{
				if (cachedShapeshifterGene == null || !cachedShapeshifterGene.Active)
				{
					cachedShapeshifterGene = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
				}
				return cachedShapeshifterGene;
			}
		}

	}

	public class Gene_PostShapeshift_Recovery : Gene_ShapeshifterDependant, IGeneShapeshift
	{

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			if (Giver == null)
			{
				return;
			}
			HediffUtility.RemoveHediffsFromList(pawn, Giver.hediffDefs);
		}

	}

	public class Gene_PostShapeshift_Regeneration : Gene_ShapeshifterDependant, IGeneShapeshift
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

	public class Gene_PostShapeshift_GiveHediff : Gene_ShapeshifterDependant, IGeneShapeshift
	{

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Hediff hediff = HediffMaker.MakeHediff(Giver.hediffDefName, pawn);
			HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = Giver.intervalRange.RandomInRange;
			}
			pawn.health.AddHediff(hediff);
		}

	}

	public class Gene_Shapeshift_TrueForm : Gene_ShapeshifterDependant
	{



	}

	public class Gene_PostShapeshift_Scarred : Gene_ShapeshifterDependant, IGeneShapeshift
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.Spawned)
			{
				Scarify();
			}
		}

		public void Scarify()
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			while (pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification) < 5)
			{
				Gene_Scarifier.Scarify(pawn);
			}
		}

		public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (hediffs[num].def != HediffDefOf.Scarification)
				{
					continue;
				}
				pawn.health.RemoveHediff(hediffs[num]);
			}
		}

		public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Scarify();
		}

	}

}
