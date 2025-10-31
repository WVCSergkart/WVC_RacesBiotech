using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ChangeDryadCaste : Window
	{

		public CompGestatedDryad comp;

		public Gene_DryadQueen gene;

		public Pawn connectedPawn;

		public Vector2 scrollPosition;

		public GauranlenGeneModeDef selectedMode;

		public GauranlenGeneModeDef currentMode;

		public float rightViewWidth;

		public List<GauranlenGeneModeDef> allGauranlenGeneModes;

		public List<GauranlenGeneModeDef> lockedModes;

		public PawnKindDef SelectedKind => selectedMode.pawnKindDef;

		// Interface

		private static readonly Vector2 OptionSize = new(190f, 46f);

		private static readonly Vector2 ButSize = new(200f, 40f);

		public override Vector2 InitialSize => new(Mathf.Min(900, UI.screenWidth), 650f);

		// Interface

		public Dialog_ChangeDryadCaste(Gene_DryadQueen thisGene, CompGestatedDryad thisComp)
		{
			// Init
			gene = thisGene;
			comp = thisComp;
			connectedPawn = gene?.pawn;
			currentMode = comp?.currentMode;
			selectedMode = currentMode;
			// Settings
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			// Xenos
			allGauranlenGeneModes = DefDatabase<GauranlenGeneModeDef>.AllDefsListForReading;
			lockedModes = GetLockedModesForGauranlenGene(allGauranlenGeneModes, connectedPawn);
		}

		public static List<GauranlenGeneModeDef> GetLockedModesForGauranlenGene(List<GauranlenGeneModeDef> modes, Pawn pawn)
		{
			List<GauranlenGeneModeDef> locked = new();
			foreach (GauranlenGeneModeDef mode in modes)
			{
				if (!Find.IdeoManager.classicMode && !mode.requiredMemes.NullOrEmpty())
				{
					foreach (MemeDef requiredMeme in mode.requiredMemes)
					{
						if (!pawn.Ideo.HasMeme(requiredMeme))
						{
							locked.Add(mode);
						}
					}
				}
				if (!mode.requiredGenes.NullOrEmpty())
				{
					foreach (GeneDef requiredGene in mode.requiredGenes)
					{
						if (!XaG_GeneUtility.HasActiveGene(requiredGene, pawn))
						{
							locked.Add(mode);
						}
					}
				}
			}
			return locked;
		}

		public override void PreOpen()
		{
			base.PreOpen();
			SetupView();
		}

		public void SetupView()
		{
			int count = 0;
			foreach (GauranlenGeneModeDef allGauranlenGeneMode in allGauranlenGeneModes)
			{
				rightViewWidth = Mathf.Max(rightViewWidth, GetPosition(InitialSize.y, count).x + OptionSize.x);
				count += 1;
			}
			nextButtonPositonY = 0f;
			nextButtonPositonX = 0f;
			rightViewWidth += 20f;
		}

		public void DrawLeftRect(Rect rect, ref float curY)
		{
			Rect rect2 = new(rect.x, curY, rect.width, rect.height)
			{
				yMax = rect.yMax
			};
			Rect rect3 = rect2.ContractedBy(4f);
			if (selectedMode == null)
			{
				Widgets.Label(rect3, "WVC_XaG_ChooseDryadCasteModeInitialDesc".Translate(comp.parent as Pawn));
				return;
			}
			Widgets.Label(rect3.x, ref curY, rect3.width, selectedMode.Description);
			curY += 10f;
			if (!Find.IdeoManager.classicMode && !selectedMode.requiredMemes.NullOrEmpty())
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "RequiredMemes".Translate() + ":");
				string text = "";
				for (int i = 0; i < selectedMode.requiredMemes.Count; i++)
				{
					MemeDef memeDef = selectedMode.requiredMemes[i];
					if (!text.NullOrEmpty())
					{
						text += "\n";
					}
					text = text + "	 - " + memeDef.LabelCap.ToString().Colorize(connectedPawn.Ideo.HasMeme(memeDef) ? Color.white : ColorLibrary.RedReadable);
				}
				Widgets.Label(rect3.x, ref curY, rect3.width, text);
				curY += 10f;
			}
			if (selectedMode.previousStage != null && selectedMode.previousStage != currentMode)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, string.Concat(("RequiredStage".Translate() + ": " + selectedMode.previousStage.pawnKindDef.LabelCap).ToString().Colorize(ColorLibrary.RedReadable)));
				curY += 10f;
			}
			if (selectedMode.displayedStats != null)
			{
				for (int j = 0; j < selectedMode.displayedStats.Count; j++)
				{
					StatDef statDef = selectedMode.displayedStats[j];
					Widgets.Label(rect3.x, ref curY, rect3.width, statDef.LabelCap + ": " + statDef.ValueToString(SelectedKind.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
				}
				curY += 10f;
			}
			if (selectedMode.hyperlinks != null)
			{
				foreach (Dialog_InfoCard.Hyperlink item in Dialog_InfoCard.DefsToHyperlinks(selectedMode.hyperlinks))
				{
					Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), item);
					curY += Text.LineHeight;
				}
				curY += 10f;
			}
			Rect rect4 = new(rect3.x, rect3.yMax - 55f, rect3.width, 55f);
			if (MeetsRequirements(selectedMode) && selectedMode != currentMode && (selectedMode.previousStage == null || currentMode == selectedMode.previousStage))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ChangeDreadCasteDescFull".Translate(), delegate
					{
						StartChange();
					});
					Find.WindowStack.Add(window);
				}
			}
			else
			{
				// string label = ((selectedMode == currentMode) ? ((string)"AlreadySelected".Translate()) : ((!MeetsRequirements(selectedMode)) ? ((string)"MissingRequiredMemes".Translate()) : ((selectedMode.previousStage == null || currentMode == selectedMode.previousStage) ? ((string)"Locked".Translate()) : ((string)("Locked".Translate() + ": " + "MissingRequiredCaste".Translate())))));
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), "Locked".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			string label = ((selectedMode != null) ? selectedMode.LabelCap : "ChangeMode".Translate());
			Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 35f), label);
			Text.Font = GameFont.Small;
			float num = inRect.y + 35f + 10f;
			float curY = num;
			float num2 = inRect.height - num;
			num2 -= ButSize.y + 10f;
			DrawLeftRect(new Rect(inRect.xMin, num, 400f, num2), ref curY);
			DrawRightRect(new Rect(inRect.x + 400f + 17f, num, inRect.width - 400f - 17f, num2));
		}

		public void DrawRightRect(Rect rect)
		{
			Widgets.DrawMenuSection(rect);
			Rect rect2 = new(0f, 0f, rightViewWidth, rect.height - 16f);
			Rect rect3 = rect2.ContractedBy(10f);
			Widgets.ScrollHorizontal(rect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
			Widgets.BeginGroup(rect3);
			int count = 0;
			foreach (GauranlenGeneModeDef allGauranlenGeneMode in allGauranlenGeneModes)
			{
				DrawStage(rect3, allGauranlenGeneMode, count);
				count += 1;
			}
			nextButtonPositonY = 0f;
			nextButtonPositonX = 0f;
			Widgets.EndGroup();
			Widgets.EndScrollView();
		}

		public bool MeetsRequirements(GauranlenGeneModeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			if (mode == currentMode)
			{
				return false;
			}
			if (mode.previousStage != null && mode.previousStage != currentMode)
			{
				return false;
			}
			if (lockedModes.Contains(mode))
			{
				return false;
			}
			return true;
		}

		public void StartChange()
		{
			comp.currentMode = selectedMode;
			Close(doCloseSound: false);
		}

		public Color GetBoxColor(GauranlenGeneModeDef mode)
		{
			Color result = TexUI.AvailResearchColor;
			if (mode == currentMode)
			{
				result = TexUI.ActiveResearchColor;
			}
			else if (!MeetsRequirements(mode))
			{
				result = TexUI.LockedResearchColor;
			}
			if (selectedMode == mode)
			{
				result += TexUI.HighlightBgResearchColor;
			}
			return result;
		}

		public Color GetBoxOutlineColor(GauranlenGeneModeDef mode)
		{
			if (selectedMode != null && selectedMode == mode)
			{
				return TexUI.HighlightBorderResearchColor;
			}
			return TexUI.DefaultBorderResearchColor;
		}

		public Color GetTextColor(GauranlenGeneModeDef mode)
		{
			if (!MeetsRequirements(mode))
			{
				return ColorLibrary.RedReadable;
			}
			return Color.white;
		}

		public void DrawStage(Rect rect, GauranlenGeneModeDef stage, float count)
		{
			Vector2 position = GetPosition(rect.height, count);
			Rect rect2 = new(position.x, position.y, OptionSize.x, OptionSize.y);
			Widgets.DrawBoxSolidWithOutline(rect2, GetBoxColor(stage), GetBoxOutlineColor(stage));
			Rect rect3 = new(rect2.x, rect2.y, rect2.height, rect2.height);
			// MiscUtility.XaG_DefIcon(rect3.ContractedBy(4f), stage);
			Widgets.DefIcon(rect3.ContractedBy(4f), stage.pawnKindDef);
			GUI.color = GetTextColor(stage);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(rect3.xMax, rect2.y, rect2.width - rect3.width, rect2.height).ContractedBy(4f), stage.LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(rect2))
			{
				selectedMode = stage;
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
		}

		public float nextButtonPositonY = 0f;
		public float nextButtonPositonX = 0f;

		public Vector2 GetPosition(float height, float count)
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
