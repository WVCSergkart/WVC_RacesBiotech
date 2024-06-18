using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ShapeshifterHeritableGenes : Dialog_FileList
	{

		public List<GeneDef> evolveGenes;

		public List<GeneDef> heritableGenes;

		public Gene_Shapeshifter gene;

		public Dialog_ShapeshifterHeritableGenes(Gene_Shapeshifter gene)
		{
			this.gene = gene;
			this.evolveGenes = XenotypeFilterUtility.GetShapeshifterHeritableGenes();
			UpdateGenes();
			interactButLabel = "WVC_XaG_ChimeraApply_Implant".Translate();
			forcePause = false;
		}

		public UpdateGenes()
		{
			heritableGenes = new();
			foreach (GeneDef gene in evolveGenes)
			{
				if (XaG_GeneUtility.HasGene(gene, gene.pawn))
				{
					heritableGenes.Add(gene);
				}
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)files.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			if (ShouldDoTypeInField)
			{
				num -= 53f;
			}
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (GeneDef file in evolveGenes)
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					Widgets.DrawBoxSolidWithOutline(rect, GetBoxColor(file), GetBoxOutlineColor(file));
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					Rect rect2 = new(rect.width - 36f, (rect.height - 36f) / 2f, 36f, 36f);
					// if (Widgets.ButtonImage(rect2, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
					// {
						// GeneSetPresets localFile = file;
						// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(localFile.name), delegate
						// {
							// chimeraDialog.gene.geneSetPresets.Remove(localFile);
						// }, destructive: true));
					// }
					// TooltipHandler.TipRegionByKey(rect2, deleteTipKey);
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect2.x - 100f, (rect.height - 36f) / 2f, 133f, 36f);
					if (Widgets.ButtonText(rect3, interactButLabel))
					{
						// DoFileInteraction(Path.GetFileNameWithoutExtension(file.FileName));
						// GeneDef localFile = file;
						// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_Dialog_ChimeraPresetsList_Load".Translate(file.LabelCap), delegate
						// {
						// }));
						if (heritableGenes.Contains(file) || heritableGenes.Count >= gene.maxEvolveGenes)
						{
							Messages.Message("WVC_XaG_ShapeshifterWarning_NeedMoreGenesSlots".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
						}
						else
						{
							gene.AddGene(file);
							UpdateGenes();
							gene.DoEffects();
						}
					}
					Rect rect4 = new(rect3.x - 94f, 0f, 94f, rect.height);
					// DrawDateAndVersion(file, rect4);
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
					// GUI.color = FileNameColor(file);
					Rect rect5 = new(8f, 0f, rect4.x - 8f - 4f, rect.height);
					Widgets.DefIcon(new(rect5.xMin, rect5.y, rect.height, rect.height), file);
					Rect rect6 = new(rect5.x + rect5.height, rect5.y, rect5.width, rect5.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Text.Font = GameFont.Small;
					string fileNameWithoutExtension = file.LabelCap;
					Widgets.Label(rect6, fileNameWithoutExtension.Truncate(rect5.width * 1.8f));
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
			if (ShouldDoTypeInField)
			{
				DoTypeInField(inRect.TopPartPixels(inRect.height - Window.CloseButSize.y - 18f));
			}
		}

		public Color GetBoxColor(GeneDef geneDef)
		{
			if (heritableGenes.Contains(geneDef))
			{
				return TexUI.ActiveResearchColor;
			}
			return TexUI.AvailResearchColor;
		}

		public Color GetBoxOutlineColor(GeneDef geneDef)
		{
			if (heritableGenes.Contains(geneDef))
			{
				return TexUI.HighlightBorderResearchColor;
			}
			return TexUI.DefaultBorderResearchColor;
		}

		protected override void DoFileInteraction(string fileName)
		{
		}

		protected override void ReloadFiles()
		{
		}

	}

}
