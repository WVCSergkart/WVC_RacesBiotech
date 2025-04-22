using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Dialog_Generemover : Window
	{

		public List<Gene> genes;
		public Gene ignoredGene;

		public Dialog_Generemover(Pawn pawn, Gene ignoredGene = null)
		{
			this.ignoredGene = ignoredGene;
			this.genes = pawn.genes.GenesListForReading;
			forcePause = true;
			doCloseButton = true;
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
			foreach (Gene gene in genes.ToList())
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, gene.def.description);
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, "Remove".Translate()))
					{
						Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneRemover_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
						{
							gene.pawn.genes.RemoveGene(gene);
							gene.pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>()?.TryOffsetResource(gene);
							genes = gene.pawn.genes.GenesListForReading;
							//SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(gene.pawn.Position, gene.pawn.Map));
							if (gene == ignoredGene)
                            {
								Close();
                            }
							ReimplanterUtility.PostImplantDebug(gene.pawn);
						});
						Find.WindowStack.Add(window);
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
