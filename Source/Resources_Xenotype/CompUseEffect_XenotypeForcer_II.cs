using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_XenotypeForcer_II : CompUseEffect
	{
		XenotypeDef xenotype = null;

		public CompProperties_UseEffect_XenotypeForcer_II Props => (CompProperties_UseEffect_XenotypeForcer_II)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true);
			xenotype = xenotypeDef.RandomElement();
			// descriptionHyperlinks = new List<DefHyperlink> { xenotype };
			// if (parent.DescriptionHyperlinks != null)
			// {
				// parent.DescriptionHyperlinks.Add(xenotype);
			// }
		}

		public override string TransformLabel(string label)
		{
			return parent.def.label + " (" + xenotype.label + ")";
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref xenotype, "xenotypeDef");
		}

		public override void DoEffect(Pawn pawn)
		{
			SerumUtility.HumanityCheck(pawn);
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			// if (xenotype == null)
			// {
				// return;
			// }
			bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
			List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
			switch (Props.xenotypeForcerType)
			{
			case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Base:
				SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, xenotype, Props.removeEndogenes, Props.removeXenogenes);
				break;
			case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Hybrid:
				SerumUtility.HybridXenotypeSerum(pawn, blackListedXenotypes, xenotype);
				break;
			case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Custom:
				SerumUtility.CustomXenotypeSerum(pawn, blackListedXenotypes);
				break;
			case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.CustomHybrid:
				SerumUtility.CustomHybridXenotypeSerum(pawn, blackListedXenotypes);
				break;
			}
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
