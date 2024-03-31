using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_XenotypeForcer : CompUseEffect
	{
		public CompProperties_UseEffect_XenotypeForcer Props => (CompProperties_UseEffect_XenotypeForcer)props;

		public override void DoEffect(Pawn pawn)
		{
			// Humanity check
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			// Main
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
			switch (Props.xenotypeForcerType)
			{
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Base:
					SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef, Props.removeEndogenes, Props.removeXenogenes);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Hybrid:
					SerumUtility.HybridXenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.Custom:
					SerumUtility.CustomXenotypeSerum(pawn, blackListedXenotypes);
					break;
				case CompProperties_UseEffect_XenotypeForcer.XenotypeForcerType.CustomHybrid:
					SerumUtility.CustomHybridXenotypeSerum(pawn, blackListedXenotypes);
					break;
			}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			SerumUtility.PostSerumUsedHook(pawn);
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!SerumUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			}
			return true;
		}

	}

	public class CompUseEffect_XenotypeNullifier : CompUseEffect
	{
		public CompProperties_UseEffect_XenotypeForcer Props => (CompProperties_UseEffect_XenotypeForcer)props;

		public override void DoEffect(Pawn pawn)
		{
			// Humanity check
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			// Main
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			if (Props.removeSkinColor)
			{
				ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			}
			else
			{
				DuplicateUtility.NullifyXenotype(pawn);
			}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			SerumUtility.PostSerumUsedHook(pawn);
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!SerumUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			}
			return true;
		}

	}

}
