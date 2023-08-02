using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public static class GeneUiUtility
    {

		public static bool ReplaceGeneBackground(GeneDef geneDef)
		{
			if (!WVC_Biotech.settings.disableUniqueGeneInterface && geneDef.defName.Contains("WVC_"))
			{
				return true;
			}
			return false;
		}

		public static string AdditionalInfo_GeneDef(GeneDef def)
		{
			string text = "";
			if (def.graphicData != null && def.graphicData.fur != null)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin".Translate().Colorize(ColoredText.TipSectionTitleColor);
				if (!def.forcedHeadTypes.NullOrEmpty())
				{
					text += " " + "WVC_XaG_NewBack_GeneIsFurskin_HeadOverride".Translate().Colorize(ColoredText.TipSectionTitleColor);
				}
				// text += ".".Colorize(ColoredText.TipSectionTitleColor);
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsFurskin_CanBeDisabled".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			if (ReimplanterUtility.GenesNonCandidatesForSerums().Contains(def))
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsNonCandidatesForSerum".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			else if (ReimplanterUtility.GenesPerfectCandidatesForSerums().Contains(def))
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneIsPerfectCandidatesForSerum".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			if (SubXenotypeUtility.GeneIsRandom(def))
			{
				if (!SubXenotypeUtility.GeneIsShuffle(def))
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsRandom".Translate().Colorize(ColoredText.SubtleGrayColor);
				}
				else
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsShuffle".Translate().Colorize(ColoredText.SubtleGrayColor);
				}
			}
			if (def.selectionWeight == 0 && !def.canGenerateInGeneSet)
			{
				text += "\n\n" + "WVC_XaG_NewBack_GeneCannotSpawnInGenepacks".Translate().Colorize(ColoredText.SubtleGrayColor);
			}
			return text;
		}

		public static string AdditionalInfo_Gene(Gene gene)
		{
			string text = "";
			if (gene.def.geneClass == typeof(Gene_Undead))
			{
				Gene_Undead undead = (Gene_Undead)gene;
				if (undead.DustogenicCanReincarnate())
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_UndeadReincarnate".Translate().Colorize(ColoredText.SubtleGrayColor);
				}
				if (undead.AnyResourceIsActive())
				{
					text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_Undead".Translate().Colorize(ColoredText.SubtleGrayColor);
				}
			}
			if (gene.def.geneClass == typeof(Gene_Scarifier))
			{
				Gene_Scarifier scarifier = (Gene_Scarifier)gene;
				text += "\n\n" + ("WVC_XaG_NewBack_GeneIsScarifier".Translate() + ": " + scarifier.cachedMaxScars.ToString()).Colorize(ColoredText.SubtleGrayColor);
			}
			return text;
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
			CachedTexture cachedTexture = new("WVC/UI/Genes/GeneBackground_Endogene");
			if (gene.biostatArc == 0)
			{
				switch (geneType)
				{
				case GeneType.Endogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_Endogene");
					break;
				case GeneType.Xenogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_Xenogene");
					break;
				}
			}
			else
			{
				switch (geneType)
				{
				case GeneType.Endogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_ArchiteGene");
					break;
				case GeneType.Xenogene:
					cachedTexture = new("WVC/UI/Genes/GeneBackground_XenoArchiteGene");
					break;
				}
			}
			return cachedTexture;
		}

    }
}
