using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class CompUseEffect_GeneGiver : CompUseEffect
	{
		public GeneDef geneDef = null;

		public CompProperties_UseEffect_XenotypeForcer_II Props => (CompProperties_UseEffect_XenotypeForcer_II)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			if (geneDef == null)
			{
				if (Props.possibleGenes.NullOrEmpty())
				{
					Log.Error("Generated serum (gene giver) with null possibleGenes.");
					return;
				}
				geneDef = Props.possibleGenes.RandomElement();
			}
		}

		public override string TransformLabel(string label)
		{
			if (geneDef == null)
			{
				return parent.def.label + " (ERR)";
			}
			return parent.def.label + " (" + geneDef.label + ")";
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look(ref geneDef, "containedGeneDef");
		}

		public override void DoEffect(Pawn pawn)
		{
			if (SerumUtility.HumanityCheck(pawn))
			{
				return;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				return;
			}
			if (XaG_GeneUtility.HasGene(geneDef, pawn))
			{
				return;
			}
			pawn.genes.AddGene(geneDef, false);
			ReimplanterUtility.UnknownXenotype(pawn);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneGiverImplanted_Label".Translate(), "WVC_XaG_GeneGiverImplanted_Desc".Translate(pawn.Name.ToString(), geneDef.LabelCap), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			SerumUtility.PostSerumUsedHook(pawn);
		}

		public override bool AllowStackWith(Thing other)
		{
			CompUseEffect_GeneGiver otherXeno = other.TryGetComp<CompUseEffect_GeneGiver>();
			if (otherXeno != null && otherXeno.geneDef != null && otherXeno.geneDef == geneDef)
			{
				return true;
			}
			return false;
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (geneDef == null)
			{
				return "ERROR geneDef is null";
			}
			if (XaG_GeneUtility.HasGene(geneDef, p))
			{
				return "WVC_XaG_GeneGiverPawnHasGene_Label".Translate(p.Name.ToString());
			}
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
