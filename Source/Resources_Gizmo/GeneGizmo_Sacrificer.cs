using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Sacrificer : Gizmo
	{

		public Pawn pawn;
		public Gene_Sacrificer gene;

		public override bool Visible => true;

		private static readonly CachedTexture EditorIcon = new("WVC/UI/XaG_General/UI_XenogenesEditorFleshmass");
		private static readonly CachedTexture ModeIcon = new("WVC/UI/XaG_General/UI_SacrificerSkullIcon");

		//public virtual TaggedString GizmoTip
		//{
		//	get
		//	{
		//		return "WVC_XaG_SacrificerGizmoTip".Translate(gene.GenelineGenes.Count, gene.ComplexityLimit, gene.ArchiteLimit, gene.ReqMetRange.TrueMin, gene.ReqMetRange.TrueMax);
		//	}
		//}

		private int nextRecache;
		private string cachedDescription;
		public TaggedString GizmoTip
		{
			get
			{
				if (cachedDescription == null)
				{
					//Log.Error("0");
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLineTagged("WVC_XaG_SacrificerGizmoTip".Translate(gene.GenelineGenes.Count, gene.ComplexityLimit, gene.ArchiteLimit, gene.ReqMetRange.TrueMin, gene.ReqMetRange.TrueMax, gene.SacrificerModeDef.LabelCap));
					stringBuilder.AppendLine();
					stringBuilder.AppendLineTagged("WVC_XaG_SacrificerGizmoTip_ModeDesc".Translate(gene.SacrificerModeDef.description.CapitalizeFirst()));
					stringBuilder.AppendLine();
					if (gene.SacrificerModeDef.rewards != null)
					{
						stringBuilder.AppendLineTagged("WVC_XaG_SacrificerGizmoTip_Rewards".Translate());
						foreach (SacrificerModeDef.ThingDefCountWithChance reward in gene.SacrificerModeDef.rewards)
						{
							//stringBuilder.AppendLineTagged("  - " + reward.LabelCap + ": " + reward.count);
							stringBuilder.AppendLineTagged("  - " + "WVC_XaG_RewardWithChance".Translate(reward.LabelCap, reward.count, reward.chance.ToStringPercent()));
						}
					}
					cachedDescription = stringBuilder.ToString().TrimEndNewlines();
					nextRecache = 240;
				}
				return cachedDescription;
			}
		}

		public GeneGizmo_Sacrificer(Gene_Sacrificer gene)
			: base()
		{
			this.gene = gene;
			pawn = gene?.pawn;
			Order = -95f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
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
			XaG_UiUtility.Button_Style(rect6, pawn, gene, true, XaG_UiUtility.Fleshshaper_StyleIcon);
			//Button2c(rect5);
			return new GizmoResult(GizmoState.Clear);
		}

		protected Rect LabelAndTip(Vector2 topLeft, float maxWidth)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Tip
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, gene.def.LabelShortAdj.CapitalizeFirst() + ": " + gene.SacrificerModeDef.LabelCap.Colorize(ColoredText.TipSectionTitleColor));
			if (nextRecache > 0)
			{
				nextRecache--;
				if (nextRecache <= 0)
				{
					cachedDescription = null;
				}
			}
			if (Mouse.IsOver(rect3))
			{
				TooltipHandler.TipRegion(rect3, gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + GizmoTip);
			}
			return rect2;
		}

		// Editor
		protected virtual void Button1c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, EditorIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_XenogenesEditor(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_SacrificerEditorTip".Translate(gene.Label));
		}

		// Mode
		protected virtual void Button2c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, ModeIcon.Texture, 1f);
			if (Gene_Sacrificer.ForcedMode)
			{
				Widgets.DrawTextureFitted(rect4, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
			}
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					if (Gene_Sacrificer.ForcedMode)
					{
						Messages.Message("WVC_XaG_SacrificerMode_Blocked".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
						return;
					}
					List<FloatMenuOption> list = new();
					List<SacrificerModeDef> modes = Gene_Sacrificer.ModeDefs.Where(mode => mode.canBeSwitchedManually).ToList();
					for (int i = 0; i < modes.Count; i++)
					{
						SacrificerModeDef mode = modes[i];
						list.Add(new FloatMenuOption(mode.LabelCap, delegate
						{
							//Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(gene.SacrificerModeDef.description.CapitalizeFirst(), delegate
							//{
							//	gene.SacrificerModeDef = mode;
							//});
							//Find.WindowStack.Add(window);
							gene.SacrificerModeDef = mode;
						}, orderInPriority: (int)(0 - mode.displayOrder)));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_SacrificerModeTip".Translate());
		}

		public override float GetWidth(float maxWidth)
		{
			//return 48f;
			//return 96f;
			return 140f;
		}

	}

}
