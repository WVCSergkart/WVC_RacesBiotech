using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Skinshaper : Window
	{

		public List<GeneSetPresets> presets;
		public Gene_Skinshaper gene;

		public Dialog_Skinshaper(Gene_Skinshaper gene, List<GeneSetPresets> presets)
		{
			//remoteContoller.RemoteControl_Recache();
			this.presets = presets;
			this.gene = gene;
			forcePause = true;
			doCloseButton = true;
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = presets.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (GeneSetPresets preset in presets)
			{
				if (preset is GeneSetPresets geneSet && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, preset.geneDefs.Select((GeneDef x) => x.label.CapitalizeFirst()).ToLineList(" - "));
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, "WVC_XaG_ChimeraApply_Implant".Translate()))
					{
						gene.ImplantPreset(preset);
						Close();
						break;
					}
					Rect rect4 = new(0f, 0f, rect.width - rect3.width, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect4, geneSet.name.CapitalizeFirst().Truncate(rect4.width));
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

}
