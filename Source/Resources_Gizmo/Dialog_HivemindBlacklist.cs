using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_HivemindBlacklist : Window
	{

		public Pawn caller;
		public List<Pawn> cachedHivemindPawns;
		public List<Pawn> cachedBlacklistPawns;

		public Dialog_HivemindBlacklist(Pawn pawn)
		{
			//remoteContoller.RemoteControl_Recache();
			caller = pawn;
			forcePause = true;
			doCloseButton = true;
			Update();
		}

		private void Update()
		{
			cachedHivemindPawns = HivemindUtility.HivemindPawns.ToList();
			cachedBlacklistPawns = Gene_Hivemind_Blacklist.BlacklistedPawns.ToList();
			cachedHivemindPawns.AddRangeSafe(cachedBlacklistPawns);
			cachedHivemindPawns.RemoveAll((p) => p == caller || p == null);
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = cachedHivemindPawns.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (Pawn hiver in cachedHivemindPawns)
			{
				if (hiver != caller && num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, hiver.Name.ToStringFull);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, "WVC_AddOrRemove".Translate()))
					{
						Gene_Hivemind_Blacklist.UpdateBlacklist(hiver);
						XaG_UiUtility.FlickSound(!Gene_Hivemind_Blacklist.BlacklistedPawns.Contains(hiver));
						Update();
						break;
					}
					Rect rect4 = new(0f, 0f, 200f, rect.height);
					//Widgets.InfoCardButton(rect4.x, rect.y, hiver);
					//Text.Anchor = TextAnchor.MiddleLeft;
					//Widgets.Label(rect4, hiver.Name.ToStringShort.Truncate(rect4.width * 1.8f));
					//Text.Anchor = TextAnchor.UpperLeft;
					Widgets.HyperlinkWithIcon(rect4, new(hiver));
					//float alpha = 1f;
					if (cachedBlacklistPawns.Contains(hiver))
					{
						Rect rect5 = new(0f, 0f, 36f, 36f);
						Widgets.DrawTextureFitted(rect5, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
					}
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

}
