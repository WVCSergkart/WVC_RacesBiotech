using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_GeneticThief : Window
	{

		public Gene_Chimera gene;

		public Vector2 scrollPosition;

		public GeneDef selectedXeno;

		// public GeneDef currentXeno;

		public float rightViewWidth;

		public List<GeneDef> allGenes;

		public List<GeneDef> eatedGenes;

		public List<GeneDef> pawnGenes;

		public List<GeneDef> pawnXenoGenes;

		public List<GeneDef> choosenGenes = new();

		// public Dictionary<XenotypeDef, Color> allGenes;

		public bool eatAllSelectedGenes = false;

		public static readonly Vector2 OptionSize = new(190f, 46f);

		public static readonly Vector2 ButSize = new(200f, 40f);

		public override Vector2 InitialSize => new(Mathf.Min(900, UI.screenWidth), 650f);

		public Dialog_GeneticThief(Gene_Chimera thisGene)
		{
			// Init
			gene = thisGene;
			// Settings
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			// Xenos
			pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.GenesListForReading);
			pawnXenoGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.Xenogenes);
			allGenes = gene?.StolenGenes;
			eatedGenes = gene?.EatedGenes;
			selectedXeno = allGenes.RandomElement();
			// selectedXeno = currentXeno;
		}

		public override void PreOpen()
		{
			base.PreOpen();
			SetupView();
		}

		public void SetupView()
		{
			int count = 0;
			foreach (GeneDef allXenotype in allGenes)
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
			if (selectedXeno == null)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneGeneticThief_Desc".Translate());
				// return;
			}
			else
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.description);
				curY += 10f;
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(selectedXeno));
				curY += 15f;
				if (!selectedXeno.customEffectDescriptions.NullOrEmpty())
				{
					curY += 10f;
					string effectDescriptions = "WVC_XaG_GeneGeneticThief_Effects".Translate().Colorize(ColoredText.TipSectionTitleColor);
					foreach (string customEffectDescription in selectedXeno.customEffectDescriptions)
					{
						effectDescriptions += "\n" + "  - " + customEffectDescription;
					}
					Widgets.Label(rect3.x, ref curY, rect3.width, effectDescriptions.ResolveTags());
				}
				Rect button = new(rect3.x, rect3.yMax - 85f, rect3.width, 25f);
				if (CanAdded(selectedXeno))
				{
					if (Widgets.ButtonText(button, "WVC_XaG_GeneGeneticThief_AddGene".Translate()))
					{
						choosenGenes.Add(selectedXeno);
					}
				}
				else if (choosenGenes.Contains(selectedXeno))
				{
					if (Widgets.ButtonText(button, "WVC_XaG_GeneGeneticThief_RemoveGene".Translate()))
					{
						choosenGenes.Remove(selectedXeno);
					}
				}
				Rect rect4 = new(rect3.x, rect3.yMax - 55f, rect3.width, 30f);
				if (MeetsRequirements(selectedXeno))
				{
					string acceptButtonLabel = eatAllSelectedGenes ? "WVC_XaG_GeneGeneticThief_AcceptLabelEat".Translate() : "WVC_XaG_GeneGeneticThief_AcceptLabelImplant".Translate(gene.pawn.LabelCap);
					if (Widgets.ButtonText(rect4, acceptButtonLabel))
					{
						string warningDesc = eatAllSelectedGenes ? "WVC_XaG_GeneGeneticThief_EatSelectedGenes".Translate() : "WVC_XaG_GeneGeneticThief_ImplantGeneSet".Translate(gene.pawn.LabelCap);
						Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(warningDesc, delegate
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
			}
			Rect rect5 = new(rect3.x, rect3.yMax - 20f, rect3.width, 25f);
			if (Widgets.ButtonText(rect5, "WVC_XaG_GeneGeneticThief_ClearXenogenesButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneGeneticThief_ClearGeneSet".Translate(gene.LabelCap), delegate
				{
					ClearXenogenes();
				});
				Find.WindowStack.Add(window);
			}
			// Rect eatButton = new(rect3.x, rect3.yMax - 85f, rect3.width, 25f);
			// if (CanEat())
			// {
				// if (Widgets.ButtonText(eatButton, "WVC_XaG_GeneGeneticThief_EatGene".Translate()))
				// {
					// Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneGeneticThief_EatSelectedGenes".Translate(gene.LabelCap), delegate
					// {
						// ClearXenogenes();
					// });
					// Find.WindowStack.Add(window);
				// }
			// }
			// else
			// {
				// Text.Anchor = TextAnchor.MiddleCenter;
				// Widgets.DrawHighlight(eatButton);
				// Widgets.Label(eatButton.ContractedBy(5f), "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate());
				// Text.Anchor = TextAnchor.UpperLeft;
			// }
			// Checkbox
			Rect rectCheckbox = new(rect5.x, rect5.yMax + 10f, rect5.width / 2f, 24f);
			Widgets.DrawHighlightIfMouseover(rectCheckbox);
			Widgets.CheckboxLabeled(rectCheckbox, "WVC_XaG_GeneGeneticThief_EatGene".Translate(), ref eatAllSelectedGenes);
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

		public void DrawRightRect(Rect rect)
		{
			Widgets.DrawMenuSection(rect);
			Rect rect2 = new(0f, 0f, rightViewWidth, rect.height - 16f);
			Rect rect3 = rect2.ContractedBy(10f);
			Widgets.ScrollHorizontal(rect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
			Widgets.BeginGroup(rect3);
			int count = 0;
			foreach (GeneDef allXenotype in allGenes)
			{
				DrawStage(rect3, allXenotype, count);
				count += 1;
			}
			nextButtonPositonY = 0f;
			nextButtonPositonX = 0f;
			Widgets.EndGroup();
			Widgets.EndScrollView();
		}

		public bool MeetsRequirements(GeneDef mode)
		{
			// if (DebugSettings.ShowDevGizmos)
			// {
				// return true;
			// }
			if (pawnGenes.Contains(mode))
			{
				return false;
			}
			if (choosenGenes.NullOrEmpty())
			{
				return false;
			}
			return true;
		}

		public bool CanAdded(GeneDef mode)
		{
			if (pawnGenes.Contains(mode))
			{
				return false;
			}
			if (mode.prerequisite != null && !pawnGenes.Contains(mode.prerequisite))
			{
				return false;
			}
			if (ConflictWith(mode, choosenGenes))
			{
				return false;
			}
			if (ConflictWith(mode, pawnXenoGenes))
			{
				return false;
			}
			return true;
		}

		// public bool CanEat()
		// {
			// if (choosenGenes.NullOrEmpty())
			// {
				// return false;
			// }
			// foreach (GeneDef geneDef in choosenGenes)
			// {
				// foreach (GeneDef eatedDef in eatedGenes)
				// {
					// if (eatedDef == geneDef)
					// {
						// return false;
					// }
				// }
			// }
			// return true;
		// }

		public void StartChange()
		{
			if (!eatAllSelectedGenes)
			{
				if (gene.Props.xenotypeDef != null)
				{
					if (gene.pawn.genes.Xenotype != gene.Props.xenotypeDef)
					{
						ReimplanterUtility.SetXenotypeDirect(null, gene.pawn, gene.Props.xenotypeDef);
					}
				}
				else
				{
					ReimplanterUtility.UnknownXenotype(gene.pawn);
				}
				foreach (GeneDef geneDef in choosenGenes)
				{
					if (!gene.pawn.genes.HasGene(geneDef))
					{
						gene.pawn?.genes?.AddGene(geneDef, xenogene: true);
					}
				}
			}
			else
			{
				foreach (GeneDef geneDef in choosenGenes)
				{
					gene.EatGene(geneDef);
				}
			}
			GeneUtility.UpdateXenogermReplication(gene.pawn);
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(gene.pawn, gene.pawn.Map).Trigger(gene.pawn, null);
			if (!gene.Props.soundDefOnImplant.NullOrUndefined())
			{
				gene.Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(gene.pawn));
			}
			Close(doCloseSound: false);
		}

		public void ClearXenogenes()
		{
			foreach (Gene gene in gene.pawn.genes.Xenogenes.ToList())
			{
				gene.pawn?.genes?.RemoveGene(gene);
			}
			if (!gene.pawn.genes.HasGene(gene.def))
			{
				gene.pawn.genes.AddGene(gene.def, false);
			}
			CheckAllOverrides();
			GeneUtility.UpdateXenogermReplication(gene.pawn);
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(gene.pawn, gene.pawn.Map).Trigger(gene.pawn, null);
			if (!gene.Props.soundDefOnImplant.NullOrUndefined())
			{
				gene.Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(gene.pawn));
			}
			Close(doCloseSound: false);
		}

		private void CheckAllOverrides()
		{
			foreach (Gene item in gene.pawn.genes.GenesListForReading)
			{
				if (item.overriddenByGene != null && !gene.pawn.genes.HasGene(item.overriddenByGene.def))
				{
					item.overriddenByGene = null;
				}
			}
		}

		public Color GetBoxColor(GeneDef mode)
		{
			Color result = TexUI.AvailResearchColor;
			if (choosenGenes.Contains(mode))
			{
				result = TexUI.ActiveResearchColor;
			}
			else if (!CanAdded(mode))
			{
				result = TexUI.LockedResearchColor;
			}
			if (selectedXeno == mode)
			{
				result += TexUI.HighlightBgResearchColor;
			}
			return result;
		}

		public Color GetBoxOutlineColor(GeneDef mode)
		{
			if (selectedXeno != null && selectedXeno == mode)
			{
				return TexUI.HighlightBorderResearchColor;
			}
			return TexUI.DefaultBorderResearchColor;
		}

		public Color GetTextColor(GeneDef mode)
		{
			if (!CanAdded(mode))
			{
				return ColorLibrary.RedReadable;
			}
			return Color.white;
		}

		public void DrawStage(Rect rect, GeneDef stage, float count)
		{
			Vector2 position = GetPosition(rect.height, count);
			Rect rect2 = new(position.x, position.y, OptionSize.x, OptionSize.y);
			Widgets.DrawBoxSolidWithOutline(rect2, GetBoxColor(stage), GetBoxOutlineColor(stage));
			Rect rect3 = new(rect2.x, rect2.y, rect2.height, rect2.height);
			// MiscUtility.XaG_DefIcon(rect3.ContractedBy(4f), stage);
			Widgets.DefIcon(rect3.ContractedBy(4f), stage, color: stage.IconColor);
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

		private static bool ConflictWith(GeneDef geneDef, List<GeneDef> geneDefs)
		{
			foreach (GeneDef item in geneDefs)
			{
				if (item.ConflictsWith(geneDef))
				{
					return true;
				}
			}
			return false;
		}

	}

}
