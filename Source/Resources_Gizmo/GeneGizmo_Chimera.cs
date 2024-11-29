using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Chimera : Gizmo
	{

		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Chimera_GizmoMenu");

		public Pawn pawn;

		public Gene_Chimera gene;

		public int collectedGenes = 0;
		public int eatedGenes = 0;
		public int destroyedGenes = 0;

		public int nextRecache = 0;

		public override bool Visible => true;

		public GeneGizmo_Chimera(Gene_Chimera geneChimera)
			: base()
		{
			gene = geneChimera;
			pawn = gene?.pawn;
			Order = -94f;
			collectedGenes = gene.CollectedGenes.Count;
			eatedGenes = gene.EatedGenes.Count;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			nextRecache--;
			if (nextRecache < 0)
			{
				collectedGenes = gene.CollectedGenes.Count;
				eatedGenes = gene.EatedGenes.Count;
				nextRecache = 360;
			}
			string text = collectedGenes.ToString("F0");
			TaggedString taggedString = "WVC_XaG_Gene_Chimera_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_Gene_Chimera_GizmoTip".Translate();
			if (eatedGenes > 0)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_Gene_Chimera_GizmoEatedLabel".Translate() + ": ")) + eatedGenes;
			}
			TooltipHandler.TipRegion(rect, taggedString);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			// Widgets.Label(rect3, "Genes".Translate().CapitalizeFirst());
			Widgets.Label(rect3, "WVC_XaG_GeneChimeraGizmoLabel".Translate().CapitalizeFirst());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			// Button
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
			Widgets.DrawTextureFitted(rect4, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_CreateChimera(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneGeneticThief_Desc".Translate());
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			// return 136f;
			return 96f;
		}

	}

}
