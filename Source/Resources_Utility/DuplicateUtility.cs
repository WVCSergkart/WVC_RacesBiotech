using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class DuplicateUtility
	{

		// Clone
		public static bool TryDuplicatePawn(Pawn caster, Pawn originalPawn, IntVec3 targetCell, Map map, out Pawn duplicatePawn, ref string customLetter, bool randomOutcome = false)
		{
			duplicatePawn = null;
			try
			{
				if (ModsConfig.AnomalyActive)
				{
					duplicatePawn = Find.PawnDuplicator.Duplicate(originalPawn);
					DuplicationOutcomes(caster, originalPawn, duplicatePawn, randomOutcome, ref customLetter);
				}
				else
				{
					PawnGenerationRequest request = DuplicateUtility.RequestCopy(originalPawn);
					duplicatePawn = PawnGenerator.GeneratePawn(request);
					DuplicatePawn(originalPawn, duplicatePawn, false);
				}
				map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(targetCell, map), targetCell, 60);
				SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(targetCell, map));
				GenSpawn.Spawn(duplicatePawn, targetCell, map);
			}
			catch (Exception arg)
			{
				Log.Error("Error during duplication of pawn " + originalPawn.NameShortColored.ToString() + ". Reason: " + arg);
			}
			return duplicatePawn != null;
		}

        public static void DuplicationOutcomes(Pawn caster, Pawn originalPawn, Pawn duplicatePawn, bool randomOutcome, ref string customLetter)
        {
			string phase = null;
            try
            {
				phase = "initial";
				if (!randomOutcome)
                {
                    return;
				}
				phase = "outcome randomazing";
				int num = Rand.RangeInclusive(0, 5);
				if (num == 1 && (duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecay) || duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecayCreepjoiner)))
                {
                    num++;
                }
                if (num == 2 && duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.CrumblingMind))
                {
                    num++;
                }
                switch (num)
                {
                    case 0:
						phase = "add duplicate sickness";
						AddDuplicateSickness(originalPawn, duplicatePawn);
                        break;
                    case 1:
						phase = "add organ decay";
						duplicatePawn.health.AddHediff(HediffDefOf.OrganDecayUndiagnosedDuplicaton);
                        break;
                    case 2:
						phase = "add crumbling mind";
						duplicatePawn.health.AddHediff(HediffDefOf.CrumblingMindUndiagnosedDuplication);
                        break;
                    case 3:
						phase = "nullify backstory";
						NullifyBackstory(duplicatePawn);
                        customLetter += "\n\n" + "WVC_XaG_GeneBackstoryDuplicateLetter".Translate(duplicatePawn.Named("PAWN"));
                        break;
                    case 4:
						phase = "randomize traits";
						int traitsCount = duplicatePawn.story.traits.allTraits.Count;
                        duplicatePawn.story?.traits?.allTraits?.RemoveAllTraits();
                        duplicatePawn.AddRandomTraits(traitsCount);
                        customLetter += "\n\n" + "WVC_XaG_GeneTraitsDuplicateLetter".Translate(duplicatePawn.Named("PAWN"));
                        break;
                    case 5:
						phase = "make hostile";
						duplicatePawn.SetFaction(Faction.OfEntities);
                        Lord newLord = LordMaker.MakeNewLord(Faction.OfEntities, new LordJob_AssaultColony(Faction.OfEntities, canKidnap: false, canTimeoutOrFlee: false, sappers: false, useAvoidGridSmart: false, canSteal: false), originalPawn.Map);
                        newLord.AddPawn(duplicatePawn);
                        //duplicatePawn.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.Berserk, null, forced: true);
                        customLetter = "WVC_XaG_GeneHostileDuplicateLetter".Translate(caster.Named("CASTER"), duplicatePawn.Named("PAWN"));
                        break;
                    default:
                        Log.Error("Unhandled outcome in pawn duplication " + num);
                        break;
                }
            }
            catch (Exception arg)
            {
				Log.Error("Failed create outcome for pawn " + duplicatePawn.NameShortColored.ToString() + ". On phase " + phase + ". Reason: " + arg);
            }
        }

        public static void AddDuplicateSickness(Pawn originalPawn, Pawn duplicatePawn)
		{
			Hediff_DuplicateSickness hediff_DuplicateSickness = (Hediff_DuplicateSickness)originalPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.DuplicateSickness);
			if (hediff_DuplicateSickness == null)
			{
				hediff_DuplicateSickness = (Hediff_DuplicateSickness)HediffMaker.MakeHediff(HediffDefOf.DuplicateSickness, originalPawn);
				originalPawn.health.AddHediff(hediff_DuplicateSickness);
			}
			else
			{
				hediff_DuplicateSickness.GetComp<HediffComp_SeverityPerDay>().severityPerDay = 0.1f;
			}
			Hediff_DuplicateSickness hediff = (Hediff_DuplicateSickness)HediffMaker.MakeHediff(HediffDefOf.DuplicateSickness, duplicatePawn);
			duplicatePawn.health.AddHediff(hediff);
		}

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

		public static void DuplicatePawn(Pawn progenitor, Pawn clone, bool addParent = true)
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
			if (addParent && clone.RaceProps.IsFlesh && progenitor.RaceProps.IsFlesh)
			{
				clone.relations.AddDirectRelation(PawnRelationDefOf.Parent, progenitor);
			}
			//if (xenotypeDef != null)
			//{
			//	clone.health.AddHediff(HediffDefOf.XenogerminationComa);
			//	GeneUtility.UpdateXenogermReplication(clone);
			//	ReimplanterUtility.ExtractXenogerm(progenitor);
			//	ReimplanterUtility.SetXenotype_DoubleXenotype(clone, xenotypeDef);
			//}
			// GestationUtility.GetBabyName(clone, progenitor);
			clone?.Drawer?.renderer?.SetAllGraphicsDirty();
			clone?.Notify_DisabledWorkTypesChanged();
		}

		public static void CopyGenes(Pawn pawn, Pawn newPawn)
        {
            ReimplanterUtility.ReimplantGenesHybrid(pawn, newPawn, xenogerm: false);
            CopyGenesOverrides(newPawn, newPawn.genes.Endogenes, pawn.genes.Endogenes);
			CopyGenesOverrides(newPawn, newPawn.genes.Xenogenes, pawn.genes.Xenogenes);
        }

		public static void CopyGenesOverrides(Pawn newPawn, List<Gene> newGenes, List<Gene> sourceGenes)
		{
			//int i;
			for (int i = 0; i < sourceGenes.Count; i++)
			{
				Gene gene = newGenes[i];
                try
				{
					if (sourceGenes[i].Overridden)
					{
						gene.overriddenByGene = newPawn.genes.GenesListForReading.First((Gene e) => e.def == sourceGenes[i].overriddenByGene.def);
					}
					else
					{
						gene.overriddenByGene = null;
					}
				}
				catch (Exception arg)
				{
					Log.Warning("Failed copy gene override for gene: " + gene.LabelCap + ". Reason: " + arg);
				}
			}
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
			//if (pawn.Spawned)
			//{
			//	return;
			//}
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
				trait.suppressedByGene = null;
				trait.sourceGene = null;
				trait.pawn?.story?.traits?.RemoveTrait(trait, true);
			}
		}

		public static void AddRandomTraits(this Pawn pawn, int traitsCount)
		{
			//int range = new IntRange(1,3).RandomInRange;
			int currentTry = 0;
			//List<TraitDef> traitsList = DefDatabase<TraitDef>.AllDefsListForReading;
			while (currentTry < traitsCount)
			{
				currentTry++;
				//if (traitsList.Where((TraitDef traitDef) => traitDef.GetGenderSpecificCommonality(pawn.gender) > 0f && pawn.story.traits.GetTrait(traitDef) == null).TryRandomElement(out TraitDef traitDef))
				//{
				//	Trait trait = new(traitDef);
				//	pawn.story?.traits?.GainTrait(trait);
				//}
				Trait trait = PawnGenerator.GenerateTraitsFor(pawn, 1, null, growthMomentTrait: false).FirstOrFallback();
				if (trait != null)
				{
					pawn.story.traits.GainTrait(trait);
				}
			}
		}

	}
}
