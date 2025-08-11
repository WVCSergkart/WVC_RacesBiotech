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
			Pawn pawn = target.Pawn;
			int squareRadius = Mathf.FloorToInt(4.9f);
			if (!CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map, squareRadius, IsValidSpawnCell, out var spawnCell, 100))
			{
				return;
			}
			float failChanceFactor = Duplicator.StatDef != null ? parent.pawn.GetStatValue(Duplicator.StatDef) : 1f;
			//Log.Error("");
			if (DuplicateUtility.TryDuplicatePawn(parent.pawn, pawn, spawnCell, pawn.Map, out Pawn duplicatePawn, out string letterDesc, out LetterDef letterType, Rand.Chance(failChanceFactor * WVC_Biotech.settings.duplicator_RandomOutcomeChance)))
            {
				Ability ability = duplicatePawn.abilities?.GetAbility(parent.def);
				if (ability?.CanCooldown == true)
				{
					ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				}
				if (PawnUtility.ShouldSendNotificationAbout(duplicatePawn))
				{
					Messages.Message("WVC_XaG_GeneDuplicationSuccessMessage".Translate(parent.pawn.Named("PAWN")), pawn, MessageTypeDefOf.NeutralEvent);
				}
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneDuplicationLetterLabel".Translate(), letterDesc, letterType, duplicatePawn);
            }
            bool IsValidSpawnCell(IntVec3 pos)
			{
				if (pos.Standable(pawn.Map) && pos.Walkable(pawn.Map))
				{
					return !pos.Fogged(pawn.Map);
				}
				return false;
			}
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
