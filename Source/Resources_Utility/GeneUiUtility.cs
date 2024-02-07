using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class GeneUiUtility
    {

		public static string OnOrOff(bool onOrOff)
		{
			if (onOrOff)
			{
				return "WVC_XaG_Gene_DustMechlink_On".Translate().Colorize(ColorLibrary.Green);
			}
			return "WVC_XaG_Gene_DustMechlink_Off".Translate().Colorize(ColorLibrary.RedReadable);
		}

		// ===========================

		public static bool ReplaceGeneBackground(GeneDef geneDef)
		{
			// geneDef.defName.Contains("WVC_")
			// if (WVC_Biotech.settings.disableUniqueGeneInterface)
			// {
				// return false;
			// }
			if (geneDef.IsXenoGenesGene())
			{
				return true;
			}
			if (geneDef.GetModExtension<GeneExtension_Background>() != null)
			{
				return true;
			}
			return false;
		}

		public static string AdditionalInfo_GeneDef(GeneDef def)
		{
			string text = "";
			if (def?.graphicData?.fur != null)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin".Translate().Colorize(ColoredText.TipSectionTitleColor);
				if (!def.forcedHeadTypes.NullOrEmpty())
				{
					text += " " + "WVC_XaG_NewBack_GeneIsFurskin_HeadOverride".Translate().Colorize(ColoredText.TipSectionTitleColor);
				}
				// text += ".".Colorize(ColoredText.TipSectionTitleColor);
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin_CanBeDisabled".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			// if (def.geneClass == typeof(Gene_AngelicStability))
			// {
				// text += "\n\n" + "WVC_XaG_NewBack_GeneIsAngelicStability".Translate().Colorize(ColoredText.SubtleGrayColor);
			// }
			// else if (def.geneClass == typeof(Gene_GeneticStability) || def.geneClass == typeof(Gene_ResurgentStability))
			// {
				// text += "\n\n" + "WVC_XaG_NewBack_GeneIsGeneticStability".Translate().Colorize(ColoredText.SubtleGrayColor);
			// }
			// else if (def.geneClass == typeof(Gene_GeneticInstability))
			// {
				// text += "\n\n" + "WVC_XaG_NewBack_GeneIsGeneticInStability".Translate().Colorize(ColoredText.SubtleGrayColor);
			// }
			//if (SubXenotypeUtility.GeneIsRandom(def))
			//{
				//if (!SubXenotypeUtility.GeneIsShuffle(def))
				//{
				//	text += "\n\n" + "WVC_XaG_NewBack_GeneIsRandom".Translate().Colorize(ColoredText.SubtleGrayColor);
				//}
				//else
				//{
				//	text += "\n\n" + "WVC_XaG_NewBack_GeneIsShuffle".Translate().Colorize(ColoredText.SubtleGrayColor);
				//}
				//text += "\n\n" + "WVC_XaG_NewBack_GeneIsRandom".Translate().Colorize(ColoredText.SubtleGrayColor);
			//}
			if (def.selectionWeight == 0 || !def.canGenerateInGeneSet)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneCannotSpawnInGenepacks".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			if (def?.GetModExtension<GeneExtension_Giver>() != null)
			{
				if (ModLister.CheckIdeology("Scarification") && def.GetModExtension<GeneExtension_Giver>().scarsCount != 0)
				{
					int scarsCount = def?.GetModExtension<GeneExtension_Giver>() != null ? def.GetModExtension<GeneExtension_Giver>().scarsCount : 0;
					string scarsLimitText = scarsCount > 0 ? "WVC_XaG_NewBack_GeneIsScarifier_SubGenesIncrease" : "WVC_XaG_NewBack_GeneIsScarifier_SubGenesDecrease";
					text += "\n\n" + scarsLimitText.Translate(scarsCount).Colorize(ColoredText.SubtleGrayColor);
				}
			}
			return text;
		}

		public static string AdditionalInfo_Gene(Gene gene)
		{
			string text = "";
			if (gene.def.geneClass == typeof(Gene_Undead))
			{
				Gene_Undead undead = (Gene_Undead)gene;
				// if (undead.UndeadCanReincarnate)
				// {
					// text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_UndeadReincarnate".Translate();
				// }
				if (undead.UndeadCanResurrect)
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsActive_Undead".Translate();
				}
				else if (undead.UndeadCanReincarnate)
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_UndeadReincarnate".Translate();
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_Undead".Translate();
				}
				else
				{
					text += "\n\n" + "WVC_XaG_Gene_DisplayStats_Undead_CanResurrectHediffs_Desc".Translate() + ":"
					+ "\n"
					+ undead.PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true);
				}
			}
			if (ModLister.CheckIdeology("Scarification") && gene.def.geneClass == typeof(Gene_Scarifier))
			{
				Gene_Scarifier scarifier = (Gene_Scarifier)gene;
				text += "\n\n" + ("WVC_XaG_NewBack_GeneIsScarifier".Translate() + ": " + scarifier.cachedMaxScars.ToString());
			}
			if (gene is Gene_Faceless faceless && !faceless.drawGraphic)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_WrongFace".Translate().Colorize(ColorLibrary.RedReadable);
			}
			// if (gene.def?.GetModExtension<GeneExtension_Giver>() != null)
			// {
				// if (gene.def.GetModExtension<GeneExtension_Giver>().scarsCount != 0)
				// {
					// int scarsCount = gene.def?.GetModExtension<GeneExtension_Giver>() != null ? gene.def.GetModExtension<GeneExtension_Giver>().scarsCount : 0;
					// string scarsLimitText = scarsCount > 0 ? "WVC_XaG_NewBack_GeneIsScarifier_SubGenesIncrease" : "WVC_XaG_NewBack_GeneIsScarifier_SubGenesDecrease";
					// text += "\n\n" + (scarsLimitText.Translate(scarsCount));
				// }
			// }
			return text.Colorize(ColoredText.SubtleGrayColor);
		}

		public static void DrawGeneBasics(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
		{
			// CachedTexture GeneBackground_Archite = new CachedTexture("WVC/UI/Genes/GeneBackground_ArchiteGene");
			// CachedTexture GeneBackground_Archite = new CachedTexture("WVC/UI/Genes/GeneBackground_Endogene");
			// CachedTexture GeneBackground_Archite = new CachedTexture("WVC/UI/Genes/GeneBackground_Xenogene");
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
			GeneExtension_Background background = gene.GetModExtension<GeneExtension_Background>();
			CachedTexture cachedTexture = new("WVC/UI/Genes/GeneBackground_Endogene");
			if (gene.biostatArc == 0)
			{
				switch (geneType)
				{
				case GeneType.Endogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_Endogene");
					if (background != null && !background.backgroundPathEndogenes.NullOrEmpty())
					{
						cachedTexture = new(background.backgroundPathEndogenes);
					}
					break;
				case GeneType.Xenogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_Xenogene");
					if (background != null && !background.backgroundPathXenogenes.NullOrEmpty())
					{
						cachedTexture = new(background.backgroundPathXenogenes);
					}
					break;
				}
			}
			else
			{
				switch (geneType)
				{
				case GeneType.Endogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_ArchiteGene");
					if (background != null && !background.backgroundPathEndoArchite.NullOrEmpty())
					{
						cachedTexture = new(background.backgroundPathEndoArchite);
					}
					break;
				case GeneType.Xenogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_XenoArchiteGene");
					if (background != null && !background.backgroundPathXenoArchite.NullOrEmpty())
					{
						cachedTexture = new(background.backgroundPathXenoArchite);
					}
					break;
				}
			}
			return cachedTexture;
		}

    }
}
