using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class PawnColumnWorker_MutantWorkerKill : PawnColumnWorker_Icon
	{

		private static readonly CachedTexture Icon = new("WVC/UI/XaG_General/KillMutant");

		public override bool VisibleCurrently
		{
			get
			{
				return true;
			}
		}

		protected override Texture2D GetIconFor(Pawn pawn)
		{
			return Icon.Texture;
		}

		protected override Color GetIconColor(Pawn pawn)
		{
			return Color.white;
		}

		protected override string GetIconTip(Pawn pawn)
		{
			return "WVC_MutantWorker_Kill".Translate();
		}

		protected override void ClickedIcon(Pawn pawn)
		{
			if (XaG_GameComponent.warningTimeout > 0)
			{
				pawn.Kill();
				return;
			}
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_MutantWorker_Warning".Translate(), delegate
			{
				pawn.Kill();
				XaG_GameComponent.warningTimeout = 3600;
			});
			Find.WindowStack.Add(window);
		}

	}

}
