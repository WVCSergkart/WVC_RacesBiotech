using RimWorld;
using System.Text;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{
	public class GeneGizmo_Fleshshaper : GeneGizmo_Shapeshifter
	{

		public IGeneXenogenesEditor xenogenesEditor;

		public GeneGizmo_Fleshshaper(Gene_Shapeshifter geneShapeshifter) : base(geneShapeshifter)
		{
			if (geneShapeshifter is IGeneXenogenesEditor xenogenesEditor)
			{
				this.xenogenesEditor = xenogenesEditor;
			}
			uncollapsedSize = 184f;
		}

		//public override TaggedString GizmoTip => "WVC_XaG_FleshshaperGizmoTip".Translate(gene.GeneticMaterial);

		private string cachedDescription;
		public override TaggedString GizmoTip
		{
			get
			{
				if (cachedDescription == null)
				{
					//Log.Error("0");
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLineTagged("WVC_XaG_FleshshaperGizmoTip".Translate(gene.ShaperResource));
					if (Gene_Chimera.ChimeraGenesLimit)
					{
						stringBuilder.AppendLineTagged("WVC_XaG_Chimera_GizmoTip_GenesLimit".Translate(xenogenesEditor.ComplexityLimit, xenogenesEditor.ArchiteLimit));
					}
					IntRange reqMetRange = xenogenesEditor.ReqMetRange;
					if (reqMetRange.TrueMin > -99 || reqMetRange.TrueMax < 99)
					{
						stringBuilder.AppendLineTagged("WVC_XaG_Chimera_GizmoTip_MetLimit".Translate(reqMetRange.TrueMin, reqMetRange.TrueMax));
					}
					cachedDescription = stringBuilder.ToString().TrimEndNewlines();
					nextRecache = 240;
				}
				return cachedDescription;
			}
		}

		private int nextRecache = 2;
		private void RecacheTick()
		{
			if (nextRecache > 0)
			{
				nextRecache--;
				return;
			}
			cachedDescription = null;
			//nextRecache = 240;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			RecacheTick();
			return base.GizmoOnGUI(topLeft, maxWidth, parms);
		}

		public override void Uncollapsed(Vector2 topLeft, float maxWidth)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			ButtonMenu(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			ButtonGenes(rect5);
			// Button
			Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
			ButtonXenogenesEditor(rect6);
			// Button
			Rect rect7 = new(rect6.x + 44f, rect6.y, rect6.width, rect6.height);
			XaG_UiUtility.StyleButton_WithoutRect(rect7, pawn, gene, true);
		}

		protected void ButtonXenogenesEditor(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, XaG_UiUtility.Shapeshifter_XenogenesEditorIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					xenogenesEditor.UpdateCache();
					xenogenesEditor.UpdSubHediffs();
					Find.WindowStack.Add(new Dialog_XenogenesEditor(xenogenesEditor));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_Fleshshaper_XenogenesEditorTip".Translate(gene.ShaperResource));
		}

	}

}
