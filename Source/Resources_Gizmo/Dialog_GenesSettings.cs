using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_GenesSettings : Window
	{

		public List<IGeneRemoteControl> genes;

		public Dialog_GenesSettings(Pawn pawn)
        {
            //remoteContoller.RemoteControl_Recache();
            UpdGenes(pawn);
            forcePause = true;
            doCloseButton = true;
        }

        private void UpdGenes(Pawn pawn)
        {
            this.genes = new();
            foreach (Gene item in pawn.genes.GenesListForReading)
            {
                if (item is IGeneRemoteControl controller && !controller.RemoteControl_Hide)
                {
                    genes.Add(controller);
                }
            }
        }

        protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)genes.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (IGeneRemoteControl controller in genes)
			{
				if (controller is Gene gene && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TaggedString taggedString = controller.RemoteActionDesc;
					TooltipHandler.TipRegion(rect, taggedString);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, controller.RemoteActionName))
					{
						controller.RemoteControl_Action(this);
						SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(gene.pawn.Position, gene.pawn.Map));
						UpdGenes(gene.pawn);
						break;
					}
					Rect rect4 = new(40f, 0f, 200f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect4, gene.LabelCap.Truncate(rect4.width * 1.8f));
					Text.Anchor = TextAnchor.UpperLeft;
					Rect rect5 = new(0f, 0f, 36f, 36f);
					XaG_UiUtility.XaG_DefIcon(rect5, gene.def, 1.2f);
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

}
