using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	public static class GestationUtility
	{

		/// <summary>
		/// Modders hook.
		/// Performance ahead. Use optimization.
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool CanBePregnant(this Pawn pawn)
		{
			if (pawn?.gender == Gender.Female)
			{
				return true;
			}
			return WVC_Biotech.settings.enable_pregnancyForAllGenders;
		}

		public static bool TryUpdChildGenes(Pawn pawn, Pawn secondParent = null)
		{
			if (pawn.health.hediffSet.TryGetHediff(HediffDefOf.PregnantHuman, out Hediff hediff))
			{
				if (hediff is Hediff_Pregnant pregnant)
				{
					GeneSet newGeneSet = new();
					HediffUtility.AddParentGenes(pawn, newGeneSet);
					HediffUtility.AddParentGenes(secondParent, newGeneSet);
					newGeneSet.SortGenes();
					pregnant.geneSet = newGeneSet;
					return true;
				}
				else
				{
					pawn.health.RemoveHediff(hediff);
				}
			}
			return false;
		}

		public static void TryImpregnateOrUpdChildGenes(Pawn mother, Pawn fatherOrSecondMother = null)
		{
			if (!TryUpdChildGenes(mother, fatherOrSecondMother))
			{
				Impregnate(mother, fatherOrSecondMother);
			}
		}

		public static void Impregnate(Pawn mother, Pawn fatherOrSecondMother = null)
		{
			if (mother?.genes == null)
			{
				return;
			}
			if (!Find.Storyteller.difficulty.ChildrenAllowed)
			{
				return;
			}
			Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, mother);
			hediff_Pregnant.Severity = PregnancyUtility.GeneratedPawnPregnancyProgressRange.TrueMin;
			GeneSet newGeneSet = new();
			HediffUtility.AddParentGenes(mother, newGeneSet);
			HediffUtility.AddParentGenes(fatherOrSecondMother, newGeneSet);
			// GeneSet inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(null, pawn, out success);
			newGeneSet.SortGenes();
			hediff_Pregnant.SetParents(mother, fatherOrSecondMother, newGeneSet);
			mother.health.AddHediff(hediff_Pregnant);
		}

		public static void GestateChild_WithGenes(Pawn parent, Thing motherOrEgg = null, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogenes = true, bool xenogenes = true)
		{
			if (motherOrEgg == null)
			{
				motherOrEgg = parent;
			}
			SpawnChild(parent, motherOrEgg, out Pawn child, endogenes, xenogenes);
			if (PawnUtility.ShouldSendNotificationAbout(parent))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_XenoTreeBirthLabel".Translate(), completeMessage.Translate(parent.LabelShort.Colorize(ColoredText.NameColor)), MainDefOf.WVC_XaG_GestationEvent, new LookTargets(child));
			}
		}


		public static void SpawnChild(Pawn parent, Thing motherOrEgg, out Pawn child, bool endogenes = true, bool xenogenes = true)
		{
			int litterSize = GestationUtility.GetLitterSize(parent);
			PawnGenerationRequest generateNewBornPawn = NewBornRequest(parent.kindDef, parent.Faction);
			child = null;
			for (int i = 0; i < litterSize; i++)
			{
				// Log.Error("4");
				TrySpawnHatchedOrBornPawn(parent, motherOrEgg, generateNewBornPawn, out child, endogenes, xenogenes);
			}
			if (motherOrEgg.Spawned)
			{
				FilthMaker.TryMakeFilth(motherOrEgg.Position, motherOrEgg.Map, ThingDefOf.Filth_Slime, 5);
				MainDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(motherOrEgg));
				if (parent.caller != null && motherOrEgg == parent)
				{
					parent.caller.DoCall();
				}
			}
			PostSpawnFilthAndSound(motherOrEgg);
		}

		public static PawnGenerationRequest NewBornRequest(PawnKindDef pawnKind, Faction faction)
		{
			return new(pawnKind, faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
		}

		public static void TrySpawnHatchedOrBornPawn(Pawn parent, Thing motherOrEgg, PawnGenerationRequest generateNewBornPawn, out Pawn newBorn, bool endogene = true, bool xenogene = true, XenotypeDef xenotypeDef = null, XenotypeHolder xenotypeHolder = null, Pawn parent2 = null)
		{
			newBorn = PawnGenerator.GeneratePawn(generateNewBornPawn);
			if (xenotypeHolder != null)
			{
				if (!xenotypeHolder.CustomXenotype)
				{
					xenotypeDef = xenotypeHolder.xenotypeDef;
				}
				else
				{
					xenotypeDef = null;
					ReimplanterUtility.SetCustomXenotype(newBorn, xenotypeHolder);
				}
			}
			if (xenotypeDef != null)
			{
				ReimplanterUtility.SetXenotype_DoubleXenotype(newBorn, xenotypeDef);
			}
			if (PawnUtility.TrySpawnHatchedOrBornPawn(newBorn, motherOrEgg))
			{
				if (parent != null)
				{
					if (newBorn.playerSettings != null && parent.playerSettings != null)
					{
						newBorn.playerSettings.AreaRestrictionInPawnCurrentMap = parent.playerSettings.AreaRestrictionInPawnCurrentMap;
					}
					if (newBorn.RaceProps.IsFlesh)
					{
						newBorn.relations.AddDirectRelation(PawnRelationDefOf.Parent, parent);
					}
					if (parent.Spawned)
					{
						parent.GetLord()?.AddPawn(newBorn);
					}
					try
					{
						if (motherOrEgg is Pawn hatcherPawn && hatcherPawn.CanReserveAndReach(newBorn, PathEndMode.Touch, Danger.Deadly))
						{
							Job job = JobMaker.MakeJob(JobDefOf.BringBabyToSafetyUnforced, newBorn);
							job.count = 1;
							//hatcherPawn.TryTakeOrderedJob(job, JobTag.Misc, false);
							hatcherPawn.jobs.SuspendCurrentJob(JobCondition.InterruptForced);
							hatcherPawn.jobs.StartJob(job, tag: JobTag.Misc);
						}
					}
					catch
					{
						Log.Warning("Failed BringBabyToSafetyUnforced job.");
					}
					if (xenotypeDef == null && xenotypeHolder == null)
					{
						ReimplanterUtility.GeneralReimplant(parent, newBorn, endogene, xenogene, false);
					}
					TaleRecorder.RecordTale(TaleDefOf.GaveBirth, parent, newBorn);
				}
				if (parent2 != null)
				{
					if (newBorn.RaceProps.IsFlesh)
					{
						newBorn.relations.AddDirectRelation(PawnRelationDefOf.Parent, parent2);
					}
				}
				SetName(newBorn, parent);
			}
			else
			{
				Find.WorldPawns.PassToWorld(newBorn, PawnDiscardDecideMode.Discard);
			}
		}

		// Xeno-Tree Spawner

		public static void GestateChild_WithXenotype(Thing motherOrEgg, XenotypeDef xenotypeDef, XenotypeHolder xenotypeHolder, string completeLetterLabel, string completeLetterDesc)
		{
			if (motherOrEgg == null)
			{
				return;
			}
			PawnGenerationRequest generateNewBornPawn = NewBornRequest(PawnKindDefOf.Colonist, Faction.OfPlayer);
			TrySpawnHatchedOrBornPawn(motherOrEgg is Pawn pawn ? pawn : null, motherOrEgg, generateNewBornPawn, out Pawn newBorn, true, true, xenotypeDef, xenotypeHolder);
			PostSpawnFilthAndSound(motherOrEgg);
			if (PawnUtility.ShouldSendNotificationAbout(newBorn))
			{
				Find.LetterStack.ReceiveLetter(completeLetterLabel.Translate(), completeLetterDesc.Translate(motherOrEgg.LabelShortCap.Colorize(ColoredText.NameColor)), MainDefOf.WVC_XaG_GestationEvent, new LookTargets(newBorn));
			}
		}


		public static void PostSpawnFilthAndSound(Thing spawnTarget)
		{
			if (spawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(spawnTarget.Position, spawnTarget.Map, ThingDefOf.Filth_Slime, 5);
				MainDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(spawnTarget));
			}
		}

		public static void SetName(Pawn baby, Pawn parent)
		{
			// baby.Name = PawnBioAndNameGenerator.GenerateFullPawnName(baby.def, baby.kindDef.GetNameMaker(baby.gender), baby.story, null, baby.RaceProps.GetNameGenerator(baby.gender), baby.Faction?.ideos?.PrimaryCulture, false, gender: baby.gender, nameCategory: baby.RaceProps.nameCategory, forcedLastName: null, false);
			Name name = PawnBioAndNameGenerator.GeneratePawnName(baby, forcedLastName: GetParentLastName(parent), forceNoNick: true, xenotype: baby.genes.Xenotype);
			// if (parent != null & baby != null)
			// {
			// string name = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).GetName(PawnNameSlot.First, baby.gender);
			// baby.Name = new NameTriple(name, null, GetParentLastName(parent));
			// }
			string parentName = GetParentLastName(parent);
			if (baby != null && name is NameTriple babyNameTriple && parentName != null)
			{
				baby.Name = new NameTriple(babyNameTriple.First, null, parentName);
			}
			else
			{
				baby.Name = name;
			}
		}

		public static string GetParentLastName(Pawn parent)
		{
			if (parent != null && parent.Name is NameTriple parentNameTriple)
			{
				return parentNameTriple.Last;
			}
			return null;
		}

		public static int GetLitterSize(Pawn pawn)
		{
			int litterSize = ((pawn.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			return litterSize;
		}

		// Raw copy from RimWorld.JobGiver_LayEgg
		public static Thing GetBestEggBox(Pawn pawn, ThingDef eggDef, PathEndMode peMode, TraverseParms tp)
		{
			//ThingDef eggDef = pawn.TryGetComp<CompEggLayer>().NextEggType();
			return GenClosest.ClosestThing_Regionwise_ReachablePrioritized(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.EggBox), peMode, tp, 30f, IsUsableBox, GetScore, 10);
			static float GetScore(Thing thing)
			{
				CompEggContainer compEggContainer = thing.TryGetComp<CompEggContainer>();
				if (compEggContainer == null || compEggContainer.Full)
				{
					return 0f;
				}
				return ((float?)compEggContainer.ContainedThing?.stackCount * 5f) ?? 0.5f;
			}
			bool IsUsableBox(Thing thing)
			{
				if (!thing.Spawned)
				{
					return false;
				}
				if (thing.IsForbidden(pawn) || !pawn.CanReserve(thing) || !pawn.Position.InHorDistOf(thing.Position, 30f))
				{
					return false;
				}
				CompEggContainer compEggContainer = thing.TryGetComp<CompEggContainer>();
				if (compEggContainer == null || !compEggContainer.Accepts(eggDef))
				{
					return false;
				}
				return true;
			}
		}

		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========

		//private static HashSet<Pawn> cachedBisexualPawns;
		//public static HashSet<Pawn> BisexualPawns
		//{
		//	get
		//	{
		//		if (cachedBisexualPawns == null)
		//		{
		//			List<Pawn> list = new();
		//			foreach (Pawn pawn in ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive)
		//			{
		//				if (pawn?.genes?.GenesListForReading?.Any(gene => gene is Gene_Gender && gene.Active) == true)
		//				{
		//					list.Add(pawn);
		//				}
		//			}
		//			//Log.Error(list.Select((pawn) => pawn.Name.ToString()).ToLineList(" - "));
		//			cachedBisexualPawns = [.. list];
		//		}
		//		return cachedBisexualPawns;
		//	}
		//}

		//public static void ResetCollection_Bisexual()
		//{
		//	cachedBisexualPawns = null;
		//}

		//private static bool hasTraitHook_Patched = false;
		//public static void HarmonyPatch_Bisexual()
		//{
		//	if (hasTraitHook_Patched)
		//	{
		//		return;
		//	}
		//	try
		//	{
		//		HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
		//		HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef), typeof(int)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
		//		foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
		//		{
		//			if (geneDef.IsGeneDefOfType<Gene_Gender>())
		//			{
		//				if (geneDef.customEffectDescriptions == null)
		//				{
		//					geneDef.customEffectDescriptions = new();
		//				}
		//				geneDef.customEffectDescriptions.Add("WVC_XaG_GeneGender_BisexualPatch".Translate());
		//			}
		//		}
		//	}
		//	catch (Exception arg)
		//	{
		//		Log.Warning("Non-critical error. Failed apply bisexual hook. Reason: " + arg.Message);
		//	}
		//	hasTraitHook_Patched = true;
		//}

		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========

	}
}
