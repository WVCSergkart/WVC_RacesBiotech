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
			List<ThingDef> blackListedThings = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				blackListedThings.AddRange(item.blackListedThingsForSerums);
			}
			if (blackListedThings.Contains(pawn.def))
			{
				return true;
			}
			// if (pawn.kindDef.race.Contains("VREA_SyntheticBody"))
			// {
				// return true;
			// }
			// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
				// {
					// return true;
				// }
			// }
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
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					if (whiteListedGenes.Contains(genesListForReading[i].def))
					{
						return true;
					}
					// GeneGraphicData geneGraphicData = genesListForReading[i].def.graphicData;
					// if (geneGraphicData != null)
					// {
						// if (geneGraphicData.fur != null)
						// {
							// return true;
						// }
					// }
					// for (int j = 0; j < whiteListedGenes.Count; j++)
					// {
					// }
					// if (whiteListedGenes.Contains(genesListForReading[i].def.defName))
					// {
						// return true;
					// }
				}
				// if (!WVC_Biotech.settings.disableFurGraphic)
				// {
				// }
				// else
				// {
					// if (genesListForReading[i].Active == true)
					// {
						// List<string> exclusionTags = genesListForReading[i].def.exclusionTags;
						// if (exclusionTags != null)
						// {
							// if (exclusionTags.Contains("Fur"))
							// {
								// return true;
							// }
						// }
					// }
				// }
			}
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
		// public static bool OtherIsMechaskinned(Pawn other)
		// {
			// if (other.genes.HasGene(WVC_GenesDefOf.WVC_MechaSkin) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Blue) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Red) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Green) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Violet) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Yellow) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_AltBlue) || other.genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Orange))
			// {
				// return true;
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

		public static void GenerateNewBornPawn(Pawn pawn, string completeMessage = "WVC_RB_Gene_MechaGestator", bool endogeneTransfer = true)
		{
			Pawn pawnParent = pawn;
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
					if (pawnParent.genes?.Xenotype != null)
					{
						pawnNewBornChild.genes?.SetXenotype(pawnParent.genes?.Xenotype);
					}
				}
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnParent))
				{
					if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
					{
						pawnNewBornChild.playerSettings.AreaRestriction = pawnParent.playerSettings.AreaRestriction;
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
				else
				{
					Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
				}
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			}
			if (pawnParent.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnParent.Position, pawnParent.Map, ThingDefOf.Filth_AmnioticFluid, pawnParent.LabelIndefinite(), 5);
				if (pawnParent.caller != null)
				{
					pawnParent.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
			// Find.LetterStack.ReceiveLetter(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), LetterDefOf.BabyBirth, pawn);
		}

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
