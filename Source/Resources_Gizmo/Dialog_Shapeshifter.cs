using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Shapeshifter : Dialog_XenotypesBase
	{
		public Gene_Shapeshifter gene;
		public GeneExtension_Undead shiftExtension;

		public SoundDef soundDefOnImplant;

		public bool genesRegrowing = false;
		public bool canEverUseShapeshift = true;
		public bool duplicateMode = false;
		// public bool shouldBeDowned = true;
		public List<XenotypeDef> preferredXenotypes;
		// public List<string> trustedXenotypes;
		public List<XenotypeDef> trueFormXenotypes = new();

		public bool xenogermComaAfterShapeshift = true;

		public bool doubleXenotypeReimplantation = true;

		public bool clearXenogenes = true;

		public ShapeshiftModeDef shiftMode;

		public Dialog_Shapeshifter(Gene_Shapeshifter thisGene)
		{
			// Init
			gene = thisGene;
			currentXeno = gene?.pawn?.genes?.Xenotype;
			selectedXeno = currentXeno;
			shiftMode = gene.ShiftMode;
			// Settings
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			// Xenos
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
			// Ideo
			preferredXenotypes = ModLister.IdeologyInstalled ? gene.pawn?.ideo?.Ideo?.PreferredXenotypes : null;
			// Extension
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Undead>();
			soundDefOnImplant = shiftExtension?.soundDefOnImplant;
			// Info
			genesRegrowing = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
			// canEverUseShapeshift = !MiscUtility.HasAnyTraits(shiftExtension?.blockingTraits, gene.pawn);
			// duplicateMode = MiscUtility.HasAnyTraits(shiftExtension?.duplicateTraits, gene.pawn) || HediffUtility.HasAnyHediff(shiftExtension?.duplicateHediffs, gene.pawn);
			duplicateMode = HediffUtility.HasAnyHediff(shiftExtension?.duplicateHediffs, gene.pawn);
			// trustedXenotypes = shiftExtension?.trustedXenotypes != null ? shiftExtension.trustedXenotypes : new();
			trueFormXenotypes = TrueFormXenotypesFromList(allXenotypes);
			// Gene stats
			xenogermComaAfterShapeshift = shiftMode.xenogermComa;
		}

		public static List<XenotypeDef> TrueFormXenotypesFromList(List<XenotypeDef> xenotypes)
		{
			List<XenotypeDef> list = new();
			foreach (XenotypeDef item in xenotypes)
			{
				// Log.Error(item.LabelCap + " checked.");
				foreach (GeneDef geneDef in item.genes)
				{
					// Log.Error(geneDef.LabelCap + " checked.");
					if (geneDef.geneClass == typeof(Gene_Shapeshift_TrueForm))
					{
						// Log.Error(geneDef.LabelCap + " is true form.");
						list.Add(item);
						break;
					}
				}
			}
			return list;
		}

		public override void DrawLeftRect(Rect rect, ref float curY)
		{
			Rect rect2 = new(rect.x, curY, rect.width, rect.height)
			{
				yMax = rect.yMax
			};
			Rect rect3 = rect2.ContractedBy(4f);
			if (selectedXeno == null)
			{
				if (allXenotypes.NullOrEmpty())
				{
					Log.Error("Non android xenotypes is null. This error has nothing to do with the mod, do not report it to the developer.");
					return;
				}
				selectedXeno = allXenotypes.RandomElement();
				return;
			}
			// Main Info
			if (selectedXeno.descriptionShort.NullOrEmpty())
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.description);
			}
			else
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.descriptionShort);
			}
			curY += 10f;
			// foreach (XenotypeDef item in XenoTreeUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			// {
				// Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				// curY += Text.LineHeight;
			// }
			List<XenotypeDef> doubleXenotypes = XaG_GeneUtility.GetXenotypeAndDoubleXenotypes(selectedXeno);
			for (int i = 0; i < doubleXenotypes.Count; i++)
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(doubleXenotypes[i]));
				curY += Text.LineHeight;
				if (i > 2)
				{
					Widgets.Label(rect3.x, ref curY, rect3.width, ("+" + (doubleXenotypes.Count - i).ToString()).Colorize(ColoredText.SubtleGrayColor));
					break;
				}
			}
			curY += 10f;
			// Info
			// if (!canEverUseShapeshift)
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DisabledPermanent".Translate().Colorize(ColorLibrary.RedReadable));
				// curY += 10f;
			// }
			Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DialogCurrentMode".Translate(shiftMode.LabelCap).Colorize(ColorLibrary.LightBlue));
			curY += 10f;
			if (genesRegrowing)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			else if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(selectedXeno))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_NotPreferredXenotype".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			// else if (duplicateMode)
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DuplicateMode".Translate().Colorize(ColorLibrary.LightBlue));
				// curY += 10f;
			// }
			// if (trustedXenotypes.Contains(selectedXeno.defName))
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_TrustedXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor));
				// curY += 10f;
			// }
			// else
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_UnknownXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor));
				// curY += 10f;
			// }
			Rect rect4 = new(rect3.x, rect3.yMax - 55f, rect3.width, 55f);
			// Checkbox
			Rect rectCheckbox = new(rect4.x, rect4.yMax - 85f, (rect4.width / 2f) - 5f, 24f);
			Widgets.DrawHighlightIfMouseover(rectCheckbox);
			Widgets.CheckboxLabeled(rectCheckbox, "WVC_XaG_GeneShapeshifter_CheckBox_ImplantDoubleXenotype".Translate(), ref doubleXenotypeReimplantation);
			// Checkbox
			Rect rectCheckbox2 = new(rectCheckbox.xMax + 10f, rectCheckbox.yMin, rectCheckbox.width, 24f);
			Widgets.DrawHighlightIfMouseover(rectCheckbox2);
			Widgets.CheckboxLabeled(rectCheckbox2, "WVC_XaG_GeneShapeshifter_CheckBox_ClearXenogenesBeforeImplant".Translate(), ref clearXenogenes);
			// Accept Button
			// Widgets.CheckboxLabeled(rect3, "TEST: Checkbox", ref doubleXenotypeReimplantation);
			if (MeetsRequirements(selectedXeno))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), delegate
					{
						StartChange();
					});
					Find.WindowStack.Add(window);
				}
			}
			else
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
			}
			// Duplicate mode
			// Rect rectCheckbox = new(rect4.x, rect4.yMax + 10f, rect4.width / 2f, 24f);
			// if (selectedXeno?.doubleXenotypeChances == null)
			// {
				// Widgets.DrawHighlightIfMouseover(rectCheckbox);
				// Widgets.CheckboxLabeled(rectCheckbox, "WVC_XaG_GeneShapeshifter_CheckBox_ImplantDoubleXenotype".Translate(), ref doubleXenotypeReimplantation);
			// }
		}

		// public override void DoWindowContents(Rect inRect)
		// {
			// Text.Font = GameFont.Medium;
			// string label = ((selectedXeno != null) ? selectedXeno.LabelCap : "WVC_XaG_XenoTreeModeChange".Translate());
			// Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 35f), label);
			// Text.Font = GameFont.Small;
			// float num = inRect.y + 35f + 10f;
			// float curY = num;
			// float num2 = inRect.height - num;
			// num2 -= ButSize.y + 10f;
			// DrawLeftRect(new Rect(inRect.xMin, num, 400f, num2), ref curY);
			// DrawRightRect(new Rect(inRect.x + 400f + 17f, num, inRect.width - 400f - 17f, num2));
		// }

		public override void StartChange()
		{
			// if (UndeadUtility.TryDuplicatePawn(gene.pawn, gene, selectedXeno, duplicateMode))
			// {
				
			// }
			// else if (UndeadUtility.TryShapeshift(gene, this))
			// {
				
			// }
			shiftMode.Worker.Shapeshift(gene, this);
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			if (!duplicateMode && trueFormXenotypes.Contains(mode))
			{
				return true;
			}
			// if (!canEverUseShapeshift)
			// {
				// return false;
			// }
			if (genesRegrowing)
			{
				return false;
			}
			if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(mode))
			{
				return false;
			}
			return true;
		}

		// public void PostSelect(Pawn pawn, Gene gene)
		// {
		// }

	}

}
