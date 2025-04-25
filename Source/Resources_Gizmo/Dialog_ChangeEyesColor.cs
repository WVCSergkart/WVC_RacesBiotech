using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ChangeEyesColor : Window
	{

		public Gene_Eyes gene;

		private List<Color> colors;

		private Color desireColor;

		private List<Color> AllGameColors
		{
			get
			{
				if (colors == null)
				{
					colors = new List<Color>();
					foreach (GeneralHolder colorHolder in gene.Props.holofaces)
					{
						if (colors.Contains(colorHolder.color))
						{
							continue;
						}
						colors.Add(colorHolder.color);
					}
					foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefsListForReading)
					{
						if (colors.Contains(allDef.color))
						{
							continue;
						}
						colors.Add(allDef.color);
					}
					colors.SortByColor((Color x) => x);
				}
				return colors;
			}
		}

		public bool closeOnAccpet;

		public Dialog_ChangeEyesColor(Gene_Eyes gene, bool closeOnAccpet)
		{
			this.gene = gene;
			forcePause = true;
			//closeOnAccept = false;
			//closeOnCancel = false;
			draggable = true;
			this.closeOnAccpet = closeOnAccpet;
			desireColor = gene.color;
			closeOnClickedOutside = !closeOnAccpet;
		}

		public override void DoWindowContents(Rect inRect)
		{
			//Text.Font = GameFont.Medium;
			Rect rect = new(inRect);
			//rect.height = Text.LineHeight * 2f;
			//Widgets.Label(rect, "WVC_XaG_ColorableEyesLabel".Translate().CapitalizeFirst());
			DrawColors(rect);
			DrawBottomButtons(inRect);
		}

		private float colorsHeight;

		private void DrawColors(Rect rect)
		{
			float y = rect.y;
			Widgets.ColorSelector(new Rect(rect.x, y, rect.width, colorsHeight), ref desireColor, AllGameColors, out colorsHeight);
			colorsHeight += Text.LineHeight * 2f;
		}

		private static readonly Vector2 ButSize = new(100f, 20f);


		private void DrawBottomButtons(Rect inRect)
		{
			if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Cancel".Translate()))
			{
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 1.5f - ButSize.x / 1.5f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Reset".Translate()))
			{
				Reset();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 3f - ButSize.x / 3f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Disable".Translate()))
			{
				Disable();
				if (this.closeOnAccpet)
				{
					Close();
				}
			}
			if (Widgets.ButtonText(new Rect(inRect.xMax - ButSize.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
			{
				Accept();
				if (this.closeOnAccpet)
				{
					Close();
				}
			}
		}

		private void Disable()
		{
			//foreach (Gene item in gene.pawn.genes.GenesListForReading)
			//{
			//	if (item is Gene_Eyes eyes)
			//	{
			//		eyes.SetColor(desireColor, false);
			//	}
			//}
			gene.SetColor(desireColor, false);
		}

		private void Accept()
		{
			//foreach (Gene item in gene.pawn.genes.GenesListForReading)
			//{
			//	if (item is Gene_Eyes eyes)
			//	{
			//		eyes.SetColor(desireColor, true);
			//	}
			//}
			gene.SetColor(desireColor, true);
		}

		private void Reset()
		{
			if (gene.DefaultEyesColor.HasValue)
			{
				desireColor = gene.DefaultEyesColor.Value;
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			else
            {
				Messages.Message("WVC_XaG_ResetEyesColorWarning".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
	}

}
