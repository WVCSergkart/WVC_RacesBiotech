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
			string letterDesc = "WVC_XaG_GeneDuplicationLetter".Translate(parent.pawn.Named("CASTER"), pawn.Named("PAWN"));
			if (DuplicateUtility.TryDuplicatePawn(pawn, spawnCell, pawn.Map, out Pawn duplicatePawn, ref letterDesc, Rand.Chance(WVC_Biotech.settings.duplicator_RandomOutcomeChance)))
            {
				Ability ability = duplicatePawn.abilities.GetAbility(parent.def);
				if (ability.CanCooldown)
				{
					ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				}
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneDuplicationLetterLabel".Translate(), letterDesc, LetterDefOf.NeutralEvent, duplicatePawn);
				if (duplicatePawn.Faction == Faction.OfPlayer)
				{
					Messages.Message("WVC_XaG_GeneDuplicationSuccessMessage".Translate(parent.pawn.Named("PAWN")), pawn, MessageTypeDefOf.NeutralEvent);
				}
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
				return base.Valid(target, throwMessages);
			}
			if (!pawn.IsHuman())
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
