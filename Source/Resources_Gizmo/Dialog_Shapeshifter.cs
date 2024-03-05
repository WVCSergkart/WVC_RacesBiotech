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
		public GeneExtension_Undead shiftExtension;

		public SoundDef soundDefOnImplant;

		public bool genesRegrowing = false;
		public bool canEverUseShapeshift = true;
		public bool duplicateMode = false;
		// public bool shouldBeDowned = true;
		public List<XenotypeDef> preferredXenotypes;
		public List<string> trustedXenotypes;

		public bool xenogermComaAfterShapeshift = true;

		public Dialog_Shapeshifter(Gene thisGene)
		{
			// Init
			gene = thisGene;
			currentXeno = gene?.pawn?.genes?.Xenotype;
			selectedXeno = currentXeno;
			// Settings
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			// Xenos
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
			// Ideo
			preferredXenotypes = ModLister.IdeologyInstalled ? gene.pawn?.ideo?.Ideo?.PreferredXenotypes : null;
			// Extension
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Undead>();
			soundDefOnImplant = shiftExtension?.soundDefOnImplant;
			// Info
			genesRegrowing = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
			canEverUseShapeshift = !MiscUtility.HasAnyTraits(shiftExtension?.blockingTraits, gene.pawn);
			duplicateMode = MiscUtility.HasAnyTraits(shiftExtension?.duplicateTraits, gene.pawn) || HediffUtility.HasAnyHediff(shiftExtension?.duplicateHediffs, gene.pawn);
			trustedXenotypes = shiftExtension?.trustedXenotypes != null ? shiftExtension.trustedXenotypes : new();
			// Gene stats
			xenogermComaAfterShapeshift = gene is not Gene_Shapeshifter shapeshifter || shapeshifter.xenogermComaAfterShapeshift;
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
				if (allXenotypes.NullOrEmpty())
				{
					Log.Error("Non android xenotypes is null. This error has nothing to do with the mod, do not report it to the developer.");
					return;
				}
				selectedXeno = allXenotypes.RandomElement();
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
			else if (duplicateMode)
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_DuplicateMode".Translate().Colorize(ColorLibrary.LightBlue));
				curY += 10f;
			}
			if (trustedXenotypes.Contains(selectedXeno.defName))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_TrustedXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor));
				curY += 10f;
			}
			else
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneShapeshifter_UnknownXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor));
				curY += 10f;
			}
			if (MeetsRequirements(selectedXeno))
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
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.DrawHighlight(rect4);
				Widgets.Label(rect4.ContractedBy(5f), "WVC_XaG_XenoTreeModeNotMeetsRequirements".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public override void StartChange()
		{
			if (UndeadUtility.TryDuplicatePawn(gene.pawn, gene, selectedXeno, duplicateMode))
			{
				Close(doCloseSound: false);
				return;
			}
			// Shapeshift START
			ReimplanterUtility.SetXenotype_DoubleXenotype(gene.pawn, selectedXeno, new() { gene.def });
			if (!gene.pawn.genes.HasGene(gene.def))
			{
				gene.pawn.genes.AddGene(gene.def, false);
			}
			if (xenogermComaAfterShapeshift)
			{
				gene.pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			}
			GeneUtility.UpdateXenogermReplication(gene.pawn);
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(gene.pawn, gene.pawn.Map).Trigger(gene.pawn, null);
			if (!soundDefOnImplant.NullOrUndefined())
			{
				soundDefOnImplant.PlayOneShot(SoundInfo.InMap(gene.pawn));
			}
			TransferGeneStats(gene.pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>());
			// Shapeshift END
			PostShapeshift(gene);
			// Letter
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

		// Misc
		public void PostShapeshift(Gene gene)
		{
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_GenesDefOf.WVC_Shapeshift, gene.pawn.Named(HistoryEventArgsNames.Doer)));
			}
		}

		public void TransferGeneStats(Gene_Shapeshifter shapeshifter)
		{
			if (shapeshifter == null)
			{
				Log.Error(gene.pawn.Name.ToString() + " pawn used shapeshift, but it is not a shapeshifter. " + gene.LabelCap + " | " + gene.def.defName);
				return;
			}
			shapeshifter.xenogermComaAfterShapeshift = xenogermComaAfterShapeshift;
		}

		// public void PostSelect(Pawn pawn, Gene gene)
		// {
		// }

	}

}
