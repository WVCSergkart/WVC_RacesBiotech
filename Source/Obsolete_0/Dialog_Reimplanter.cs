using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class Dialog_ReimplanterXenotype : Dialog_XenotypesBase
	{

		public Gene_XenotypeImplanter gene;

		public List<XenotypeDef> preferredXenotypes;

		public Dialog_ReimplanterXenotype(Gene_XenotypeImplanter thisGene)
		{
			// Init
			gene = thisGene;
			currentXeno = gene?.xenotypeDef;
			selectedXeno = currentXeno;
			// Settings
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			// Xenos
			allXenotypes = ListsUtility.GetAllXenotypesExceptAndroids();
			// Ideo
			preferredXenotypes = ModLister.IdeologyInstalled ? gene.pawn?.ideo?.Ideo?.PreferredXenotypes : null;
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
			if (selectedXeno.descriptionShort.NullOrEmpty())
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.description);
			}
			else
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.descriptionShort);
			}
			curY += 10f;
			Rect rect4 = new(rect3.x, rect3.yMax - 55f, rect3.width, 55f);
			foreach (XenotypeDef item in XaG_GeneUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				curY += Text.LineHeight;
			}
			curY += 10f;
			if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(selectedXeno))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_NotPreferredXenotype".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			if (MeetsRequirements(selectedXeno))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					StartChange();
				}
			}
			else
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void StartChange()
		{
			gene.xenotypeDef = selectedXeno;
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(mode))
			{
				return false;
			}
			return true;
		}

	}

}
