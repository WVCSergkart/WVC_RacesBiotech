using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Dialog_ChangeGraphic_Simple : Window
	{

		public IGeneCustomGraphic gene;

		public Dialog_ChangeGraphic_Simple(IGeneCustomGraphic gene)
		{
			this.gene = gene;
			forcePause = true;
			//closeOnAccept = false;
			closeOnCancel = true;
			draggable = true;
			closeOnClickedOutside = false;
		}

		public override void DoWindowContents(Rect inRect)
		{
			DrawBottomButtons(inRect);
		}

		private static readonly Vector2 ButSize = new(100f, 20f);


		private void DrawBottomButtons(Rect inRect)
		{
			if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Cancel".Translate()))
			{
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 1.5f - ButSize.x / 1.5f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Next".Translate()))
			{
				Next();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 3f - ButSize.x / 3f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Back".Translate()))
			{
				Back();
			}
			//if (Widgets.ButtonText(new Rect(inRect.xMax - ButSize.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
			//{
			//	Accept();
			//}
		}

		private void Back()
		{
			gene.CurrentTextID--;
			if (gene.CurrentTextID < 0)
			{
				gene.CurrentTextID = 1;
			}
		}

		private void Next()
		{
			gene.CurrentTextID++;
		}

	}

}
