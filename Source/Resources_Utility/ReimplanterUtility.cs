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
			ReimplanterUtility.GeneralReimplant(caster, recipient, endogenes, xenogenes);
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

		//public static bool TryGetXenogermReplicatingDuration(Pawn pawn, out HediffComp_Disappears hediffComp_Disappears)
		//{
		//	hediffComp_Disappears = null;
		//	Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
		//	if (firstHediffOfDef != null)
		//	{
		//		hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
		//		if (hediffComp_Disappears != null)
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		public static void XenogermReplicating_WithCustomDuration(Pawn pawn, IntRange durationIntervalRange)
		{
			XenogermReplicating_WithCustomDuration(pawn, durationIntervalRange, pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
		}

		public static void XenogermReplicating_WithCustomDuration(Pawn pawn, IntRange durationIntervalRange, Hediff xenogermReplicating)
		{
			//Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (xenogermReplicating != null)
			{
				HediffComp_Disappears hediffComp_Disappears = xenogermReplicating.TryGetComp<HediffComp_Disappears>();
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
			pawn.genes.xenotypeName = xenotypeName ?? GeneUtility.GenerateXenotypeNameFromGenes(XaG_GeneUtility.ConvertToDefs(pawn.genes.GenesListForReading));
			pawn.genes.iconDef = xenotypeIconDef ?? DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
		}

		private static List<XenotypeDef> cachedChimerkinXenotypes;
		public static void UnknownChimerkin(Pawn recipient)
		{
			if (cachedChimerkinXenotypes == null)
			{
				cachedChimerkinXenotypes = ListsUtility.GetAllXenotypesExceptAndroids().Where((xenotypeDef) => xenotypeDef?.GetModExtension<GeneExtension_General>()?.isChimerkin == true).ToList();
			}
			XenotypeDef xenotypeDef = null;
			foreach (XenotypeDef xenos in cachedChimerkinXenotypes)
			{
				if (XaG_GeneUtility.GenesIsMatch(recipient.genes.Endogenes, xenos.genes, 1f))
				{
					xenotypeDef = xenos;
					break;
				}
			}
			if (xenotypeDef == null)
			{
				foreach (XenotypeDef xenos in ListsUtility.GetAllXenotypesExceptAndroids().Where((xenotypeDef) => xenotypeDef.inheritable))
				{
					if (XaG_GeneUtility.GenesIsMatch(recipient.genes.Endogenes, xenos.genes, 1f))
					{
						xenotypeDef = xenos;
						break;
					}
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

		//[Obsolete]
		//public static bool CanAbsorbGenogerm(Pawn pawn)
		//{
		//	if (pawn?.genes == null)
		//	{
		//		return false;
		//	}
		//	if (pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure)
		//	{
		//		return true;
		//	}
		//	if (!pawn.Downed)
		//	{
		//		return false;
		//	}
		//	if (!pawn.genes.GenesListForReading.Any())
		//	{
		//		return false;
		//	}
		//	return true;
		//}

		public static bool IsHuman(this Pawn pawn)
		{
			if (pawn?.RaceProps?.Humanlike != true || pawn.IsAndroid())
			{
				return false;
			}
			return true;
		}

		public static bool IsHumanMutant(this Pawn pawn)
		{
			if (!pawn.IsMutant)
			{
				return false;
            }
            if (!pawn.IsHuman())
            {
                return false;
            }
			if (pawn.IsSubhuman)
			{
				return false;
			}
            return false;
		}

		//public static bool IsAnomaly(this Pawn pawn)
		//{
		//	if (ModsConfig.AnomalyActive)
		//	{
		//		if (pawn.IsMutant)
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

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
				//pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
				//GeneUtility.UpdateXenogermReplication(pawn);
				ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
			}
		}

		public static void GeneralReimplant(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true, bool xenogerm = true)
		{
			Pawn_GeneTracker recipientGenes = recipient.genes;
            if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
            {
                SetXenotypeDirect(caster, recipient);
            }
			//else if (recipientGenes.Xenogenes.NullOrEmpty())
			//{
			//	if (caster.genes.Xenotype.inheritable || caster.genes.UniqueXenotype)
			//	{
			//		SetXenotypeDirect(caster, recipient);
			//	}
			//	else
			//	{
			//		SetXenotypeDirect(null, null, caster.GetXenotype(false));
			//	}
			//}
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
				UpdateXenogermReplication_WithComa(recipient);
            }
            //XaG_GameComponent.AddMissingGeneAbilities(recipient);
            //FixGeneTraits(recipient);
            ReimplanterUtility.PostImplantDebug(recipient);
        }

        public static void UpdateXenogermReplication_WithComa(Pawn recipient)
        {
            recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
            GeneUtility.UpdateXenogermReplication(recipient);
        }

        // =============================== New ===============================

        public static void PostImplantDebug(Pawn pawn)
        {
            try
            {
				List<AbilityDef> pawnAbilities = pawn.abilities.abilities.ConvertToDefs();
                List<Gene> genesListForReading = pawn.genes.GenesListForReading;
				List<Gene> endogenes = pawn.genes.Endogenes;
				List<Gene> xenogenes = pawn.genes.Xenogenes;
				foreach (Gene xenogene in xenogenes)
				{
					if (xenogene.Overridden)
					{
						continue;
					}
					foreach (Gene endogene in endogenes)
					{
						if (endogene.Overridden)
						{
							continue;
						}
						if (xenogene.def.ConflictsWith(endogene.def))
						{
							endogene.OverrideBy(xenogene);
						}
					}
				}
				foreach (Gene item in genesListForReading)
				{
					if (item.overriddenByGene != null && !genesListForReading.Contains(item.overriddenByGene))
					{
						item.OverrideBy(null);
					}
					XaG_GameComponent.AddMissingGeneAbilities(pawn, pawnAbilities, item);
					NotifyGenesChanged(item);
				}
				//XaG_GameComponent.AddMissingGeneAbilities(pawn);
				TraitsUtility.FixGeneTraits(pawn, genesListForReading);
				//NotifyGenesChanged(pawn);
				XaG_GeneUtility.ResetGenesInspectString(pawn);
				ThoughtWorker_Precept_Shapeshifter.UpdCollection();
				if (DebugSettings.ShowDevGizmos)
                {
                    Log.Warning("Post implant debug called.");
                }
            }
            catch (Exception arg)
			{
				Log.Error("Failed do post implant debug. Reason: " + arg);
			}
        }

        public static void NotifyGenesChanged(Gene item)
        {
            if (item is IGeneNotifyGenesChanged iGeneNotifyGenesChanged)
            {
                iGeneNotifyGenesChanged.Notify_GenesChanged(null);
			}
			//if (item is IGeneMetabolism metabol)
			//{
			//	metabol.UpdateMetabolism();
			//}
		}

		public static void NotifyGenesChanged(Pawn pawn)
		{
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				NotifyGenesChanged(item);
			}
		}

		public static void OverrideAllConflictsWith(Gene gene, List<Gene> genes)
		{
			foreach (Gene item in genes)
			{
				if (item != gene && item.def.ConflictsWith(gene.def))
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
                recipientGenes?.AddGene(MainDefOf.Skin_SheerWhite, false);
            }
            if (!xenotypeHasHairColor)
            {
                recipientGenes?.AddGene(MainDefOf.Hair_SnowWhite, false);
            }
		}

		public static void FindSkinAndHairGenes(Pawn pawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor)
        {
            recipientGenes = pawn.genes;
            xenotypeHasSkinColor = false;
            xenotypeHasHairColor = false;
            foreach (Gene gene in recipientGenes.Endogenes)
            {
                IsSkinOrHairGene(ref xenotypeHasSkinColor, ref xenotypeHasHairColor, gene.def);
            }
        }

        public static void IsSkinOrHairGene(ref bool xenotypeHasSkinColor, ref bool xenotypeHasHairColor, GeneDef geneDef)
        {
            if (XaG_GeneUtility.IsSkinGeneDef(geneDef))
            {
                xenotypeHasSkinColor = true;
            }
            if (XaG_GeneUtility.IsHairGeneDef(geneDef))
            {
                xenotypeHasHairColor = true;
            }
		}

		// =============================== Debug Xenotype ===============================

		public static bool TryFixPawnXenotype_Beta(Pawn pawn)
		{
			bool isBaseliner = true;
			if (XaG_GeneUtility.PawnIsBaseliner(pawn))
            {
				return false;
            }
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (!gene.def.passOnDirectly)
				{
					continue;
				}
				if (gene.def.biostatCpx > 0 || gene.def.biostatArc > 0 || gene.def.biostatMet > 0)
				{
					isBaseliner = false;
					break;
				}
				if (gene.def.skinColorOverride != null || gene.def.hairColorOverride != null || gene.def.skinColorBase != null)
				{
					continue;
				}
			}
			if (isBaseliner)
			{
				SetXenotypeDirect(null, pawn, XenotypeDefOf.Baseliner);
			}
			return isBaseliner;
		}

		//private static bool HasAnyNonSkinOrHairGene(Pawn pawn)
		//{
		//	bool hasAnyNonSkinOrHairGene = false;
		//	foreach (Gene gene in pawn.genes.GenesListForReading)
  //          {
		//		if (gene.def.biostatCpx > 0 || gene.def.biostatArc > 0 || gene.def.biostatMet > 0)
  //              {
		//			hasAnyNonSkinOrHairGene = true;
		//			break;
  //              }
		//		if (gene.def.skinColorOverride != null || gene.def.hairColorOverride != null || gene.def.skinColorBase != null)
  //              {
		//			continue;
  //              }
		//		hasAnyNonSkinOrHairGene = true;
		//		break;
		//	}
		//	return hasAnyNonSkinOrHairGene;
		//}

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

		public static void SetXenotype(Pawn pawn, XenotypeHolder xenotypeHolder, Gene_Shapeshifter shapeshiferGene, bool xenogenes = true, bool debug = false)
		{
			Pawn_GeneTracker recipientGenes = pawn.genes;
			if (recipientGenes.Xenogenes.Where((gene) => gene != shapeshiferGene).ToList().NullOrEmpty() || xenogenes)
			{
				ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeHolder.xenotypeDef, true);
				if (!xenotypeHolder.name.NullOrEmpty() || xenotypeHolder.iconDef != null)
				{
					pawn.genes.xenotypeName = xenotypeHolder.name;
					pawn.genes.iconDef = xenotypeHolder.iconDef;
				}
			}
			if (xenogenes || !xenotypeHolder.inheritable || xenotypeHolder.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Xenogenes.ToList())
				{
					if (!xenotypeHolder.inheritable && xenotypeHolder.genes.Contains(gene.def))
					{
						continue;
					}
					//RemoveGene(gene);
					shapeshiferGene.RemoveGene(gene);
				}
			}
			if (xenotypeHolder.inheritable || xenotypeHolder.Baseliner)
			{
				foreach (Gene gene in recipientGenes.Endogenes.ToList())
				{
					if (xenotypeHolder.inheritable && xenotypeHolder.genes.Contains(gene.def))
					{
						continue;
					}
					//RemoveGene(gene);
					shapeshiferGene.RemoveGene(gene);
				}
			}
			foreach (GeneDef geneDef in xenotypeHolder.genes)
			{
				if (!xenotypeHolder.inheritable && XaG_GeneUtility.HasXenogene(geneDef, pawn) || xenotypeHolder.inheritable && XaG_GeneUtility.HasEndogene(geneDef, pawn))
				{
					continue;
				}
				shapeshiferGene.AddGene(geneDef, xenotypeHolder.inheritable);
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
			if (debug)
			{
				ReimplanterUtility.PostImplantDebug(pawn);
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
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_XenotypeSerumUsed, pawn.Named(HistoryEventArgsNames.Doer)));
				if (isXenoMod)
				{
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RimWorld.HistoryEventDefOf.InstalledProsthetic, pawn.Named(HistoryEventArgsNames.Doer)));
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

		public static bool ImplanterValidation(AbilityDef def, Pawn parent, LocalTargetInfo target, bool throwMessages, bool checkIdeo = true)
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
					Messages.Message("MessageCantUseOnResistingPerson".Translate(def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			else if (!pawn.IsPrisoner && !pawn.IsSlave && pawn.Faction != parent.Faction)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_IsNeutralFactionOfPawn".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (checkIdeo && !PawnIdeoCanAcceptReimplant(parent, pawn))
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
			if (implantee.Downed || implantee.Deathresting)
			{
				return true;
			}
			return CompAbilityEffect_ReimplantXenogerm.PawnIdeoCanAcceptReimplant(implanter, implantee);
		}

	}
}
