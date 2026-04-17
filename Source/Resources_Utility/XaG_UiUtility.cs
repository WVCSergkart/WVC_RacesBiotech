using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class XaG_UiUtility
	{

		public static readonly CachedTexture GeneBackground_Endogene = new("WVC/UI/Genes/GeneBackground_Endogene");
		public static readonly CachedTexture GeneBackground_Xenogene = new("WVC/UI/Genes/GeneBackground_Xenogene");
		public static readonly CachedTexture GeneBackground_ArchiteEndogene = new("WVC/UI/Genes/GeneBackground_ArchiteGene");
		public static readonly CachedTexture GeneBackground_ArchiteXenogene = new("WVC/UI/Genes/GeneBackground_XenoArchiteGene");

		public static readonly CachedTexture GenesSettingsGizmo = new("WVC/UI/XaG_General/UI_GenesSettings_Gizmo");
		public static readonly CachedTexture GenesSettingsIcon = new("WVC/UI/XaG_General/GenesSettings_ShapeshifterGizmo");

		public static readonly CachedTexture StyleIcon = new("WVC/UI/XaG_General/Shapeshifter_GizmoStyle");

		public static readonly CachedTexture CollapseIcon = new("WVC/UI/XaG_General/UI_CollapseButton");
		public static readonly CachedTexture NonAggressiveRedCancelIcon = new("WVC/UI/XaG_General/UI_NonAggressiveRed_Cancel");

		public static readonly CachedTexture GermlineImplanterIcon = new("WVC/UI/XaG_General/ThrallMaker_Implanter_Gizmo_v0");

		public static readonly CachedTexture GenelineIconMark = new("WVC/UI/XaG_General/GenelineBackground");

		public static readonly CachedTexture ShapeshifterGenMatReqTex = new("WVC/UI/XaG_General/GenMatTex_Req");
		public static readonly CachedTexture ShapeshifterGenMatHasTex = new("WVC/UI/XaG_General/GenMatTex_Has");

		//public static readonly CachedTexture FeatheredGenMatReqTex = new("WVC/UI/XaG_General/GenMatTex_Req");
		//public static readonly CachedTexture FeatheredGenMatHasTex = new("WVC/UI/XaG_General/GenMatTex_Has");

		public static readonly CachedTexture FeatheredGenMatReqTex = new("WVC/UI/XaG_General/GenMatTex_FeathersReq");
		public static readonly CachedTexture FeatheredGenMatHasTex = new("WVC/UI/XaG_General/GenMatTex_FeathersHas");

		public static Gizmo GetRemoteControllerGizmo(Pawn pawn, bool remoteControllerCached, IGeneRemoteControl gene)
		{
			if (!remoteControllerCached)
			{
				gene.RemoteControl_Recache();
			}
			//if (XaG_GeneUtility.SelectorDraftedFactionMap(pawn))
			//{
			//	yield break;
			//}
			return new Command_GenesSettings
			{
				defaultLabel = "WVC_XaG_GenesSettings".Translate(),
				defaultDesc = "WVC_XaG_GenesSettingsDesc".Translate(),
				icon = XaG_UiUtility.GenesSettingsGizmo.Texture,
				shrinkable = true,
				visible = !XaG_GeneUtility.SelectorDraftedFactionMap(pawn),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_GenesSettings(pawn));
				}
			};
			//if (gene is Gene genegene)
			//{
			//    Log.Error(genegene.def.defName);
			//}
		}

		public static void RecacheRemoteController(Pawn pawn, ref bool remoteControllerCached, ref bool enabled)
		{
			SetAllRemoteControllersTo(pawn, false);
			//foreach (Gene gene in pawn.genes.GenesListForReading)
			//{
			//	if (gene is IGeneRemoteControl geneRemoteControl)
			//	{
			//		geneRemoteControl.RemoteControl_Enabled = false;
			//	}
			//}
			remoteControllerCached = true;
			enabled = true;
		}

		public static void SetAllRemoteControllersTo(Pawn pawn, bool setTo = true)
		{
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is IGeneRemoteControl geneRemoteControl)
				{
					geneRemoteControl.RemoteControl_Enabled = setTo;
				}
				//else if (gene is IGeneRemoteMainframe mainframe && mainframe != mainGene)
				//            {
				//	mainframe.RemoteMainframe_Reset();
				//}
			}
		}

		public static string OnOrOff(bool on)
		{
			if (on)
			{
				return "On".Translate().Colorize(ColorLibrary.Green);
			}
			return "Off".Translate().Colorize(ColorLibrary.RedReadable);
		}

		public static void FlickSound(bool flick)
		{
			if (flick)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}

		public static Gizmo ITab_InspectBabyGenes()
		{
			return new Command_Action
			{
				defaultLabel = "InspectBabyGenes".Translate() + "...",
				defaultDesc = "InspectGenesHediffDesc".Translate(),
				icon = GeneSetHolderBase.GeneticInfoTex.Texture,
				action = delegate
				{
					InspectPaneUtility.OpenTab(typeof(ITab_GenesCustomPregnancy));
				}
			};
		}

		public static void GizmoButton(Rect rect3, ref bool collapse)
		{
			Text.Anchor = TextAnchor.UpperRight;
			Rect buttonRect = new(rect3.xMax - rect3.height, rect3.y, rect3.height, rect3.height);
			Widgets.DrawTextureFitted(buttonRect, CollapseIcon.Texture, 1f);
			if (Mouse.IsOver(buttonRect))
			{
				Widgets.DrawHighlight(buttonRect);
				if (Widgets.ButtonInvisible(buttonRect))
				{
					collapse = !collapse;
					XaG_UiUtility.FlickSound(collapse);
				}
			}
			TooltipHandler.TipRegion(buttonRect, "WVC_XaG_CollapseButtonDesc".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public static void StyleButton_WithoutRect(Rect rect5, Pawn pawn, Gene gene, bool unlocked)
		{
			Widgets.DrawTextureFitted(rect5, StyleIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					Find.WindowStack.Add(new Dialog_StylingGene(pawn, gene, unlocked));
					//if (ModLister.IdeologyInstalled)
					//{
					//}
					//else
					//{
					//	Messages.Message("WVC_XaG_ReqIdeology".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
					//}
				}
			}
			TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneShapeshifterStyles_Desc".Translate());
		}

		public static string ToStringHuman(this Gender gender)
		{
			return gender switch
			{
				Gender.None => "WVC_None".Translate(),
				Gender.Female => "Female".Translate(),
				Gender.Male => "Male".Translate(),
				_ => "WVC_None".Translate(),
			};
		}

		public static string ToStringHuman(this RotStage rotStage)
		{
			return rotStage switch
			{
				RotStage.Fresh => "RotStateFresh".Translate(),
				RotStage.Rotting => "RotStateRotting".Translate(),
				RotStage.Dessicated => "RotStateDessicated".Translate(),
				_ => "error",
			};
		}

		public static void DrawStat(Rect iconRect, CachedTexture icon, string stat, float iconWidth)
		{
			GUI.DrawTexture(iconRect, icon.Texture);
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.LabelFit(new Rect(iconRect.xMax, iconRect.y, 38f - iconWidth, iconWidth), stat);
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public static void MiddleLabel_None(Rect rect3)
		{
			Text.Anchor = TextAnchor.MiddleCenter;
			GUI.color = ColoredText.SubtleGrayColor;
			Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		// ===========================

		//[Obsolete]
		//public static bool ReplaceGeneBackground(GeneDef geneDef)
		//{
		//	if (geneDef.IsXenoGenesDef())
		//	{
		//		return true;
		//	}
		//	// if (geneDef.GetModExtension<GeneExtension_Graphic>() != null)
		//	// {
		//		// return true;
		//	// }
		//	return false;
		//}

		public static string AdditionalInfo_GeneDef(GeneDef def)
		{
			string text = "";
			if (def.fur != null)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin".Translate().Colorize(ColoredText.TipSectionTitleColor);
				if (!def.forcedHeadTypes.NullOrEmpty())
				{
					text += " " + "WVC_XaG_NewBack_GeneIsFurskin_HeadOverride".Translate().Colorize(ColoredText.TipSectionTitleColor);
				}
				//text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin_CanBeDisabled".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			if (def.selectionWeight <= 0 && !def.canGenerateInGeneSet)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneCannotSpawnInGenepacks".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			//if (ModLister.CheckIdeology("Scarification") && def?.GetModExtension<GeneExtension_Giver>() != null)
			//{
			//	if (def.GetModExtension<GeneExtension_Giver>().scarsCount != 0)
			//	{
			//		int scarsCount = def?.GetModExtension<GeneExtension_Giver>() != null ? def.GetModExtension<GeneExtension_Giver>().scarsCount : 0;
			//		string scarsLimitText = scarsCount > 0 ? "WVC_XaG_NewBack_GeneIsScarifier_SubGenesIncrease" : "WVC_XaG_NewBack_GeneIsScarifier_SubGenesDecrease";
			//		text += "\n\n" + scarsLimitText.Translate(scarsCount).Colorize(ColoredText.SubtleGrayColor);
			//	}
			//}
			return text;
		}

		//[Obsolete]
		//public static string AdditionalInfo_Gene(Gene gene)
		//{
		//	string text = "";
		//	if (gene is Gene_Undead undead)
		//	{
		//		if (undead.UndeadCanResurrect)
		//		{
		//			text += "\n\n" + "WVC_XaG_NewBack_GeneIsActive_Undead".Translate();
		//		}
		//		else if (undead.PreventResurrectionHediffs != null)
		//		{
		//			text += "\n\n" + "WVC_XaG_Gene_DisplayStats_Undead_CanResurrectHediffs_Desc".Translate() + ":"
		//			+ "\n"
		//			+ undead.PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true);
		//		}
		//	}
		//	else if (gene is Gene_Reincarnation reincarnation)
		//	{
		//		if (reincarnation.ReincarnationActive())
		//		{
		//			text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_UndeadReincarnate".Translate();
		//		}
		//	}
		//	else if (gene is Gene_Scarifier scarifier)
		//	{
		//		text += "\n\n" + ("WVC_XaG_NewBack_GeneIsScarifier".Translate() + ": " + scarifier.cachedMaxScars.ToString());
		//	}
		//	return text.Colorize(ColoredText.SubtleGrayColor);
		//}

		public static void DrawGeneBasics(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			if (doBackground)
			{
				Widgets.DrawHighlight(rect);
				GUI.color = new(1f, 1f, 1f, 0.05f);
				Widgets.DrawBox(rect);
				GUI.color = Color.white;
			}
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = gene.IconColor;
			if (overridden)
			{
				iconColor.a = 0.75f;
				GUI.color = ColoredText.SubtleGrayColor;
			}
			CachedTexture cachedTexture = BackgroundTexture(gene, geneType);
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			Widgets.DefIcon(rect2, gene, null, 0.9f, null, drawPlaceholder: false, iconColor);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(gene.LabelCap, rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			if (overridden)
			{
				GUI.color = ColoredText.SubtleGrayColor;
			}
			if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
			{
				rect3.y -= 3f;
			}
			Widgets.Label(rect3, gene.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			if (clickable)
			{
				if (Widgets.ButtonInvisible(rect))
				{
					Find.WindowStack.Add(new Dialog_InfoCard(gene));
				}
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
			}
			GUI.EndGroup();
		}

		public static CachedTexture BackgroundTexture(GeneDef gene, GeneType geneType)
		{
			CachedTexture cachedTexture = GeneBackground_Endogene;
			//if (gene is GenelineDef xenogene)
			//{
			//	return xenogene.BackgroundIcon;
			//}
			if (gene.biostatArc == 0)
			{
				switch (geneType)
				{
					case GeneType.Endogene:
						cachedTexture = GeneBackground_Endogene;
						break;
					case GeneType.Xenogene:
						cachedTexture = GeneBackground_Xenogene;
						break;
				}
			}
			else
			{
				switch (geneType)
				{
					case GeneType.Endogene:
						cachedTexture = GeneBackground_ArchiteEndogene;
						break;
					case GeneType.Xenogene:
						cachedTexture = GeneBackground_ArchiteXenogene;
						break;
				}
			}
			return cachedTexture;
		}

		// Settings

		public static bool ButtonTextWithTooltip(this Listing_Standard ls, string label, bool drawbackground = false, string highlightTag = null, float widthPct = 1f, string tooltip = null)
		{
			Rect rect = ls.GetRect(30f, widthPct);
			bool result = false;
			if (!ls.BoundingRectCached.HasValue || rect.Overlaps(ls.BoundingRectCached.Value))
			{
				result = Widgets.ButtonText(rect, label, drawbackground, overrideTextAnchor: TextAnchor.MiddleCenter);
				if (highlightTag != null)
				{
					UIHighlighter.HighlightOpportunity(rect, highlightTag);
				}
				else if (Mouse.IsOver(rect) && !drawbackground)
				{
					Widgets.DrawHighlight(rect);
				}
			}
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			ls.Gap(ls.verticalSpacing);
			return result;
		}

		public static void SliderLabeledWithRef(this Listing_Standard ls, string label, ref float val, float min = 0f, float max = 1f, string tooltip = null, int round = 2)
		{
			Rect rect = ls.GetRect(Text.LineHeight);
			Rect rect2 = rect.LeftPart(0.5f).Rounded();
			Rect rect3 = rect.RightPart(0.62f).Rounded().LeftPart(0.97f).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			float _ = (val = Widgets.HorizontalSlider(rect3, val, min, max, middleAlignment: true));
			val = (float)Math.Round(val, round);
			Text.Anchor = TextAnchor.MiddleRight;
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			ls.Gap(ls.verticalSpacing);
		}

		public static void IntRangeLabeledWithRef(this Listing_Standard ls, string label, ref IntRange range, int min = 0, int max = 1, string tooltip = null)
		{
			Rect rect = ls.GetRect(Text.LineHeight);
			Rect rect2 = rect.LeftPart(0.5f).Rounded();
			Rect rect3 = rect.RightPart(0.62f).Rounded().LeftPart(0.97f).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			Text.Anchor = TextAnchor.MiddleRight;
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			if (!ls.BoundingRectCached.HasValue || rect3.Overlaps(ls.BoundingRectCached.Value))
			{
				Widgets.IntRange(rect3, (int)ls.CurHeight, ref range, min, max);
			}
			ls.Gap(ls.verticalSpacing);
		}

		public static void XaG_DefIcon(Rect rect, Def def, float scale = 1f, Color? color = null, Material material = null)
		{
			if (def is ThrallDef thrallDef)
			{
				GUI.color = color ?? Color.white;
				Widgets.DrawTextureFitted(rect, thrallDef.xenotypeIconDef.Icon, scale, material);
				GUI.color = Color.white;
			}
			else if (def is XenotypeDef xenotypeDef)
			{
				GUI.color = color ?? XenotypeDef.IconColor;
				Widgets.DrawTextureFitted(rect, xenotypeDef.Icon, scale, material);
				GUI.color = Color.white;
			}
			else if (def is XenotypeIconDef xenotypeIconDef)
			{
				GUI.color = color ?? XenotypeDef.IconColor;
				Widgets.DrawTextureFitted(rect, xenotypeIconDef.Icon, scale, material);
				GUI.color = Color.white;
			}
			else if (def is GeneDef geneDef)
			{
				GUI.color = color ?? geneDef.IconColor;
				Widgets.DrawTextureFitted(rect, geneDef.Icon, scale, material);
				GUI.color = Color.white;
			}
		}

		// public static void XaG_CustomXenotypeIcon(Rect rect, CustomXenotype customXenotype, float scale = 1f, Color? color = null, Material material = null)
		// {
		// GUI.color = color ?? Color.white;
		// Widgets.DrawTextureFitted(rect, customXenotype.IconDef.Icon, scale, material);
		// GUI.color = Color.white;
		// }

		public static void XaG_Icon(Rect rect, Texture icon, float scale = 1f, Color? color = null, Material material = null)
		{
			GUI.color = color ?? Color.white;
			Widgets.DrawTextureFitted(rect, icon, scale, material);
			GUI.color = Color.white;
		}


		public static void DevOptions(Listing_Standard listingStandard, Mod mod)
		{
			if (!Prefs.DevMode)
			{
				return;
			}
			if (listingStandard.ButtonText("DEV: Count active WVC_ genes"))
			{
				List<GeneDef> genes = new();
				int genesCount = 0;
				foreach (Def def in mod.Content.AllDefs)
				{
					if (def is GeneDef geneDef)
					{
						genesCount++;
						genes.Add(geneDef);
					}
				}
				Log.Error("WVC Genes: " + genesCount.ToString());
				if (!genes.NullOrEmpty())
				{
					Log.Error("All active XaG genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
				}
				else
				{
					Log.Error("Genes list is null");
				}
			}
			if (listingStandard.ButtonText("DEV: Count used in xenotypes genes"))
			{
				List<GeneDef> genes = new();
				int genesCount = 0;
				foreach (Def def in mod.Content.AllDefs)
				{
					if (def is XenotypeDef xenotypeDef)
					{
						foreach (GeneDef geneDef in xenotypeDef.genes)
						{
							if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
							{
								genesCount++;
								genes.Add(geneDef);
							}
						}
					}
				}
				Log.Error("WVC Genes: " + genesCount.ToString());
				if (!genes.NullOrEmpty())
				{
					Log.Error("All xenotype genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
				}
				else
				{
					Log.Error("Genes list is null");
				}
			}
			if (listingStandard.ButtonText("DEV: Count used in thrallDefs genes"))
			{
				List<GeneDef> genes = new();
				int genesCount = 0;
				foreach (Def def in mod.Content.AllDefs)
				{
					if (def is ThrallDef xenotypeDef)
					{
						foreach (GeneDef geneDef in xenotypeDef.genes)
						{
							if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
							{
								genesCount++;
								genes.Add(geneDef);
							}
						}
					}
				}
				Log.Error("WVC Genes: " + genesCount.ToString());
				if (!genes.NullOrEmpty())
				{
					Log.Error("All thrall genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
				}
				else
				{
					Log.Error("Genes list is null");
				}
			}
			if (listingStandard.ButtonText("DEV: Log genes weights"))
			{
				// foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
				// {
				// Log.Error(geneDef.defName + " | " + geneDef.LabelCap + ": " + geneDef.selectionWeight.ToString());
				// }
				Log.Error("Genes weights:" + "\n" + DefDatabase<GeneDef>.AllDefsListForReading.Select((GeneDef x) => x.defName + " | " + x.LabelCap + ": " + x.selectionWeight).ToLineList(" - "));
			}
			if (listingStandard.ButtonText("DEV: Log Gene_CustomHair"))
			{
				Log.Error("Custom hair genes:" + "\n" + DefDatabase<GeneDef>.AllDefsListForReading.Where((g) => g.IsGeneDefOfType<Gene_CustomHair>()).Select((GeneDef x) => x.defName + " | " + x.LabelCap + ": " + x.selectionWeight).ToLineList(" - "));
			}
			if (listingStandard.ButtonText("DEV: Count unused in xenotypes genes"))
			{
				List<GeneDef> genes = new();
				int genesCount = 0;
				foreach (Def def in mod.Content.AllDefs)
				{
					if (def is XenotypeDef xenotypeDef)
					{
						foreach (GeneDef geneDef in xenotypeDef.genes)
						{
							if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
							{
								genesCount++;
								genes.Add(geneDef);
							}
						}
					}
				}
				Log.Error("WVC used genes: " + genesCount.ToString());
				if (!genes.NullOrEmpty())
				{
					Log.Error("All unused genes:" + "\n" + mod.Content.AllDefs.Where((Def x) => x is GeneDef geneDef && !genes.Contains(geneDef) && geneDef.GetModExtension<GeneExtension_Obsolete>() == null).Select((Def x) => x.defName).ToLineList(" - "));
				}
				else
				{
					Log.Error("Genes list is null");
				}
			}
			if (listingStandard.ButtonText("DEV: Log used genes"))
			{
				Dictionary<GeneDef, int> genes = new();
				foreach (Def def in mod.Content.AllDefs)
				{
					if (def is XenotypeDef xenotypeDef)
					{
						foreach (GeneDef geneDef in xenotypeDef.genes)
						{
							if (geneDef.IsXenoGenesDef())
							{
								if (!genes.TryGetValue(geneDef, out int dgene))
								{
									genes[geneDef] = 1;
								}
								else
								{
									// Log.Error(geneDef.defName + " +1");
									genes[geneDef] += 1;
								}
							}
						}
					}
				}
				if (!genes.NullOrEmpty())
				{
					string text = "";
					foreach (var item in genes)
					{
						text += "\n" + item.Key.defName + ": " + item.Value.ToString();
					}
					Log.Error("Genes:" + text);
				}
				else
				{
					Log.Error("Genes list is null");
				}
			}
			if (listingStandard.ButtonText("DEV: Log obsolete genes"))
			{
				Log.Error("Obsolete genes:" + "\n" + DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.GetModExtension<GeneExtension_Obsolete>()?.logInDevMode == true).Select((GeneDef x) => x.defName + " | " + x.LabelCap + ": " + x.selectionWeight).ToLineList(" - "));
			}
			if (listingStandard.ButtonText("DEV: Log obsolete genes in xenotypes"))
			{
				string log = "Xenotypes:";
				foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
				{
					log += "\n " + xenotypeDef.defName + " | " + xenotypeDef.label + ":";
					if (xenotypeDef.genes == null)
					{
						continue;
					}
					foreach (GeneDef geneDef in xenotypeDef.genes)
					{
						if (geneDef.IsObsolete())
						{
							log += "\n" + geneDef.defName + " | " + geneDef.label;
						}
					}
				}
				log += "\nThralls:";
				foreach (ThrallDef xenotypeDef in DefDatabase<ThrallDef>.AllDefsListForReading)
				{
					log += "\n " + xenotypeDef.defName + " | " + xenotypeDef.label + ":";
					if (xenotypeDef.genes == null)
					{
						continue;
					}
					foreach (GeneDef geneDef in xenotypeDef.genes)
					{
						if (geneDef.IsObsolete())
						{
							log += "\n" + geneDef.defName + " | " + geneDef.label;
						}
					}
				}
				Log.Error("Obsolete genes:" + "\n" + log);
			}
			if (listingStandard.ButtonText("DEV: Log chimerkins"))
			{
				Log.Error("Chimera xenotypes:" + "\n" + ListsUtility.ChimeraXenotypes.Select((XenotypeDef x) => x.defName + " | " + x.LabelCap.ToString()).ToLineList(" - "));
			}
			if (listingStandard.ButtonText("DEV: Log xenotypes"))
			{
				IEnumerable<XenotypeDef> enumerable = ListsUtility.GetAllXenotypesExceptAndroids().Where(xenos => xenos.IsXenoGenesDef());
				Log.Error("All xenotypes " + enumerable.Count() + ":" + "\n" + enumerable.Select((XenotypeDef x) => x.defName + " | " + x.LabelCap.ToString()).ToLineList(" - "));
			}
			if (listingStandard.ButtonText("DEV: Log genes dump for wiki"))
			{
				string text = "Dump: ";
				List<GeneDef> allDefsListForReading = DefDatabase<GeneDef>.AllDefsListForReading;
				List<GeneCategoryDef> geneCategoryDefs = DefDatabase<GeneCategoryDef>.AllDefsListForReading.Where(def => allDefsListForReading.Any(geneDef => geneDef.IsXenoGenesDef() && !geneDef.IsObsolete() && geneDef.displayCategory == def)).ToList();
				geneCategoryDefs.SortBy(def => -def.displayPriorityInXenotype);
				allDefsListForReading.SortBy(def => def.displayOrderInCategory);
				foreach (GeneCategoryDef geneCategoryDef in geneCategoryDefs)
				{
					text += "\n\n# " + geneCategoryDef.LabelCap;
					foreach (GeneDef geneDef in allDefsListForReading)
					{
						if (geneDef.displayCategory != geneCategoryDef || geneDef.IsObsolete() || !geneDef.IsXenoGenesDef())
						{
							continue;
						}
						//text += "\n## " + geneDef.LabelCap;
						//text += "\n" + "defName: `" + geneDef.defName + "` | geneClass: `" + geneDef.geneClass + "`";
						text += "\n##" + XaG_GeneUtility.GetDescriptionFull_Wiki(geneDef);
						//text += "\n\n" + "Selection weight (Genepack chance): " + (geneDef.selectionWeight * 100) + "% or " + geneDef.selectionWeight;
					}
				}
				// Without this, tags will be buggy
				text += "\n\n" + allDefsListForReading.RandomElement().LabelCap;
				text.ResolveTags();
				Log.Error(text);
			}
			if (listingStandard.ButtonText("DEV: Log mutant excepted genes"))
			{
				Log.Error("Ghoul disables genes:" + "\n" + MutantDefOf.Ghoul.disablesGenes.Select((GeneDef x) => "<li>" + x.defName + "</li>" + "	<!-- " + x.label + " -->").ToLineList("		"));
			}
			if (listingStandard.ButtonText("DEV: Average gene complexity"))
			{
				Log.Error("Average cpx: " + XaG_GeneUtility.GetAverageCpx);
			}
			if (listingStandard.ButtonText("DEV: Log potentially cosmetic genes"))
			{
				List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading.Where(def => XaG_GeneUtility.IsCosmeticGene(def)).ToList();
				Log.Error("Cosmetic genes:" + "\n" + geneDefs.Select((GeneDef x) => "<li>" + x.defName + "</li>" + "	<!-- " + x.label + " -->").ToLineList("		"));
			}
			if (listingStandard.ButtonText("DEV: Log cosmetic genes except furskins"))
			{
				List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading.Where(def => def.IsXenoGenesDef() && XaG_GeneUtility.IsCosmeticGene(def) && def.fur == null).ToList();
				Log.Error("Cosmetic genes:" + "\n" + geneDefs.Select((GeneDef x) => "<li>" + x.defName + "</li>" + "	<!-- " + x.label + " -->").ToLineList("		"));
			}
		}

	}
}
