using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_Duplicator : CompAbilityEffect
	{

		[Unsaved(false)]
		private Gene_Duplicator cachedDuplicatorGene;
		public Gene_Duplicator Duplicator
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = parent.pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator>();
				}
				return cachedDuplicatorGene;
			}
		}

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn source = target.Pawn;
			if (!CellFinder.TryFindRandomCellNear(source.Position, source.Map, Mathf.FloorToInt(4.9f), IsValidSpawnCell, out var spawnCell, 100))
			{
				return;
			}
			Pawn caster = parent.pawn;
			Duplicator.Notify_GenesChanged(null);
            float failChanceFactor = Duplicator.StatDef != null ? caster.GetStatValue(Duplicator.StatDef) : 1f;
			//Log.Error("");
			if (DuplicateUtility.TryDuplicatePawn(caster, source, spawnCell, source.Map, out Pawn duplicatePawn, out string letterDesc, out LetterDef letterType, Rand.Chance(failChanceFactor * WVC_Biotech.settings.duplicator_RandomOutcomeChance), addDuplicate: true))
			{
				Ability ability = duplicatePawn.abilities?.GetAbility(parent.def);
				if (ability?.CanCooldown == true)
				{
					ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				}
				if (PawnUtility.ShouldSendNotificationAbout(duplicatePawn))
				{
					Messages.Message("WVC_XaG_GeneDuplicationSuccessMessage".Translate(caster.Named("PAWN")), source, MessageTypeDefOf.NeutralEvent);
				}
				if (ModsConfig.AnomalyActive)
				{
					Gene_Duplicator sourceDuplicator = source.genes?.GetFirstGeneOfType<Gene_Duplicator>();
					if (sourceDuplicator != null)
					{
						sourceDuplicator.Notify_DuplicateCreated(duplicatePawn);
						if (Rand.Chance(WVC_Biotech.settings.duplicator_RandomGeneChance * failChanceFactor))
						{
							sourceDuplicator.TryAddNewSubGene(source.IsDuplicate);
						}
					}
				}
				//Duplicator.Notify_GenesChanged(null);
				//source.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.Notify_DuplicateCreated(duplicatePawn);
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneDuplicationLetterLabel".Translate(), letterDesc, letterType, duplicatePawn);
			}

            bool IsValidSpawnCell(IntVec3 pos)
			{
				if (pos.Standable(source.Map) && pos.Walkable(source.Map))
				{
					return !pos.Fogged(source.Map);
				}
				return false;
			}
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_MechResurrectWarmupOnTarget, new Vector3(0f, 0f, 0.3f));
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (!pawn.IsHuman() || Duplicator == null)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return base.Valid(target, throwMessages);
		}

	}
}
