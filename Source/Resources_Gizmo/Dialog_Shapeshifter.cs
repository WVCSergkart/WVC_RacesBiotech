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

		public Dialog_Shapeshifter(Gene thisGene)
		{
			gene = thisGene;
			currentXeno = thisGene?.pawn?.genes?.Xenotype != null ? thisGene.pawn.genes.Xenotype : null;
			// selectedXeno = currentXeno;
			// connectedPawn = xenoTree.ConnectedPawn;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
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
			if (gene.pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
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
			gene.pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(gene.pawn);
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			if (gene.pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return false;
			}
			return true;
		}

	}

}
