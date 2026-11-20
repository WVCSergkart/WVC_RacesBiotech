using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_XenogenesEditor : Gizmo
	{

		private CachedTexture cachedTexture;
		public virtual CachedTexture MenuIcon
		{
			get
			{
				if (cachedTexture == null)
				{
					cachedTexture = gene.Extension_Giver?.iconPath != null ? new(gene.Extension_Giver.iconPath) : XaG_UiUtility.ChimeraMenuIcon;
				}
				return cachedTexture;
			}
		}

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

		protected virtual void EditorButton(Rect rect4)
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

	public class GeneGizmo_XenogenesEditor_Mainframe : GeneGizmo_XenogenesEditor
	{

		private Gene_XenogenesEditor_Mainframe mainframe;

		public GeneGizmo_XenogenesEditor_Mainframe(Gene_XenogenesEditor_General geneChimera) : base(geneChimera)
		{
			if (geneChimera is Gene_XenogenesEditor_Mainframe gene_XenogenesEditor_Mainframe)
			{
				mainframe = gene_XenogenesEditor_Mainframe;
			}
		}


		protected override void EditorButton(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					List<FloatMenuOption> list = new();
					List<Pawn> pawns = MechanoidsUtility.GetAllMechanitors(gene.pawn.MapHeld);
					for (int i = 0; i < pawns.Count; i++)
					{
						Pawn pawn = pawns[i];
						list.Add(new FloatMenuOption(pawn.NameFullColored, delegate
						{
							mainframe.SetTarget(pawn);
							gene.UpdateCache();
							gene.UpdSubHediffs();
							Find.WindowStack.Add(new Dialog_XenogenesEditor(gene));
						}, orderInPriority: 0 + i));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_XenogenesEditorTip".Translate());
		}

	}

}
