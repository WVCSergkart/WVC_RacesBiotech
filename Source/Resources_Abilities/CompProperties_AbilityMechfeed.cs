using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityMechfeederBite : CompProperties_AbilityEffect
	{

		public HediffDef casterHediffDef;

		public HediffDef mechHediffDef;

		public CompProperties_AbilityMechfeederBite()
		{
			compClass = typeof(CompAbilityEffect_MechfeederBite);
		}

	}

	public class CompAbilityEffect_MechfeederBite : CompAbilityEffect
	{

		public new CompProperties_AbilityMechfeederBite Props => (CompProperties_AbilityMechfeederBite)props;

		public override bool ShouldHideGizmo => parent.pawn.mechanitor == null;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				parent.pawn.health.AddHediff(Props.casterHediffDef);
				pawn.health.AddHediff(Props.mechHediffDef);
				GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(0, 1));
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
			if (parent.pawn.mechanitor == null)
			{
				return false;
			}
			if (!parent.pawn.mechanitor.ControlledPawns.Contains(pawn))
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(Props.mechHediffDef))
			{
				return false;
			}
			return true;
		}

	}

}
