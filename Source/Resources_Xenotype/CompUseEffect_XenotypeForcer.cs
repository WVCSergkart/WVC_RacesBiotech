using RimWorld;
using System.Collections.Generic;
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
			// if (MechanoidizationUtility.PawnIsAndroid(pawn) || !pawn.RaceProps.Humanlike || MechanoidizationUtility.PawnCannotUseSerums(pawn))
			// {
			// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
			// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			// return;
			// }
			// Main
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
			// if (Props.perfectCandidate != null && Props.perfectCandidate == pawn.genes?.Xenotype)
			// {
			// perfectCandidate = true;
			// }
			// if (perfectCandidates.Contains(pawn.genes?.Xenotype.defName))
			// {
			// perfectCandidate = true;
			// }
			// List<string> blackListedXenotypes = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
			// blackListedXenotypes.AddRange(item.blackListedXenotypesForSerums);
			// }
			// foreach (string item in XenotypesFilterStartup.filterBlackListedXenotypesForSerums)
			// {
			// blackListedXenotypes.Add(item);
			// }
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
			// if (!Props.hybrid)
			// {
			// XenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef, Props.removeEndogenes, Props.removeXenogenes);
			// }
			// else
			// {
			// HybridXenotypeSerum(pawn, blackListedXenotypes, Props.xenotypeDef);
			// }
			if (!perfectCandidate)
			{
				pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			}
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
		}
	}

}
