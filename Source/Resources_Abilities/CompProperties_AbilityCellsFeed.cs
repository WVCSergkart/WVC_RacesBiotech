using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityCellsfeederBite : CompProperties_AbilityEffect
	{

		public float daysGain = 10f;

		// public ThoughtDef thoughtDefToGiveTarget;

		// public ThoughtDef opinionThoughtDefToGiveTarget;

		// public float resistanceGain;

		public float nutritionGain = 0.2f;

		public float targetBloodLoss = 0.03f;

		public float cellsConsumeFactor = 0.5f;

		public IntRange bloodFilthToSpawnRange = new(1, 1);

		public CompProperties_AbilityCellsfeederBite()
		{
			compClass = typeof(CompAbilityEffect_CellsfeederBite);
		}

		public override IEnumerable<string> ExtraStatSummary()
		{
			yield return "WVC_XaG_DaysGainFromBite".Translate() + ": " + ((int)(daysGain * 60000)).ToStringTicksToDays();
		}

	}

	public class CompAbilityEffect_CellsfeederBite : CompAbilityEffect
	{

		public new CompProperties_AbilityCellsfeederBite Props => (CompProperties_AbilityCellsfeederBite)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				GeneFeaturesUtility.DoCellsBite(parent.pawn, pawn, Props.daysGain, Props.cellsConsumeFactor, Props.nutritionGain, Props.bloodFilthToSpawnRange, Props.targetBloodLoss);
			}
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (!GeneFeaturesUtility.CanCellsFeedNowWith(parent.pawn, pawn))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_GeneralWrongTarget".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				string text = null;
				if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
				{
					text += "MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY"));
				}
				return text;
			}
			return null;
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			return null;
		}

	}

}
