using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ReimplanterUtility
	{

		public static bool TryReimplant(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true, bool extractXenogerm = true)
		{
			if (!recipient.IsHuman() || !caster.IsHuman())
			{
				return false;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			ReimplanterUtility.ReimplantGenesHybrid(caster, recipient, endogenes, xenogenes);
			if (extractXenogerm)
			{
				ExtractXenogerm(caster);
			}
			return true;
		}

		public static void RemoteReimplant(Pawn caster, Pawn recipient, bool extractXenogerm = false)
		{
			if (ReimplanterUtility.TryReimplant(caster, recipient, true, true, extractXenogerm))
			{
				if (PawnUtility.ShouldSendNotificationAbout(recipient))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(recipient.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(recipient));
				}
			}
		}

		public static void ExtractXenogerm(Pawn pawn, int overrideDurationTicks = -1)
		{
			pawn.health.AddHediff(HediffDefOf.XenogermLossShock);
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
			{
				SetXenotype(pawn, XenotypeDefOf.Baseliner);
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
			if (overrideDurationTicks > 0)
			{
				hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
			}
			pawn.health.AddHediff(hediff);
		}

		public static void XenogermReplicating_WithCustomDuration(Pawn pawn, IntRange durationIntervalRange)
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (firstHediffOfDef != null)
			{
				HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
				if (hediffComp_Disappears != null)
				{
					hediffComp_Disappears.ticksToDisappear += durationIntervalRange.RandomInRange;
				}
			}
			else
			{
				Hediff cooldownHediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
				HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
				if (hediffComp_Disappears != null)
				{
					hediffComp_Disappears.ticksToDisappear = durationIntervalRange.RandomInRange;
				}
				pawn.health.AddHediff(cooldownHediff);
			}
		}

		public static void SetXenotypeDirect(Pawn caster, Pawn recipient, XenotypeDef xenotypeDef = null, bool changeXenotype = true)
		{
			if (changeXenotype)
			{
				recipient.genes?.SetXenotypeDirect(caster != null ? caster.genes.Xenotype : xenotypeDef);
				recipient.genes.xenotypeName = caster?.genes.xenotypeName;
				recipient.genes.iconDef = caster?.genes.iconDef;
			}
		}

		public static void UnknownXenotype(Pawn pawn, string xenotypeName = null, XenotypeIconDef xenotypeIconDef = null)
		{
			// pawn.genes.xenotypeName = "WVC_XaG_SubXenotypeUnknownXenotypeError".Translate();
			// pawn.genes.iconDef = WVC_GenesDefOf.WVC_XenoRandomKindc;
			pawn.genes?.SetXenotypeDirect(XenotypeDefOf.Baseliner);
			pawn.genes.xenotypeName = xenotypeName ?? GeneUtility.GenerateXenotypeNameFromGenes(XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading));
			pawn.genes.iconDef = xenotypeIconDef ?? DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
		}

		public static void UnknownChimerkin(Pawn recipient)
		{
			List<XenotypeDef> chimeraXenotypes = DefDatabase<XenotypeDef>.AllDefsListForReading.Where((xenotypeDef) => xenotypeDef?.GetModExtension<GeneExtension_General>()?.isChimerkin == true).ToList();
			XenotypeDef xenotypeDef = null;
			foreach (XenotypeDef xenos in chimeraXenotypes)
            {
				if (XaG_GeneUtility.GenesIsMatch(recipient.genes.Endogenes, xenos.genes, 1f))
                {
					xenotypeDef = xenos;
					break;
				}
            }
			if (xenotypeDef != null)
			{
				ReimplanterUtility.SetXenotypeDirect(null, recipient, xenotypeDef);
			}
		}

		public static void GiveReimplantJob(Pawn pawn, Pawn targPawn, JobDef absorbJob)
		{
			pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(absorbJob, targPawn), JobTag.Misc);
			if (targPawn.HomeFaction != null && !targPawn.HomeFaction.Hidden && targPawn.HomeFaction != pawn.Faction && !targPawn.HomeFaction.HostileTo(Faction.OfPlayer))
			{
				Messages.Message("MessageAbsorbingXenogermWillAngerFaction".Translate(targPawn.HomeFaction, targPawn.Named("PAWN")), pawn, MessageTypeDefOf.CautionInput, historical: false);
			}
		}

		[Obsolete]
		public static bool CanAbsorbGenogerm(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			if (pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure)
			{
				return true;
			}
			if (!pawn.Downed)
			{
				return false;
			}
			if (!pawn.genes.GenesListForReading.Any())
			{
				return false;
			}
			return true;
		}

		public static bool IsHuman(this Pawn pawn)
		{
			if (pawn?.RaceProps?.Humanlike != true || pawn.IsAndroid())
			{
				return false;
			}
			return true;
		}

		public static bool IsAnomaly(this Pawn pawn)
		{
			if (ModsConfig.AnomalyActive)
			{
				if (pawn.IsMutant)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsMutantOfDef(this Pawn pawn, MutantDef mutantDef)
		{
			if (mutantDef == null || pawn.mutant == null)
			{
				return false;
			}
			return mutantDef == pawn.mutant.Def;
		}

		// ===============================================================

		public static void SaveReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			if (pawn.genes != null && pawn.genes.Xenogenes.NullOrEmpty())
			{
				SetXenotype(pawn, xenotypeDef);
				pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(pawn);
			}
		}

		public static void ReimplantGenesHybrid(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true, bool xenogerm = true)
		{
			Pawn_GeneTracker recipientGenes = recipient.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				SetXenotypeDirect(caster, recipient);
			}
			if (xenogenes && !recipientGenes.Xenogenes.NullOrEmpty())
			{
				// foreach (Gene item in recipientGenes.Xenogenes.ToList())
				// {
					// recipient.genes?.RemoveGene(item);
				// }
				recipientGenes.Xenogenes.RemoveAllGenes();
			}
			if (endogenes && !recipientGenes.Endogenes.NullOrEmpty())
			{
				// foreach (Gene item in recipientGenes.Endogenes.ToList())
				// {
					// recipient.genes?.RemoveGene(item);
				// }
				recipientGenes.Endogenes.RemoveAllGenes();
			}
			if (endogenes)
			{
				foreach (Gene endogene in caster.genes.Endogenes)
				{
					recipient.genes.AddGene(endogene.def, xenogene: false);
				}
			}
			if (xenogenes)
			{
				foreach (Gene xenogene in caster.genes.Xenogenes)
				{
					recipient.genes.AddGene(xenogene.def, xenogene: true);
				}
			}
			if (xenogerm)
			{
				if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
				{
					caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
				}
				recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(recipient);
			}
            //XaG_GameComponent.AddMissingGeneAbilities(recipient);
            //FixGeneTraits(recipient);
            ReimplanterUtility.PostImplantDebug(recipient);
        }

		// =============================== New ===============================

		public static void PostImplantDebug(Pawn pawn)
		{
            foreach (Gene item in pawn.genes.GenesListForReading)
            {
                if (item.overriddenByGene != null && !XaG_GeneUtility.HasGene(item.overriddenByGene.def, pawn))
                {
                    item.OverrideBy(null);
                }
            }
            XaG_GameComponent.AddMissingGeneAbilities(pawn);
			ReimplanterUtility.FixGeneTraits(pawn);
		}

		public static void NotifyGenesChanged(Pawn pawn)
		{
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (item is IGeneNotifyGenesChanged targetGene)
				{
					targetGene.Notify_GenesChanged(null);
				}
			}
		}

		public static void OverrideAllConflictsWith(Gene gene, List<Gene> genes)
		{
			foreach (Gene item in genes)
			{
				if (item.def.ConflictsWith(gene.def))
				{
					item.OverrideBy(gene);
				}
			}
		}

		public static void TrySetSkinAndHairGenes(Pawn pawn)
        {
            FindSkinAndHairGenes(pawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor);
            if (!xenotypeHasSkinColor)
            {
                recipientGenes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, false);
            }
            if (!xenotypeHasHairColor)
            {
                recipientGenes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, false);
            }
		}

		public static void FindSkinAndHairGenes(Pawn pawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor)
        {
            recipientGenes = pawn.genes;
            xenotypeHasSkinColor = false;
            xenotypeHasHairColor = false;
            foreach (Gene gene in recipientGenes.Endogenes)
            {
                if (gene.def.skinColorBase != null || gene.def.skinColorOverride != null)
                {
                    xenotypeHasSkinColor = true;
                }
                if (gene.def.hairColorOverride != null)
                {
                    xenotypeHasHairColor = true;
                }
            }
        }

        public static void FixGeneTraits(Pawn pawn)
		{
			foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			{
				if (trait.sourceGene != null && !XaG_GeneUtility.HasGene(trait.sourceGene.def, pawn))
				{
					trait.sourceGene = null;
					trait.suppressedByGene = null;
					trait.suppressedByTrait = false;
					pawn.story.traits.RemoveTrait(trait, true);
					pawn.story.traits.allTraits?.Remove(trait);
				}
				if (trait.suppressedByGene != null && !XaG_GeneUtility.HasGene(trait.suppressedByGene.def, pawn))
				{
					trait.suppressedByGene = null;
				}
			}
			//foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			//{
			//	if (trait.suppressedByTrait && !TraitHasConflicts(pawn, trait))
			//	{
			//		trait.suppressedByTrait = false;
			//	}
			//}
		}

		public static bool TraitHasConflicts(Pawn pawn, Trait selectedTrait)
		{
			foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			{
				if (selectedTrait.def.ConflictsWith(trait))
				{
					//Log.Error(trait.def.defName + " conflict with " + selectedTrait.def.defName);
					return true;
				}
			}
			return false;
		}

		// =============================== Setter ===============================

		public static void SetXenotype_DoubleXenotype(Pawn pawn, XenotypeDef xenotypeDef, bool changeXenotype = true)
		{
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			{
				SetXenotype(pawn, result.xenotype, changeXenotype);
			}
			SetXenotype(pawn, xenotypeDef, changeXenotype);
		}

		public static void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef, bool changeXenotype = true)
		{
			Pawn_GeneTracker genes = pawn.genes;
			genes.Xenogenes.RemoveAllGenes();
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				genes.Endogenes.RemoveAllGenes();
			}
			SetXenotypeDirect(null, pawn, xenotypeDef, changeXenotype);
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				genes.AddGene(geneDef, !xenotypeDef.inheritable);
			}
			TrySetSkinAndHairGenes(pawn);
			ReimplanterUtility.PostImplantDebug(pawn);
		}

		public static void SetXenotype(Pawn pawn, XenotypeHolder xenotypeHolder)
		{
			if (xenotypeHolder.CustomXenotype)
			{
				SetCustomXenotype(pawn, xenotypeHolder);
			}
			else
			{
				SetXenotype(pawn, xenotypeHolder.xenotypeDef, true);
			}
		}

		public static void SetCustomXenotype(Pawn pawn, CustomXenotype xenotypeDef)
        {
            SetCustomGenes(pawn, xenotypeDef.genes, xenotypeDef.iconDef, xenotypeDef.name, xenotypeDef.inheritable);
		}

		public static void SetCustomXenotype(Pawn pawn, XenotypeHolder xenotypeDef)
		{
			SetCustomGenes(pawn, xenotypeDef.genes, xenotypeDef.iconDef, xenotypeDef.name, xenotypeDef.inheritable);
		}

		public static void SetBrokenXenotype(Pawn pawn, XenotypeHolder xenotypeDef, float chance)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			genes.Shuffle();
			int currentTry = 0;
			foreach (GeneDef geneDef in genes.ToList())
            {
				float chanceFactor = currentTry * 0.02f;
				if (Rand.Chance(chance + chanceFactor))
                {
					genes.Remove(geneDef);
				}
				currentTry++;
			}
			SetCustomGenes(pawn, genes, null, "ERR", xenotypeDef.inheritable);
			UnknownXenotype(pawn);
		}

		public static void ConvertXenogenesToEndogenes(Pawn pawn, float percent = 1f)
		{
			List<Gene> genes = pawn.genes.Xenogenes.Where((Gene gene) => !XaG_GeneUtility.HasActiveEndogene(gene.def, pawn)).ToList();
			float maxCycleTry = genes.Count * percent;
			int currentTry = 0;
			foreach (Gene gene in genes.ToList())
			{
				if (maxCycleTry <= currentTry)
                {
					break;
                }
				if (XaG_GeneUtility.TryRemoveAllConflicts(pawn, gene.def))
				{
					pawn.genes.AddGene(gene.def, false);
					currentTry++;
				}
			}
			ReimplanterUtility.PostImplantDebug(pawn);
		}

		//public static bool TrySetCustomGenes(Pawn pawn, List<GeneDef> genes, XenotypeIconDef iconDef, string name, bool inheritable)
		//{
		//	if (!pawn.IsHuman())
		//	{
		//		return false;
		//	}
		//	SetCustomGenes(pawn, genes, iconDef, name, inheritable);
		//	return true;
		//}

		public static void SetCustomGenes(Pawn pawn, List<GeneDef> genes, XenotypeIconDef iconDef, string name, bool inheritable)
        {
            Pawn_GeneTracker geneTracker = pawn.genes;
			geneTracker.Xenogenes.RemoveAllGenes();
            if (inheritable)
            {
				geneTracker.Endogenes.RemoveAllGenes();
            }
			geneTracker.SetXenotypeDirect(XenotypeDefOf.Baseliner);
			geneTracker.xenotypeName = name;
			geneTracker.iconDef = iconDef;
            foreach (GeneDef geneDef in genes)
            {
				geneTracker.AddGene(geneDef, !inheritable);
            }
            TrySetSkinAndHairGenes(pawn);
            ReimplanterUtility.PostImplantDebug(pawn);
        }

        // Ideology Hook

        public static void PostSerumUsedHook(Pawn pawn, bool isXenoMod)
		{
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_HistoryEventDefOf.WVC_XenotypeSerumUsed, pawn.Named(HistoryEventArgsNames.Doer)));
				if (isXenoMod)
				{
					Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, pawn.Named(HistoryEventArgsNames.Doer)));
				}
			}
		}

		public static void FleckAndLetter(Pawn caster, Pawn victim)
		{
			FleckMaker.AttachedOverlay(victim, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
			if (PawnUtility.ShouldSendNotificationAbout(caster) || PawnUtility.ShouldSendNotificationAbout(victim))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(caster.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(caster, victim));
			}
		}

		public static bool ImplanterValidation(Pawn parent, LocalTargetInfo target, bool throwMessages)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (pawn.IsQuestLodger())
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.HostileTo(parent) && !pawn.Downed)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!PawnIdeoCanAcceptReimplant(parent, pawn))
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
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
			return true;
		}

		public static bool PawnIdeoCanAcceptReimplant(Pawn implanter, Pawn implantee)
		{
			if (implantee.Downed)
			{
				return true;
			}
			return CompAbilityEffect_ReimplantXenogerm.PawnIdeoCanAcceptReimplant(implanter, implantee);
		}

	}
}
