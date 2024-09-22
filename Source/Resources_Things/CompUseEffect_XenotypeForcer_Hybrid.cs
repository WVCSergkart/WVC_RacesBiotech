using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_XenotypeForcer_Hybrid : CompUseEffect
	{

		public XenotypeDef endotype = null;

		public XenotypeDef xenotype = null;

		public bool customMode = false;

		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void PostPostMake()
		{
			base.PostPostMake();
			List<XenotypeDef> xenotypeDefs = ListsUtility.GetWhiteListedXenotypes(true);
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
			if (xenotype == null || endotype == null)
			{
				Log.Error("Xeno/endotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
			}
		}

		public override string TransformLabel(string label)
		{
			if (customMode)
			{
				return parent.def.label + " (" + "WVC_Custom".Translate() + ")";
			}
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
			Scribe_Values.Look(ref customMode, "customMode");
		}

		// public override bool AllowStackWith(Thing other)
		// {
			// CompUseEffect_XenotypeForcer_Hybrid otherXeno = other.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>();
			// if (otherXeno != null && otherXeno.xenotype != null && otherXeno.xenotype == xenotype && otherXeno.endotype != null && otherXeno.endotype == endotype && otherXeno.customMode == customMode)
			// {
				// return true;
			// }
			// return false;
		// }

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (xenotype == null || endotype == null)
			{
				return "WVC_XaG_SuremRetuneShouldBeTunedWarn_Label".Translate();
			}
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			// if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			// {
				// return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			// }
			return true;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_SerumCustomMode_Label".Translate() + " " + customMode.ToStringYesNo(),
				defaultDesc = "WVC_XaG_SerumCustomMode_Desc".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					customMode = !customMode;
				}
			};
			if (Props.retuneJob == null)
			{
				yield break;
			}
		}

		public override void DoEffect(Pawn pawn)
		{
			// SerumUtility.HumanityCheck(pawn);
			// if (SerumUtility.HumanityCheck(pawn))
			// {
				// return;
			// }
			// if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			// {
				// pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
				// return;
			// }
			// SerumUtility.DoubleXenotypeSerum(pawn, endotype, xenotype);
			if (customMode)
			{
				List<CustomXenotype> xenotypes = ListsUtility.GetCustomXenotypesList();
				CustomXenotype endoCustomXenotype = xenotypes.Where((CustomXenotype randomXenotypeDef) => randomXenotypeDef.name != pawn.genes.xenotypeName && randomXenotypeDef.inheritable).RandomElement();
				CustomXenotype xenoCustomXenotype = xenotypes.Where((CustomXenotype randomXenotypeDef) => randomXenotypeDef.name != pawn.genes.xenotypeName && !randomXenotypeDef.inheritable).RandomElement();
				ReimplanterUtility.SetCustomXenotype(pawn, endoCustomXenotype);
				ReimplanterUtility.SetCustomXenotype(pawn, xenoCustomXenotype);
			}
			else
			{
				if (xenotype == null || endotype == null)
				{
					Log.Error("Xeno/endotype is still null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
					return;
				}
				ReimplanterUtility.SetXenotype(pawn, endotype, false);
				ReimplanterUtility.SetXenotype(pawn, xenotype);
			}
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			ReimplanterUtility.PostSerumUsedHook(pawn, true);
		}

	}

}
