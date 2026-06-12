using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ActivityManager : Window
	{

		public List<Gene> genes;
		//public List<Gene> genesForOverride;
		private Gene masterGene;
		private Pawn pawn;
		private List<GeneCategoryDef> geneCategoryDefs = null;

		public Dialog_ActivityManager(Pawn pawn, Gene masterGene, List<GeneCategoryDef> geneCategoryDefs = null)
		{
			forcePause = true;
			doCloseButton = true;
			this.masterGene = masterGene;
			this.pawn = pawn;
			this.geneCategoryDefs = geneCategoryDefs;
			UpdGenes(pawn);
		}

		private void UpdGenes(Pawn pawn)
		{
			this.genes = new();
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				try
				{
					if (item is IGeneDisconnectable controller)
					{
						if (masterGene.def.IsGeneDefOfType(controller.MasterClass))
						{
							genes.Add(item);
						}
					}
					else if (geneCategoryDefs != null && geneCategoryDefs.Contains(item.def.displayCategory) && (!item.Overridden || item.overriddenByGene == masterGene))
					{
						genes.Add(item);
					}
				}
				catch
				{
					Log.Error(item.def.defName);
				}
			}
			genes.SortBy(item => item is not IGeneDisconnectable);
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = genes.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (Gene gene in genes)
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					if (DrawGenes(vector, num2, num3, gene))
					{
						SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
						UpdGenes(pawn);
						break;
					}
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

		private bool DrawGenes(Vector2 vector, float num2, int num3, Gene gene)
		{
			IGeneDisconnectable controller = gene is IGeneDisconnectable ? gene as IGeneDisconnectable : null;
			Rect rect = new(0f, num2, vector.x, vector.y);
			TooltipHandler.TipRegion(rect, gene.def.DescriptionFull + "\n\n" + XaG_UiUtility.ClickTo(controller != null ? !controller.Disabled : !gene.Overridden));
			if (num3 % 2 == 0)
			{
				Widgets.DrawAltRect(rect);
			}
			Widgets.BeginGroup(rect);
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
			if (controller != null)
			{
				if (Widgets.ButtonText(rect3, XaG_UiUtility.OnOrOff(!controller.Disabled)))
				{
					controller.Disabled = !controller.Disabled;
					controller.UpdateCache();
					return true;
				}
			}
			else
			{
				if (Widgets.ButtonText(rect3, XaG_UiUtility.OverrideOrUnoverride(!gene.Overridden)))
				{
					if (gene.Overridden)
					{
						gene.OverrideBy(null);
					}
					else
					{
						gene.OverrideBy(masterGene);
					}
					return true;
				}
			}
			Rect rect4 = new(40f, 0f, 200f, rect.height);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect4, gene.LabelCap.Truncate(rect4.width * 1.8f));
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect5 = new(0f, 0f, 36f, 36f);
			XaG_UiUtility.XaG_DefIcon(rect5, gene.def, 1.2f);
			Widgets.EndGroup();
			return false;
		}

		public override void Close(bool doCloseSound = true)
		{
			base.Close(doCloseSound);
			MiscUtility.Notify_DebugPawn(pawn);
			//ReimplanterUtility.NotifyGenesChanged(pawn);
		}

	}

}
