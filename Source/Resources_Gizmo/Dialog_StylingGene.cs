using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_StylingGene : Window
	{
		public enum StylingTab
		{
			Hair,
			Beard,
			TattooFace,
			TattooBody,
			Other
		}

		protected Pawn pawn;
		protected Gene gene;
		protected HairDef initialHairDef;
		protected BeardDef initialBeardDef;
		protected TattooDef initialFaceTattoo;
		protected TattooDef initialBodyTattoo;
		protected Color desiredHairColor;
		protected StylingTab curTab;
		protected Vector2 hairScrollPosition;
		private Vector2 beardScrollPosition;
		private Vector2 faceTattooScrollPosition;
		private Vector2 bodyTattooScrollPosition;

		// private Vector2 apparelColorScrollPosition;

		public List<TabRecord> tabs = new();

		public Dictionary<Apparel, Color> apparelColors = new();

		private float viewRectHeight;

		private bool showHeadgear;

		private bool showClothes;

		public bool devEditMode;

		// private List<Color> allColors;

		public List<Color> allHairColors;

		public float colorsHeight;

		private static readonly Vector2 ButSize = new(200f, 40f);

		private static readonly Vector3 PortraitOffset = new(0f, 0f, 0.15f);

		public static List<string> tmpUnwantedStyleNames = new();

		protected List<StyleItemDef> cachedStyleItems;

		// private bool DevMode => stylingStation == null;

		public override Vector2 InitialSize => new(950f, 750f);

		public virtual List<Color> AllHairColors
		{
			get
			{
				if (allHairColors == null)
				{
					allHairColors = new List<Color>();
					foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefsListForReading)
					{
						Color color = allDef.color;
						if (allDef.displayInStylingStationUI && !allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
						{
							allHairColors.Add(color);
						}
					}
					foreach (GeneDef allDef in DefDatabase<GeneDef>.AllDefsListForReading)
					{
						if (!allDef.hairColorOverride.HasValue)
						{
							continue;
						}
						Color color = allDef.hairColorOverride.Value;
						AddColor_FromGene(color);
					}
					if (pawn.genes.GenesListForReading.Any(gene => gene.def.skinIsHairColor))
					{
						foreach (GeneDef allDef in DefDatabase<GeneDef>.AllDefsListForReading)
						{
							if (!allDef.skinColorOverride.HasValue)
							{
								continue;
							}
							Color color = allDef.skinColorOverride.Value;
							AddColor_FromGene(color);
						}
					}
					allHairColors.SortByColor((Color x) => x);
				}
				return allHairColors;

				void AddColor_FromGene(Color color)
				{
					if (!allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
					{
						allHairColors.Add(color);
					}
				}
			}
		}

		public bool unlockTattoos;
		//public bool unlockEyesRecolor;

		public Dialog_StylingGene(Pawn pawn, Gene gene, bool unlockTattoos)
		{
			this.pawn = pawn;
			this.gene = gene;
			initialHairDef = pawn.story.hairDef;
			desiredHairColor = pawn.story.HairColor;
			initialBeardDef = pawn.style.beardDef;
			initialFaceTattoo = pawn.style.FaceTattoo;
			initialBodyTattoo = pawn.style.BodyTattoo;
			this.unlockTattoos = unlockTattoos;
			//this.unlockEyesRecolor = unlockEyesRecolor;
			forcePause = true;
			showClothes = false;
			closeOnAccept = false;
			closeOnCancel = true;
			foreach (Apparel item in pawn.apparel.WornApparel)
			{
				if (item.TryGetComp<CompColorable>() != null)
				{
					apparelColors.Add(item, item.DesiredColor ?? item.DrawColor);
				}
			}
		}

		public override void PostOpen()
		{
			if (!ModLister.CheckIdeology("Styling station"))
			{
				Close();
			}
			else
			{
				base.PostOpen();
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Rect rect = new(inRect);
			rect.height = Text.LineHeight * 2f;
			Widgets.Label(rect, "StylePawn".Translate().CapitalizeFirst() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(pawn.Name.ToStringShort, pawn.gender, plural: false, name: true).ApplyTag(TagType.Name));
			Text.Font = GameFont.Small;
			inRect.yMin = rect.yMax + 4f;
			Rect rect2 = inRect;
			rect2.width *= 0.3f;
			rect2.yMax -= ButSize.y + 4f;
			DrawPawn(rect2);
			Rect rect3 = inRect;
			rect3.xMin = rect2.xMax + 10f;
			rect3.yMax -= ButSize.y + 4f;
			DrawTabs(rect3);
			DrawBottomButtons(inRect);
			if (Prefs.DevMode)
			{
				Widgets.CheckboxLabeled(new Rect(inRect.xMax - 120f, 0f, 120f, 30f), "DEV: Show all", ref devEditMode);
			}
		}

		private void DrawPawn(Rect rect)
		{
			Rect rect2 = rect;
			rect2.yMin = rect.yMax - Text.LineHeight * 2f;
			Widgets.CheckboxLabeled(new Rect(rect2.x, rect2.y, rect2.width, rect2.height / 2f), "ShowHeadgear".Translate(), ref showHeadgear);
			Widgets.CheckboxLabeled(new Rect(rect2.x, rect2.y + rect2.height / 2f, rect2.width, rect2.height / 2f), "ShowApparel".Translate(), ref showClothes);
			rect.yMax = rect2.yMin - 4f;
			Widgets.BeginGroup(rect);
			for (int i = 0; i < 3; i++)
			{
				Rect position = new Rect(0f, rect.height / 3f * i, rect.width, rect.height / 3f).ContractedBy(4f);
				RenderTexture image = PortraitsCache.Get(pawn, new Vector2(position.width, position.height), new Rot4(2 - i), PortraitOffset, 1.1f, supersample: true, compensateForUIScale: true, showHeadgear, showClothes, apparelColors, desiredHairColor, stylingStation: true);
				GUI.DrawTexture(position, image);
			}
			Widgets.EndGroup();
			if (pawn.style.HasAnyUnwantedStyleItem)
			{
				string text = "PawnUnhappyWithStyleItems".Translate(pawn.Named("PAWN")) + ": ";
				tmpUnwantedStyleNames.Clear();
				if (pawn.style.HasUnwantedHairStyle)
				{
					tmpUnwantedStyleNames.Add("Hair".Translate());
				}
				if (pawn.style.HasUnwantedBeard)
				{
					tmpUnwantedStyleNames.Add("Beard".Translate());
				}
				if (pawn.style.HasUnwantedFaceTattoo)
				{
					tmpUnwantedStyleNames.Add("TattooFace".Translate());
				}
				if (pawn.style.HasUnwantedBodyTattoo)
				{
					tmpUnwantedStyleNames.Add("TattooBody".Translate());
				}
				GUI.color = ColorLibrary.RedReadable;
				Widgets.Label(new Rect(rect.x, rect.yMin - 30f, rect.width, Text.LineHeight * 2f + 10f), "Warning".Translate() + ": " + text + tmpUnwantedStyleNames.ToCommaList().CapitalizeFirst());
				GUI.color = Color.white;
			}
		}

		public virtual void DrawTabs(Rect rect)
		{
			tabs.Clear();
			tabs.Add(new TabRecord("Hair".Translate().CapitalizeFirst(), delegate
			{
				SetTab(StylingTab.Hair);
			}, curTab == StylingTab.Hair));
			if (pawn.style.CanWantBeard || devEditMode)
			{
				tabs.Add(new TabRecord("Beard".Translate().CapitalizeFirst(), delegate
				{
					SetTab(StylingTab.Beard);
				}, curTab == StylingTab.Beard));
			}
			if (unlockTattoos)
			{
				tabs.Add(new TabRecord("TattooFace".Translate().CapitalizeFirst(), delegate
				{
					SetTab(StylingTab.TattooFace);
				}, curTab == StylingTab.TattooFace));
				tabs.Add(new TabRecord("TattooBody".Translate().CapitalizeFirst(), delegate
				{
					SetTab(StylingTab.TattooBody);
				}, curTab == StylingTab.TattooBody));
			}
			tabs.Add(new TabRecord("WVC_BiotechSettings_Tab_ExtraSettings".Translate().CapitalizeFirst(), delegate
			{
				SetTab(StylingTab.Other);
			}, curTab == StylingTab.Other));
			// tabs.Add(new TabRecord("ApparelColor".Translate().CapitalizeFirst(), delegate
			// {
			// curTab = StylingTab.ApparelColor;
			// }, curTab == StylingTab.ApparelColor));
			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(rect, tabs);
			rect = rect.ContractedBy(18f);
			switch (curTab)
			{
				case StylingTab.Hair:
					rect.yMax -= colorsHeight;
					DrawStylingItemType(rect, ref hairScrollPosition, delegate (Rect r, HairDef h)
					{
						GUI.color = desiredHairColor;
						Widgets.DefIcon(r, h, null, 1.25f);
						GUI.color = Color.white;
					}, delegate (HairDef h)
					{
						pawn.story.hairDef = h;
					}, (StyleItemDef h) => pawn.story.hairDef == h, (StyleItemDef h) => initialHairDef == h, (StyleItemDef h) => PawnStyleItemChooser.WantsToUseStyle(pawn, h), doColors: unlockTattoos);
					break;
				case StylingTab.Beard:
					DrawStylingItemType(rect, ref beardScrollPosition, delegate (Rect r, BeardDef b)
					{
						GUI.color = desiredHairColor;
						Widgets.DefIcon(r, b, null, 1.25f);
						GUI.color = Color.white;
					}, delegate (BeardDef b)
					{
						pawn.style.beardDef = b;
					}, (StyleItemDef b) => pawn.style.beardDef == b, (StyleItemDef b) => initialBeardDef == b, extraValidator: (StyleItemDef b) => PawnStyleItemChooser.WantsToUseStyle(pawn, b));
					break;
				case StylingTab.TattooFace:
					DrawStylingItemType(rect, ref faceTattooScrollPosition, delegate (Rect r, TattooDef t)
					{
						Widgets.DefIcon(r, t);
					}, delegate (TattooDef t)
					{
						pawn.style.FaceTattoo = t;
					}, (StyleItemDef t) => pawn.style.FaceTattoo == t, (StyleItemDef t) => initialFaceTattoo == t, (StyleItemDef t) => ((TattooDef)t).tattooType == TattooType.Face && PawnStyleItemChooser.WantsToUseStyle(pawn, t));
					break;
				case StylingTab.TattooBody:
					DrawStylingItemType(rect, ref bodyTattooScrollPosition, delegate (Rect r, TattooDef t)
					{
						Widgets.DefIcon(r, t);
					}, delegate (TattooDef t)
					{
						pawn.style.BodyTattoo = t;
					}, (StyleItemDef t) => pawn.style.BodyTattoo == t, (StyleItemDef t) => initialBodyTattoo == t, (StyleItemDef t) => ((TattooDef)t).tattooType == TattooType.Body && PawnStyleItemChooser.WantsToUseStyle(pawn, t));
					break;
				case StylingTab.Other:
					// Do nothing
					break;
			}
		}

		public void SetTab(StylingTab newTab)
		{
			cachedStyleItems = null;
			if (newTab == StylingTab.Other)
			{
				Find.WindowStack.Add(new Dialog_StylingExtra(pawn, gene, unlockTattoos, true, this));
			}
			else
			{
				curTab = newTab;
			}
		}

		public virtual void DrawHairColors(Rect rect)
		{
			float y = rect.y;
			Widgets.ColorSelector(new Rect(rect.x, y, rect.width, colorsHeight), ref desiredHairColor, AllHairColors, out colorsHeight);
			colorsHeight += Text.LineHeight * 2f;
		}

		public void DrawStylingItemType<T>(Rect rect, ref Vector2 scrollPosition, Action<Rect, T> drawAction, Action<T> selectAction, Func<StyleItemDef, bool> hasStyleItem, Func<StyleItemDef, bool> hadStyleItem, Func<StyleItemDef, bool> extraValidator = null, bool doColors = false, int currentStyleId = -1) where T : StyleItemDef
		{
			Rect viewRect = new(rect.x, rect.y, rect.width - 16f, viewRectHeight);
			int num = Mathf.FloorToInt(viewRect.width / 60f) - 1;
			float num2 = (viewRect.width - num * 60f - (num - 1) * 10f) / 2f;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			//cachedStyleItems.Clear();
			if (cachedStyleItems == null)
			{
				//Log.Error("1");
				cachedStyleItems = new();
				cachedStyleItems.AddRange(DefDatabase<T>.AllDefsListForReading.Where((T x) => devEditMode || hadStyleItem(x) || (extraValidator == null || extraValidator(x)) && (x is not StyleGeneDef styleGeneDef || styleGeneDef.AllowedForStyle(currentStyleId))));
				cachedStyleItems.SortBy((StyleItemDef x) => 0f - PawnStyleItemChooser.FrequencyFromGender(x, pawn));
			}
			if (cachedStyleItems.NullOrEmpty())
			{
				Widgets.NoneLabelCenteredVertically(rect, "(" + "NoneUsableForPawn".Translate(pawn.Named("PAWN")) + ")");
			}
			else
			{
				Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
				foreach (StyleItemDef tmpStyleItem in cachedStyleItems)
				{
					if (num5 >= num - 1)
					{
						num5 = 0;
						num4++;
					}
					else if (num3 > 0)
					{
						num5++;
					}
					Rect rect2 = new(rect.x + num2 + num5 * 60f + num5 * 10f, rect.y + num4 * 60f + num4 * 10f, 60f, 60f);
					Widgets.DrawHighlight(rect2);
					if (Mouse.IsOver(rect2))
					{
						Widgets.DrawHighlight(rect2);
						TooltipHandler.TipRegion(rect2, tmpStyleItem.LabelCap);
					}
					drawAction?.Invoke(rect2, tmpStyleItem as T);
					if (hasStyleItem(tmpStyleItem))
					{
						Widgets.DrawBox(rect2, 2);
					}
					if (Widgets.ButtonInvisible(rect2))
					{
						selectAction?.Invoke(tmpStyleItem as T);
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						pawn.Drawer.renderer.SetAllGraphicsDirty();
					}
					num3++;
				}
				if (Event.current.type == EventType.Layout)
				{
					viewRectHeight = (num4 + 1) * 60f + num4 * 10f + 10f;
				}
				Widgets.EndScrollView();
			}
			if (doColors)
			{
				DrawHairColors(new Rect(rect.x, rect.yMax + 10f, rect.width, colorsHeight));
			}
		}

		private void DrawBottomButtons(Rect inRect)
		{
			if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), CloseButtonText))
			{
				Reset();
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 2f - ButSize.x / 2f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Reset".Translate()))
			{
				Reset();
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMax - ButSize.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
			{
				Accept();
			}
		}

		public virtual void Accept()
		{
			if (pawn.story.hairDef != initialHairDef || pawn.style.beardDef != initialBeardDef || pawn.style.FaceTattoo != initialFaceTattoo || pawn.style.BodyTattoo != initialBodyTattoo || pawn.story.HairColor != desiredHairColor)
			{
				pawn.story.HairColor = desiredHairColor;
				pawn.style.Notify_StyleItemChanged();
				if (gene is IGeneWithEffects effecter)
				{
					effecter.DoEffects();
				}
			}
			Close();
		}

		public virtual void Reset(bool resetColors = true)
		{
			if (resetColors)
			{
				apparelColors.Clear();
				foreach (Apparel item in pawn.apparel.WornApparel)
				{
					if (item.TryGetComp<CompColorable>() != null)
					{
						apparelColors.Add(item, item.DesiredColor ?? item.DrawColor);
					}
				}
				desiredHairColor = pawn.story.HairColor;
			}
			pawn.story.hairDef = initialHairDef;
			pawn.style.beardDef = initialBeardDef;
			pawn.style.FaceTattoo = initialFaceTattoo;
			pawn.style.BodyTattoo = initialBodyTattoo;
			pawn.Drawer.renderer.SetAllGraphicsDirty();
		}
	}

}
