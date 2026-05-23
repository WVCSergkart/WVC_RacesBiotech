using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_Overrider : CompAbilityEffect
	{

		[Unsaved(false)]
		private Gene_SelfOverrider_Implanter cachedGene;
		public Gene_SelfOverrider_Implanter Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = parent.pawn?.genes?.GetFirstGeneOfType<Gene_SelfOverrider_Implanter>();
				}
				return cachedGene;
			}
		}

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn source = target.Pawn;
			Pawn caster = parent.pawn;
			if (DuplicateUtility.TryDuplicatePawn(caster, caster, source.PositionHeld, caster.Map, out Pawn duplicatePawn, out _, out _, false, addDuplicate: true))
			{
				//Ability ability = duplicatePawn.abilities?.GetAbility(parent.def);
				//if (ability?.CanCooldown == true)
				//{
				//	ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				//}
				Gene.StartCooldown();
				duplicatePawn.genes?.GetFirstGeneOfType<Gene_SelfOverrider_Implanter>()?.StartCooldown();
				Find.LetterStack.ReceiveLetter("WVC_XaG_OverridedImplant".Translate(), "WVC_XaG_OverridedImplant_Desc".Translate(caster, source), LetterDefOf.NeutralEvent, duplicatePawn);
			}
			ReimplanterUtility.SetXenotype(source, XenotypeDefOf.Baseliner);
			CompAbilityEffect_Devourer.KillTarget(source, caster);
		}

		//public override bool CanCast
		//{
		//	get
		//	{
		//		if (base.CanCast)
		//		{
		//			return Gene?.CanCast == true;
		//		}
		//		return false;
		//	}
		//}

		public override bool GizmoDisabled(out string reason)
		{
			if (Gene == null)
			{
				reason = "WVC_XaG_OverridedImplant_NotActive".Translate();
				return true;
			}
			if (!Gene.CanCast)
			{
				reason = "WVC_Cooldown".Translate(Gene.cooldownTicks.ToStringTicksToDays());
				return true;
			}
			return base.GizmoDisabled(out reason);
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
