using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class UndeadUtility
	{

		// Dust

		public static void OffsetNeedFood(Pawn pawn, float offset)
		{
			Need_Food need_Food = pawn.needs?.food;
			if (need_Food != null)
			{
				need_Food.CurLevel += offset;
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

		public static void ReimplantGenes(Pawn caster, Pawn recipient)
		{
			ReimplanterUtility.Reimplanter(caster, recipient);
			if (PawnUtility.ShouldSendNotificationAbout(recipient))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(recipient.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(recipient));
			}
		}

		// Shape

		public static void AddRandomTraitFromListWithChance(Pawn pawn, GeneExtension_Undead geneExtension)
		{
			TraitSet traitSet = pawn.story.traits;
			if (geneExtension == null || traitSet.allTraits.Count > 1)
			{
				return;
			}
			GeneExtension_Undead.TraitDefWithWeight traitDefWithWeight = geneExtension.possibleTraits.RandomElementByWeight((GeneExtension_Undead.TraitDefWithWeight x) => x.weight);
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

		// public static bool TraitDefWithWeight_HasAnyConflicts(List<TraitDef> traitDefs, List<Trait> traits)
		// {
			// foreach (TraitDef item in traitDefs)
			// {
				// if (MiscUtility.TraitHasAnyConflicts(traits, item))
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		// public static List<TraitDef> GetAllTraitsFrom_TraitDefWithWeight(GeneExtension_Shapeshifter geneExtension)
		// {
			// List<TraitDef> traits = new();
			// foreach (GeneExtension_Shapeshifter.TraitDefWithWeight traitDefWithWeight in geneExtension.possibleTraits)
			// {
				// traits.Add(traitDefWithWeight.traitDef);
			// }
			// return traits;
		// }

		// Clone
		public static bool TryDuplicatePawn(Pawn progenitor, Gene gene = null, XenotypeDef xenotypeDef = null, bool duplicateMode = false)
		{
			if (gene == null || progenitor == null || xenotypeDef == null || duplicateMode == false)
			{
				return false;
			}
			PawnGenerationRequest generateNewBornPawn = new(progenitor.kindDef, progenitor.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, fixedChronologicalAge: progenitor.ageTracker.AgeBiologicalTicks, fixedGender: progenitor.gender, fixedBiologicalAge: progenitor.ageTracker.AgeBiologicalTicks, forceNoIdeo: false, forceNoBackstory: true, forbidAnyTitle: true, forceDead: false, forcedXenotype: progenitor.genes.Xenotype, developmentalStages: progenitor.DevelopmentalStage);
			Pawn clone = PawnGenerator.GeneratePawn(generateNewBornPawn);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(clone, progenitor))
			{
				Shapeshift_ClonePawn(progenitor, clone);
				clone.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(clone);
				ReimplanterUtility.ExtractXenogerm(progenitor);
				ReimplanterUtility.SetXenotype_DoubleXenotype(clone, xenotypeDef);
				if (!clone.genes.HasGene(gene.def))
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
				SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(progenitor));
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

		public static void Shapeshift_ClonePawn(Pawn progenitor, Pawn clone)
		{
			clone.apparel.DestroyAll();
			clone.ageTracker.AgeBiologicalTicks = progenitor.ageTracker.AgeBiologicalTicks;
			clone.ageTracker.AgeChronologicalTicks = 0L;
			clone.gender = progenitor.gender;
			clone.story.Childhood = WVC_GenesDefOf.WVC_XaG_Shapeshifter0_Child;
			clone.story.headType = progenitor.story.headType;
			clone.story.bodyType = progenitor.story.bodyType;
			clone.story.hairDef = progenitor.story.hairDef;
			clone.story.favoriteColor = progenitor.story.favoriteColor;
			clone.style.beardDef = progenitor.style.beardDef;
			// clone.style.FaceTattoo = null;
			// clone.style.BodyTattoo = null;
			clone.style.SetupTattoos_NoIdeology();
			MiscUtility.TransferTraits(clone, progenitor);
			MiscUtility.TransferSkills(clone, progenitor);
			if (clone.ideo != null)
			{
				clone.ideo.SetIdeo(progenitor.ideo.Ideo);
			}
			if (clone.playerSettings != null && progenitor.playerSettings != null)
			{
				clone.playerSettings.AreaRestriction = progenitor.playerSettings.AreaRestriction;
			}
			if (clone.RaceProps.IsFlesh && progenitor.RaceProps.IsFlesh)
			{
				clone.relations.AddDirectRelation(PawnRelationDefOf.Parent, progenitor);
			}
			if (progenitor.Spawned)
			{
				progenitor.GetLord()?.AddPawn(clone);
			}
			GestationUtility.GetBabyName(clone, progenitor);
		}

		// Coma TEST

		[Obsolete]
		public static void ShouldUndeadRegenComaOrDeathrest(bool resurrect, Pawn pawn)
		{
			if (resurrect)
			{
				Gene_Undead undead = pawn?.genes?.GetFirstGeneOfType<Gene_Undead>();
				if (undead != null)
				{
					RegenComaOrDeathrest(pawn, undead);
				}
			}
		}

		public static void RegenComaOrDeathrest(Pawn pawn, Gene_Undead gene)
		{
			// Brain test
			if (pawn?.health?.hediffSet?.GetBrain() == null)
			{
				Gene_BackstoryChanger.BackstoryChanger(pawn, gene.Giver.childhoodDef, gene.Giver.adulthoodDef);
				foreach (SkillRecord item in pawn.skills.skills)
				{
					if (!item.TotallyDisabled && item.XpTotalEarned > 0f)
					{
						float num = item.XpTotalEarned;
						item.Learn(0f - num, direct: true);
					}
				}
			}
			// Undead Resurrect
			ResurrectWithSickness(pawn, WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
			// ResurrectionUtility.Resurrect(pawn);
			// pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
			pawn.health.AddHediff(WVC_GenesDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
			// pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
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

		public static void ResurrectWithSickness(Pawn pawn, ThoughtDef resurrectThought = null)
		{
			ResurrectionUtility.Resurrect(pawn);
			pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
			if (resurrectThought != null)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(resurrectThought);
			}
		}

		[Obsolete]
		public static void NewUndeadResurrect(Pawn pawn, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null, Gene_ResurgentCells resurgentGene = null, float resourceLossPerDay = 0f)
		{
			ResurrectWithSickness(pawn, WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
			if (resurgentGene == null)
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
				Gene_BackstoryChanger.BackstoryChanger(pawn, childBackstoryDef, adultBackstoryDef);
				foreach (SkillRecord item in pawn.skills.skills)
				{
					if (!item.TotallyDisabled && item.XpTotalEarned > 0f)
					{
						float num = item.XpTotalEarned;
						item.Learn(0f - num, direct: true);
					}
				}
				SubXenotypeUtility.XenotypeShapeshifter(pawn);
			}
			else
			{
				resurgentGene.Value -= resourceLossPerDay;
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndeadResurgent".Translate(pawn.Named("TARGET")), WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(pawn));
			}
		}

		public static void OffsetResurgentCells(Pawn pawn, float offset)
		{
			if (!ModsConfig.BiotechActive)
			{
				return;
			}
			Gene_ResurgentCells gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Hemogen != null)
			{
				gene_Hemogen.Value += offset;
			}
		}

		// ResourceUtility
		public static void TickResourceDrain(IGeneResourceDrain drain)
		{
			if (drain.CanOffset && drain.Resource != null)
			{
				OffsetResource(drain, (0f - drain.ResourceLossPerDay) / 60000f);
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

		public static bool GetUndeadGene(this Pawn pawn, out Gene_Undead gene)
		{
			gene = pawn?.genes?.GetFirstGeneOfType<Gene_Undead>();
			return gene != null;
		}

	}
}
