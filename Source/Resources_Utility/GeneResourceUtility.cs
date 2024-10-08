using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class GeneResourceUtility
	{

		// Psylinks

		public static void AddPsylink(Pawn pawn, ref bool pawnHadPsylinkBefore)
		{
			if (!WVC_Biotech.settings.link_addedPsylinkWithGene)
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
				ChangePsylinkLevel(pawn);
			}
			else
			{
				pawnHadPsylinkBefore = true;
			}
		}

		public static void ChangePsylinkLevel(Pawn pawn)
		{
			if (pawn.Spawned)
			{
				return;
			}
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier);
			if (Rand.Chance(0.34f) && firstHediffOfDef != null)
			{
				IntRange level = new(1, 5);
				((Hediff_Level)firstHediffOfDef).ChangeLevel(level.RandomInRange);
			}
		}

		public static void PsyfocusOffset(Pawn pawn, Gene gene, ref float recoveryRate, GeneExtension_Giver props)
		{
			if (!pawn.IsHashIntervalTick(750))
			{
				return;
			}
			if (!gene.Active)
			{
				return;
			}
			pawn?.psychicEntropy?.OffsetPsyfocusDirectly(recoveryRate);
			if (!pawn.IsHashIntervalTick(7500))
			{
				return;
			}
			if (pawn.HasPsylink)
			{
				recoveryRate = GetRecoveryRate(pawn, props);
			}
		}

		public static float GetRecoveryRate(Pawn pawn, GeneExtension_Giver giver)
		{
			if (giver == null)
			{
				return 0.01f * pawn.GetPsylinkLevel();
			}
			return giver.curve.Evaluate(pawn.GetPsylinkLevel());
		}

		// Dust

		public static void IngestedThingWithFactor(Gene gene, Thing thing, Pawn pawn, float factor)
		{
			if (!gene.Active || thing?.def?.ingestible == null)
			{
				return;
			}
			float nutrition = thing.def.ingestible.CachedNutrition;
			if (nutrition > 0f)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, nutrition * factor);
			}
		}

		public static void OffsetNeedFood(Pawn pawn, float offset, bool alwaysMaxValue = false)
		{
			Need_Food need_Food = pawn.needs?.food;
			if (need_Food != null)
			{
				if (alwaysMaxValue)
				{
					need_Food.CurLevel = need_Food.MaxLevel;
				}
				else
				{
					// need_Food.CurLevel += need_Food.MaxLevel >= (need_Food.CurLevel + offset) ? offset : (need_Food.MaxLevel - need_Food.CurLevel);
					need_Food.CurLevel = Mathf.Clamp(need_Food.CurLevel + offset, 0f, need_Food.MaxLevel);
				}
			}
		}

		public static bool PawnDowned(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || RestUtility.InBed(pawn))
			{
				return true;
			}
			return false;
		}

		// Shape

		public static void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			foreach (Gene gene in shapeshiftGene.pawn.genes.GenesListForReading)
			{
				if (gene is IGeneShapeshift geneShapeshifter && gene.Active)
				{
					geneShapeshifter.Notify_PreShapeshift(shapeshiftGene);
				}
			}
		}

		public static void Notify_PostShapeshift_Traits(Gene_Shapeshifter shapeshiftGene)
		{
			foreach (Trait trait in shapeshiftGene.pawn.story.traits.allTraits)
			{
				GeneExtension_Undead extension = trait?.def?.GetModExtension<GeneExtension_Undead>();
				if (extension == null)
				{
					continue;
				}
				if (!extension.thoughtDefs.NullOrEmpty())
				{
					foreach (ThoughtDef thoughtDef in extension.thoughtDefs)
					{
						shapeshiftGene.pawn.needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef);
					}
				}
				if (!extension.hediffDefs.NullOrEmpty())
				{
					foreach (HediffDef hediff in extension.hediffDefs)
					{
						HediffUtility.TryAddHediff(hediff, shapeshiftGene.pawn, null);
					}
				}
			}
		}

		public static void Notify_PostShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			foreach (Gene gene in shapeshiftGene.pawn.genes.GenesListForReading)
			{
				if (gene is IGeneShapeshift geneShapeshifter && gene.Active)
				{
					geneShapeshifter.Notify_PostShapeshift(shapeshiftGene);
				}
			}
		}

		[Obsolete]
		public static void AddRandomTraitFromListWithChance(Pawn pawn, GeneExtension_Undead geneExtension)
		{
			TraitSet traitSet = pawn.story.traits;
			if (geneExtension == null || traitSet.allTraits.Count > 1)
			{
				return;
			}
			XaG_CountWithChance traitDefWithWeight = geneExtension.possibleTraits.RandomElementByWeight((XaG_CountWithChance x) => x.weight);
			float chance = traitDefWithWeight.weight;
			Trait trait = new(traitDefWithWeight.traitDef);
			if (traitSet.allTraits.Contains(trait) || MiscUtility.TraitHasAnyConflicts(traitSet.allTraits, trait))
			{
				return;
			}
			if (Rand.Chance(chance))
			{
				traitSet.GainTrait(trait);
			}
		}

		// Shapeshift
		public static bool TryShapeshift(Gene_Shapeshifter geneShapeshifter, Dialog_Shapeshifter dialog)
		{
			int num = 0;
			try
			{
				num = 1;
				geneShapeshifter.PreShapeshift(geneShapeshifter, dialog.genesRegrowing);
				num = 2;
				geneShapeshifter.Shapeshift(dialog.selectedXenotype, dialog.genesRegrowing || dialog.clearXenogenes, dialog.genesRegrowing || dialog.doubleXenotypeReimplantation);
				num = 3;
				geneShapeshifter.PostShapeshift(geneShapeshifter, dialog.genesRegrowing);
				num = 4;
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneShapeshifter_ShapeshiftLetterLabel".Translate(), "WVC_XaG_GeneShapeshifter_ShapeshiftLetterDesc".Translate(geneShapeshifter.pawn.Named("TARGET"), dialog.selectedXenotype.LabelCap, geneShapeshifter.LabelCap)
				+ "\n\n" + (dialog.selectedXenotype.descriptionShort.NullOrEmpty() ? dialog.selectedXenotype.description : dialog.selectedXenotype.descriptionShort),
				WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(geneShapeshifter.pawn));
				return true;
			}
			catch (Exception arg)
			{
				// Log.Error(geneShapeshifter.pawn.Name.ToString() + " critical error during shapeshift. " + geneShapeshifter.LabelCap + " | " + geneShapeshifter.def.defName);
				Log.Error($"Error while shapeshifting {geneShapeshifter.ToStringSafe()} during phase {num}: {arg} (Gene: " + geneShapeshifter.LabelCap + " | " + geneShapeshifter.def.defName + ")");
			}
			return false;
		}

		// Clone
		public static bool TryDuplicatePawn(Pawn progenitor, Gene gene = null, XenotypeDef xenotypeDef = null, bool duplicateMode = false)
		{
			if (gene == null || progenitor == null || xenotypeDef == null || duplicateMode == false)
			{
				return false;
			}
			PawnGenerationRequest request = DuplicateUtility.RequestCopy(progenitor);
			// PawnGenerationRequest generateNewBornPawn = new(progenitor.kindDef, progenitor.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, fixedChronologicalAge: progenitor.ageTracker.AgeBiologicalTicks, fixedGender: progenitor.gender, fixedBiologicalAge: progenitor.ageTracker.AgeBiologicalTicks, forceNoIdeo: false, forceNoBackstory: true, forbidAnyTitle: true, forceDead: false, forcedXenotype: progenitor.genes.Xenotype, developmentalStages: progenitor.DevelopmentalStage);
			Pawn clone = PawnGenerator.GeneratePawn(request);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(clone, progenitor))
			{
				DuplicateUtility.DuplicatePawn(progenitor, clone, xenotypeDef);
				if (!XaG_GeneUtility.HasGene(gene.def, clone))
				{
					clone.genes.AddGene(gene.def, false);
				}
				// HealingUtility.RegrowAllBodyParts(progenitor);
			}
			else
			{
				Find.WorldPawns.PassToWorld(clone, PawnDiscardDecideMode.Discard);
			}
			if (progenitor.Spawned)
			{
				FilthMaker.TryMakeFilth(progenitor.Position, progenitor.Map, ThingDefOf.Filth_Slime, 5);
				WVC_GenesDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(progenitor));
				// WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(progenitor, progenitor.Map).Trigger(progenitor, null);
				WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(clone, clone.Map).Trigger(clone, null);
				if (clone.caller != null)
				{
					clone.caller.DoCall();
				}
			}
			if (PawnUtility.ShouldSendNotificationAbout(clone))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneShapeshifter_DuplicateLetterLabel".Translate(), "WVC_XaG_GeneShapeshifter_DuplicateLetterDesc".Translate(progenitor.Named("TARGET"), xenotypeDef.LabelCap, gene.LabelCap)
				+ "\n\n" + (xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.description : xenotypeDef.descriptionShort),
				WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(progenitor));
			}
			return true;
		}

		// Coma TEST

		public static void RegenComaOrDeathrest(Pawn pawn, Gene_Undead gene)
		{
			// Brain test
			if (pawn?.health?.hediffSet?.GetBrain() == null)
			{
				DuplicateUtility.NullifyBackstory(pawn);
				DuplicateUtility.NullifySkills(pawn);
			}
			// Undead Resurrect
			if (!TryResurrectWithSickness(pawn, WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected, true, 0.77f))
			{
				return;
			}
			pawn.health.AddHediff(WVC_GenesDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_GenesDefOf.WVC_UndeadResurrection, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			// Evolve
			XenotypeDef mainXenotype = pawn.genes.CustomXenotype == null ? pawn.genes.Xenotype : null;
			string letterDesc = "WVC_LetterTextSecondChance_GeneUndead";
			SubXenotypeUtility.XenotypeShapeshifter(pawn);
			if (mainXenotype != null && mainXenotype != pawn.genes.Xenotype)
			{
				letterDesc = "WVC_LetterTextSecondChance_GeneUndeadShapeshift";
			}
			// Letter
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				string shapeshiftXenotype = pawn?.genes?.Xenotype != null ? pawn.genes.Xenotype.LabelCap : "ERROR";
				Find.LetterStack.ReceiveLetter(gene.LabelCap, letterDesc.Translate(pawn.Named("PAWN"), gene.LabelCap, shapeshiftXenotype), WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(pawn));
			}
		}

		// Resurrection

		public static bool TryResurrectWithSickness(Pawn pawn, ThoughtDef resurrectThought = null, bool resurrectionSickness = true, float scarsChance = 0.2f)
		{
			ResurrectionParams resurrectionParams = new();
			resurrectionParams.restoreMissingParts = true;
			resurrectionParams.noLord = true;
			resurrectionParams.removeDiedThoughts = true;
			resurrectionParams.canPickUpOpportunisticWeapons = false;
			resurrectionParams.gettingScarsChance = scarsChance;
			resurrectionParams.canKidnap = false;
			resurrectionParams.canSteal = false;
			resurrectionParams.breachers = false;
			if (ResurrectionUtility.TryResurrect(pawn, resurrectionParams) == true)
			{
				if (resurrectionSickness)
				{
					pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
				}
				if (resurrectThought != null)
				{
					pawn.needs?.mood?.thoughts?.memories.TryGainMemory(resurrectThought);
				}
				return true;
			}
			else
			{
				Log.Error("Failed resurrect " + pawn.Name.ToString());
			}
			return false;
		}

		public static void ResurrectWithSickness(Pawn pawn, ThoughtDef resurrectThought = null)
		{
			TryResurrectWithSickness(pawn, resurrectThought);
		}

		// Resource

		public static void OffsetResurgentCells(Pawn pawn, float offset)
		{
			Gene_ResurgentCells gene_Hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Hemogen != null)
			{
				OffsetResource(gene_Hemogen, offset);
			}
		}

		// ResourceUtility
		public static void TickResourceDrain(IGeneResourceDrain drain, int tick = 1)
		{
			if (drain?.Resource != null && drain.CanOffset)
			{
				OffsetResource(drain, ((0f - drain.ResourceLossPerDay) / 60000f) * tick);
			}
		}

		public static void TickHemogenDrain(IGeneResourceDrain drain, int tick = 1)
		{
			// Log.Error("1 TickHemogenDrain " + tick.ToString() + " | 120");
			if (drain.Resource != null && drain.CanOffset)
			{
				// Log.Error("TickHemogenDrain ticks: " + tick.ToString());
				GeneResourceDrainUtility.OffsetResource(drain, ((0f - drain.ResourceLossPerDay) / 60000f) * tick);
			}
		}

		public static void OffsetResource(IGeneResourceDrain drain, float amnt)
		{
			// float value = drain.Resource.Value;
			drain.Resource.Value += amnt;
			// PostResourceOffset(drain, value);
		}

		public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
		{
			if (DebugSettings.ShowDevGizmos && drain.Resource != null)
			{
				Gene_Resource resource = drain.Resource;
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " -10",
					action = delegate
					{
						OffsetResource(drain, -0.1f);
					}
				};
				yield return command_Action;
				Command_Action command_Action2 = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " +10",
					action = delegate
					{
						OffsetResource(drain, 0.1f);
					}
				};
				yield return command_Action2;
			}
		}

		// public static void PostResourceOffset(IGeneResourceDrain drain, float oldValue)
		// {
		// if (oldValue > 0f && drain.Resource.Value <= 0f)
		// {
		// Pawn pawn = drain.Pawn;
		// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.ResurrectionSickness))
		// {
		// pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
		// }
		// }
		// }

		public static bool IsUndead(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Undead>() != null;
		}

		public static bool IsShapeshifter(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>() != null;
		}

		public static bool IsBloodeater(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Bloodeater>() != null;
		}

		public static bool GetUndeadGene(this Pawn pawn, out Gene_Undead gene)
		{
			return (gene = pawn?.genes?.GetFirstGeneOfType<Gene_Undead>()) != null;
		}

	}
}
