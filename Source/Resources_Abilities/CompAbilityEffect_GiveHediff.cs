// RimWorld.CompAbilityEffect_GiveHediff
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_AbilityGiveHediff : RimWorld.CompProperties_AbilityGiveHediff
	{

		public bool humanityCheck = false;

		public bool mechanoidCheck = false;
		public GeneDef overseerShouldHaveGene;

		public bool serumsCheck = false;

		public bool onlyReproductive = false;

		public bool psychicSensitive = false;

		public string simpleMessage = "ERROR";

	}

	public class CompAbilityEffect_GiveHediff : CompAbilityEffect_WithDuration
	{
		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			if (!Props.onlyApplyToSelf && Props.applyToTarget)
			{
				ApplyInner(target.Pawn, parent.pawn);
			}
			if (Props.applyToSelf || Props.onlyApplyToSelf)
			{
				ApplyInner(parent.pawn, target.Pawn);
			}
		}

		public override bool GizmoDisabled(out string reason)
		{
			if (Props.mechanoidCheck && parent.pawn.RaceProps.IsMechanoid)
			{
				Pawn overseer = parent.pawn.GetOverseer();
				if (Props.overseerShouldHaveGene != null && !XaG_GeneUtility.HasActiveGene(Props.overseerShouldHaveGene, overseer))
				{
					reason = "WVC_XaG_OverseerShouldHaveGene".Translate(Props.overseerShouldHaveGene.label.CapitalizeFirst());
					return true;
				}
			}
			reason = null;
			return false;
		}

		protected void ApplyInner(Pawn target, Pawn other)
		{
			if (target == null)
			{
				return;
			}
			// Log.Error("1");
			if ((Props.humanityCheck && !ReimplanterUtility.IsHuman(target)))
			{
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), target, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			// Log.Error("2");
			if (Props.onlyReproductive && !target.ageTracker.CurLifeStage.reproductive)
			{
				return;
			}
			// Log.Error("3");
			if (Props.replaceExisting)
			{
				Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
				if (firstHediffOfDef != null)
				{
					target.health.RemoveHediff(firstHediffOfDef);
				}
			}
			// Log.Error("4");
			Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, target, Props.onlyBrain ? target.health.hediffSet.GetBrain() : null);
			HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = GetDurationSeconds(target).SecondsToTicks();
			}
			if (Props.severity >= 0f)
			{
				hediff.Severity = Props.severity;
			}
			// Log.Error("5");
			HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
			if (hediffComp_Link != null)
			{
				hediffComp_Link.other = other;
				hediffComp_Link.drawConnection = target == parent.pawn;
			}
			HediffComp_ChangeXenotype hediffComp_ChangeXenotype = hediff.TryGetComp<HediffComp_ChangeXenotype>();
			if (hediffComp_ChangeXenotype != null)
			{
				hediffComp_ChangeXenotype.genesOwner = other;
			}
			// Log.Error("6");
			target.health.AddHediff(hediff);
		}
	}

	public class CompAbilityEffect_SimpleGiveHediff : CompAbilityEffect
	{

		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (Props.hediffDef != null)
			{
				Hediff hediff = pawn.health.GetOrAddHediff(Props.hediffDef);
				PostHediffAdd(hediff);
				Messages.Message(Props.simpleMessage.Translate(), new LookTargets(target.Pawn, parent.pawn), MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
            Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (!GeneResourceUtility.CanDo_General(pawn))
			{
				return false;
			}
			if (Props.humanityCheck && !ReimplanterUtility.IsHuman(pawn))
			{
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (Props.psychicSensitive && pawn.IsPsychicSensitive())
			{
				// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}

		public virtual void PostHediffAdd(Hediff hediff)
		{

		}

	}

	public class CompAbilityEffect_ChimeraDeathMarkHediff : CompAbilityEffect_SimpleGiveHediff
	{

		public override void PostHediffAdd(Hediff hediff)
        {
            HediffComp_ChimeraDeathMark mark = hediff.TryGetComp<HediffComp_ChimeraDeathMark>();
			if (mark != null)
            {
				mark.caster = parent.pawn;
			}
		}

    }

}
