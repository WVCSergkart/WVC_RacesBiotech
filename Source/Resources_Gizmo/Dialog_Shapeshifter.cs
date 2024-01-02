using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_Shapeshifter : Dialog_XenotypesBase
	{
		public Gene gene;

		public SoundDef soundDefOnImplant;

		public bool genesRegrowing = false;

		public Dialog_Shapeshifter(Gene thisGene)
		{
			gene = thisGene;
			currentXeno = thisGene?.pawn?.genes?.Xenotype != null ? thisGene.pawn.genes.Xenotype : null;
			// selectedXeno = currentXeno;
			soundDefOnImplant = gene?.def?.GetModExtension<GeneExtension_Giver>()?.soundDefOnImplant;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
			genesRegrowing = gene.pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating);
		}

		public override void DrawLeftRect(Rect rect, ref float curY)
		{
			Rect rect2 = new(rect.x, curY, rect.width, rect.height)
			{
				yMax = rect.yMax
			};
			Rect rect3 = rect2.ContractedBy(4f);
			if (selectedXeno == null)
			{
				Widgets.Label(rect3, "WVC_XaG_GeneShapeshifter_Desc".Translate());
				return;
			}
			if (selectedXeno.descriptionShort.NullOrEmpty())
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.description);
			}
			else
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, selectedXeno.descriptionShort);
			}
			curY += 10f;
			Rect rect4 = new(rect3.x, rect3.yMax - 55f, rect3.width, 55f);
			foreach (XenotypeDef item in XenoTreeUtility.GetXenotypeAndDoubleXenotypes(selectedXeno))
			{
				Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(item));
				curY += Text.LineHeight;
			}
			curY += 10f;
			if (genesRegrowing)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, HediffDefOf.XenogermReplicating.description.Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			if (MeetsRequirements(selectedXeno) && selectedXeno != currentXeno)
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), delegate
					{
						StartChange();
					});
					Find.WindowStack.Add(window);
				}
			}
			else
			{
				string label = (!MeetsRequirements(selectedXeno)) ? "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate() : ((string)"Locked".Translate());
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void StartChange()
		{
			List<GeneDef> dontRemove = new();
			dontRemove.Add(gene.def);
			ReimplanterUtility.SetXenotype_DoubleXenotype(gene.pawn, selectedXeno, dontRemove.ToList());
			if (!gene.pawn.genes.HasGene(gene.def))
			{
				gene.pawn.genes.AddGene(gene.def, false);
			}
			if (!SerumUtility.HasCandidateGene(gene.pawn))
			{
				gene.pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			}
			GeneUtility.UpdateXenogermReplication(gene.pawn);
			if (!soundDefOnImplant.NullOrUndefined())
			{
				soundDefOnImplant.PlayOneShot(SoundInfo.InMap(gene.pawn));
			}
			Find.LetterStack.ReceiveLetter("WVC_XaG_GeneShapeshifter_ShapeshiftLetterLabel".Translate(), "WVC_XaG_GeneShapeshifter_ShapeshiftLetterDesc".Translate(gene.pawn.Named("TARGET"), selectedXeno.LabelCap, gene.LabelCap)
				+ "\n\n" + (selectedXeno.descriptionShort.NullOrEmpty() ? selectedXeno.description : selectedXeno.descriptionShort),
				WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(gene.pawn));
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			// if (selectedXeno == currentXeno)
			// {
				// return false;
			// }
			if (genesRegrowing)
			{
				return false;
			}
			return true;
		}

	}

}
