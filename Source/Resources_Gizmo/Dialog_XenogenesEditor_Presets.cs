using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenogenesEditor_Presets : Dialog_FileList
	{

		public Dialog_XenogenesEditor xenogenesEditor;
		public List<GeneSetPreset> Presets => xenogenesEditor.gene.GeneSetPresets;

		public Dialog_XenogenesEditor_Presets(Dialog_XenogenesEditor dialog)
		{
			this.xenogenesEditor = dialog;
			interactButLabel = "LoadGameButton".Translate();
			deleteTipKey = "DeleteThisXenotype";
		}

		private Vector2 scrollPosition;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = Presets.Count * y;
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
			foreach (GeneSetPreset file in Presets.ToList())
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					Rect rect2 = new(rect.width - 36f, (rect.height - 36f) / 2f, 36f, 36f);
					if (Widgets.ButtonImage(rect2, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
					{
						GeneSetPreset localFile = file;
						Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(localFile.name), delegate
						{
							Presets.Remove(localFile);
						}, destructive: true));
					}
					TooltipHandler.TipRegionByKey(rect2, deleteTipKey);
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect2.x - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, interactButLabel))
					{
						GeneSetPreset localFile = file;
						Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_Dialog_ChimeraPresetsList_Load".Translate(localFile.name), delegate
						{
							xenogenesEditor.SetPreset(localFile);
							Close();
						}, destructive: true));
					}
					Rect rect4 = new(rect3.x - 94f, 0f, 94f, rect.height);
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
					Rect rect5 = new(8f, 0f, rect4.x - 8f - 4f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Text.Font = GameFont.Small;
					string fileNameWithoutExtension = file.name.CapitalizeFirst();
					Widgets.Label(rect5, fileNameWithoutExtension.Truncate(rect5.width * 1.8f));
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

		protected override void DoFileInteraction(string fileName)
		{
		}

		protected override void ReloadFiles()
		{
		}

	}

}
