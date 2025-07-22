using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// =======================================

	// =======================================

	// =======================================

	public class CompProperties_AbilityPawnPregnant : CompProperties_AbilityEffect
	{

		public int recacheFrequency = 2320;

		public CompProperties_AbilityPawnPregnant()
		{
			compClass = typeof(CompAbilityEffect_PawnPregnant);
		}

	}

	public class CompAbilityEffect_PawnPregnant : CompAbilityEffect
	{

		public new CompProperties_AbilityPawnPregnant Props => (CompProperties_AbilityPawnPregnant)props;

		private int nextRecache = -1;
		private string cachedReason = null;
		private bool cachedResult = false;

		private void Cache(string reason)
		{
			cachedReason = reason;
			cachedResult = reason != null;
		}

		public override bool GizmoDisabled(out string reason)
		{
			if (Find.TickManager.TicksGame < nextRecache)
			{
				reason = cachedReason;
				return cachedResult;
			}
			nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
			Pawn pawn = parent.pawn;
			Hediff pregnancy = HediffUtility.GetFirstHediffPreventsPregnancy(pawn.health.hediffSet.hediffs);
			if (pregnancy != null)
			{
				reason = "WVC_XaG_GeneXenoGestator_Disabled".Translate(pregnancy.def.label);
				Cache(reason);
				return true;
			}
			reason = null;
			Cache(reason);
			return false;
		}

	}

	// =======================================

	public class CompProperties_AbilityTargetValidation : CompProperties_AbilityEffect
	{

		public bool humanityCheck = false;

		[Obsolete]
		public bool serumsCheck = false;

		[Obsolete]
		public bool reimplanterDelayCheck = true;

		public CompProperties_AbilityTargetValidation()
		{
			compClass = typeof(CompAbilityEffect_TargetValidation);
		}

	}

	public class CompAbilityEffect_TargetValidation : CompAbilityEffect
	{

		public new CompProperties_AbilityTargetValidation Props => (CompProperties_AbilityTargetValidation)props;

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (Props.humanityCheck && !ReimplanterUtility.IsHuman(pawn))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

	}

	// =======================================

	public class CompProperties_AbilityHideIfPawnMultiSelected : CompProperties_AbilityEffect
	{

		public CompProperties_AbilityHideIfPawnMultiSelected()
		{
			compClass = typeof(CompAbilityEffect_HideIfPawnMultiSelected);
		}

	}

	public class CompAbilityEffect_HideIfPawnMultiSelected : CompAbilityEffect
	{

		public override bool ShouldHideGizmo => Find.Selector.SelectedPawns.Count > 1;

	}

	public class CompAbilityEffect_HideIfBloodeater : CompAbilityEffect
	{

		public bool shouldHide = false;

        public override bool ShouldHideGizmo
        {
            get
            {
                return shouldHide;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref shouldHide, "shouldHide", false);
		}

    }

	// =======================================

	public class CompProperties_AbilityGeneIsActive : CompProperties_AbilityEffect
	{

		public List<GeneDef> anyOfGenes;

		public List<GeneDef> eachOfGenes;

		public Gender gender = Gender.None;

		public int recacheFrequency = 5420;

		public CompProperties_AbilityGeneIsActive()
		{
			compClass = typeof(CompAbilityEffect_GeneIsActive);
		}

	}

	public class CompAbilityEffect_GeneIsActive : CompAbilityEffect
	{

		public new CompProperties_AbilityGeneIsActive Props => (CompProperties_AbilityGeneIsActive)props;

		private int nextRecache = -1;
		private string cachedReason = null;
		private bool cachedResult = false;

		// This should improve optimization at least a little.
		private void Cache(string reason)
		{
			cachedReason = reason;
			cachedResult = reason != null;
		}

		public override bool GizmoDisabled(out string reason)
		{
			if (Find.TickManager.TicksGame < nextRecache)
			{
				reason = cachedReason;
				return cachedResult;
			}
			nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
			Pawn pawn = parent.pawn;
			if (Props.gender != Gender.None)
			{
				if (Props.gender != pawn.gender)
				{
					reason = "WVC_XaG_AbilityGeneIsActive_PawnWrongGender".Translate(pawn);
					Cache(reason);
					return true;
				}
			}
			if (pawn?.genes == null)
			{
				reason = "WVC_XaG_AbilityGeneIsActive_PawnBaseliner".Translate(pawn);
				Cache(reason);
				return true;
			}
			if (!Props.eachOfGenes.NullOrEmpty())
			{
				foreach (GeneDef allSelectedItem in Props.eachOfGenes)
				{
					if (!XaG_GeneUtility.HasActiveGene(allSelectedItem, pawn))
					{
						reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn) + ": " + "\n" + Props.eachOfGenes.Select((GeneDef x) => x.label).ToLineList(" - ", capitalizeItems: true);
						Cache(reason);
						return true;
					}
				}
			}
			if (!Props.anyOfGenes.NullOrEmpty())
			{
				foreach (GeneDef allSelectedItem in Props.anyOfGenes)
				{
					if (XaG_GeneUtility.HasActiveGene(allSelectedItem, pawn))
					{
						reason = null;
						Cache(reason);
						return false;
					}
				}
				reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn) + ": " + "\n" + Props.anyOfGenes.Select((GeneDef x) => x.label).ToLineList(" - ", capitalizeItems: true);
				Cache(reason);
				return true;
			}
			reason = null;
			Cache(reason);
			return false;
		}

	}

}
