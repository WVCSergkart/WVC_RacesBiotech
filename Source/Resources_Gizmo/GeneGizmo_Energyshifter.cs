using RimWorld;
using System.Text;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{

	// Dev
	public class GeneGizmo_Energyshifter : GeneGizmo_Shapeshifter
	{

		private static readonly CachedTexture StyleIcon = new("WVC/UI/XaG_General/UI_GeneStyle_Gizmo1c");
		private static readonly CachedTexture EnergyBarIcon = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI0c");
		private static readonly CachedTexture NewMenuIcon = new("WVC/UI/XaG_General/Gene_Energyshifter_UI");

		protected override CachedTexture Shapeshifter_MenuIcon => NewMenuIcon;

		private Gene_Energyshifter gene_Energyshifter;

		public GeneGizmo_Energyshifter(Gene_Shapeshifter geneShapeshifter) : base(geneShapeshifter)
		{
			if (geneShapeshifter is Gene_Energyshifter gene_Energyshifter)
			{
				this.gene_Energyshifter = gene_Energyshifter;
			}
			uncollapsedSize = 184f;
		}

		public override TaggedString GizmoTip => "WVC_XaG_EnergyshaperGizmoTip".Translate(gene_Energyshifter.Consumption * 100);

		protected override void RecacheTick()
		{
			if (nextRecache > 0)
			{
				nextRecache--;
				return;
			}
			nextRecache = 60;
			gene_Energyshifter.Update(true);
		}

		//public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		//{
		//	return base.GizmoOnGUI(topLeft, maxWidth, parms);
		//}

		public override void Uncollapsed(Vector2 topLeft, float maxWidth)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Button1c(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Button2c(rect5);
			// Energy bar
			Rect rect6 = new(rect5.x + 44f, rect5.y, (rect5.width * 2), rect5.height);
			// Energy bar
			RecacheTick();
			EnergyBar(rect6);
		}

		protected override void Button2c(Rect rect4)
		{
			XaG_UiUtility.StyleButton_WithoutRect(rect4, pawn, gene, true, StyleIcon);
		}

		protected void EnergyBar(Rect rect6)
		{
			CachedTexture cachedTexture = EnergyBarIcon;
			switch (gene.ShaperResource_Raw)
			{
				case >= 1.00f:
					cachedTexture = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI5c");
					break;
				case > 0.75f:
					cachedTexture = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI4c");
					break;
				case > 0.50f:
					cachedTexture = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI3c");
					break;
				case > 0.25f:
					cachedTexture = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI2c");
					break;
				case > 0.00f:
					cachedTexture = new("WVC/UI/XaG_General/Gene_Energyshifter_ChargeUI1c");
					break;
			}
			Widgets.DrawTextureFitted(rect6, cachedTexture.Texture, 1f);
			if (Mouse.IsOver(rect6))
			{
				Widgets.DrawHighlight(rect6);
				if (Widgets.ButtonInvisible(rect6))
				{
					Find.WindowStack.Add(new Dialog_ActivityManager(gene.pawn, gene_Energyshifter));
				}
			}
			TooltipHandler.TipRegion(rect6, "WVC_XaG_GeneEnergyshifter_BarTip".Translate(gene_Energyshifter.Consumption * 100));
		}

	}

}
