using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ChooseXenotype : Window
	{
		public Gene gene;

		private Vector2 scrollPosition;

		public XenotypeDef selectedXeno;

		private float rightViewWidth;

		public List<XenotypeDef> allXenotypes;

		public HediffDef HediffDefName => gene.def.GetModExtension<GeneExtension_XenotypeGestator>().gestationHediffDef;
		public HediffDef CooldownHediffDef => gene.def.GetModExtension<GeneExtension_XenotypeGestator>().cooldownHediffDef;
		public float MatchPercent => gene.def.GetModExtension<GeneExtension_XenotypeGestator>().matchPercent;
		public int MinimumDays => gene.def.GetModExtension<GeneExtension_XenotypeGestator>().minimumDays;
		public int CooldownDays => gene.def.GetModExtension<GeneExtension_XenotypeGestator>().cooldownDays;

		private readonly int ticksInDay = 60000;

		private static readonly Vector2 OptionSize = new(190f, 46f);

		private static readonly Vector2 ButSize = new(200f, 40f);

		public override Vector2 InitialSize => new(Mathf.Min(900, UI.screenWidth), 650f);

		public Dialog_ChooseXenotype(Gene thisGene)
		{
			gene = thisGene;
			// currentXeno = xenoTree.chosenXenotype;
			// selectedXeno = currentXeno;
			// connectedPawn = xenoTree.ConnectedPawn;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
		}

		public override void PreOpen()
		{
			base.PreOpen();
			SetupView();
		}

		private void SetupView()
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

		private void DrawLeftRect(Rect rect, ref float curY)
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
			foreach (XenotypeDef item in XenoTreeUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				curY += Text.LineHeight;
			}
			Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneXenoGestator_GestationTime".Translate((GetGestationTime() * ticksInDay).ToStringTicksToPeriod()).Resolve());
			curY += 10f;
			if (!XaG_GeneUtility.GenesIsMatch(gene.pawn.genes.GenesListForReading, selectedXeno.genes, MatchPercent))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneXenoGestator_GestationGenesMatch".Translate((MatchPercent * 100).ToString()).Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			if (MeetsRequirements(selectedXeno))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneXenoGestator_GestationWarning".Translate(gene.pawn.LabelCap), delegate
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

		private int GetGestationTime()
		{
			return (int)(XaG_GeneUtility.GetXenotype_Cpx(selectedXeno) * 0.1f) + MinimumDays;
		}

		private void StartChange()
		{
			// xenoTree.chosenXenotype = selectedXeno;
			// xenoTree.changeCooldown = Find.TickManager.TicksGame + xenoTree.Props.xenotypeChangeCooldown;
			// SoundDefOf.GauranlenProductionModeSet.PlayOneShotOnCamera();
			//SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			// SoundDefOf.MechGestatorCycle_Started.PlayOneShot(SoundInfo.OnCamera());
			SoundDefOf.MechGestatorCycle_Started.PlayOneShot(new TargetInfo(gene.pawn));
			// HediffDef hediffDef = gene?.def?.GetModExtension<GeneExtension_Giver>()?.hediffDefName;
			// Log.Error(hediffDef.LabelCap);
			if (HediffDefName != null)
			{
				// Log.Error("1");
				Hediff hediff = HediffMaker.MakeHediff(HediffDefName, gene.pawn);
				HediffComp_XenotypeGestator xeno_Gestator = hediff.TryGetComp<HediffComp_XenotypeGestator>();
				// Log.Error("2");
				if (xeno_Gestator != null)
				{
					xeno_Gestator.xenotypeDef = selectedXeno;
					xeno_Gestator.gestationIntervalDays = GetGestationTime() + MinimumDays;
				}
				HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
				if (hediff_GeneCheck != null)
				{
					hediff_GeneCheck.geneDef = gene.def;
				}
				// Log.Error("3");
				gene.pawn.health.AddHediff(hediff);
				// Log.Error("4");
				if (CooldownHediffDef != null)
				{
					if (gene is Gene_XenotypeGestator geneGestator)
					{
						geneGestator.cooldownHediffDef = CooldownHediffDef;
					}
					Hediff cooldownHediff = HediffMaker.MakeHediff(CooldownHediffDef, gene.pawn);
					HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
					if (hediffComp_Disappears != null)
					{
						hediffComp_Disappears.ticksToDisappear = ticksInDay * CooldownDays;
					}
					HediffComp_RemoveIfGeneIsNotActive cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (cooldownHediff_GeneCheck != null)
					{
						cooldownHediff_GeneCheck.geneDef = gene.def;
					}
					gene.pawn.health.AddHediff(cooldownHediff);
				}
			}
			Close(doCloseSound: false);
		}

		private void DrawRightRect(Rect rect)
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

		// private bool MeetsMemeRequirements(XenotypeDef stage)
		// {
			// if (XenotypeUtility.XenoTree_CanSpawn(stage, xenoTree.parent) || DebugSettings.ShowDevGizmos)
			// {
				// return true;
			// }
			// return false;
		// }

		private bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			// TEST
			if (XaG_GeneUtility.GenesIsMatch(gene?.pawn?.genes?.GenesListForReading, mode.genes, MatchPercent))
			{
				return true;
			}
			// if (Find.TickManager.TicksGame < xenoTree.changeCooldown)
			// {
				// return false;
			// }
			// if (XenoTreeUtility.XenoTree_CanSpawn(mode, xenoTree.parent))
			// {
				// return true;
			// }
			return false;
		}

		private Color GetBoxColor(XenotypeDef mode)
		{
			Color result = TexUI.AvailResearchColor;
			// if (mode == currentXeno)
			// {
				// result = TexUI.ActiveResearchColor;
			// }
			if (!MeetsRequirements(mode))
			{
				result = TexUI.LockedResearchColor;
			}
			if (selectedXeno == mode)
			{
				result += TexUI.HighlightBgResearchColor;
			}
			return result;
		}

		private Color GetBoxOutlineColor(XenotypeDef mode)
		{
			if (selectedXeno != null && selectedXeno == mode)
			{
				return TexUI.HighlightBorderResearchColor;
			}
			return TexUI.DefaultBorderResearchColor;
		}

		private Color GetTextColor(XenotypeDef mode)
		{
			if (!MeetsRequirements(mode))
			{
				return ColorLibrary.RedReadable;
			}
			return Color.white;
		}

		private void DrawDryadStage(Rect rect, XenotypeDef stage, float count)
		{
			Vector2 position = GetPosition(rect.height, count);
			Rect rect2 = new(position.x, position.y, OptionSize.x, OptionSize.y);
			Widgets.DrawBoxSolidWithOutline(rect2, GetBoxColor(stage), GetBoxOutlineColor(stage));
			Rect rect3 = new(rect2.x, rect2.y, rect2.height, rect2.height);
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

		public Color GetXenotypeColor(XenotypeDef xenotype)
		{
			int allGenesCount = XenoTreeUtility.GetAllGenesCount(xenotype);
			Color color = ColoredText.SubtleGrayColor;
			if (allGenesCount >= 7)
			{
				color = ColorLibrary.LightGreen;
			}
			if (allGenesCount >= 14)
			{
				color = ColorLibrary.LightBlue;
			}
			if (allGenesCount >= 21)
			{
				color = ColorLibrary.LightPurple;
			}
			if (allGenesCount >= 28)
			{
				color = ColorLibrary.LightOrange;
			}
			return color;
		}

		private float nextButtonPositonY = 0f;
		private float nextButtonPositonX = 0f;

		private Vector2 GetPosition(float height, float count)
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
