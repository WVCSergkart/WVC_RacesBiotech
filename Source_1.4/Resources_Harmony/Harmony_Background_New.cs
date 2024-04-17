using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	namespace HarmonyPatches
	{

		// This is a rather sloppy way, but unlike the transpiler, it is less conflicting.
		[HarmonyPatch(typeof(GeneUIUtility), "DrawGene")]
		public static class Patch_GeneUIUtility_DrawGene
		{

			[HarmonyPrefix]
			public static bool Prefix(ref Gene gene, ref Rect geneRect, ref GeneType geneType, ref bool doBackground, ref bool clickable)
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
							// || gene.def.geneClass == typeof(Gene_FacelessShuffle)
							if (gene.def.geneClass == typeof(Gene_Faceless))
							{
								text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_WrongFace".Translate().Colorize(ColorLibrary.RedReadable);
							}
							// else if (gene.def.geneClass == typeof(Gene_Undead))
							// {
								// text += "\n\n" + "WVC_XaG_NewBack_GeneIsNotActive_Undead".Translate().Colorize(ColoredText.SubtleGrayColor);
							// }
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
		}

		[HarmonyPatch(typeof(GeneUIUtility), "DrawGeneDef_NewTemp")]
		public static class Patch_GeneUIUtility_DrawGeneDef_NewTemp
		{

			[HarmonyPrefix]
			public static bool Prefix(ref GeneDef gene, ref Rect geneRect, ref GeneType geneType, ref Func<string> extraTooltip, ref bool doBackground, ref bool clickable, ref bool overridden)
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
						// TooltipHandler.TipRegion(geneRect, delegate
						// {
							// string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.DescriptionFull;
							// if (extraTooltip != null)
							// {
								// string text2 = extraTooltip();
								// if (!text2.NullOrEmpty())
								// {
									// text = text + "\n\n" + text2.Colorize(ColorLibrary.RedReadable);
								// }
							// }
							// if (clickable)
							// {
								// text = text + "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
							// }
							// return text;
						// }, 316238373);
					}
					return false;
				}
				return true;
			}
		}

	}

}
