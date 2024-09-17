using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class DuplicateUtility
	{

		public static PawnGenerationRequest RequestCopy(Pawn pawn)
		{
			float ageBiologicalYearsFloat = pawn.ageTracker.AgeBiologicalYearsFloat;
			float num = pawn.ageTracker.AgeChronologicalYearsFloat;
			if (num > ageBiologicalYearsFloat)
			{
				num = ageBiologicalYearsFloat;
			}
			PawnGenerationRequest request = new (pawn.kindDef, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 0f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 0f, null, null, null, null, null, fixedGender: pawn.gender, fixedIdeo: pawn.Ideo, fixedBiologicalAge: ageBiologicalYearsFloat, fixedChronologicalAge: num, fixedLastName: GestationUtility.GetParentLastName(pawn), fixedBirthName: null, fixedTitle: null, forceNoIdeo: false, forceNoBackstory: true, forbidAnyTitle: false, forceDead: false, forcedXenogenes: null, forcedEndogenes: null, forcedXenotype: pawn.genes.Xenotype, forcedCustomXenotype: pawn.genes.CustomXenotype, allowedXenotypes: null, forceBaselinerChance: 0f, developmentalStages: DevelopmentalStage.Adult, pawnKindDefGetter: null, excludeBiologicalAgeRange: null, biologicalAgeRange: null, forceRecruitable: false, dontGiveWeapon: false, onlyUseForcedBackstories: false, maximumAgeTraits: 0, minimumAgeTraits: 0, forceNoGear: true);
			request.IsCreepJoiner = pawn.IsCreepJoiner;
			request.ForceNoIdeoGear = true;
			return request;
		}

		public static void DuplicatePawn(Pawn progenitor, Pawn clone, XenotypeDef xenotypeDef = null)
		{
			clone.apparel.DestroyAll();
			DuplicateUtility.CopyStory(progenitor, clone);
			DuplicateUtility.CopyApperance(progenitor, clone);
			DuplicateUtility.CopyTraits(progenitor, clone);
			DuplicateUtility.CopyGenes(progenitor, clone);
			DuplicateUtility.CopySkills(progenitor, clone);
			// DuplicateUtility.CopyNeeds(progenitor, clone);
			if (ModsConfig.IdeologyActive)
			{
				clone.ideo.SetIdeo(progenitor.ideo.Ideo);
			}
			if (clone.playerSettings != null && progenitor.playerSettings != null)
			{
				clone.playerSettings.AreaRestrictionInPawnCurrentMap = progenitor.playerSettings.AreaRestrictionInPawnCurrentMap;
			}
			if (clone.RaceProps.IsFlesh && progenitor.RaceProps.IsFlesh)
			{
				clone.relations.AddDirectRelation(PawnRelationDefOf.Parent, progenitor);
			}
			if (xenotypeDef != null)
			{
				clone.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(clone);
				ReimplanterUtility.ExtractXenogerm(progenitor);
				ReimplanterUtility.SetXenotype_DoubleXenotype(clone, xenotypeDef);
			}
			// GestationUtility.GetBabyName(clone, progenitor);
		}

		public static void CopyGenes(Pawn pawn, Pawn newPawn)
		{
			newPawn.genes.Endogenes.RemoveAllGenes();
			List<Gene> sourceEndogenes = pawn.genes.Endogenes;
			foreach (Gene item in sourceEndogenes)
			{
				newPawn.genes.AddGene(item.def, xenogene: false);
			}
			int j;
			for (j = 0; j < sourceEndogenes.Count; j++)
			{
				Gene gene = newPawn.genes.Endogenes[j];
				if (sourceEndogenes[j].Overridden)
				{
					gene.overriddenByGene = newPawn.genes.Endogenes.First((Gene e) => e.def == sourceEndogenes[j].overriddenByGene.def);
				}
				else
				{
					gene.overriddenByGene = null;
				}
			}
			newPawn.genes.Xenogenes.RemoveAllGenes();
			List<Gene> sourceXenogenes = pawn.genes.Xenogenes;
			foreach (Gene item2 in sourceXenogenes)
			{
				newPawn.genes.AddGene(item2.def, xenogene: true);
			}
			int i;
			for (i = 0; i < sourceXenogenes.Count; i++)
			{
				Gene gene2 = newPawn.genes.Xenogenes[i];
				if (sourceXenogenes[i].Overridden)
				{
					gene2.overriddenByGene = newPawn.genes.Xenogenes.First((Gene e) => e.def == sourceXenogenes[i].overriddenByGene.def);
				}
				else
				{
					gene2.overriddenByGene = null;
				}
			}
			ReimplanterUtility.SetXenotypeDirect(pawn, newPawn);
		}

		// Broke stuff
		public static void CopyNeeds(Pawn pawn, Pawn newPawn)
		{
			newPawn.needs.AllNeeds.Clear();
			foreach (Need allNeed in pawn.needs.AllNeeds)
			{
				Need need = (Need)Activator.CreateInstance(allNeed.def.needClass, newPawn);
				need.def = allNeed.def;
				newPawn.needs.AllNeeds.Add(need);
				need.SetInitialLevel();
				need.CurLevel = allNeed.CurLevel;
				newPawn.needs.BindDirectNeedFields();
			}
			if (pawn.needs.mood == null)
			{
				return;
			}
			List<Thought_Memory> memories = newPawn.needs.mood.thoughts.memories.Memories;
			memories.Clear();
			foreach (Thought_Memory memory in pawn.needs.mood.thoughts.memories.Memories)
			{
				Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(memory.def);
				thought_Memory.CopyFrom(memory);
				thought_Memory.pawn = newPawn;
				memories.Add(thought_Memory);
			}
		}

		public static void CopyStory(Pawn pawn, Pawn newPawn)
		{
			newPawn.gender = pawn.gender;
			// newPawn.Name = NameTriple.FromString(pawn.Name.ToString());
			newPawn.story.favoriteColor = pawn.story.favoriteColor;
			newPawn.story.Childhood = pawn.story.Childhood;
			newPawn.story.Adulthood = pawn.story.Adulthood;
		}

		public static void CopyTraits(Pawn pawn, Pawn newPawn)
		{
			newPawn.story.traits.allTraits.RemoveAllTraits();
			newPawn.story.traits.allTraits.Clear();
			foreach (Trait allTrait in pawn.story.traits.allTraits)
			{
				if (allTrait.sourceGene == null && allTrait.def.GetGenderSpecificCommonality(newPawn.gender) > 0f)
				{
					newPawn.story.traits.GainTrait(new Trait(allTrait.def, allTrait.Degree, allTrait.ScenForced));
				}
			}
		}

		public static void CopyApperance(Pawn pawn, Pawn newPawn)
		{
			newPawn.story.headType = pawn.story.headType;
			newPawn.story.bodyType = pawn.story.bodyType;
			newPawn.story.hairDef = pawn.story.hairDef;
			newPawn.story.HairColor = pawn.story.HairColor;
			newPawn.story.SkinColorBase = pawn.story.SkinColorBase;
			newPawn.story.skinColorOverride = pawn.story.skinColorOverride;
			newPawn.story.furDef = pawn.story.furDef;
			newPawn.style.beardDef = pawn.style.beardDef;
			if (ModsConfig.IdeologyActive)
			{
				newPawn.style.BodyTattoo = pawn.style.BodyTattoo;
				newPawn.style.FaceTattoo = pawn.style.FaceTattoo;
			}
		}

		public static void CopySkills(Pawn pawn, Pawn newPawn)
		{
			newPawn.skills.skills.Clear();
			foreach (SkillRecord skill in pawn.skills.skills)
			{
				SkillRecord item = new(newPawn, skill.def)
				{
					levelInt = skill.levelInt,
					passion = skill.passion,
					xpSinceLastLevel = skill.xpSinceLastLevel,
					xpSinceMidnight = skill.xpSinceMidnight
				};
				newPawn.skills.skills.Add(item);
			}
		}

		public static void NullifySkills(Pawn pawn)
		{
			foreach (SkillRecord skill in pawn.skills.skills.ToList())
			{
				pawn.skills.skills.Remove(skill);
				SkillRecord item = new(pawn, skill.def)
				{
					levelInt = 0,
					passion = skill.passion,
					xpSinceLastLevel = 0,
					xpSinceMidnight = 0
				};
				pawn.skills.skills.Add(item);
			}
		}

		public static void NullifyXenotype(Pawn pawn)
		{
			// remove all genes
			Pawn_GeneTracker genes = pawn.genes;
			genes.Xenogenes.RemoveAllGenes();
			genes.Endogenes.RemoveAllGenes();
			// foreach (Gene item in genes.Xenogenes.ToList())
			// {
				// pawn.genes?.RemoveGene(item);
			// }
			// foreach (Gene item in genes.Endogenes.ToList())
			// {
				// pawn.genes?.RemoveGene(item);
			// }
			ReimplanterUtility.SetXenotypeDirect(null, pawn, XenotypeDefOf.Baseliner, true);
			FloatRange floatRange = new(0f, 1f);
			pawn.genes.InitializeGenesFromOldSave(floatRange.RandomInRange);
		}

		public static void NullifyBackstory(Pawn pawn)
		{
			if (pawn.Spawned)
			{
				return;
			}
			if (pawn.story.Childhood != null)
			{
				pawn.story.Childhood = WVC_GenesDefOf.WVC_RacesBiotech_Amnesia_Child;
			}
			if (pawn.story.Adulthood != null)
			{
				pawn.story.Adulthood = WVC_GenesDefOf.WVC_RacesBiotech_Amnesia_Adult;
			}
		}

		public static void RemoveAllGenes(this List<Gene> genes)
		{
			foreach (Gene gene in genes.ToList())
			{
				gene.pawn?.genes?.RemoveGene(gene);
			}
		}

		public static void RemoveAllGenes_Overridden(Pawn pawn)
		{
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene.Overridden)
				{
					pawn.genes?.RemoveGene(gene);
				}
			}
		}

		public static void RemoveAllTraits(this List<Trait> traits)
		{
			foreach (Trait trait in traits.ToList())
			{
				trait.pawn?.story?.traits?.RemoveTrait(trait, true);
			}
		}

	}
}
