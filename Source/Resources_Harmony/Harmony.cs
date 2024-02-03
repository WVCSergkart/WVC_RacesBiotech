using HarmonyLib;
using RimWorld;
using System;
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
				if (WVC_Biotech.settings.hideXaGGenes)
				{
					new Harmony("wvc.sergkart.races.biotech.hidegenes").Patch(AccessTools.Method(typeof(Dialog_CreateXenotype), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Patch_HideGenes")));
				}
				if (!WVC_Biotech.settings.disableUniqueGeneInterface)
				{
					new Harmony("wvc.sergkart.races.biotech.geneback").Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Xag_DrawGene")));
					new Harmony("wvc.sergkart.races.biotech.geneback").Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGeneDef_NewTemp"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Xag_DrawGeneDef")));
				}
				// if (WVC_Biotech.settings.harmonyTelepathy)
				// {
					// new Harmony("wvc.sergkart.races.biotech.telepathy").Patch(AccessTools.Method(typeof(IsGoodPositionForInteraction), "IsGoodPositionForInteraction", new Type[] {typeof(Pawn), typeof(Pawn)} ), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("TelepathyGene")));
				// }
				if (!WVC_Biotech.settings.disableFurGraphic)
				{
					new Harmony("wvc.sergkart.races.biotech.bodygraphic").Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("FurskinIsSkin")));
				}
			}

			// Patches

			// Hide genes in editor

			public static bool Patch_HideGenes(GeneDef geneDef, ref bool __result)
			{
				if (geneDef.modContentPack != null && geneDef.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech"))
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
							if (gene.def.geneClass == typeof(Gene_Faceless))
							{
								text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_WrongFace".Translate().Colorize(ColorLibrary.RedReadable);
							}
							else
							{
								text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive".Translate().Colorize(ColorLibrary.RedReadable);
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
					__result = true;
				}
			}

			// Body graphic

			public static void FurskinIsSkin(PawnGraphicSet __instance)
			{
				Pawn pawn = __instance.pawn;
				if (!ModsConfig.BiotechActive || pawn == null || pawn.RaceProps?.Humanlike != true || pawn?.genes == null)
				{
					return;
				}
				if (pawn.story?.furDef == null)
				{
					return;
				}
				Gene_Exoskin gene_Exoskin = pawn.genes.GetFirstGeneOfType<Gene_Exoskin>();
				if (gene_Exoskin == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = gene_Exoskin.def.GetModExtension<GeneExtension_Graphic>();
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

		}

	}

}
