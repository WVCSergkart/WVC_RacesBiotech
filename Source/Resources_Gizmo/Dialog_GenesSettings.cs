using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_GenesSettings : Window
	{

		public List<Setting> settings;
		public Pawn pawn;

		public Dialog_GenesSettings(Pawn pawn)
		{
			//remoteContoller.RemoteControl_Recache();
			this.pawn = pawn;
			UpdGenes(pawn);
			forcePause = true;
			doCloseButton = true;
		}

		public class Setting
		{

			public Texture icon;
			public Def def;
			public string labelCap;
			public string name;
			public string description;

			public Action action; 

			public Setting()
			{

			}

			public Setting(IGeneRemoteControl geneRemoteControl, Dialog_GenesSettings dialog_GenesSettings)
			{
				if (geneRemoteControl is Gene gene)
				{
					labelCap = gene.LabelCap;
					def = gene.def;
				}
				name = geneRemoteControl.RemoteActionName.ToString();
				description = geneRemoteControl.RemoteActionDesc.ToString();
				action = delegate
				{
					geneRemoteControl.RemoteControl_Action(dialog_GenesSettings);
				};
			}

			public Setting(Command_Action command_Action)
			{
				name = "Action";
				labelCap = command_Action.defaultLabel;
				description = command_Action.defaultDesc;
				action = command_Action.action;
				icon = command_Action.icon;
			}

		}

		private void UpdGenes(Pawn pawn)
		{
			this.settings = new();
			if (DebugSettings.ShowDevGizmos)
			{
				//CompHumanlike compHumanlike = pawn.HumanComponent();
				//if (compHumanlike != null)
				//{
				//	foreach (Gizmo gizmo in compHumanlike.DevGizmos())
				//	{
				//		if (gizmo is Command_Action command_Action)
				//		{
				//			settings.Add(new(command_Action));
				//		}
				//	}
				//}
				foreach (ThingComp thingComp in pawn.AllComps)
				{
					if (thingComp is IThingDevSettings thingDevSettings)
					{
						foreach (Gizmo gizmo in thingDevSettings.DevGizmos())
						{
							if (gizmo is Command_Action command_Action)
							{
								settings.Add(new(command_Action));
							}
						}
					}
				}
			}
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (item is IGeneRemoteControl controller && !controller.RemoteControl_Hide)
				{
					settings.Add(new(controller, this));
				}
			}
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = settings.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (Setting controller in settings)
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, controller.description);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, controller.name))
					{
						controller.action();
						SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
						UpdGenes(pawn);
						break;
					}
					Rect rect4 = new(40f, 0f, 200f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect4, controller.labelCap.Truncate(rect4.width * 1.8f));
					Text.Anchor = TextAnchor.UpperLeft;
					Rect rect5 = new(0f, 0f, 36f, 36f);
					Icon(controller, rect5);
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

		private void Icon(Setting controller, Rect rect5)
		{
			if (controller.def != null)
			{
				XaG_UiUtility.XaG_DefIcon(rect5, controller.def, 1.2f);
			}
			else if (controller.icon != null)
			{
				Widgets.DrawTextureFitted(rect5, controller.icon, 1.2f);
			}
		}

		//public override void DoWindowContents(Rect inRect)
		//{
		//	Vector2 vector = new(inRect.width - 16f, 40f);
		//	float y = vector.y;
		//	float height = genes.Count * y;
		//	Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
		//	float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
		//	Rect outRect = inRect.TopPartPixels(num);
		//	Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		//	float num2 = 0f;
		//	int num3 = 0;
		//	foreach (IGeneRemoteControl controller in genes)
		//	{
		//		if (controller is Gene gene && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
		//		{
		//			Rect rect = new(0f, num2, vector.x, vector.y);
		//			TooltipHandler.TipRegion(rect, controller.RemoteActionDesc);
		//			if (num3 % 2 == 0)
		//			{
		//				Widgets.DrawAltRect(rect);
		//			}
		//			Widgets.BeginGroup(rect);
		//			GUI.color = Color.white;
		//			Text.Font = GameFont.Small;
		//			Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
		//			if (Widgets.ButtonText(rect3, controller.RemoteActionName))
		//			{
		//				controller.RemoteControl_Action(this);
		//				SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(gene.pawn.Position, gene.pawn.Map));
		//				UpdGenes(gene.pawn);
		//				break;
		//			}
		//			Rect rect4 = new(40f, 0f, 200f, rect.height);
		//			Text.Anchor = TextAnchor.MiddleLeft;
		//			Widgets.Label(rect4, gene.LabelCap.Truncate(rect4.width * 1.8f));
		//			Text.Anchor = TextAnchor.UpperLeft;
		//			Rect rect5 = new(0f, 0f, 36f, 36f);
		//			XaG_UiUtility.XaG_DefIcon(rect5, gene.def, 1.2f);
		//			Widgets.EndGroup();
		//		}
		//		num2 += vector.y;
		//		num3++;
		//	}
		//	Widgets.EndScrollView();
		//}

	}

}
