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
		public static bool TryDuplicatePawn(Pawn caster, Pawn originalPawn, IntVec3 targetCell, Map map, out Pawn duplicatePawn, out string customLetter, out LetterDef letterDef, bool randomOutcome = false, bool doEffects = true)
		{
			duplicatePawn = null;
			customLetter = null;
			letterDef = null;
			try
			{
				if (ModsConfig.AnomalyActive)
				{
					duplicatePawn = Find.PawnDuplicator.Duplicate(originalPawn);
					DuplicationOutcomes(caster, originalPawn, duplicatePawn, randomOutcome, out customLetter, out letterDef);
				}
				else
				{
					PawnGenerationRequest request = RequestCopy(originalPawn);
					duplicatePawn = PawnGenerator.GeneratePawn(request);
					DuplicatePawn(originalPawn, duplicatePawn, false);
					letterDef = LetterDefOf.PositiveEvent;
					customLetter = "WVC_XaG_GeneDuplicationLetter".Translate(caster.Named("CASTER"), originalPawn.Named("PAWN"));
				}
				if (doEffects)
				{
					//map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(targetCell, map), targetCell, 60);
					//SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(targetCell, map));
					MiscUtility.DoSkipEffects(targetCell, map);
				}
				GenSpawn.Spawn(duplicatePawn, targetCell, map);
			}
			catch (Exception arg)
			{
				Log.Error("Error during duplication of pawn " + originalPawn.NameShortColored.ToString() + ". Reason: " + arg);
			}
			return duplicatePawn != null;
		}

		public static void DuplicationOutcomes(Pawn caster, Pawn originalPawn, Pawn duplicatePawn, bool randomOutcome, out string customLetter, out LetterDef letterDef)
		{
			string phase = null;
			letterDef = LetterDefOf.NeutralEvent;
			customLetter = "WVC_XaG_GeneDuplicationLetter".Translate(caster.Named("CASTER"), originalPawn.Named("PAWN"));
			try
			{
				phase = "initial";
				if (!randomOutcome)
				{
					letterDef = LetterDefOf.PositiveEvent;
					return;
				}
				phase = "outcome randomizing";
				int num = Rand.RangeInclusive(0, 6);
				if (num == 1 && (duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecay) || duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.OrganDecayCreepjoiner)))
				{
					num++;
				}
				if (num == 2 && duplicatePawn.health.hediffSet.HasHediffOrWillBecome(HediffDefOf.CrumblingMind))
				{
					num++;
				}
				HediffUtility.TryGetBestMutation(duplicatePawn, out HediffDef mutation);
                if (num == 3 && mutation == null)
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
						phase = "try mutate";
						TryMutate(duplicatePawn, ref customLetter, ref letterDef, mutation);
						break;
					case 4:
						phase = "randomize backstory";
						customLetter = RandomizeBackstory(duplicatePawn, customLetter);
						break;
					case 5:
						phase = "randomize traits";
						customLetter = RandomizeTraits(duplicatePawn, customLetter);
						break;
					case 6:
						phase = "make hostile";
						customLetter = MakeHostile(caster, originalPawn, duplicatePawn, out letterDef);
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

        private static void TryMutate(Pawn duplicatePawn, ref string customLetter, ref LetterDef letterDef, HediffDef mutation)
        {
            if (FleshbeastUtility.TryGiveMutation(duplicatePawn, mutation))
            {
                customLetter += "\n\n" + "WVC_XaG_GeneDuplicatorMutationLetter".Translate(duplicatePawn.Named("PAWN"));
            }
            else
            {
                letterDef = LetterDefOf.PositiveEvent;
            }
        }

        private static string RandomizeBackstory(Pawn duplicatePawn, string customLetter)
        {
            if (TryGetRandomBackstory(duplicatePawn, out BackstoryDef childstory, out BackstoryDef adultstory))
            {
                duplicatePawn.story.Childhood = childstory;
				if (adultstory != null)
				{
					duplicatePawn.story.Adulthood = adultstory;
				}
			}
			else
			{
				NullifyBackstory(duplicatePawn);
			}
			customLetter += "\n\n" + "WVC_XaG_GeneBackstoryDuplicateLetter".Translate(duplicatePawn.Named("PAWN"));
            return customLetter;
        }

		private static string RandomizeTraits(Pawn duplicatePawn, string customLetter)
		{
			List<Trait> allTraits = duplicatePawn.story.traits.allTraits;
			int traitsCount = 0;
			foreach (Trait trait in allTraits.ToList())
			{
				if (trait.sourceGene == null)
				{
					//trait.suppressedByGene = null;
					//trait.sourceGene = null;
					//trait.suppressedByTrait = false;
					//trait.pawn?.story?.traits?.RemoveTrait(trait, true);
					trait.RemoveTrait(trait.pawn);
					traitsCount++;
				}
			}
			duplicatePawn.AddRandomTraits(traitsCount);
			customLetter += "\n\n" + "WVC_XaG_GeneTraitsDuplicateLetter".Translate(duplicatePawn.Named("PAWN"));
			return customLetter;
		}

        private static string MakeHostile(Pawn caster, Pawn originalPawn, Pawn duplicatePawn, out LetterDef letterDef)
		{
			string customLetter = "WVC_XaG_GeneHostileDuplicateLetter".Translate(caster.Named("CASTER"), duplicatePawn.Named("PAWN"));
			letterDef = LetterDefOf.ThreatBig;
			if (!duplicatePawn.WorkTagIsDisabled(WorkTags.Violent))
			{
				duplicatePawn.SetFaction(Faction.OfEntities);
				Lord newLord = LordMaker.MakeNewLord(Faction.OfEntities, new LordJob_AssaultColony(Faction.OfEntities, canKidnap: false, canTimeoutOrFlee: false, sappers: false, useAvoidGridSmart: false, canSteal: false), originalPawn.Map);
				newLord.AddPawn(duplicatePawn);
			}
			else if (!duplicatePawn.Inhumanized() && duplicatePawn.mindState.mentalBreaker.TryDoMentalBreak("WVC_XaG_MentalBreakReason_Duplicator".Translate(), MentalBreakDefOf.HumanityBreak))
			{
				customLetter = "WVC_XaG_GeneInhumanDuplicateLetter".Translate(caster.Named("CASTER"), duplicatePawn.Named("PAWN"));
				letterDef = LetterDefOf.NeutralEvent;
			}
			else
			{
				duplicatePawn.mindState?.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.Berserk, null, forced: true);
			}
			return customLetter;
        }

        public static bool TryGetRandomBackstory(Pawn duplicatePawn, out BackstoryDef childstory, out BackstoryDef adultstory)
        {
			List<BackstoryDef> allDefsListForReading = DefDatabase<BackstoryDef>.AllDefsListForReading.Where((BackstoryDef def) => def.shuffleable && !def.spawnCategories.NullOrEmpty()).ToList();
			if (duplicatePawn.story?.Childhood?.spawnCategories.NullOrEmpty() != false || !allDefsListForReading.Where((BackstoryDef def) => def != duplicatePawn.story.Childhood && def.spawnCategories.Contains(duplicatePawn.story?.Childhood?.spawnCategories?.First())).ToList().TryRandomElement(out childstory))
            {
				childstory = null;
			}
			if (duplicatePawn.story?.Adulthood?.spawnCategories.NullOrEmpty() != false || !allDefsListForReading.Where((BackstoryDef def) => def != duplicatePawn.story.Adulthood && def.spawnCategories.Contains(duplicatePawn.story?.Adulthood?.spawnCategories?.First())).ToList().TryRandomElement(out adultstory))
			{
				adultstory = null;
			}
			return childstory != null;
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
            CopyPawn(progenitor, clone);
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

        public static void CopyPawn(Pawn progenitor, Pawn clone)
        {
            CopyStory(progenitor, clone);
            CopyApperance(progenitor, clone);
            CopyTraits(progenitor, clone);
            CopyGenes(progenitor, clone);
            CopySkills(progenitor, clone);
			//CopyHediffs(progenitor, clone);
			//CopyNeeds(progenitor, clone);
		}

        public static void CopyGenes(Pawn pawn, Pawn newPawn)
        {
            ReimplanterUtility.GeneralReimplant(pawn, newPawn, xenogerm: false);
            CopyGenesOverrides(newPawn, newPawn.genes.Endogenes, pawn.genes.Endogenes);
			CopyGenesOverrides(newPawn, newPawn.genes.Xenogenes, pawn.genes.Xenogenes);
        }

		public static void CopyGenesOverrides(Pawn newPawn, List<Gene> newGenes, List<Gene> sourceGenes)
		{
			//int i;
			for (int i = 0; i < sourceGenes.Count; i++)
			{
				Gene gene = newGenes[i];
				Gene sourceGene = sourceGenes[i];
				try
				{
                    if (sourceGene.Overridden)
					{
						gene.overriddenByGene = GetOverriderGene(newPawn.genes.GenesListForReading, sourceGene, gene);
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

		public static Gene GetOverriderGene(List<Gene> genes, Gene sourceGene, Gene newGene)
		{
			foreach (Gene item in genes)
            {
				if ((newGene != item || sourceGene.overriddenByGene == sourceGene) && item.def == sourceGene.overriddenByGene.def)
                {
					return item;
                }
            }
			return null;
		}

		public static void CopyNeeds(Pawn pawn, Pawn newPawn)
		{
			// Broke stuff
			//newPawn.needs.AllNeeds.Clear();
			//foreach (Need allNeed in pawn.needs.AllNeeds)
			//{
			//	Need need = (Need)Activator.CreateInstance(allNeed.def.needClass, newPawn);
			//	need.def = allNeed.def;
			//	newPawn.needs.AllNeeds.Add(need);
			//	need.SetInitialLevel();
			//	need.CurLevel = allNeed.CurLevel;
			//	newPawn.needs.BindDirectNeedFields();
			//}
			//if (pawn.needs.mood == null)
			//{
			//	return;
			//}
			//List<Thought_Memory> memories = newPawn.needs.mood.thoughts.memories.Memories;
			//memories.Clear();
			//foreach (Thought_Memory memory in pawn.needs.mood.thoughts.memories.Memories)
			//{
			//	Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(memory.def);
			//	thought_Memory.CopyFrom(memory);
			//	thought_Memory.pawn = newPawn;
			//	memories.Add(thought_Memory);
			//}
			if (pawn?.needs == null || newPawn?.needs == null)
            {
				return;
            }
			foreach (Need need in pawn.needs.AllNeeds)
			{
				foreach (Need newNeed in newPawn.needs.AllNeeds)
				{
					if (need.def == newNeed.def)
					{
						newNeed.CurLevel = need.CurLevel;
					}
				}
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

		public static void CopyHediffs(Pawn pawn, Pawn newPawn)
		{
			//newPawn.health.hediffSet.hediffs.Clear();
			foreach (Hediff item in newPawn.health.hediffSet.hediffs)
			{
				if (item.def.duplicationAllowed)
				{
					newPawn.health.RemoveHediff(item);
				}
			}
			foreach (Hediff item in pawn.health.hediffSet.hediffs)
			{
				if (item.def.duplicationAllowed)
				{
					Hediff hediff = HediffMaker.MakeHediff(item.def, newPawn, item.Part);
					hediff.CopyFrom(item);
					newPawn.health.hediffSet.AddDirect(hediff);
				}
			}
		}

		public static void SetSkills(Pawn pawn, List<SkillRange> skills)
		{
			foreach (SkillRange skillRange in skills)
			{
				foreach (SkillRecord skill in pawn.skills.skills.ToList())
				{
					if (skillRange.Skill != skill.def)
                    {
						continue;
                    }
					pawn.skills.skills.Remove(skill);
					SkillRecord item = new(pawn, skill.def)
					{
						levelInt = skillRange.Range.RandomInRange,
						passion = Passion.None,
						xpSinceLastLevel = 10,
						xpSinceMidnight = 0
					};
					if (skillRange.Range.min > 8)
					{
						item.passion = Passion.Minor;
					}
					if (skillRange.Range.min > 16)
					{
						item.passion = Passion.Major;
					}
					pawn.skills.skills.Add(item);
				}
			}
		}

		public static void NullifySkills(Pawn pawn, bool removePassion = false)
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
				if (removePassion)
                {
					item.passion = Passion.None;
				}
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
				pawn.story.Childhood = MainDefOf.WVC_RacesBiotech_Amnesia_Child;
			}
			if (pawn.story.Adulthood != null)
			{
				pawn.story.Adulthood = MainDefOf.WVC_RacesBiotech_Amnesia_Adult;
			}
		}

		public static void RemoveAllGenes(this List<Gene> genes, List<GeneDef> exclude = null)
		{
			foreach (Gene gene in genes.ToList())
			{
				if (exclude != null && exclude.Contains(gene.def))
                {
					continue;
                }
				gene.pawn?.genes?.RemoveGene(gene);
			}
		}

		public static void RemoveAllGenes_Overridden(Pawn pawn)
		{
			foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
			{
				if (gene.Overridden)
				{
					pawn.genes?.RemoveGene(gene);
				}
			}
			foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
			{
				if (gene.def.prerequisite != null && !XaG_GeneUtility.HasGene(gene.def.prerequisite, pawn))
				{
					pawn.genes?.RemoveGene(gene);
				}
			}
		}

		public static void RemoveAllTraits(this List<Trait> traits)
		{
			foreach (Trait trait in traits.ToList())
			{
				//trait.suppressedByGene = null;
				//trait.sourceGene = null;
				//trait.suppressedByTrait = false;
				//trait.pawn?.story?.traits?.RemoveTrait(trait, true);
				trait.RemoveTrait(trait.pawn);
			}
		}

		public static void AddRandomTraits(this Pawn pawn, int traitsCount)
		{
			int currentTry = 0;
			while (currentTry < traitsCount)
			{
				currentTry++;
				Trait trait = PawnGenerator.GenerateTraitsFor(pawn, 1, null, growthMomentTrait: false).FirstOrFallback();
				if (trait != null)
				{
					pawn.story.traits.GainTrait(trait);
				}
			}
		}

		public static bool Debug_IsDuplicate_CompCheckOnly(this Pawn diplicate)
		{
			return diplicate.TryGetComp<CompHumanlike>()?.IsDuplicate == true;
		}

	}
}
