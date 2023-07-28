using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompUseEffect_XenotypeForcer_II : CompUseEffect
    {
        public XenotypeDef xenotype = null;

        public CompProperties_UseEffect_XenotypeForcer_II Props => (CompProperties_UseEffect_XenotypeForcer_II)props;

        public override void PostPostMake()
        {
            base.PostPostMake();
            if (xenotype == null)
            {
                List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true);
                switch (Props.xenotypeType)
                {
                    case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Base:
                        xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
                        break;
                    case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Archite:
                        xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
                        break;
                        // case CompProperties_UseEffect_XenotypeForcer_II.XenotypeType.Hybrid:
                        // xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => SerumUtility.XenotypeHasArchites(randomXenotypeDef)).RandomElement();
                        // break;
                }
                if (xenotype == null)
                {
                    // We assign a random xenotype if there are no alternatives.
                    Log.Error("Generated serum with null xenotype. Choose random.");
                    xenotype = xenotypeDef.RandomElement();
                }
            }
            // descriptionHyperlinks = new List<DefHyperlink> { xenotype };
            // if (this.DescriptionHyperlinks != null)
            // {
            // this.DescriptionHyperlinks.Add(xenotype);
            // }
        }

        public override string TransformLabel(string label)
        {
            if (xenotype == null)
            {
                return parent.def.label + " (" + "ERR" + ")";
            }
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
            if (xenotype == null)
            {
                // This is already a problem created by the player. Occurs if the player has blacklisted ALL xenotypes.
                Log.Error("Xenotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
                // The following code will try a few more times to try to assign the xenotype, and if it doesn't work, it will spam everything with errors.
            }
            bool perfectCandidate = SerumUtility.HasCandidateGene(pawn);
            List<string> blackListedXenotypes = XenotypeFilterUtility.BlackListedXenotypesForSerums(false);
            SerumUtility.XenotypeSerum(pawn, blackListedXenotypes, xenotype, Props.removeEndogenes, Props.removeXenogenes);
            // switch (Props.xenotypeForcerType)
            // {
            // case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Base:
            // break;
            // case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Hybrid:
            // SerumUtility.HybridXenotypeSerum(pawn, blackListedXenotypes, xenotype);
            // break;
            // case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.Custom:
            // SerumUtility.CustomXenotypeSerum(pawn, blackListedXenotypes);
            // break;
            // case CompProperties_UseEffect_XenotypeForcer_II.XenotypeForcerType.CustomHybrid:
            // SerumUtility.CustomHybridXenotypeSerum(pawn, blackListedXenotypes);
            // break;
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
