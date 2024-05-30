using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityCorpseEater : CompProperties_AbilityEffect
	{

		// Basic meal
		// public float nutritionGain = 0.8f;

		public List<RotStage> acceptableRotStages = new() { RotStage.Fresh, RotStage.Rotting };

		public CompProperties_AbilityCorpseEater()
		{
			compClass = typeof(CompAbilityEffect_CorpseEater);
		}

	}

	public class CompAbilityEffect_CorpseEater : CompAbilityEffect
	{

		public new CompProperties_AbilityCorpseEater Props => (CompProperties_AbilityCorpseEater)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				Need_Food need_Food = parent.pawn.needs?.food;
				if (need_Food == null)
				{
					return;
				}
				float nutritionGain = 0;
				float neededNutrition = need_Food.MaxLevel - need_Food.CurLevel;
				int partCount = corpse.InnerPawn.health.hediffSet.GetNotMissingParts().ToList().Count;
				for (int j = 0; j < partCount; j++)
				{
					IngestedCalculateAmounts(corpse, parent.pawn, neededNutrition, out float nutritionIngested);
					nutritionGain += nutritionIngested;
					neededNutrition -= nutritionGain;
					if (neededNutrition <= 0f)
					{
						break;
					}
				}
				// IngestedCalculateAmounts(corpse, parent.pawn, neededNutrition, out float nutritionIngested);
				need_Food.CurLevel += nutritionGain;
			}
		}

		public static void IngestedCalculateAmounts(Corpse corpse, Pawn ingester, float nutritionWanted, out float nutritionIngested)
		{
			BodyPartRecord bodyPartRecord = corpse.GetBestBodyPartToEat(nutritionWanted);
			if (bodyPartRecord == null)
			{
				bodyPartRecord = GetBestBodyPartToEat_Rotting(nutritionWanted, corpse);
				if (bodyPartRecord == null)
				{
					// Log.Error(string.Concat(ingester, " ate ", corpse, " but no body part was found. Replacing with core part."));
					bodyPartRecord = corpse.InnerPawn.RaceProps.body.corePart;
				}
			}
			float bodyPartNutrition = FoodUtility.GetBodyPartNutrition(corpse, bodyPartRecord);
			if (bodyPartRecord == corpse.InnerPawn.RaceProps.body.corePart)
			{
				if (ingester != null && PawnUtility.ShouldSendNotificationAbout(corpse.InnerPawn) && corpse.InnerPawn.RaceProps.Humanlike)
				{
					Messages.Message("MessageEatenByPredator".Translate(corpse.InnerPawn.LabelShort, ingester.Named("PREDATOR"), corpse.InnerPawn.Named("EATEN")).CapitalizeFirst(), ingester, MessageTypeDefOf.NegativeEvent);
				}
			}
			else
			{
				Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, bodyPartRecord);
				if (ingester != null)
				{
					hediff_MissingPart.lastInjury = HediffDefOf.Bite;
				}
				hediff_MissingPart.IsFresh = true;
				corpse.InnerPawn.health.AddHediff(hediff_MissingPart);
			}
			nutritionIngested = bodyPartNutrition;
		}

		public static BodyPartRecord GetBestBodyPartToEat_Rotting(float nutritionWanted, Corpse corpse)
		{
			IEnumerable<BodyPartRecord> source = from x in corpse.InnerPawn.health.hediffSet.GetNotMissingParts()
				where x.depth == BodyPartDepth.Outside
				select x;
			if (!source.Any())
			{
				return null;
			}
			return source.MinBy((BodyPartRecord x) => Mathf.Abs(0.2f - nutritionWanted));
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				if (!Props.acceptableRotStages.Contains(corpse.GetRotStage()))
				{
					if (throwMessages)
					{
						Messages.Message("WVC_XaG_MessageWrongRottingStage".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				return parent.pawn.needs?.food != null;
			}
			return false;
		}

	}

}
