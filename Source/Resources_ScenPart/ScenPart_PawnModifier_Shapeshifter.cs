// RimWorld.StatPart_Age
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;
using static System.Net.Mime.MediaTypeNames;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Shapeshifter : ScenPart_PawnModifier
	{

		public override void PostGameStart()
		{
			base.PostGameStart();
			SetGeneral();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 60000);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGeneral();
			}
		}

		private int nextTick = 60000;
		public override void Tick()
		{
			if (GeneResourceUtility.CanTick(ref nextTick, 60000, 1))
			{
				FaultyShapeDisease();
			}
		}

		private void FaultyShapeDisease()
		{
			try
			{
				foreach (Pawn pawn in ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive)
				{
					if (!pawn.IsShapeshifter())
					{
						continue;
					}
					if (pawn.genes.Endogenes.Count > 35)
					{
						Gene gene = pawn.genes.Endogenes.RandomElement();
						pawn.genes.RemoveGene(gene);
						Message(pawn, gene);
						continue;
					}
					if (pawn.genes.Xenogenes.Count > XenotypeDefOf.Sanguophage.genes.Count + 3)
					{
						Gene gene = pawn.genes.Xenogenes.RandomElement();
						pawn.genes.RemoveGene(gene);
						Message(pawn, gene);
						continue;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove shapeshifter(s) random gene. Reason: " + arg.Message);
			}

			static void Message(Pawn pawn, Gene gene)
			{
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("WVC_XaG_FaultyShapeDisease".Translate(pawn, gene.Label), pawn, MessageTypeDefOf.NegativeEvent);
				}
			}
		}

		public override void PostWorldGenerate()
		{
			SetupGenes();
		}

		private void SetGeneral()
		{
			SetupGenes();
			Gene_Shapeshifter.xenotypesOverride = ListsUtility.GetAllXenotypesHolders().Where(xenos => !xenos.genes.Any(gene => gene.biostatArc != 0)).ToList();
			HarmonyPatch_Bisexual();
		}

		private static bool genesSetted = false;
		private void SetupGenes()
		{
			if (genesSetted)
			{
				return;
			}
			try
			{
				foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading.Where(gene => !gene.IsAndroid()))
				{
					geneDef.biostatCpx /= 2;
					if (geneDef.biostatArc > 0)
					{
						int newArc = geneDef.biostatArc / 2;
						if (newArc < 1)
						{
							newArc = 1;
						}
						geneDef.biostatArc = newArc;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed setup genes. Reason: " + arg.Message);
			}
			genesSetted = true;
		}

		private static bool hasTraitHook_Patched = false;
		public static void HarmonyPatch_Bisexual()
		{
			if (hasTraitHook_Patched)
			{
				return;
			}
			try
			{
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef), typeof(int)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
			}
			catch (Exception arg)
			{
				Log.Warning("Non-critical error. Failed apply bisexual hook. Reason: " + arg.Message);
			}
			hasTraitHook_Patched = true;
		}

	}

}
