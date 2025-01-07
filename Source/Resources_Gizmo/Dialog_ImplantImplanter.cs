using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_ImplantImplanter : Window
	{

		public List<BodyPartRecord> possibleParts;
		public Pawn pawn;
		public RecipeDef recipeDef;

		public Dialog_ImplantImplanter(Pawn pawn, RecipeDef recipeDef, List<BodyPartRecord> possibleRecipes)
		{
			this.possibleParts = possibleRecipes;
			this.pawn = pawn;
			this.recipeDef = recipeDef;
			forcePause = true;
			doCloseButton = false;
			draggable = true;
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)possibleParts.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (BodyPartRecord controller in possibleParts)
			{
				if (controller is BodyPartRecord bodyPartRecord && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, controller.Label);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, controller.LabelShortCap))
					{
						SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
						HediffUtility.ApplyImplantOnPawn(pawn, recipeDef, new() { controller });
						Close();
					}
					Rect rect4 = new(0f, 0f, 200f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Text.Font = GameFont.Small;
					Widgets.Label(rect4, bodyPartRecord.LabelCap.Truncate(rect4.width * 1.8f));
					Text.Anchor = TextAnchor.UpperLeft;
					Rect rect5 = new(0f, 0f, 36f, 36f);
					//XaG_UiUtility.XaG_DefIcon(rect5, bodyPartRecord.def, 1.2f);
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

	public class Dialog_BodyPartsHarvest : Window
	{

		public List<BodyPartRecord> possibleParts;
		public Pawn pawn;
		public Pawn corpse;

		public Dialog_BodyPartsHarvest(Pawn pawn, Pawn corpse, List<BodyPartRecord> possibleRecipes)
		{
			this.possibleParts = possibleRecipes;
			this.pawn = pawn;
			this.corpse = corpse;
			forcePause = true;
			doCloseButton = false;
			draggable = true;
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)possibleParts.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (BodyPartRecord bodyPart in possibleParts)
			{
				if (bodyPart is BodyPartRecord bodyPartRecord && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, bodyPart.Label);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, bodyPart.LabelShortCap))
					{
						SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
						pawn.health.RemoveHediff(pawn.health.hediffSet.GetMissingPartFor(bodyPart));
						corpse.health.AddHediff(HediffDefOf.MissingBodyPart, bodyPart);
						Close();
					}
					Rect rect4 = new(0f, 0f, 200f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Text.Font = GameFont.Small;
					Widgets.Label(rect4, bodyPartRecord.LabelCap.Truncate(rect4.width * 1.8f));
					Text.Anchor = TextAnchor.UpperLeft;
					Rect rect5 = new(0f, 0f, 36f, 36f);
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

}
