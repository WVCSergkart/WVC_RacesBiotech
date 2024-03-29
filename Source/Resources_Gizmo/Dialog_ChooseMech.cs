using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_ChooseMech : Dialog_XenotypesBase
	{
		public Gene gene;

		public GeneExtension_XenotypeGestator geneExtension;

		public HediffDef hediffDefName;
		public HediffDef cooldownHediffDef;
		public float matchPercent;
		// public int minimumDays;
		public int gestationPeriodDays;
		public float xenotypeComplexityFactor;
		public int cooldownDays;

		public List<XenotypeDef> allMatchedXenotypes;

		private readonly int ticksInDay = 60000;

		public Dialog_ChooseMech(Gene thisGene)
		{
			gene = thisGene;
			geneExtension = gene?.def?.GetModExtension<GeneExtension_XenotypeGestator>();
			hediffDefName = geneExtension?.gestationHediffDef;
			cooldownHediffDef = geneExtension?.cooldownHediffDef;
			matchPercent = geneExtension == null ? 1f : geneExtension.matchPercent;
			// minimumDays = geneExtension == null ? 3 : geneExtension.minimumDays;
			// gestationPeriodFactor = geneExtension == null ? 1f : geneExtension.gestationPeriodFactor;
			xenotypeComplexityFactor = geneExtension == null ? 0.1f : geneExtension.xenotypeComplexityFactor;
			cooldownDays = geneExtension == null ? 5 : geneExtension.cooldownDays;
			gestationPeriodDays = (int)(gene.pawn.RaceProps.gestationPeriodDays * (geneExtension == null ? 1f : geneExtension.gestationPeriodFactor));
			// currentXeno = xenoTree.chosenXenotype;
			// selectedXeno = currentXeno;
			// connectedPawn = xenoTree.ConnectedPawn;
			forcePause = true;
			closeOnAccept = false;
			doCloseX = true;
			doCloseButton = true;
			allXenotypes = XenotypeFilterUtility.AllXenotypesExceptAndroids();
			allMatchedXenotypes = XaG_GeneUtility.GetAllMatchedXenotypes(gene?.pawn, allXenotypes, matchPercent);
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
				Widgets.Label(rect3, "WVC_XaG_GeneXenoGestator_Desc".Translate());
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
			Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneXenoGestator_GestationTime".Translate((GetGestationTime() * ticksInDay).ToStringTicksToPeriod()).Resolve());
			curY += 10f;
			if (!allMatchedXenotypes.Contains(selectedXeno))
			{
				Widgets.Label(rect3.x, ref curY, rect3.width, "WVC_XaG_GeneXenoGestator_GestationGenesMatch".Translate((matchPercent * 100).ToString()).Colorize(ColorLibrary.RedReadable));
				curY += 10f;
			}
			if (MeetsRequirements(selectedXeno))
			{
				if (Widgets.ButtonText(rect4, "Accept".Translate()))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneXenoGestator_GestationWarning".Translate(gene.pawn.LabelCap), delegate
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

		private int GetGestationTime()
		{
			return (int)((XaG_GeneUtility.GetXenotype_Cpx(selectedXeno) * xenotypeComplexityFactor) + gestationPeriodDays);
		}

		public override void StartChange()
		{
			// xenoTree.chosenXenotype = selectedXeno;
			// xenoTree.changeCooldown = Find.TickManager.TicksGame + xenoTree.Props.xenotypeChangeCooldown;
			// SoundDefOf.GauranlenProductionModeSet.PlayOneShotOnCamera();
			//SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			// SoundDefOf.MechGestatorCycle_Started.PlayOneShot(SoundInfo.OnCamera());
			SoundDefOf.MechGestatorCycle_Started.PlayOneShot(new TargetInfo(gene.pawn));
			// HediffDef hediffDef = gene?.def?.GetModExtension<GeneExtension_Giver>()?.hediffDefName;
			// Log.Error(hediffDef.LabelCap);
			if (hediffDefName != null && !gene.pawn.health.hediffSet.HasHediff(hediffDefName))
			{
				// Log.Error("1");
				Hediff hediff = HediffMaker.MakeHediff(hediffDefName, gene.pawn);
				HediffComp_XenotypeGestator xeno_Gestator = hediff.TryGetComp<HediffComp_XenotypeGestator>();
				// Log.Error("2");
				if (xeno_Gestator != null)
				{
					xeno_Gestator.xenotypeDef = selectedXeno;
					xeno_Gestator.gestationIntervalDays = GetGestationTime();
				}
				HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
				if (hediff_GeneCheck != null)
				{
					hediff_GeneCheck.geneDef = gene.def;
				}
				// Log.Error("3");
				gene.pawn.health.AddHediff(hediff);
				// Log.Error("4");
				if (cooldownHediffDef != null)
				{
					// if (gene is Gene_XenotypeGestator geneGestator)
					// {
						// geneGestator.cooldownHediffDef = cooldownHediffDef;
					// }
					Hediff cooldownHediff = HediffMaker.MakeHediff(cooldownHediffDef, gene.pawn);
					HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
					if (hediffComp_Disappears != null)
					{
						hediffComp_Disappears.ticksToDisappear = ticksInDay * (cooldownDays + GetGestationTime());
					}
					HediffComp_RemoveIfGeneIsNotActive cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (cooldownHediff_GeneCheck != null)
					{
						cooldownHediff_GeneCheck.geneDef = gene.def;
					}
					gene.pawn.health.AddHediff(cooldownHediff);
				}
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneXenoGestator_GestatorIsOnLetterLabel".Translate(), "WVC_XaG_GeneXenoGestator_GestatorIsOnLetterDesc".Translate(gene.pawn.Named("TARGET")), LetterDefOf.NeutralEvent, new LookTargets(gene.pawn));
			}
			Close(doCloseSound: false);
		}

		public override bool MeetsRequirements(XenotypeDef mode)
		{
			if (DebugSettings.ShowDevGizmos)
			{
				return true;
			}
			// if (XaG_GeneUtility.GenesIsMatch(gene?.pawn?.genes?.GenesListForReading, mode.genes, matchPercent))
			// {
				// return true;
			// }
			if (allMatchedXenotypes.Contains(mode))
			{
				return true;
			}
			// if (Find.TickManager.TicksGame < xenoTree.changeCooldown)
			// {
				// return false;
			// }
			// if (XenoTreeUtility.XenoTree_CanSpawn(mode, xenoTree.parent))
			// {
				// return true;
			// }
			return false;
		}

	}

}
