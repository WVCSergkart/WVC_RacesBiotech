using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	//public class WVC_XenotypesAndGenes_Main : Mod
	//{
	//	public WVC_XenotypesAndGenes_Main(ModContentPack content)
	//		: base(content)
	//	{
	//		new Harmony("wvc.sergkart.races.biotech").PatchAll();
	//	}
	//}

	namespace HarmonyPatches
	{

		public static class HarmonyUtility
		{

			public static void HarmonyPatches()
			{
				var harmony = new Harmony("wvc.sergkart.races.biotech");
				if (WVC_Biotech.settings.generateXenotypeForceGenes || WVC_Biotech.settings.generateSkillGenes || WVC_Biotech.settings.enable_dryadQueenMechanicGenerator)
				{
					harmony.Patch(AccessTools.Method(typeof(GeneDefGenerator), "ImpliedGeneDefs"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Patch_GeneDefGenerator_ImpliedGeneDefs))));
				}
				if (WVC_Biotech.settings.hideXaGGenes)
				{
					harmony.Patch(AccessTools.Method(typeof(Dialog_CreateXenotype), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Patch_HideGenes))));
				}
				//Log.Error("0");
				//if (WVC_Biotech.settings.genesCanTickOnlyOnMap)
				//{
				//	harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "GeneTrackerTick"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(GeneTickOptimization))));
				//}
				//Log.Error("1");
				if (WVC_Biotech.settings.hideXaGGenes)
				{
					harmony.Patch(AccessTools.Method(typeof(Dialog_CreateXenotype), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Patch_HideGenes))));
				}
				//Log.Error("2");
				if (!WVC_Biotech.settings.disableUniqueGeneInterface)
				{
					harmony.Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGene"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Xag_DrawGene))));
					harmony.Patch(AccessTools.Method(typeof(GeneUIUtility), "DrawGeneDef"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Xag_DrawGeneDef))));
				}
				if (WVC_Biotech.settings.enable_HideMechanitorButtonsPatch)
				{
					harmony.Patch(AccessTools.Method(typeof(Pawn_MechanitorTracker), "GetGizmos"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(MechanitorHideWithGene))));
				}
				//Log.Error("3");
				if (WVC_Biotech.settings.fixVanillaGeneImmunityCheck)
				{
					// harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "HediffGiversCanGive"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("Immunity_hediffGivers")));
					harmony.Patch(AccessTools.Method(typeof(ImmunityHandler), "AnyGeneMakesFullyImmuneTo"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Immunity_makeImmuneTo))));
					//harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "CheckForOverrides"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("FixOverrides")));
					harmony.Patch(AccessTools.Method(typeof(GeneUtility), "ReimplantXenogerm"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(BasicImplanterDebug))));
					harmony.Patch(AccessTools.Method(typeof(GeneUtility), "ImplantXenogermItem"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(BasicXenogermDebug))));
					harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateGenes"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(BasicGenerateGenesDebug))));
					harmony.Patch(AccessTools.Method(typeof(AnomalyUtility), "OpenCodexGizmo"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(AnomalyCodexNullRefFix))));
					harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GeneratePawnRelations"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Patch_PawnGenerator_GeneratePawnRelations))));
				}
				//Log.Error("4");
				//if (WVC_Biotech.settings.enableHarmonyTelepathyGene)
				//{
				//	harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "IsGoodPositionForInteraction", new Type[] {typeof(Pawn), typeof(Pawn)} ), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(TelepathyGene))));
				//}
				//Log.Error("5");
				if (!WVC_Biotech.settings.disableFurGraphic)
				{
					harmony.Patch(AccessTools.Method(typeof(PawnRenderNode_Body), "GraphicFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(FurskinIsSkin))));
					//harmony.Patch(AccessTools.Method(typeof(PawnRenderNode_Head), "GraphicFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HeadIsFurskin))));
					// harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveGeneGraphics"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("SpecialGeneGraphic")));
					//harmony.Patch(AccessTools.Method(typeof(PawnRenderNode_Hair), "GraphicFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("GlowingHair")));
				}
				//Log.Error("6");
				// if (WVC_Biotech.settings.enableBodySizeGenes)
				// {
				// harmony.Patch(AccessTools.Method(typeof(PawnRenderNodeWorker), "ScaleFor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("GraphicSize")));
				// harmony.Patch(AccessTools.Method(typeof(HumanlikeMeshPoolUtility), "HumanlikeBodyWidthForPawn"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("BodyGraphicSize")));
				// harmony.Patch(AccessTools.Method(typeof(HumanlikeMeshPoolUtility), "HumanlikeHeadWidthForPawn"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("HeadGraphicSize")));
				// }
				if (!WVC_Biotech.settings.disableNonAcceptablePreyGenes)
				{
					harmony.Patch(AccessTools.Method(typeof(FoodUtility), "IsAcceptablePreyFor"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(IsNotAcceptablePrey))));
				}
				//Log.Error("7");
				if (WVC_Biotech.settings.enable_OverOverridableGenesMechanic)
				{
					harmony.Patch(AccessTools.Method(typeof(GameComponent_PawnDuplicator), "CopyGenes"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(PawnDuplicatorFix))));
				}
				//Log.Error("8");
				if (WVC_Biotech.settings.enableIncestLoverGene)
				{
					harmony.Patch(AccessTools.Method(typeof(RelationsUtility), "Incestuous"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Incestuous_Relations))));
					harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Incestuous_LovinChanceFactor))));
				}
				//Log.Error("9");
				if (WVC_Biotech.settings.harmony_EnableGenesMechanicsTriggers)
				{
					harmony.Patch(AccessTools.Method(typeof(Gene), "OverrideBy"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(OverrideTrigger))));
					harmony.Patch(AccessTools.Method(typeof(LifeStageWorker), "Notify_LifeStageStarted"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(Notify_LifeStageStarted))));
					harmony.Patch(AccessTools.Method(typeof(SanguophageUtility), "DoBite"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(BloodeaterTrigger))));
					//harmony.Patch(AccessTools.Method(typeof(CompBiosculpterPod), "TryAcceptPawn", new Type[] { typeof(Pawn), typeof(CompBiosculpterPod_Cycle) }), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("XenosculpterPod_TryAcceptPawn_Patch")));
					//harmony.Patch(AccessTools.Method(typeof(CompBiosculpterPod), "OrderToPod"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod("XenosculpterPod_OrderToPod_Patch")));
					//harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(MechanitorBandwidthGizmo), "Visible"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(MechanitorHideWithGene))));
				}
				//Log.Error("10");
			}

			// Patches
			public static IEnumerable<GeneDef> Patch_GeneDefGenerator_ImpliedGeneDefs(IEnumerable<GeneDef> values)
			{
				List<GeneDef> geneDefList = values.ToList();
				GeneratorUtility.Aptitudes(geneDefList);
				GeneratorUtility.HybridForcerGenes(geneDefList);
				//GeneratorUtility.Spawners(geneDefList);
				//GeneratorUtility.AutoColorGenes(geneDefList);
				GeneratorUtility.GauranlenTreeModeDef();
				return geneDefList;
			}
			public static bool Patch_PawnGenerator_GeneratePawnRelations(Pawn pawn)
			{
				//if (!XaG_GeneUtility.HasAnyActiveGene(new() { MainDefOf.WVC_FemaleOnly, MainDefOf.WVC_MaleOnly }, pawn))
				//{
				//	return true;
				//}
				if (pawn.genes?.GetFirstGeneOfType<Gene_Gender>() == null)
				{
					return true;
				}
				return false;
			}

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
				if (gene.def.IsXenoGenesDef())
				{
					XaG_UiUtility.DrawGeneBasics(gene.def, geneRect, geneType, doBackground, clickable, !gene.Active);
					if (Mouse.IsOver(geneRect))
                    {
                        string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.DescriptionFull;
                        text += XaG_UiUtility.AdditionalInfo_GeneDef(gene.def);
                        text = OverridenByGene(gene, text);
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

            private static string OverridenByGene(Gene gene, string text)
            {
                if (gene.Overridden)
                {
                    text += "\n\n";
					if (gene.overriddenByGene != gene)
					{
						text = ((gene.overriddenByGene.def != gene.def) ? (text + ("OverriddenByGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)) : (text + ("OverriddenByIdenticalGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)));
					}
					else
					{
						text += ("WVC_XaG_OverriddenByItself".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable);
					}
                }
                return text;
            }

            public static bool Xag_DrawGeneDef(ref GeneDef gene, ref Rect geneRect, ref GeneType geneType, ref Func<string> extraTooltip, ref bool doBackground, ref bool clickable, ref bool overridden)
			{
				if (gene.IsXenoGenesDef())
				{
					XaG_UiUtility.DrawGeneBasics(gene, geneRect, geneType, doBackground, clickable, overridden);
					if (Mouse.IsOver(geneRect))
					{
						string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.DescriptionFull;
						text += XaG_UiUtility.AdditionalInfo_GeneDef(gene);
						if (extraTooltip != null)
						{
							string text2 = extraTooltip();
							if (!text2.NullOrEmpty())
							{
								text = text + "\n\n" + text2.Colorize(ColorLibrary.RedReadable);
							}
						}
						if (Prefs.DevMode)
						{
							text += "\n\n DevMode:".Colorize(ColoredText.TipSectionTitleColor);
							text += "\n - selectionWeight: " + gene.selectionWeight.ToString();
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

            //public static void TelepathyGene(ref bool __result, Pawn p, Pawn recipient)
            //{
            //	if (__result)
            //	{
            //		return;
            //	}
            //	if (recipient.IsPsychicSensitive() && p?.genes?.GetFirstGeneOfType<Gene_Telepathy>() != null)
            //	{
            //		if (p.Map != null)
            //		{
            //			FleckMaker.AttachedOverlay(p, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
            //		}
            //		__result = true;
            //	}
            //}

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

            // public static void BodyGraphicSize(ref float __result, ref Pawn pawn)
            // {
            // Gene_BodySize gene = pawn?.genes?.GetFirstGeneOfType<Gene_BodySize>();
            // if (gene == null)
            // {
            // return;
            // }
            // GeneExtension_Graphic modExtension = gene.Graphic;
            // if (modExtension == null)
            // {
            // return;
            // }
            // __result *= modExtension.bodyScaleFactor;
            // }

            // public static void HeadGraphicSize(ref float __result, ref Pawn pawn)
            // {
            // Gene_BodySize gene = pawn?.genes?.GetFirstGeneOfType<Gene_BodySize>();
            // if (gene == null)
            // {
            // return;
            // }
            // GeneExtension_Graphic modExtension = gene.Graphic;
            // if (modExtension == null)
            // {
            // return;
            // }
            // __result *= modExtension.headScaleFactor;
            // }

            //public static void GlowingHair(Pawn pawn, ref Graphic __result, PawnRenderNode_Hair __instance)
            //{
            //	if (__result == null)
            //	{
            //		return;
            //	}
            //	if (!GeneFeaturesUtility.HasLuminescentHairGene(pawn))
            //	{
            //		return;
            //	}
            //	__result = GraphicDatabase.Get<Graphic_Multi>(pawn.story.hairDef.texPath, ShaderDatabase.MoteGlow, Vector2.one, __instance.ColorFor(pawn));
            //}

			//public static void HeadIsFurskin(Pawn pawn, ref Graphic __result, PawnRenderNode_Head __instance)
			//{
			//	FurDef furDef = pawn?.story?.furDef;
			//	if (furDef == null)
			//	{
			//		return;
			//	}
			//	GeneExtension_Graphic modExtension = furDef?.GetModExtension<GeneExtension_Graphic>();
			//	if (modExtension == null)
			//	{
			//		return;
			//	}
			//	if (XaG_GeneUtility.Furskin_ShouldNotDrawNow(pawn))
			//	{
			//		return;
			//	}
			//	if (pawn.IsMutant && !pawn.mutant.Def.bodyTypeGraphicPaths.NullOrEmpty())
			//	{
			//		return;
			//	}
			//	if (pawn.IsCreepJoiner && pawn.story.bodyType != null && !pawn.creepjoiner.form.bodyTypeGraphicPaths.NullOrEmpty())
			//	{
			//		return;
			//	}
			//	if (modExtension.furIsSkinTransparent)
			//	{
			//		Color headColor = __instance.ColorFor(pawn);
			//		headColor.a = modExtension.alpha;
			//		__result = pawn.story?.headType?.GetGraphic(pawn, headColor);
			//	}
			//}

			public static void FurskinIsSkin(Pawn pawn, ref Graphic __result, PawnRenderNode_Body __instance)
			{
				FurDef furDef = pawn?.story?.furDef;
				if (furDef == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = furDef.GetModExtension<GeneExtension_Graphic>();
				if (modExtension == null)
				{
					return;
				}
				if (XaG_GeneUtility.Furskin_ShouldNotDrawNow(pawn))
				{
					return;
				}
				if (pawn.IsMutant && !pawn.mutant.Def.bodyTypeGraphicPaths.NullOrEmpty())
				{
					return;
				}
				if (pawn.IsCreepJoiner && pawn.story.bodyType != null && !pawn.creepjoiner.form.bodyTypeGraphicPaths.NullOrEmpty())
				{
					return;
				}
				//if (modExtension.invisible)
				//{
				//	__result = GraphicDatabase.Get<Graphic_Multi>("WVC/UI/InvisibleThing", ShaderDatabase.Cutout);
				//	return;
				//}
				string bodyPath = furDef.GetFurBodyGraphicPath(pawn);
				Color skinColor = __instance.ColorFor(pawn);
				//if (modExtension.furIsSkinTransparent)
				//{
				//	skinColor.a = modExtension.alpha;
				//}
				if (modExtension.furIsSkinWithHair)
				{
					__result = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, skinColor, pawn.story.HairColor);
				}
				else if (modExtension.furIsSkinWithMask)
				{
					__result = GraphicDatabase.Get<Graphic_Multi>(bodyPath, WVC_Biotech.settings.useMaskForFurskinGenes ? ShaderDatabase.CutoutComplex : ShaderUtility.GetSkinShader(pawn), Vector2.one, skinColor);
				}
				else if (modExtension.furIsSkin)
				{
					__result = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn), Vector2.one, skinColor);
				}
			}

			public static void Notify_LifeStageStarted(ref Pawn pawn)
			{
				if (pawn?.genes == null)
				{
					return;
				}
				foreach (Gene gene in pawn.genes.GenesListForReading)
				{
					if (gene is IGeneLifeStageStarted gene_LifeStageStarted && gene.Active)
					{
						try
						{
							gene_LifeStageStarted.Notify_LifeStageStarted();
						}
						catch
						{
							Log.Error("Failed trigger Notify_LifeStageStarted for gene " + gene.def.defName);
						}
					}
				}
				ReimplanterUtility.PostImplantDebug(pawn);
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
					if (gene?.def?.makeImmuneTo == null || !gene.Active)
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

			public static void BasicImplanterDebug(Pawn recipient)
			{
				ReimplanterUtility.PostImplantDebug(recipient);
			}

			public static void BasicXenogermDebug(Pawn pawn)
			{
				ReimplanterUtility.PostImplantDebug(pawn);
			}

			public static void BasicGenerateGenesDebug(Pawn pawn)
			{
				ReimplanterUtility.PostImplantDebug(pawn);
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

			// Bloodeater

			public static void BloodeaterTrigger(ref Pawn biter, ref Pawn victim)
			{
				if (biter == null || victim == null)
				{
					return;
				}
				foreach (Gene gene in biter.genes.GenesListForReading)
				{
					if (gene is IGeneBloodfeeder geneBloodfeeder && gene.Active)
					{
						try
						{
							geneBloodfeeder.Notify_Bloodfeed(victim);
						}
						catch
						{
							Log.Error("Failed trigger Notify_Bloodfeed for gene " + gene.def.defName);
						}
					}
				}
			}

			// GeneOverride

			public static void OverrideTrigger(Gene __instance, Gene overriddenBy)
			{
				if (__instance is IGeneOverridden geneOverridden)
				{
					if (overriddenBy != null)
					{
						geneOverridden.Notify_OverriddenBy(overriddenBy);
					}
					else
					{
						geneOverridden.Notify_Override();
					}
				}
				if (__instance is IGeneInspectInfo || __instance is IGeneFloatMenuOptions)
				{
					// Log.Error("ResetGenesInspectString");
					XaG_GeneUtility.ResetGenesInspectString(__instance.pawn);
				}
				//if (__instance is IGeneRemoteControl remote)
				//{
				//	remote.RemoteControl_Recache();
				//}
				// if (__instance is IGeneNotifyGenesChanged geneNotifyGenesChanged)
				// {
				// geneNotifyGenesChanged.Notify_GenesChanged(overriddenBy);
				// }
			}

			public static void FixOverrides(Pawn_GeneTracker __instance)
			{
				List<Gene> xenogenes = __instance.pawn.genes.Xenogenes;
				List<Gene> endogenes = __instance.pawn.genes.Endogenes;
				if (xenogenes.NullOrEmpty() || endogenes.NullOrEmpty())
				{
					return;
				}
				for (int i = 0; i < xenogenes.Count; i++)
				{
					Gene xenogene = xenogenes[i];
					if (!xenogene.Overridden)
					{
						continue;
					}
					for (int j = 0; j < endogenes.Count; j++)
					{
						Gene endogene = endogenes[i];
						if (!endogene.Overridden)
						{
							continue;
						}
						if (xenogene.def.ConflictsWith(endogene.def) || endogene.def.ConflictsWith(xenogene.def))
						{
							endogene.OverrideBy(xenogene);
						}
					}
				}
				__instance.pawn?.skills?.DirtyAptitudes();
				__instance.pawn?.Notify_DisabledWorkTypesChanged();
				//__instance.pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			}

			// Predators

			public static bool IsNotAcceptablePrey(ref bool __result, ref Pawn prey)
			{
				if (prey.RaceProps.Humanlike && GeneFeaturesUtility.IsNotAcceptablePrey(prey))
				{
					__result = false;
					return false;
				}
				return true;
			}

			// PawnDuplicatorFix

			public static bool PawnDuplicatorFix(ref Pawn pawn, ref Pawn newPawn)
			{
				DuplicateUtility.CopyGenes(pawn, newPawn);
				return false;
			}

			// CompBiosculpterPod

			//public static void XenosculpterPod_TryAcceptPawn_Patch(ref bool __result, ref CompBiosculpterPod_Cycle cycle, ref float ___currentCycleTicksRemaining, CompBiosculpterPod __instance)
			//{
			//	if (__result && cycle is CompBiosculpterPod_XenotypeHolderCycle holderCycle)
			//	{
			//		___currentCycleTicksRemaining += holderCycle.additionalCycleDays * 60000;
			//		if (holderCycle.ShouldInterrupt)
			//		{
			//			holderCycle.ResetCycle();
			//			__instance.EjectContents(interrupted: true, playSounds: true);
			//		}
			//	}
			//}

			//         public static void XenosculpterPod_OrderToPod_Patch(ref CompBiosculpterPod_Cycle cycle)
			//         {
			//             if (cycle is CompBiosculpterPod_XenotypeHolderCycle holderCycle)
			//	{
			//		holderCycle.StartCycle();
			//	}
			//         }

			// FoodPolicy

			// public static void StartingFoodRestrictions(List<FoodPolicy> ___foodRestrictions, FoodRestrictionDatabase __instance)
			// {
			// List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef x) => x.GetStatValueAbstract(StatDefOf.Nutrition) > 0f).ToList();
			// FoodPolicy bloodEaterFoodPolicy = __instance.MakeNewFoodRestriction();
			// bloodEaterFoodPolicy.label = "WVC_XaG_BloodEaterFoodPolicy".Translate();
			// foreach (ThingDef item in thingDefs.Where((ThingDef x) => x.ingestible != null))
			// {
			// if (item.ingestible.foodType == FoodTypeFlags.Fluid)
			// {
			// bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
			// }
			// else
			// {
			// bloodEaterFoodPolicy.filter.SetAllow(item, allow: false);
			// }
			// }
			// FoodPolicy energyFoodPolicy = __instance.MakeNewFoodRestriction();
			// energyFoodPolicy.label = "WVC_XaG_EnergyFoodPolicy".Translate();
			// foreach (ThingDef item in thingDefs)
			// {
			// energyFoodPolicy.filter.SetAllow(item, allow: false);
			// }
			// }

			// PawnDuplicatorFix

			public static bool MechanitorHideWithGene(ref IEnumerable<Gizmo> __result, ref Pawn ___pawn)
			{
				if (StaticCollectionsClass.hideMechanitorButton.Contains(___pawn))
				{
					__result = MechanitorHideWithGene();
					return false;
				}
				return true;

				static IEnumerable<Gizmo> MechanitorHideWithGene()
				{
					yield break;
				}
			}

			// Codex spam fix

			public static bool AnomalyCodexNullRefFix(ref Gizmo __result, ref Thing thing)
			{
				if (thing is Pawn pawn && pawn.IsMutant && pawn.mutant.Def?.codexEntry == null)
				{
					__result = null;
					return false;
				}
				return true;
			}

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
