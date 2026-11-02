using RimWorld;
using System.Text;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{
	public class GeneGizmo_Fleshshaper : GeneGizmo_Shapeshifter
	{

		protected override CachedTexture Shapeshifter_MenuIcon => XaG_UiUtility.Fleshshaper_MenuIcon;
		protected override CachedTexture Shapeshifter_BonusIcon => XaG_UiUtility.Fleshshaper_BonusGenesIcon;
		protected override CachedTexture Shapeshifter_EditorIcon => XaG_UiUtility.Fleshshaper_XenogenesEditorIcon;
		protected override CachedTexture Shapeshifter_StyleIcon => XaG_UiUtility.Fleshshaper_StyleIcon;

		public IGeneXenogenesEditor xenogenesEditor;
		private Gene_Fleshshaper gene_Fleshshaper;

		public GeneGizmo_Fleshshaper(Gene_Shapeshifter geneShapeshifter) : base(geneShapeshifter)
		{
			if (geneShapeshifter is IGeneXenogenesEditor xenogenesEditor)
			{
				this.xenogenesEditor = xenogenesEditor;
			}
			if (geneShapeshifter is Gene_Fleshshaper gene_Fleshshaper)
			{
				this.gene_Fleshshaper = gene_Fleshshaper;
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
					stringBuilder.AppendLineTagged("WVC_XaG_FleshshaperGizmoTip".Translate(gene.ShaperResource, gene_Fleshshaper.OverloadLevel));
					if (Gene_Chimera.XenogenesEditorLimit)
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

		protected override void RecacheTick()
		{
			if (nextRecache > 0)
			{
				nextRecache--;
				return;
			}
			cachedDescription = null;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			RecacheTick();
			return base.GizmoOnGUI(topLeft, maxWidth, parms);
		}

		protected override void Uncollapsed(Vector2 topLeft, float maxWidth)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Button1c(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Button2c(rect5);
			// Button
			Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
			ButtonXenogenesEditor(rect6);
			// Button
			Rect rect7 = new(rect6.x + 44f, rect6.y, rect6.width, rect6.height);
			XaG_UiUtility.Button_Style(rect7, pawn, gene, true, Shapeshifter_StyleIcon);
		}

		protected void ButtonXenogenesEditor(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, Shapeshifter_EditorIcon.Texture, 1f);
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
