using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompUseEffect_XenotypeForcer_Hybrid : CompUseEffect
	{

		public XenotypeDef endotype = null;

		public XenotypeDef xenotype = null;

		public CompProperties_UseEffect_XenotypeForcer_Hybrid Props => (CompProperties_UseEffect_XenotypeForcer_Hybrid)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			List<XenotypeDef> xenotypeDefs = XenotypeFilterUtility.WhiteListedXenotypes(true);
			endotype = Props.endotypeDef;
			xenotype = Props.xenotypeDef;
			if (endotype == null)
			{
				endotype = xenotypeDefs.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef.inheritable).RandomElement();
				if (endotype == null)
				{
					Log.Error("Generated serum with null endotype. Choose random.");
					endotype = xenotypeDefs.RandomElement();
				}
			}
			if (xenotype == null)
			{
				xenotype = xenotypeDefs.Where((XenotypeDef randomXenotypeDef) => !randomXenotypeDef.inheritable).RandomElement();
				if (xenotype == null)
				{
					Log.Error("Generated serum with null xenotype. Choose random.");
					xenotype = xenotypeDefs.RandomElement();
				}
			}
		}

		public override string TransformLabel(string label)
		{
			if (xenotype == null || endotype == null)
			{
				return parent.def.label + " (" + "ERR" + ")";
			}
			return parent.def.label + " (" + endotype.label + " + " + xenotype.label + ")";
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref endotype, "endotypeDef");
			Scribe_Defs.Look(ref xenotype, "xenotypeDef");
		}

		public override bool AllowStackWith(Thing other)
		{
			CompUseEffect_XenotypeForcer_Hybrid otherXeno = other.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>();
			if (otherXeno != null && otherXeno.xenotype != null && otherXeno.xenotype == xenotype && otherXeno.endotype != null && otherXeno.endotype == endotype)
			{
				return true;
			}
			return false;
		}

		public override void DoEffect(Pawn pawn)
		{
			// SerumUtility.HumanityCheck(pawn);
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			if (xenotype == null || endotype == null)
			{
				Log.Error("Xeno/endotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				return;
			}
			bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
			SerumUtility.DoubleXenotypeSerum(pawn, endotype, xenotype);
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
			SerumUtility.PostSerumUsedHook(pawn);
		}

	}

}
