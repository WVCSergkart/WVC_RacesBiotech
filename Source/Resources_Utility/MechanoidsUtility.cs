using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class MechanoidsUtility
	{

		public static void SetOverseer(Pawn newOverseer, Pawn mech)
		{
			//if (newOverseer.mechanitor == null)
			//{
			//	return;
			//}
            mech.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, mech);
			if (mech.Faction != newOverseer.Faction)
			{
				mech.SetFaction(newOverseer.Faction);
			}
			newOverseer.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mech);
		}

		// In Dev
		public static float ToFloatFactor(this MechWeightClassDef weightClass)
		{
			//return weightClass switch
			//{
			//	MechWeightClassDefOf.Light => 1,
			//	MechWeightClassDefOf.Medium => 2,
			//	MechWeightClassDefOf.Heavy => 3,
			//	MechWeightClassDefOf.UltraHeavy => 4,
			//	_ => 5,
			//};
			if (weightClass == MechWeightClassDefOf.Light)
            {
				return 1;
			}
			else if (weightClass == MechWeightClassDefOf.Medium || weightClass == MechWeightClassDefOf.Heavy)
			{
				return 1.5f;
			}
			else
			{
				return 2;
			}
		}

		// Golems

		public static string GolemsEnergyPerDayInPercent(float energy)
		{
			return ((24f * WVC_Biotech.settings.golemnoids_ShutdownRechargePerTick) / 100f / (energy / 100)).ToStringPercent();
		}

		public static bool HasEnoughGolembond(Pawn mechanitor, float additionalBond = 0)
		{
			float maxBond = TotalGolembond(mechanitor);
			float consumedBond = GetConsumedGolembond(mechanitor) + additionalBond;
			return maxBond >= consumedBond;
		}

		public static List<PawnKindDef> GetAllControlledGolems_PawnKinds(Pawn mechanitor)
		{
			List<Pawn> currentGolems = GetAllControlledGolems(mechanitor);
			if (currentGolems.NullOrEmpty())
			{
				return null;
			}
			List<PawnKindDef> list = new();
			foreach (Pawn item in currentGolems)
			{
				list.Add(item.kindDef);
			}
			return list;
		}

		public static List<Pawn> GetAllControlledGolems(Pawn mechanitor)
		{
			List<Pawn> mechs = mechanitor?.mechanitor?.OverseenPawns;
			if (mechs.NullOrEmpty())
			{
				return null;
			}
			List<Pawn> list = new();
			foreach (Pawn item in mechs)
			{
				if (item.health.Dead)
				{
					continue;
				}
				if (!item.IsGolemlike())
				{
					continue;
				}
				// if (compGolem.Props.golemIndex == golemIndex)
				// {
				// }
				list.Add(item);
			}
			return list;
		}

		// [Obsolete]
		// public static List<Pawn> GetAllControlledGolemsOfIndex(Pawn mechanitor, int golemIndex)
		// {
			// List<Pawn> mechs = mechanitor?.mechanitor?.OverseenPawns;
			// if (mechs.NullOrEmpty())
			// {
				// return null;
			// }
			// List<Pawn> list = new();
			// foreach (Pawn item in mechs)
			// {
				// if (item.health.Dead)
				// {
					// continue;
				// }
				// if (!item.IsGolemlike())
				// {
					// continue;
				// }
				// CompGolem compGolem = item.GetComp<CompGolem>();
				// if (compGolem == null)
				// {
					// continue;
				// }
				// if (compGolem.Props.golemIndex == golemIndex)
				// {
					// list.Add(item);
				// }
			// }
			// return list;
		// }

		public static bool IsGolemlike(this Pawn pawn)
		{
			if (pawn.RaceProps.IsMechanoid)
			{
				return pawn.TryGetComp<CompGolem>() != null;
			}
			return false;
		}

		public static bool IsGolemlike(this PawnKindDef pawnkind)
		{
			if (pawnkind.race.race.IsMechanoid)
			{
				return pawnkind.race.GetCompProperties<CompProperties_Golem>() != null;
			}
			return false;
		}

		// public static bool GetGolemistGene(this Pawn pawn, out Gene_MechlinkWithGizmo mechlinkWithGizmo)
		// {
			// return (mechlinkWithGizmo = pawn?.genes?.GetFirstGeneOfType<Gene_MechlinkWithGizmo>())?.def?.resourceGizmoType == typeof(GeneGizmo_Golems);
		// }


		public static float TotalGolembond(Pawn mechanitor)
		{
			return mechanitor.GetStatValue(MainDefOf.WVC_GolemBond);
		}

		public static float GetConsumedGolembond(Pawn mechanitor)
		{
			float result = 0;
			List<Pawn> golems = mechanitor?.mechanitor?.OverseenPawns;
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				if (golem.Dead)
				{
					continue;
				}
				result += golem.GetStatValue(MainDefOf.WVC_GolemBondCost);
			}
			return result;
		}

		// public static bool IsGolemistOfIndex(this Pawn mechanitor, int golemsIndex)
		// {
			// if (MechanitorUtility.IsMechanitor(mechanitor))
			// {
				// return mechanitor.GetGolemistGene(out Gene_MechlinkWithGizmo mechlinkWithGizmo) && mechlinkWithGizmo?.Giver?.golemistTypeIndex == golemsIndex;
			// }
			// return false;
		// }

		// [Obsolete]
		// public static bool MechanitorIsLich(Pawn mechanitor)
		// {
			// return mechanitor?.genes?.GetFirstGeneOfType<Gene_Sporelink>() != null;
		// }

		public static bool MechanitorIsGolemist(Pawn mechanitor)
		{
			return mechanitor?.genes?.GetFirstGeneOfType<Gene_Golemlink>() != null;
		}

		// Ideo

		// public static bool CanSummonMechanoidsIdeo(Pawn pawn)
		// {
		// if (!ModLister.CheckIdeology("Ideology"))
		// {
		// return true;
		// }
		// List<Precept> preceptsListForReading = pawn?.ideo?.Ideo?.PreceptsListForReading;
		// if (preceptsListForReading.NullOrEmpty())
		// {
		// return true;
		// }
		// foreach (Precept precept in preceptsListForReading)
		// {
		// PreceptExtension_General preceptExtension = precept?.def?.GetModExtension<PreceptExtension_General>();
		// if (preceptExtension != null)
		// {
		// return !preceptExtension.blesslinkCannotSummonMechanoids;
		// }
		// }
		// return true;
		// }

		public static bool TrySummonMechanoids(Pawn pawn, int countSpawn, List<MechWeightClassDef> allowedMechWeightClasses, out List<Thing> summonList, HediffDef hediffDef = null)
        {
            List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef, allowedMechWeightClasses)).ToList();
            return TrySummonMechanoids(pawn, countSpawn, pawnKindDefs, out summonList, hediffDef);
		}

		public static bool TrySummonMechanoids(Pawn pawn, int countSpawn, List<GolemModeDef> golemModeDefs, out List<Thing> summonList, HediffDef hediffDef = null)
        {
			summonList = new();
			List<PawnKindDef> pawnKindDefs = new();
			foreach (GolemModeDef golemModeDef in golemModeDefs)
            {
				if (golemModeDef.CanBeSummoned)
                {
					pawnKindDefs.Add(golemModeDef.pawnKindDef);
				}
            }
			if (pawnKindDefs.Empty())
            {
				return false;
            }
			return TrySummonMechanoids(pawn, countSpawn, pawnKindDefs, out summonList, hediffDef);
		}

		public static bool TrySummonMechanoids(Pawn pawn, int countSpawn, List<PawnKindDef> pawnKindDefs, out List<Thing> summonList, HediffDef hediffDef = null)
		{
            summonList = new();
            for (int i = 0; i < countSpawn; i++)
            {
                PawnGenerationRequest request = new(pawnKindDefs.RandomElement(), pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
                Pawn mech = PawnGenerator.GeneratePawn(request);
                AgelessUtility.SetAge(mech, 3600000 * new IntRange(12, 74).RandomInRange);
                if (hediffDef != null)
                {
                    mech.health.AddHediff(hediffDef);
                }
                MechanoidsUtility.SetOverseer(pawn, mech);
                summonList.Add(mech);
            }
			if (MiscUtility.TrySummonDropPod(pawn.Map, summonList))
			{
				Find.LetterStack.ReceiveLetter("WVC_XaG_MechanoidSummon_Label".Translate(), "WVC_XaG_MechanoidSummon_Letter".Translate(summonList.Select((Thing thing) => (thing as Pawn).KindLabel).ToCommaList(useAnd: true).UncapitalizeFirst(), pawn), LetterDefOf.PositiveEvent, new LookTargets(summonList));
				return true;
			}
			return false;
        }

		[Obsolete]
        public static void MechSummonQuest(Pawn pawn, QuestScriptDef quest)
		{
			if (pawn.Map.IsUnderground())
			{
				return;
			}
			Slate slate = new();
			slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(pawn.Map));
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
			// QuestUtility.SendLetterQuestAvailable(quest);
		}

		// ===============================================================

		// public static bool LimitExceedCheck(Pawn mechanitor, StatDef bandwidth)
		// {
			// float totalBandwidth = mechanitor.GetStatValue(bandwidth);
			// float golems = GolemsUtility.GetConsumedBandwidth(mechanitor);
			// float walkers = WalkingUtility.GetConsumedBandwidth(mechanitor);
			// if (totalBandwidth >= golems && totalBandwidth >= walkers)
			// {
				// return true;
			// }
			// return false;
		// }

		// public static float GetConsumedBandwidth(Pawn mechanitor, StatDef bandwidth)
		// {
			// float result = 0;
			// List <Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			// if (mechs.NullOrEmpty())
			// {
				// return result;
			// }
			// foreach (Pawn mech in mechs)
			// {
				// float bandwidth = mech.GetStatValue(bandwidth);
				// if (bandwidth > 0)
				// {
					// result += bandwidth;
				// }
			// }
			// return result;
		// }

		// =====================================

		public static List<Pawn> GetAllMechanitors(Map map)
		{
			List<Pawn> list = new();
			List<Pawn> colonists = map.mapPawns.FreeColonists;
			if (!colonists.NullOrEmpty())
			{
				foreach (Pawn colonist in colonists)
				{
					if (MechanitorUtility.IsMechanitor(colonist))
					{
						list.Add(colonist);
					}
				}
			}
			return list;
		}

		// public static bool CanSpawnMoreMechanoids(Pawn mechanitor, Pawn mech)
		// {
			// float weight = mechanitor.mechanitor.TotalBandwidth - (mechanitor.mechanitor.UsedBandwidth + mech.GetStatValue(StatDefOf.BandwidthCost, cacheStaleAfterTicks: 360000));
			// if (weight < 0f)
			// {
				// return false;
			// }
			// return true;
		// }

		// public static float GetConsumedBandwidth(Pawn mechanitor)
		// {
			// float result = 0;
			// List <Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			// if (mechs.NullOrEmpty())
			// {
				// return result;
			// }
			// foreach (Pawn mech in mechs)
			// {
				// float bandwidth = mech.GetStatValue(StatDefOf.BandwidthCost);
				// if (bandwidth > 0)
				// {
					// result += bandwidth;
				// }
			// }
			// return result;
		// }

		public static void OffsetNeedEnergy(Pawn pawn, float offset, int hours)
		{
			if (offset > 0 && hours > 0)
			{
				Need_MechEnergy energy = pawn?.needs?.energy;
				if (energy?.IsSelfShutdown == true)
				{
					energy.CurLevel += offset * hours;
				}
			}
		}

		// [Obsolete]
		// public static bool MechanoidIsGolemlike(Pawn mech)
		// {
			// if (WalkingUtility.PawnIsBoneGolem(mech) || GolemsUtility.PawnIsGolem(mech))
			// {
				// return true;
			// }
			// return false;
		// }

		// Mecha summon
		public static bool MechanoidIsPlayerMechanoid(PawnKindDef mech, List<MechWeightClassDef> allowedMechWeightClasses)
		{
			if (!MechanoidIsPlayerMechanoid(mech)
			|| !mech.defName.Contains("Mech_")
			|| mech.race.race.thinkTreeMain != MainDefOf.Mechanoid
			|| mech.race.race.thinkTreeConstant != MainDefOf.MechConstant
			|| mech.race.race.lifeStageAges.Count <= 1)
			{
				return false;
			}
			if (allowedMechWeightClasses != null && !allowedMechWeightClasses.Contains(mech.race.race.mechWeightClass))
			{
				return false;
			}
			return true;
		}

		public static bool MechanoidIsPlayerMechanoid(PawnKindDef mech)
		{
			if (!mech.race.race.IsMechanoid
			|| MechDefNameContainsBannedWords(mech.defName)
			|| MechDefNameContainsBannedWords(mech.race.defName)
			|| !EverControllable(mech.race)
			|| !EverRepairable(mech.race))
			{
				return false;
			}
			return true;
		}

		public static bool MechDefNameContainsBannedWords(string defName)
		{
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				foreach (string name in item.mechDefNameShouldNotContain)
				{
					if (defName.Contains(name))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool EverControllable(ThingDef def)
		{
			return def.HasComp<CompOverseerSubject>();
		}

		public static bool EverRepairable(ThingDef def)
		{
			return def.HasComp<CompMechRepairable>();
		}


	}
}
