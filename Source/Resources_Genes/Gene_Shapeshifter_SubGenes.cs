using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ShapeshifterDependant : Gene, IGeneShapeshift
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

		public override void Tick()
		{

		}

        public virtual void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
        {
            
        }

        public virtual void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
        {
            
        }

        // public override void PostRemove()
        // {
        // base.PostRemove();
        // HediffUtility.Notify_GeneRemoved(this, pawn);
        // }

    }

	public class Gene_PostShapeshift_Recovery : Gene_ShapeshifterDependant
	{

		private bool? savedBool;

		public override void PostAdd()
        {
            base.PostAdd();
			if (Shapeshifter != null)
			{
				savedBool = Shapeshifter.genesRegrowAfterShapeshift;
				Shapeshifter.genesRegrowAfterShapeshift = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (Shapeshifter != null && savedBool.HasValue)
			{
				Shapeshifter.genesRegrowAfterShapeshift = savedBool.Value;
			}
		}
		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			if (savedBool.HasValue)
			{
				shapeshiftGene.genesRegrowAfterShapeshift = savedBool.Value;
			}
		}

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			//shapeshiftGene.genesRegrowAfterShapeshift = savedBool;
			if (Giver == null)
			{
				return;
			}
			savedBool = newShapeshiftGene.genesRegrowAfterShapeshift;
			newShapeshiftGene.genesRegrowAfterShapeshift = false;
			HediffUtility.RemoveHediffsFromList(pawn, Giver.hediffDefs);
		}

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref savedBool, "savedGenesRegrowStatus");
		}

    }

	public class Gene_PostShapeshift_Regeneration : Gene_ShapeshifterDependant
	{

		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			RemoveAllRemovableBadHediffs(pawn);
		}

		public static void RemoveAllRemovableBadHediffs(Pawn pawn)
		{
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
			{
				if (!hediff.def.isBad || !hediff.def.everCurableByItem)
				{
					continue;
				}
				pawn.health.RemoveHediff(hediff);
			}
		}

	}

	public class Gene_PostShapeshift_GiveHediff : Gene_ShapeshifterDependant
	{

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
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

	public class Gene_PostShapeshift_Scarred : Gene_ShapeshifterDependant
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (Current.ProgramState != ProgramState.Playing)
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
			int currentTry = 0;
			while (pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification) < 5)
			{
				Gene_Scarifier.Scarify(pawn);
				currentTry++;
				if (currentTry > 8)
				{
					break;
				}
			}
		}

		public override void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			if (!ModLister.CheckIdeology("Scarification"))
			{
				return;
			}
			// List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			// for (int num = 0; num < hediffs.Count; num++)
			// {
				// if (hediffs[num].def != HediffDefOf.Scarification)
				// {
					// continue;
				// }
				// pawn.health.RemoveHediff(hediffs[num]);
			// }
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
			{
				if (hediff.def != HediffDefOf.Scarification)
				{
					continue;
				}
				pawn.health.RemoveHediff(hediff);
			}
		}

		public override void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		{
			Scarify();
		}

	}

}
