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
					foreach (XaG_CountWithChance colorHolder in gene.Props.holofaces)
					{
						colors.Add(colorHolder.color);
					}
					foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefs)
					{
						colors.Add(allDef.color);
					}
					colors.SortByColor((Color x) => x);
				}
				return colors;
			}
		}

		public Dialog_ChangeEyesColor(Gene_Eyes gene)
		{
			this.gene = gene;
			forcePause = true;
			//closeOnAccept = false;
			//closeOnCancel = false;
			desireColor = gene.color;
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
				Reset();
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 1.5f - ButSize.x / 1.5f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Reset".Translate()))
			{
				Reset();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 3f - ButSize.x / 3f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Disable".Translate()))
			{
				Disable();
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMax - ButSize.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
			{
				Accept();
				Close();
			}
		}

		private void Disable()
		{
			gene.SetColor(desireColor, false);
		}

		private void Accept()
		{
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
