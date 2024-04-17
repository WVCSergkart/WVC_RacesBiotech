using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenotypesBase : Window
	{

		public Vector2 scrollPosition;

		public XenotypeDef selectedXeno;

		public XenotypeDef currentXeno;

		public float rightViewWidth;

		public List<XenotypeDef> allXenotypes;

		// public Dictionary<XenotypeDef, Color> allXenotypes;

		public static readonly Vector2 OptionSize = new(190f, 46f);

		public static readonly Vector2 ButSize = new(200f, 40f);

		public override Vector2 InitialSize => new(Mathf.Min(900, UI.screenWidth), 650f);

		public override void PreOpen()
		{
			base.PreOpen();
			SetupView();
		}

		public virtual void SetupView()
		{
			int count = 0;
			foreach (XenotypeDef allXenotype in allXenotypes)
			{
				rightViewWidth = Mathf.Max(rightViewWidth, GetPosition(InitialSize.y, count).x + OptionSize.x);
				count += 1;
			}
			nextButtonPositonY = 0f;
			nextButtonPositonX = 0f;
			rightViewWidth += 20f;
		}

		public virtual void DrawLeftRect(Rect rect, ref float curY)
		{
			Rect rect2 = new(rect.x, curY, rect.width, rect.height)
			{
				yMax = rect.yMax
			};
			Rect rect3 = rect2.ContractedBy(4f);
			if (selectedXeno == null)
			{
				Widgets.Label(rect3, "WVC_XaG_GeneXenoGestator_Desc".Translate());
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
			if (MeetsRequirements(selectedXeno))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneXenoGestator_GestationWarning".Translate(), delegate
					{
						StartChange();
					});
					Find.WindowStack.Add(window);
				}
			}
			else
			{
				string label = (!MeetsRequirements(selectedXeno)) ? "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate() : ((string)"Locked".Translate());
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			string label = ((selectedXeno != null) ? selectedXeno.LabelCap : "WVC_XaG_XenoTreeModeChange".Translate());
			Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 35f), label);
			Text.Font = GameFont.Small;
			float num = inRect.y + 35f + 10f;
			float curY = num;
			float num2 = inRect.height - num;
			num2 -= ButSize.y + 10f;
			DrawLeftRect(new Rect(inRect.xMin, num, 400f, num2), ref curY);
			DrawRightRect(new Rect(inRect.x + 400f + 17f, num, inRect.width - 400f - 17f, num2));
		}

		public virtual void DrawRightRect(Rect rect)
		{
			Widgets.DrawMenuSection(rect);
			Rect rect2 = new(0f, 0f, rightViewWidth, rect.height - 16f);
			Rect rect3 = rect2.ContractedBy(10f);
			Widgets.ScrollHorizontal(rect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
			Widgets.BeginGroup(rect3);
			int count = 0;
			foreach (XenotypeDef allXenotype in allXenotypes)
			{
				DrawDryadStage(rect3, allXenotype, count);
				count += 1;
			}
			nextButtonPositonY = 0f;
			nextButtonPositonX = 0f;
			Widgets.EndGroup();
			Widgets.EndScrollView();
		}

		public virtual bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			return false;
		}

		public virtual void StartChange()
		{
			Close(doCloseSound: false);
		}

		public virtual Color GetBoxColor(XenotypeDef mode)
		{
			Color result = TexUI.AvailResearchColor;
			if (mode == currentXeno)
			{
				result = TexUI.ActiveResearchColor;
			}
			else if (!MeetsRequirements(mode))
			{
				result = TexUI.LockedResearchColor;
			}
			if (selectedXeno == mode)
			{
				result += TexUI.HighlightBgResearchColor;
			}
			return result;
		}

		public virtual Color GetBoxOutlineColor(XenotypeDef mode)
		{
			if (selectedXeno != null && selectedXeno == mode)
			{
				return TexUI.HighlightBorderResearchColor;
			}
			return TexUI.DefaultBorderResearchColor;
		}

		public virtual Color GetTextColor(XenotypeDef mode)
		{
			if (!MeetsRequirements(mode))
			{
				return ColorLibrary.RedReadable;
			}
			return Color.white;
		}

		public virtual void DrawDryadStage(Rect rect, XenotypeDef stage, float count)
		{
			Vector2 position = GetPosition(rect.height, count);
			Rect rect2 = new(position.x, position.y, OptionSize.x, OptionSize.y);
			Widgets.DrawBoxSolidWithOutline(rect2, GetBoxColor(stage), GetBoxOutlineColor(stage));
			Rect rect3 = new(rect2.x, rect2.y, rect2.height, rect2.height);
			// if (!stage.iconPath.NullOrEmpty())
			// {
			// }
			Widgets.DefIcon(rect3.ContractedBy(4f), stage, color: GetXenotypeColor(stage));
			GUI.color = GetTextColor(stage);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(rect3.xMax, rect2.y, rect2.width - rect3.width, rect2.height).ContractedBy(4f), stage.LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(rect2))
			{
				selectedXeno = stage;
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
		}

		public virtual Color GetXenotypeColor(XenotypeDef xenotype)
		{
			if (xenotype.GetType() != typeof(XenotypeDef))
			{
				return ColorLibrary.LightPink;
			}
			// int allGenesCount = XenoTreeUtility.GetAllGenesCount(xenotype);
			int allGenesCount = xenotype.AllGenes.Count;
			Color color = ColoredText.SubtleGrayColor;
			if (allGenesCount >= 28)
			{
				color = ColorLibrary.LightOrange;
			}
			else if (allGenesCount >= 21)
			{
				color = ColorLibrary.LightPurple;
			}
			else if (allGenesCount >= 14)
			{
				color = ColorLibrary.LightBlue;
			}
			else if (allGenesCount >= 7)
			{
				color = ColorLibrary.LightGreen;
			}
			return color;
		}

		public float nextButtonPositonY = 0f;
		public float nextButtonPositonX = 0f;

		public virtual Vector2 GetPosition(float height, float count)
		{
			if (count > 0f)
			{
				nextButtonPositonY += 0.1665f;
			}
			if (nextButtonPositonY > 1.16f)
			{
				nextButtonPositonY = 0f;
				nextButtonPositonX += 0.833f;
			}
			return new Vector2(nextButtonPositonX * OptionSize.x + nextButtonPositonX * 52f, (height - OptionSize.y) * nextButtonPositonY);
		}
	}

}
