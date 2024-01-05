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
		public GeneExtension_Shapeshifter shiftExtension;

		public SoundDef soundDefOnImplant;

		public bool genesRegrowing = false;
		public bool canEverUseShapeshift = true;
		// public bool duplicateMode = false;
		public List<XenotypeDef> preferredXenotypes;

		public Dialog_Shapeshifter(Gene thisGene)
		{
			gene = thisGene;
			currentXeno = thisGene?.pawn?.genes?.Xenotype != null ? thisGene.pawn.genes.Xenotype : null;
			selectedXeno = currentXeno;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
			preferredXenotypes = ModLister.IdeologyInstalled ? gene.pawn?.ideo?.Ideo?.PreferredXenotypes : null;
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Shapeshifter>();
			soundDefOnImplant = shiftExtension?.soundDefOnImplant;
			// genesRegrowing = gene.pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa) || gene.pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating);
			// canEverUseShapeshift = !gene.pawn.story.traits.HasTrait(WVC_GenesDefOf.WVC_XaG_ShapeshiftPhobia);
			genesRegrowing = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
			canEverUseShapeshift = !MiscUtility.HasAnyTraits(shiftExtension?.blockingTraits, gene.pawn);
			// duplicateMode = HediffUtility.HasAnyHediff(shiftExtension?.duplicateHediffs, gene.pawn);
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
			if (!canEverUseShapeshift)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DisabledPermanent".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			else if (genesRegrowing)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			else if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(selectedXeno))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_NotPreferredXenotype".Translate().Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			// else if (duplicateMode)
			// {
				// Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DuplicateMode".Translate().Colorize(ColorLibrary.LightBlue));
				// curY += 10f;
			// }
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
			// if (duplicateMode)
			// {
				// GestationUtility.DuplicatePawn_WithChosenXenotype(gene, selectedXeno);
			// }
			// else
			// {
			// }
			// List<GeneDef> dontRemove = new() { gene.def };
			// dontRemove.Add(gene.def);
			ReimplanterUtility.SetXenotype_DoubleXenotype(gene.pawn, selectedXeno, new() { gene.def });
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
			if (!canEverUseShapeshift)
			{
				return false;
			}
			if (genesRegrowing)
			{
				return false;
			}
			if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(mode))
			{
				return false;
			}
			return true;
		}

	}

}
