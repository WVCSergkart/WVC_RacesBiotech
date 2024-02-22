using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ShapeshifterUtility
	{

		public static void AddRandomTraitFromListWithChance(Pawn pawn, GeneExtension_Shapeshifter geneExtension)
		{
			TraitSet traitSet = pawn.story.traits;
			if (geneExtension == null || traitSet.allTraits.Count > 1)
			{
				return;
			}
			GeneExtension_Shapeshifter.TraitDefWithWeight traitDefWithWeight = geneExtension.possibleTraits.RandomElementByWeight((GeneExtension_Shapeshifter.TraitDefWithWeight x) => x.weight);
			float chance = traitDefWithWeight.weight;
			// TraitDef trait = traitDefWithWeight.traitDef;
			Trait trait = new(traitDefWithWeight.traitDef);
			if (Rand.Chance(chance))
			{
				traitSet.GainTrait(trait);
			}
		}

		// Misc
		// public static void PostShapeshift(Gene gene)
		// {
			// if (ModLister.IdeologyInstalled)
			// {
				// Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_GenesDefOf.WVC_Shapeshift, progenitor.Named(HistoryEventArgsNames.Doer)));
			// }
		// }

		// Clone
		public static bool TryDuplicatePawn(Pawn progenitor, Gene gene = null, XenotypeDef xenotypeDef = null, bool duplicateMode = false)
		{
			if (gene == null || progenitor == null || xenotypeDef == null || duplicateMode == false)
			{
				return false;
			}
			PawnGenerationRequest generateNewBornPawn = new(progenitor.kindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: true, forbidAnyTitle: true, forceDead: false, null, null, null, null, null, 0f, progenitor.DevelopmentalStage);
			Pawn clone = PawnGenerator.GeneratePawn(generateNewBornPawn);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(clone, progenitor))
			{
				clone.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(clone);
				ReimplanterUtility.ExtractXenogerm(progenitor);
				clone.ageTracker.AgeBiologicalTicks = progenitor.ageTracker.AgeBiologicalTicks;
				clone.ageTracker.AgeChronologicalTicks = 0L;
				clone.gender = progenitor.gender;
				clone.story.Childhood = WVC_GenesDefOf.WVC_XaG_Shapeshifter0_Child;
				// if (clone.story.Adulthood != null)
				// {
					// clone.story.Adulthood = WVC_GenesDefOf.Colonist97;
				// }
				MiscUtility.TransferTraits(clone, progenitor);
				clone.story.headType = progenitor.story.headType;
				clone.story.bodyType = progenitor.story.bodyType;
				clone.story.hairDef = progenitor.story.hairDef;
				clone.story.favoriteColor = progenitor.story.favoriteColor;
				GeneFeaturesUtility.TryGetSkillsFromPawn(clone, progenitor, 1.0f, false);
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

	}
}
