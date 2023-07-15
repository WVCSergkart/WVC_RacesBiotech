using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	// [StaticConstructorOnStartup]
	public static class MechanoidizationUtility
	{

		public static bool ShouldNotSendNotificationAbout(Pawn pawn)
		{
			// if (pawn.RaceProps.Humanlike)
			// {
				// Gene_Undead undead = pawn.genes?.GetFirstGeneOfType<Gene_Undead>();
				// if (undead != null)
				// {
					// return undead.PawnCanResurrect;
				// }
			// }
			GeneExtension_General modExtension = pawn.def.GetModExtension<GeneExtension_General>();
			if (modExtension != null)
			{
				return !modExtension.shouldSendNotificationAbout;
			}
			return false;
		}

		// public static void RandomTaggedGene(GeneDef def, Pawn pawn, Gene gene)
		// {
			// bool geneIsXenogene = true;
			// List<Gene> endogenes = pawn.genes.Endogenes;
			// if (endogenes.Contains(gene))
			// {
				// geneIsXenogene = false;
			// }
			// GeneDef geneDef = DefDatabase<GeneDef>.AllDefs.Where((GeneDef randomGeneDef) => randomGeneDef != null && randomGeneDef.geneClass == def.geneClass && randomGeneDef.exclusionTags != null && randomGeneDef.exclusionTags == def.exclusionTags && randomGeneDef != def).RandomElement();
			// if (!pawn.genes.HasEndogene(geneDef))
			// {
				// pawn.genes.AddGene(geneDef, xenogene: geneIsXenogene);
				// pawn.genes.RemoveGene(gene);
			// }
		// }

		// public static bool PawnIsMechalike(Pawn pawn)
		// {
			// if (PawnIsMechaskinned(pawn) || PawnHasSubCoreInstalled(pawn))
			// {
				// return true;
			// }
			// return false;
		// }

		// public static bool PawnHasSubCoreInstalled(Pawn pawn)
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
						// if (modExtension.geneIsSubcore)
						// {
							// return true;
						// }
					// }
				// }
			// }
			// return false;
		// }

		public static bool PawnIsAndroid(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		public static bool PawnIsGolem(Pawn pawn)
		{
			if (pawn.RaceProps.IsMechanoid)
			{
				// List<ThingDef> listedGolems = new List<ThingDef>();
				// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				// {
					// listedGolems.AddRange(item.listedGolems);
				// }
				// if (listedGolems.Contains(pawn.def))
				// {
					// return true;
				// }
				return pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_SelfPopulationRegulation_Golems);
			}
			return false;
		}

		public static bool PawnCannotUseSerums(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return true;
			}
			List<Def> blackListedThings = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				blackListedThings.AddRange(item.blackListedDefsForSerums);
			}
			if (blackListedThings.Contains(pawn.def))
			{
				return true;
			}
			if (pawn?.genes == null)
			{
				return false;
			}
			List<GeneDef> nonCandidates = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				nonCandidates.AddRange(item.nonCandidatesForSerums);
			}
			for (int i = 0; i < nonCandidates.Count; i++)
			{
				if (HasActiveGene(nonCandidates[i], pawn))
				{
					return true;
				}
			}
			return false;
		}

		public static bool DelayedReimplanterIsActive(Pawn pawn)
		{
			// if (!pawn.RaceProps.Humanlike)
			// {
				// return true;
			// }
			if (pawn.health != null && pawn.health.hediffSet != null)
			{
				List<HediffDef> hediffDefs = new();
				foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				{
					hediffDefs.AddRange(item.blackListedHediffDefForReimplanter);
				}
				for (int i = 0; i < hediffDefs.Count; i++)
				{
					if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
					{
						return true;
					}
					// return pawn.health.hediffSet.HasHediff(hediffDefs[i]);
				}
			}
			return false;
		}

		// public static bool OtherHasSubCoreInstalled(Pawn other)
		// {
			// if (other.RaceProps.Humanlike)
			// {
				// return other.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_Base); //MechaSkin
			// }
			// return false;
		// }

		public static bool PawnIsExoskinned(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<GeneDef> whiteListedGenes = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				whiteListedGenes.AddRange(item.whiteListedExoskinGenes);
			}
			for (int i = 0; i < whiteListedGenes.Count; i++)
			{
				if (HasActiveGene(whiteListedGenes[i], pawn))
				{
					return true;
				}
			}
			// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (genesListForReading[i].Active == true)
				// {
					// if (whiteListedGenes.Contains(genesListForReading[i].def))
					// {
						// return true;
					// }
				// }
			// }
			return false;
		}

		// public static bool PawnIsImmortal(Pawn pawn)
		// {
			// return pawn.genes.HasGene(GeneDefOf.Deathless);
		// }

		// public static bool PawnHasPowerSource(Pawn pawn)
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
						// if (modExtension.geneIsPowerSource)
						// {
							// return true;
						// }
					// }
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

		public static bool IsAngelBeauty(Pawn pawn)
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
						if (modExtension.geneIsAngelBeauty)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// public static bool IsIncestLover(Pawn pawn)
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
						// if (modExtension.geneIsIncestous)
						// {
							// return true;
						// }
					// }
				// }
			// }
			// return false;
		// }

		public static bool HasActiveGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true && genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		// public static bool TryGetGeneFromPrecept(Pawn pawn, out GeneDef gene)
		// {
			// List<Precept> precept = pawn.ideo.Ideo.PreceptsListForReading;
			// foreach (Precept item in precept)
			// {
				// GeneDef geneDef = null;
				// ThoughtExtension_General extension = item.def.GetModExtension<ThoughtExtension_General>();
				// if (extension != null)
				// {
					// geneDef = extension.geneDef;
				// }
				// gene = geneDef;
				// return true;
			// }
			// gene = null;
			// return false;
		// }

		// public static bool HasGeneWithTag(Pawn pawn)
		// {
			// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (genesListForReading[i].def.GetModExtension<GeneExtension_General>().geneIsMechaskin && genesListForReading[i].Active == true)
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		// ===============================================================

		// public static float Archites(Pawn pawn)
		// {
			// float num = 0f;
			// foreach (Gene item in pawn.genes.GenesListForReading)
			// {
				// if (!item.Overridden)
				// {
					// num += item.def.biostatArc;
				// }
			// }
			// return num;
		// }

		// public static bool GeneIsMechaskin(Pawn pawn)
		// {
			// return false;
		// }

		// public static float ArchitesOther(Pawn other)
		// {
			// float num = 0f;
			// foreach (Gene item in other.genes.GenesListForReading)
			// {
				// if (!item.Overridden)
				// {
					// num += item.def.biostatArc;
				// }
			// }
			// return num;
		// }

		// public static int SubCoreNetworkCompatiblePawn(Pawn pawn)
		// {
			// int num = 0;
			// foreach (Pawn item in pawn.MapHeld.mapPawns.SpawnedPawnsInFaction(pawn.Faction))
			// {
				// if (item.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_Base) && (item.IsPrisonerOfColony || item.IsSlaveOfColony || item.IsColonist))
				// {
					// num++;
				// }
			// }
			// return num;
		// }

		// ===============================================================

		public static void ReimplantEndogerm(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			recipient.genes.SetXenotype(caster.genes.Xenotype);
			// recipient.genes.SetXenotypeDirect(caster.genes.Xenotype);
			recipient.genes.xenotypeName = caster.genes.xenotypeName;
			recipient.genes.iconDef = caster.genes.iconDef;
			// recipient.genes.ClearXenogenes();
			// recipient.genes.ClearEndogenes();
			Pawn_GeneTracker recipientGenes = recipient.genes;
			// for (int numGenes = recipientGenes.GenesListForReading.Count - 1; numGenes >= 0; numGenes--)
			// {
				// recipientGenes.RemoveGene(recipientGenes.GenesListForReading[numGenes]);
			// }
			if (recipientGenes != null && recipientGenes.GenesListForReading.Count > 0)
			{
				foreach (Gene item in recipient.genes?.GenesListForReading)
				{
					recipient.genes?.RemoveGene(item);
				}
			}
			// for (int num = recipient.genes.Endogenes.Count - 1; num >= 0; num--)
			// {
				// recipient.genes.RemoveGene(recipient.genes.Endogenes[num]);
			// }
			// for (int i = 0; i < caster.genes.Endogenes.Count; i++)
			// {
				// recipient.genes.AddGene(caster.genes.Endogenes[i], xenogene: false);
			// }
			// for (int i = 0; i < caster.genes.Xenogenes.Count; i++)
			// {
				// recipient.genes.AddGene(caster.genes.Xenogenes[i], xenogene: true);
			// }
			foreach (Gene endogene in caster.genes.Endogenes)
			{
				recipient.genes.AddGene(endogene.def, xenogene: false);
			}
			foreach (Gene xenogene in caster.genes.Xenogenes)
			{
				recipient.genes.AddGene(xenogene.def, xenogene: true);
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.ExtractXenogerm(caster);
			GeneUtility.UpdateXenogermReplication(recipient);
		}
		// public static void ClearEndogenes()
		// {
			// for (int num = endogenes.Count - 1; num >= 0; num--)
			// {
				// Pawn_GeneTracker.RemoveGene(endogenes[num]);
			// }
		// }

		// ===============================================================

		public static void MechSummonQuest(Pawn pawn, QuestScriptDef quest)
		{
			Slate slate = new();
			slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(pawn.Map));
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
			// QuestUtility.SendLetterQuestAvailable(quest);
		}
		
	}
}
