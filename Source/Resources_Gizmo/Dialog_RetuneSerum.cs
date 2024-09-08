using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_RetuneSerum : Dialog_XenotypesBase
	{

		public CompUseEffect_XenogermSerum xenotypeForcer;

		public Dialog_RetuneSerum(Thing serum)
		{
			xenotypeForcer = serum.TryGetComp<CompUseEffect_XenogermSerum>();
			currentXeno = xenotypeForcer.xenotype;
			selectedXeno = currentXeno;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.WhiteListedXenotypes(true);
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
			foreach (XenotypeDef item in XaG_GeneUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				curY += Text.LineHeight;
			}
			curY += 10f;
			if (MeetsRequirements(selectedXeno) && selectedXeno != currentXeno)
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					// Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_SuremRetuneJob_ChangeDesc".Translate(selectedXeno.LabelCap), delegate
					// {
						// StartChange();
					// });
					// Find.WindowStack.Add(window);
					StartChange();
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
			xenotypeForcer.xenotype = selectedXeno;
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (SerumUtility.XenotypeHasArchites(mode))
			{
				if (xenotypeForcer.Props.xenotypeType == CompProperties_UseEffect_XenogermSerum.XenotypeType.Archite)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (xenotypeForcer.Props.xenotypeType == CompProperties_UseEffect_XenogermSerum.XenotypeType.Base)
			{
				return true;
			}
			return false;
		}

	}

}
