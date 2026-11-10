using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_XenogenesEditor : Gizmo
	{

		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Chimera_GizmoMenu");

		public Pawn pawn;

		public Gene_XenogenesEditor_General gene;

		public override bool Visible => true;

		private string cachedDescription;
		public string Tooltip
		{
			get
			{
				if (cachedDescription == null)
				{
					//Log.Error("0");
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLineTagged(gene.def.LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.AppendLineTagged(gene.def.resourceDescription);
					stringBuilder.AppendLine();
					stringBuilder.AppendLineTagged("WVC_XaG_Chimera_GizmoTip_GenesLimit".Translate(gene.ComplexityLimit, gene.ArchiteLimit));
					IntRange reqMetRange = gene.ReqMetRange;
					if (reqMetRange.TrueMin > -99 || reqMetRange.TrueMax < 99)
					{
						stringBuilder.AppendLineTagged("WVC_XaG_Chimera_GizmoTip_MetLimit".Translate(reqMetRange.TrueMin, reqMetRange.TrueMax));
					}
					cachedDescription = stringBuilder.ToString().TrimEndNewlines();
				}
				return cachedDescription;
			}
		}

		public GeneGizmo_XenogenesEditor(Gene_XenogenesEditor_General geneChimera)
			: base()
		{
			gene = geneChimera;
			pawn = gene?.pawn;
			Order = -94f;
			cachedDescription = null;
			nextRecache = 60;
		}

		private int nextRecache = 2;

		private void RecacheTick()
		{
			nextRecache--;
			if (nextRecache < 0)
			{
				//Log.Error("0");
				cachedDescription = null;
				nextRecache = 240;
			}
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Basic(topLeft, maxWidth, out Rect rect2, out Rect rect3);
			// Button
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
			EditorButton(rect4);
			return new GizmoResult(GizmoState.Clear);
		}

		private void Basic(Vector2 topLeft, float maxWidth, out Rect rect2, out Rect rect3)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			RecacheTick();
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, Tooltip);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, gene.def.LabelShortAdj.CapitalizeFirst());
		}

		private void EditorButton(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					gene.UpdateCache();
					gene.UpdSubHediffs();
					Find.WindowStack.Add(new Dialog_XenogenesEditor(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_XenogenesEditorTip".Translate());
		}

		public override float GetWidth(float maxWidth)
		{
			return 96f;
		}

	}

}
