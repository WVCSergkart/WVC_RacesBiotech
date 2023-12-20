using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ChangeXenotype : Dialog_XenotypesBase
	{
		public CompXenoTree xenoTree;

		public List<Pawn> allColonists;

		public float matchPercent;

		public Dialog_ChangeXenotype(Thing tree)
		{
			xenoTree = tree.TryGetComp<CompXenoTree>();
			currentXeno = xenoTree.chosenXenotype;
			selectedXeno = currentXeno;
			allColonists = tree.Map.mapPawns.FreeColonistsAndPrisoners.ToList();
			matchPercent = xenoTree.Props.minMatchingGenes;
			// connectedPawn = xenoTree.ConnectedPawn;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
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
				Widgets.Label(rect3, "WVC_XaG_XenoTreeModeChangeDescInitial".Translate());
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
			foreach (XenotypeDef item in XenoTreeUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				curY += Text.LineHeight;
			}
			curY += 10f;
			if (Find.TickManager.TicksGame < xenoTree.changeCooldown)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_XenoTreeXenotypeChangeCooldown".Translate(xenoTree.changeCooldown.ToStringTicksToPeriod()).Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			if (!XaG_GeneUtility.GenesIsMatchForPawns(allColonists, selectedXeno.genes, matchPercent))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneXenoGestator_GestationGenesMatch".Translate((matchPercent * 100).ToString()).Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			// if (!XenoTreeUtility.XenoTree_ToxResCheck(selectedXeno, xenoTree.parent))
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_XenoTreeModeRequiredPollution".Translate(xenoTree.parent.LabelCap).Colorize(ColorLibrary.RedReadable));
				// curY += 10f;
			// }
			// if (!XenoTreeUtility.XenoTree_ColdResCheck(selectedXeno, xenoTree.parent))
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_XenoTreeModeRequiredCold".Translate(xenoTree.parent.LabelCap).Colorize(ColorLibrary.RedReadable));
				// curY += 10f;
			// }
			// if (!XenoTreeUtility.XenoTree_HeatResCheck(selectedXeno, xenoTree.parent))
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_XenoTreeModeRequiredHeat".Translate(xenoTree.parent.LabelCap).Colorize(ColorLibrary.RedReadable));
				// curY += 10f;
			// }
			if (MeetsRequirements(selectedXeno) && selectedXeno != currentXeno)
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_XenoTreeModeChangeDescFull".Translate(xenoTree.parent.LabelCap), delegate
					{
						StartChange();
					});
					Find.WindowStack.Add(window);
				}
			}
			else
			{
				string label = ((selectedXeno == currentXeno) ? ((string)"WVC_XaG_XenoTreeMode_AlreadySelected".Translate()) : ((!MeetsRequirements(selectedXeno)) ? ((string)"WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate()) : ((string)"Locked".Translate())));
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void StartChange()
		{
			xenoTree.chosenXenotype = selectedXeno;
			xenoTree.changeCooldown = Find.TickManager.TicksGame + xenoTree.Props.xenotypeChangeCooldown;
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			// SoundDefOf.GauranlenProductionModeSet.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			if (Find.TickManager.TicksGame < xenoTree.changeCooldown)
			{
				return false;
			}
			// if (XenoTreeUtility.XenoTree_CanSpawn(mode, xenoTree.parent))
			// {
				// return true;
			// }
			if (XaG_GeneUtility.GenesIsMatchForPawns(allColonists, mode.genes, matchPercent))
			{
				return true;
			}
			return false;
		}

	}

}
