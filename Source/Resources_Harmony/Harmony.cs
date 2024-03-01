using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	public class WVC_XenotypesAndGenes_Main : Mod
	{
		public WVC_XenotypesAndGenes_Main(ModContentPack content)
			: base(content)
		{
			new Harmony("wvc.sergkart.races.biotech").PatchAll();
			// var harmony = new Harmony("wvc.sergkart.races.biotech");
			// harmony.PatchAll();
			// if (WVC_Biotech.settings.hideXaGGenes)
			// {
				// new Harmony("wvc.sergkart.races.biotech").Patch(typeof(Dialog_CreateXenotype).GetMethod("DrawGene"), prefix: new HarmonyMethod(typeof(Patch_Dialog_CreateXenotype_DrawGene).GetMethod("Prefix")));
			// }
		}
	}

	namespace HarmonyPatches
	{

		public static class HarmonyUtility
		{

			public static void PostInitialPatches()
			{
				var harmony = new Harmony("wvc.sergkart.races.biotech");
				if (WVC_Biotech.settings.genesCanTickOnlyOnMap)
				{
					harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "GeneTrackerTick"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("GeneTickOptimization")));
				}
				if (WVC_Biotech.settings.hideXaGGenes)
				{
					harmony.Patch(AccessTools.Method(typeof(Dialog_CreateXenotype), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Patch_HideGenes")));
				}
				if (!WVC_Biotech.settings.disableUniqueGeneInterface)
				{
					harmony.Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Xag_DrawGene")));
					harmony.Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGeneDef_NewTemp"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Xag_DrawGeneDef")));
				}
				if (WVC_Biotech.settings.fixVanillaGeneImmunityCheck)
				{
					harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "HediffGiversCanGive"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Immunity_hediffGivers")));
					harmony.Patch(AccessTools.Method(typeof(ImmunityHandler), "AnyGeneMakesFullyImmuneTo"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Immunity_makeImmuneTo")));
				}
				if (WVC_Biotech.settings.enableHarmonyTelepathyGene)
				{
					harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "IsGoodPositionForInteraction", new Type[] {typeof(Pawn), typeof(Pawn)} ), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("TelepathyGene")));
				}
				if (!WVC_Biotech.settings.disableFurGraphic)
				{
					harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("FurskinIsSkin")));
					harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveGeneGraphics"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("SpecialGeneGraphic")));
				}
				if (WVC_Biotech.settings.enableIncestLoverGene)
				{
					harmony.Patch(AccessTools.Method(typeof(RelationsUtility), "Incestuous"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Incestuous_Relations")));
					harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Incestuous_LovinChanceFactor")));
				}
			}

			// Patches

			// Hide genes in editor

			public static bool Patch_HideGenes(GeneDef geneDef, ref bool __result)
			{
				if (geneDef.IsFromXenoGenes())
				{
					__result = false;
					return false;
				}
				return true;
			}

			// Backgroud

			public static bool Xag_DrawGene(ref Gene gene, ref Rect geneRect, ref GeneType geneType, ref bool doBackground, ref bool clickable)
			{
				if (GeneUiUtility.ReplaceGeneBackground(gene.def))
				{
					GeneUiUtility.DrawGeneBasics(gene.def, geneRect, geneType, doBackground, clickable, !gene.Active);
					if (Mouse.IsOver(geneRect))
					{
						string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.DescriptionFull;
						text += GeneUiUtility.AdditionalInfo_Gene(gene);
						text += GeneUiUtility.AdditionalInfo_GeneDef(gene.def);
						if (gene.Overridden)
						{
							text += "\n\n";
							text = ((gene.overriddenByGene.def != gene.def) ? (text + ("OverriddenByGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)) : (text + ("OverriddenByIdenticalGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)));
						}
						else if (!gene.Active)
						{
							text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive".Translate().Colorize(ColorLibrary.RedReadable);
						}
						if (Prefs.DevMode)
						{
							text += "\n\n DevMode:".Colorize(ColoredText.TipSectionTitleColor);
							text += "\n - defName: " + gene.def.defName.ToString();
							text += "\n - geneClass: " + gene.GetType().ToString();
						}
						if (clickable)
						{
							text += "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
						}
						TooltipHandler.TipRegion(geneRect, text);
					}
					return false;
				}
				return true;
			}

			public static bool Xag_DrawGeneDef(ref GeneDef gene, ref Rect geneRect, ref GeneType geneType, ref Func<string> extraTooltip, ref bool doBackground, ref bool clickable, ref bool overridden)
			{
				if (GeneUiUtility.ReplaceGeneBackground(gene))
				{
					GeneUiUtility.DrawGeneBasics(gene, geneRect, geneType, doBackground, clickable, overridden);
					if (Mouse.IsOver(geneRect))
					{
						string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.DescriptionFull;
						text += GeneUiUtility.AdditionalInfo_GeneDef(gene);
						if (extraTooltip != null)
						{
							string text2 = extraTooltip();
							if (!text2.NullOrEmpty())
							{
								text = text + "\n\n" + text2.Colorize(ColorLibrary.RedReadable);
							}
						}
						if (clickable)
						{
							text += "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
						}
						TooltipHandler.TipRegion(geneRect, text);
					}
					return false;
				}
				return true;
			}

			// Telepathy

			public static void TelepathyGene(ref bool __result, Pawn p, Pawn recipient)
			{
				if (__result)
				{
					return;
				}
				if (recipient.PawnPsychicSensitive() && p?.genes?.GetFirstGeneOfType<Gene_Telepathy>() != null)
				{
					if (p.Map != null)
					{
						FleckMaker.AttachedOverlay(p, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
					}
					__result = true;
				}
			}

			// Body graphic

			public static void FurskinIsSkin(PawnGraphicSet __instance)
			{
				Pawn pawn = __instance.pawn;
				// if (!ModsConfig.BiotechActive || pawn == null || pawn.RaceProps?.Humanlike != true || pawn?.genes == null)
				// {
					// return;
				// }
				if (pawn?.genes == null || pawn?.story?.furDef == null)
				{
					return;
				}
				Gene_Exoskin gene_Exoskin = pawn.genes.GetFirstGeneOfType<Gene_Exoskin>();
				if (gene_Exoskin == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = gene_Exoskin.Graphic;
				if (modExtension == null)
				{
					return;
				}
				__instance.furCoveredGraphic = null;
				string bodyPath = pawn.story.furDef.GetFurBodyGraphicPath(pawn);
				if (modExtension.furIsSkinWithHair)
				{
					__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
				}
				else if (modExtension.furIsSkin)
				{
					__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
				}
				if (modExtension.furCanRot)
				{
					__instance.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColorOverriden ? (PawnGraphicSet.RottingColorDefault * pawn.story.SkinColor) : PawnGraphicSet.RottingColorDefault);
				}
			}

			public static void SpecialGeneGraphic(PawnGraphicSet __instance)
			{
				// foreach (Gene item in __instance.pawn.genes.GenesListForReading)
				// {
					// if (!item.Active || item is not Gene_Faceless faceless || faceless.drawGraphic)
					// {
						// continue;
					// }
					// foreach (GeneGraphicRecord graphicRecord in __instance.geneGraphics.ToList())
					// {
						// if (graphicRecord.sourceGene != item)
						// {
							// continue;
						// }
						// __instance.geneGraphics.Remove(graphicRecord);
					// }
				// }
				foreach (GeneGraphicRecord graphicRecord in __instance.geneGraphics.ToList())
				{
					if (graphicRecord.sourceGene is not Gene_Faceless faceless || faceless.drawGraphic)
					{
						continue;
					}
					__instance.geneGraphics.Remove(graphicRecord);
				}
			}

			// Romance

			public static void Incestuous_Relations(ref bool __result, ref Pawn one)
			{
				if (!__result)
				{
					return;
				}
				if (one?.genes?.GetFirstGeneOfType<Gene_IncestLover>() != null)
				{
					__result = false;
				}
			}

			public static void Incestuous_LovinChanceFactor(ref float __result, Pawn ___pawn, ref Pawn otherPawn, Pawn_RelationsTracker __instance)
			{
				if (__instance.FamilyByBlood.Contains(otherPawn) && ___pawn?.genes?.GetFirstGeneOfType<Gene_IncestLover>() != null)
				{
					__result *= 100f;
				}
			}

			// Immunity: Replaces the vanilla check (without gene Active check) with a version with gene Active check

			public static bool Immunity_hediffGivers(ref bool __result, ref HediffDef hediff, Pawn_GeneTracker __instance)
			{
				// if (!ModLister.BiotechInstalled)
				// {
					// __result = true;
					// return false;
				// }
				for (int i = 0; i < __instance.GenesListForReading.Count; i++)
				{
					Gene gene = __instance.GenesListForReading[i];
					if (gene?.Active != true || gene?.def?.hediffGiversCannotGive == null)
					{
						continue;
					}
					if (gene.def.hediffGiversCannotGive.Contains(hediff))
					{
						__result = false;
						return false;
					}
				}
				__result = true;
				return false;
			}

			public static bool Immunity_makeImmuneTo(ref bool __result, ref HediffDef def, ImmunityHandler __instance)
			{
				// if (!ModsConfig.BiotechActive || __instance?.pawn?.genes == null)
				// {
					// __result = false;
					// return false;
				// }
				if (__instance?.pawn?.genes == null)
				{
					__result = false;
					return false;
				}
				for (int i = 0; i < __instance.pawn.genes.GenesListForReading.Count; i++)
				{
					Gene gene = __instance.pawn.genes.GenesListForReading[i];
					if (gene?.Active != true || gene?.def?.makeImmuneTo == null)
					{
						continue;
					}
					if (gene.def.makeImmuneTo.Contains(def))
					{
						__result = true;
						return false;
					}
				}
				__result = false;
				return false;
			}

			// Rottable

			// public static bool CompRottable_RotProgress(CompRottable __instance)
			// {
				// if (__instance.parent is Pawn pawn && pawn.IsNotRottable())
				// {
					// return false;
				// }
				// return true;
			// }

			// public static bool CompRottable_CompInspectStringExtra(CompRottable __instance, ref string __result)
			// {
				// if (__instance.parent is Corpse corpse && corpse.InnerPawn.IsNotRottable())
				// {
					// __result = null;
					// return false;
				// }
				// return true;
			// }

			// public static bool CompRottable_Active(CompRottable __instance, ref bool __result)
			// {
				// if (__instance.parent is Corpse corpse && corpse.InnerPawn.IsNotRottable())
				// {
					// __result = false;
					// return false;
				// }
				// return true;
			// }

			// Dev TESTS

			public static bool GeneTickOptimization(Pawn_GeneTracker __instance)
			{
				if (__instance.pawn.Map == null)
				{
					return false;
				}
				return true;
			}

		}

	}

}
