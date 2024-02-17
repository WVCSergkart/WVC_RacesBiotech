using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class GeneFeaturesUtility
	{

		// ============================= GENE PSY BLOODFEEDER =============================

		public static bool TryPsyFeedRandomly(Pawn pawn, Gene_Hemogen gene_Hemogen)
		{
			if (pawn?.Map == null || gene_Hemogen == null || pawn.Downed)
			{
				return false;
			}
			if (gene_Hemogen.hemogenPacksAllowed || !gene_Hemogen.ShouldConsumeHemogenNow())
			{
				return false;
			}
			List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			workingList.Shuffle();
			for (int i = 0; i < workingList.Count; i++)
			{
				Pawn p = workingList[i];
				if (!p.RaceProps.Humanlike)
				{
					continue;
				}
				if (!p.PawnPsychicSensitive())
				{
					continue;
				}
				if (p == pawn)
				{
					continue;
				}
				if (CanPsyFeedNowWith(pawn, p))
				{
					DoPsychicBite(pawn, p, 0.2f, 0.4499f);
					return true;
				}
			}
			return false;
		}

		public static bool CanPsyFeedNowWith(Pawn parent, Pawn pawn)
		{
			if (pawn == null)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
			{
				return false;
			}
			if (pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() != null)
			{
				return false;
			}
			if (pawn.Faction != null && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
			{
				if (pawn.Faction.HostileTo(parent.Faction))
				{
					if (!pawn.Downed)
					{
						return false;
					}
				}
				else if (pawn.IsQuestLodger() || pawn.Faction != parent.Faction)
				{
					return false;
				}
			}
			if (pawn.IsWildMan() && !pawn.IsPrisonerOfColony && !pawn.Downed)
			{
				return false;
			}
			if (pawn.InMentalState)
			{
				return false;
			}
			return true;
		}

		public static void DoPsychicBite(Pawn biter, Pawn victim, float targetHemogenGain, float targetBloodLoss)
		{
			float num = SanguophageUtility.HemogenGainBloodlossFactor(victim, targetBloodLoss);
			float num2 = targetHemogenGain * victim.BodySize * num;
			GeneUtility.OffsetHemogen(biter, num2);
			GeneUtility.OffsetHemogen(victim, 0f - num2);
			if (targetBloodLoss > 0f)
			{
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim);
				hediff.Severity = targetBloodLoss;
				victim.health.AddHediff(hediff);
			}
			FleckDef fleckDef = DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect");
			FleckMaker.AttachedOverlay(biter, fleckDef, Vector3.zero);
			FleckMaker.AttachedOverlay(victim, fleckDef, Vector3.zero);
		}

		// ============================= GENE OPINION =============================

		public static void MyOpinionAboutPawnMap(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				// Log.Error(pawn.Name.ToString() + " hate " + pawns[i].Name.ToString());
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawns[i]);
			}
		}

		public static void PawnMapOpinionAboutMe(Pawn pawn, Gene gene, ThoughtDef thoughtDef, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (pawn?.genes == null)
			{
				return;
			}
			List<Pawn> pawns = pawn.Map?.mapPawns?.FreeColonistsAndPrisoners ?? pawn.GetCaravan()?.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < pawns.Count; i++)
			{
				if (!CanSetOpinion(pawn, pawns[i], gene, shouldBePsySensitive, shouldBeFamily, ignoreIfHasGene, onlySameXenotype))
				{
					continue;
				}
				pawns[i].needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef, pawn);
			}
		}

		public static bool CanSetOpinion(Pawn pawn, Pawn other, Gene gene, bool shouldBePsySensitive = false, bool shouldBeFamily = false, bool ignoreIfHasGene = false, bool onlySameXenotype = false)
		{
			if (!other.RaceProps.Humanlike)
			{
				return false;
			}
			if (other == pawn)
			{
				return false;
			}
			if (shouldBeFamily && !pawn.relations.FamilyByBlood.Contains(other))
			{
				return false;
			}
			if (ignoreIfHasGene && XaG_GeneUtility.HasActiveGene(gene.def, other))
			{
				return false;
			}
			if (shouldBePsySensitive && !other.PawnPsychicSensitive())
			{
				return false;
			}
			if (onlySameXenotype && !GeneUtility.SameXenotype(pawn, other))
			{
				return false;
			}
			return true;
		}

		// ============================= Checker Gene Features =============================

		// public static bool PawnIsExoskinned(Pawn pawn)
		// {
			// if (pawn?.genes == null)
			// {
				// return false;
			// }
			// List<GeneDef> whiteListedGenes = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// whiteListedGenes.AddRange(item.whiteListedExoskinGenes);
			// }
			// for (int i = 0; i < whiteListedGenes.Count; i++)
			// {
				// if (XaG_GeneUtility.HasActiveGene(whiteListedGenes[i], pawn))
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		public static bool IsNotAcceptablePrey(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (!modExtension.canBePredatorPrey)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool PawnSkillsNotDecay(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (modExtension.noSkillDecay)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// public static bool IsAngelBeauty(Pawn pawn)
		// {
			// if (pawn?.genes == null)
			// {
				// return false;
			// }
			// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (genesListForReading[i].Active == true)
				// {
					// GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					// if (modExtension != null)
					// {
						// if (modExtension.geneIsAngelBeauty)
						// {
							// return true;
						// }
					// }
				// }
			// }
			// return false;
		// }

		public static bool EyesShouldBeInvisble(Pawn pawn)
		{
			if (pawn?.genes == null || pawn?.story == null || pawn?.story?.headType == null)
			{
				return false;
			}
			if (pawn.story.headType.defName.Contains("WVC_Faceless"))
			{
				return true;
			}
			return false;
		}


	}
}
