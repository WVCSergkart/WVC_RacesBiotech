// RimWorld.GeneUIUtility
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static void DrawGeneBasics(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
	{
		GUI.BeginGroup(geneRect);
		Rect rect = geneRect.AtZero();
		if (doBackground)
		{
			Widgets.DrawHighlight(rect);
			GUI.color = new Color(1f, 1f, 1f, 0.05f);
			Widgets.DrawBox(rect);
			GUI.color = Color.white;
		}
		float num = rect.width - Text.LineHeight;
		Rect rect2 = new Rect(geneRect.width / 2f - num / 2f, 0f, num, num);
		Color iconColor = gene.IconColor;
		if (overridden)
		{
			iconColor.a = 0.75f;
			GUI.color = ColoredText.SubtleGrayColor;
		}
		CachedTexture cachedTexture = GeneBackground_Archite;
		if (gene.biostatArc == 0)
		{
			switch (geneType)
			{
			case GeneType.Endogene:
				cachedTexture = GeneBackground_Endogene;
				break;
			case GeneType.Xenogene:
				cachedTexture = GeneBackground_Xenogene;
				break;
			}
		}
		GUI.DrawTexture(rect2, cachedTexture.Texture);
		Widgets.DefIcon(rect2, gene, null, 0.9f, null, drawPlaceholder: false, iconColor);
		Text.Font = GameFont.Tiny;
		float num2 = Text.CalcHeight(gene.LabelCap, rect.width);
		Rect rect3 = new Rect(0f, rect.yMax - num2, rect.width, num2);
		GUI.DrawTexture(new Rect(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
		Text.Anchor = TextAnchor.LowerCenter;
		if (overridden)
		{
			GUI.color = ColoredText.SubtleGrayColor;
		}
		if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
		{
			rect3.y -= 3f;
		}
		Widgets.Label(rect3, gene.LabelCap);
		GUI.color = Color.white;
		Text.Anchor = TextAnchor.UpperLeft;
		Text.Font = GameFont.Small;
		if (clickable)
		{
			if (Widgets.ButtonInvisible(rect))
			{
				Find.WindowStack.Add(new Dialog_InfoCard(gene));
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
		}
		GUI.EndGroup();
	}

}
