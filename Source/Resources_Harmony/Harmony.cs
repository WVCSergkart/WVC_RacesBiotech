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
					harmony.Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGeneDef"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Xag_DrawGeneDef")));
				}
				if (WVC_Biotech.settings.fixVanillaGeneImmunityCheck)
				{
					// harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "HediffGiversCanGive"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Immunity_hediffGivers")));
					harmony.Patch(AccessTools.Method(typeof(ImmunityHandler), "AnyGeneMakesFullyImmuneTo"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Immunity_makeImmuneTo")));
				}
				if (WVC_Biotech.settings.enableHarmonyTelepathyGene)
				{
					harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "IsGoodPositionForInteraction", new Type[] {typeof(Pawn), typeof(Pawn)} ), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("TelepathyGene")));
				}
				if (!WVC_Biotech.settings.disableFurGraphic)
				{
					harmony.Patch(AccessTools.Method(typeof(PawnRenderNode_Body), "GraphicFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("FurskinIsSkin")));
					// harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveGeneGraphics"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("SpecialGeneGraphic")));
				}
				// if (WVC_Biotech.settings.enableBodySizeGenes)
				// {
					// harmony.Patch(AccessTools.Method(typeof(PawnRenderNodeWorker), "ScaleFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("GraphicSize")));
					// harmony.Patch(AccessTools.Method(typeof(HumanlikeMeshPoolUtility), "HumanlikeBodyWidthForPawn"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("BodyGraphicSize")));
					// harmony.Patch(AccessTools.Method(typeof(HumanlikeMeshPoolUtility), "HumanlikeHeadWidthForPawn"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("HeadGraphicSize")));
				// }
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
				if (geneDef.IsXenoGenesDef())
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

			// public static void GraphicSize(ref Vector3 __result, PawnRenderNode node)
			// {
				// if (node is PawnRenderNode_Body)
				// {
					// if (node.Props is PawnRenderNodeProperties_Size size)
					// {
						// __result *= size.bodyScaleFactor;
					// }
				// }
				// else if (node is PawnRenderNode_Head)
				// {
					// if (node.Props is PawnRenderNodeProperties_Size size)
					// {
						// __result *= size.headScaleFactor;
					// }
				// }
			// }

			public static void BodyGraphicSize(ref float __result, ref Pawn pawn)
			{
				Gene_BodySize gene = pawn?.genes?.GetFirstGeneOfType<Gene_BodySize>();
				if (gene == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = gene.Graphic;
				if (modExtension == null)
				{
					return;
				}
				__result *= modExtension.bodyScaleFactor;
			}

			public static void HeadGraphicSize(ref float __result, ref Pawn pawn)
			{
				Gene_BodySize gene = pawn?.genes?.GetFirstGeneOfType<Gene_BodySize>();
				if (gene == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = gene.Graphic;
				if (modExtension == null)
				{
					return;
				}
				__result *= modExtension.headScaleFactor;
			}

			public static void FurskinIsSkin(Pawn pawn, ref Graphic __result)
			{
				// if (pawn?.genes == null)
				// {
					// return;
				// }
				FurDef furDef = pawn?.story?.furDef;
				if (furDef == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = furDef?.GetModExtension<GeneExtension_Graphic>();
				if (modExtension == null)
				{
					return;
				}
				RotDrawMode curRotDrawMode = pawn.Drawer?.renderer != null ? pawn.Drawer.renderer.CurRotDrawMode : RotDrawMode.Fresh;
				if (curRotDrawMode == RotDrawMode.Dessicated)
				{
					return;
				}
				if (ModsConfig.AnomalyActive)
				{
					if (pawn.IsMutant && !pawn.mutant.Def.bodyTypeGraphicPaths.NullOrEmpty())
					{
						return;
					}
					if (pawn.IsCreepJoiner && pawn.story.bodyType != null && !pawn.creepjoiner.form.bodyTypeGraphicPaths.NullOrEmpty())
					{
						return;
					}
				}
				string bodyPath = furDef?.GetFurBodyGraphicPath(pawn);
				Color skinColor = curRotDrawMode == RotDrawMode.Rotting ? PawnRenderUtility.GetRottenColor(pawn.story.SkinColor) : pawn.story.SkinColor;
				if (modExtension.furIsSkinWithHair)
				{
					__result = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, skinColor, pawn.story.HairColor);
				}
				else if (modExtension.furIsSkin)
				{
					__result = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn), Vector2.one, skinColor);
				}
			}

			// public static void SpecialGeneGraphic(PawnGraphicSet __instance)
			// {
				// foreach (GeneGraphicRecord graphicRecord in __instance.geneGraphics.ToList())
				// {
					// if (graphicRecord.sourceGene is not Gene_Faceless faceless || faceless.drawGraphic)
					// {
						// continue;
					// }
					// __instance.geneGraphics.Remove(graphicRecord);
				// }
			// }

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
