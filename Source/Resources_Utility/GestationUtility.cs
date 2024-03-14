using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    // [StaticConstructorOnStartup]
	public static class GestationUtility
	{

		public static void GenerateNewBornPawn(Pawn pawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true, bool xenogeneTransfer = true, Pawn spawnPawn = null)
		{
			Pawn pawnParent = pawn;
			Pawn pawnTarget = spawnPawn;
			if (pawnTarget == null)
			{
				pawnTarget = pawn;
			}
			int litterSize = ((pawnParent.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawnParent.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			PawnGenerationRequest generateNewBornPawn = new(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			for (int i = 0; i < litterSize; i++)
			{
				Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
				GeneTransfer(pawnNewBornChild, pawnParent, endogeneTransfer, xenogeneTransfer);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnTarget))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestrictionInPawnCurrentMap = pawnParent.playerSettings.AreaRestrictionInPawnCurrentMap;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
					GetBabyName(pawnNewBornChild, pawnParent);
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (pawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnTarget.Position, pawnTarget.Map, ThingDefOf.Filth_AmnioticFluid, pawnTarget.LabelIndefinite(), 5);
				WVC_GenesDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(pawnTarget));
				if (pawnTarget.caller != null)
				{
					pawnTarget.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawnParent))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_XenoTreeBirthLabel".Translate(), completeMessage.Translate(pawnParent.LabelShort.Colorize(ColoredText.NameColor)), WVC_GenesDefOf.WVC_XaG_GestationEvent, new LookTargets(pawn));
			}
			// Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
		}

		public static void GeneTransfer(Pawn pawnNewBornChild, Pawn pawnParent, bool endogeneTransfer = true, bool xenogeneTransfer = true)
		{
			if (endogeneTransfer)
			{
				for (int numEndogenes = pawnNewBornChild.genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawnNewBornChild.genes.RemoveGene(pawnNewBornChild.genes.Endogenes[numEndogenes]);
				}
				List<Gene> list = pawnParent.genes?.Endogenes;
				foreach (Gene item in list)
				{
					pawnNewBornChild.genes?.AddGene(item.def, xenogene: false);
				}
			}
			if (xenogeneTransfer)
			{
				for (int numXenogenes = pawnNewBornChild.genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawnNewBornChild.genes.RemoveGene(pawnNewBornChild.genes.Xenogenes[numXenogenes]);
				}
				List<Gene> list = pawnParent.genes?.Xenogenes;
				foreach (Gene item in list)
				{
					pawnNewBornChild.genes?.AddGene(item.def, xenogene: true);
				}
			}
			// if (pawnParent.genes?.Xenotype != null)
			// {
			// pawnNewBornChild.genes?.SetXenotypeDirect(pawnParent.genes?.Xenotype);
			// }
			pawnNewBornChild.genes?.SetXenotypeDirect(pawnParent.genes?.Xenotype);
			pawnNewBornChild.genes.xenotypeName = pawnParent.genes.xenotypeName;
			pawnNewBornChild.genes.iconDef = pawnParent.genes.iconDef;
		}

		public static void GenerateNewBornPawn_Thing(Pawn pawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true, bool xenogeneTransfer = true, Thing spawnTarget = null)
		{
			Pawn pawnParent = pawn;
			Thing pawnTarget = spawnTarget;
			if (pawnTarget == null)
			{
				// pawnTarget = pawn;
				return;
			}
			int litterSize = ((pawnParent.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawnParent.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			PawnGenerationRequest generateNewBornPawn = new(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			for (int i = 0; i < litterSize; i++)
			{
				Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
				GeneTransfer(pawnNewBornChild, pawnParent, endogeneTransfer, xenogeneTransfer);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnTarget))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestrictionInPawnCurrentMap = pawnParent.playerSettings.AreaRestrictionInPawnCurrentMap;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
					GetBabyName(pawnNewBornChild, pawnParent);
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (pawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnTarget.Position, pawnTarget.Map, ThingDefOf.Filth_Slime, 5);
				WVC_GenesDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(pawnTarget));
				// if (pawn.caller != null)
				// {
				// pawn.caller.DoCall();
				// }
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawnParent))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_XenoTreeBirthLabel".Translate(), completeMessage.Translate(pawnParent.LabelShort.Colorize(ColoredText.NameColor)), WVC_GenesDefOf.WVC_XaG_GestationEvent, new LookTargets(pawn));
			}
		}

		// Xeno-Tree Spawner

		public static void GenerateNewBornPawn_WithChosenXenotype(Thing spawnTarget, XenotypeDef xenotypeDef, string completeLetterLabel, string completeLetterDesc, bool xenogerminationComa = false)
		{
			if (spawnTarget == null || xenotypeDef == null)
			{
				return;
			}
			PawnGenerationRequest generateNewBornPawn = new(PawnKindDefOf.Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawnNewBornChild, xenotypeDef);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, spawnTarget))
			{
				Pawn pawnParent = spawnTarget is Pawn ? spawnTarget as Pawn : null;
				GetBabyName(pawnNewBornChild, pawnParent);
				if (xenogerminationComa)
				{
					pawnNewBornChild.health.AddHediff(HediffDefOf.XenogerminationComa);
					GeneUtility.UpdateXenogermReplication(pawnNewBornChild);
				}
				if (pawnParent != null)
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestrictionInPawnCurrentMap = pawnParent.playerSettings.AreaRestrictionInPawnCurrentMap;
					}
					if (pawnNewBornChild.RaceProps.IsFlesh)
					{
						pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
					}
					if (pawnParent.Spawned)
					{
						pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
					}
				}
			}
			else
			{
				Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
			}
			if (spawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(spawnTarget.Position, spawnTarget.Map, ThingDefOf.Filth_Slime, 5);
				WVC_GenesDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(spawnTarget));
				if (pawnNewBornChild.caller != null)
				{
					pawnNewBornChild.caller.DoCall();
				}
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawnNewBornChild))
			{
				Find.LetterStack.ReceiveLetter(completeLetterLabel.Translate(), completeLetterDesc.Translate(spawnTarget.LabelShortCap.Colorize(ColoredText.NameColor)), WVC_GenesDefOf.WVC_XaG_GestationEvent, new LookTargets(pawnNewBornChild));
			}
		}

		public static void GetBabyName(Pawn baby, Pawn parent)
		{
			baby.Name = PawnBioAndNameGenerator.GenerateFullPawnName(baby.def, baby.kindDef.GetNameMaker(baby.gender), baby.story, null, baby.RaceProps.GetNameGenerator(baby.gender), baby.Faction?.ideos?.PrimaryCulture, false, gender: baby.gender, nameCategory: baby.RaceProps.nameCategory, forcedLastName: GetParentLastName(parent), false);
			// if (baby.Name is NameTriple nameTriple)
			// {
				// if (parent != null && parent.Name is NameTriple parentNameTriple)
				// {
					// baby.Name = new NameTriple(nameTriple.First, null, parentNameTriple.Last);
				// }
			// }
		}

		public static string GetParentLastName(Pawn parent)
		{
			if (parent != null && parent.Name is NameTriple parentNameTriple)
			{
				return parentNameTriple.Last;
			}
			return null;
		}

		// Shapeshifter Overclock

		// public static void DuplicatePawn_WithChosenXenotype(Gene gene, XenotypeDef xenotypeDef)
		// {
			// if (gene == null || gene.pawn == null || xenotypeDef == null)
			// {
				// return;
			// }
			// PawnGenerationRequest generateNewBornPawn = new(gene.pawn.kindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, gene.pawn.DevelopmentalStage);
			// Pawn clone = PawnGenerator.GeneratePawn(generateNewBornPawn);
			// if (PawnUtility.TrySpawnHatchedOrBornPawn(clone, gene.pawn))
			// {
				// if (!SerumUtility.HasCandidateGene(clone))
				// {
					// clone.health.AddHediff(HediffDefOf.XenogerminationComa);
				// }
				// GeneUtility.UpdateXenogermReplication(clone);
				// ReimplanterUtility.ExtractXenogerm(gene.pawn);
				// if (gene.pawn != null)
				// {
					// clone.ageTracker.AgeBiologicalTicks = gene.pawn.ageTracker.AgeBiologicalTicks;
					// clone.ageTracker.AgeChronologicalTicks = 0L;
					// clone.gender = gene.pawn.gender;
					// clone.story.Childhood = gene.pawn.story.Childhood;
					// clone.story.Adulthood = gene.pawn.story.Adulthood;
					// clone.story.traits.allTraits = gene.pawn.story.traits.allTraits;
					// clone.story.headType = gene.pawn.story.headType;
					// clone.story.bodyType = gene.pawn.story.bodyType;
					// clone.story.hairDef = gene.pawn.story.hairDef;
					// clone.story.favoriteColor = gene.pawn.story.favoriteColor;
					// clone.Name = gene.pawn.Name;
					// if (clone.skills != null && clone.skills.skills.NullOrEmpty())
					// {
						// List<SkillRecord> cloneSkills = clone.skills.skills;
						// List<SkillRecord> sourceSkills = gene.pawn.skills.skills;
						// for (int i = 0; i < cloneSkills.Count; i++)
						// {
							// if (cloneSkills[i].TotallyDisabled)
							// {
								// continue;
							// }
							// for (int j = 0; j < sourceSkills.Count; j++)
							// {
								// if (sourceSkills[j].TotallyDisabled)
								// {
									// continue;
								// }
								// if (cloneSkills[i].def == sourceSkills[j].def)
								// {
									// cloneSkills[i].passion = sourceSkills[j].passion;
									// cloneSkills[i].levelInt = sourceSkills[j].levelInt;
								// }
							// }
						// }
						// clone.skills.skills = gene.pawn.skills.skills;
					// }
					// if (clone.ideo != null)
					// {
						// clone.ideo.SetIdeo(gene.pawn.ideo.Ideo);
					// }
					// if (clone.playerSettings != null && gene.pawn.playerSettings != null)
					// {
						// clone.playerSettings.AreaRestriction = gene.pawn.playerSettings.AreaRestriction;
					// }
					// if (clone.RaceProps.IsFlesh && gene.pawn.RaceProps.IsFlesh)
					// {
						// clone.relations.AddDirectRelation(PawnRelationDefOf.Sibling, gene.pawn);
						// gene.pawn.relations.AddDirectRelation(PawnRelationDefOf.Sibling, clone);
					// }
					// if (gene.pawn.Spawned)
					// {
						// gene.pawn.GetLord()?.AddPawn(clone);
					// }
				// }
			// }
			// else
			// {
				// Find.WorldPawns.PassToWorld(clone, PawnDiscardDecideMode.Discard);
			// }
			// ReimplanterUtility.SetXenotype_DoubleXenotype(clone, xenotypeDef);
			// if (gene.pawn.Spawned)
			// {
				// FilthMaker.TryMakeFilth(gene.pawn.Position, gene.pawn.Map, ThingDefOf.Filth_Slime, 5);
				// SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(gene.pawn));
				// if (clone.caller != null)
				// {
					// clone.caller.DoCall();
				// }
			// }
			// if (PawnUtility.ShouldSendNotificationAbout(clone))
			// {
				// Find.LetterStack.ReceiveLetter("WVC_XaG_GeneShapeshifter_DuplicateLetterLabel".Translate(), "WVC_XaG_GeneShapeshifter_DuplicateLetterDesc".Translate(gene.pawn.Named("TARGET"), xenotypeDef.LabelCap, gene.LabelCap)
				// + "\n\n" + (xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.description : xenotypeDef.descriptionShort),
				// WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(gene.pawn));
			// }
		// }

	}
}
