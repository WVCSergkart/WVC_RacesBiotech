using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenotypeGestator : Dialog_XenotypeHolderBasic
	{
		public Gene_XenotypeGestator gene;

		public GeneExtension_Spawner geneExtension;

		public HediffDef hediffDefName;
		public HediffDef cooldownHediffDef;
		public float matchPercent;
		// public int minimumDays;
		public int gestationPeriodDays;
		public float xenotypeComplexityFactor;
		public int cooldownDays;
		//public bool canGestateAny = false;

		//public List<XenotypeDef> allMatchedXenotypes;

		private readonly int ticksInDay = 60000;

		protected override string Header => gene.LabelCap;

		public override List<XenotypeHolder> XenotypesInOrder
		{
			get
			{
				if (cachedXenotypeDefsInOrder == null)
				{
					cachedXenotypeDefsInOrder = new();
					foreach (XenotypeHolder allDef in allXenotypes)
					{
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((XenotypeHolder x) => 0f - x.displayPriority);
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		public Dialog_XenotypeGestator(Gene_XenotypeGestator thisGene)
		{
			//xenotypeName = string.Empty;
			//forcePause = true;
			//closeOnAccept = false;
			//absorbInputAroundWindow = true;
			//alwaysUseFullBiostatsTableHeight = true;
			//searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			//allXenotypes = ListsUtility.GetAllXenotypesHolders();
			gene = thisGene;
			geneExtension = gene?.def?.GetModExtension<GeneExtension_Spawner>();
			hediffDefName = geneExtension?.gestationHediffDef;
			cooldownHediffDef = geneExtension?.cooldownHediffDef;
			xenotypeComplexityFactor = geneExtension == null ? 0.1f : geneExtension.xenotypeComplexityFactor;
			cooldownDays = geneExtension == null ? 5 : geneExtension.cooldownDays;
			gestationPeriodDays = (int)(gene.pawn.RaceProps.gestationPeriodDays * (geneExtension == null ? 1f : geneExtension.gestationPeriodFactor));
			matchPercent = Mathf.Clamp(gene.ReqMatch, 0f, 1f);
			SetMatchedHolders(gene, allXenotypes, matchPercent);
			disabled = HediffUtility.GetFirstHediffPreventsPregnancy(gene.pawn.health.hediffSet.hediffs) != null;
			OnGenesChanged();
		}

        private int GestationTime
        {
            get
            {
                XaG_GeneUtility.GetBiostatsFromList(selectedXenoHolder.genes, out int cpx, out _, out _);
                return (int)(((cpx * xenotypeComplexityFactor) + gestationPeriodDays) * WVC_Biotech.settings.xenotypeGestator_GestationTimeFactor);
            }
        }

        //public static int GetXenotype_Cpx(XenotypeHolder xenotypeDef)
        //{
        //	XaG_GeneUtility.GetBiostatsFromList(xenotypeDef.genes, out int cpx, out _, out _);
        //	return cpx;
        //}

        protected override bool CanAccept()
		{
			if (disabled)
			{
				Messages.Message("WVC_XaG_Gene_SimpleGestatorFailMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			if (selectedXenoHolder.isOverriden)
			{
				Messages.Message("WVC_XaG_GeneXenoGestator_GestationGenesMatch".Translate((matchPercent * 100).ToString()), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}

		private void SetMatchedHolders(Gene_XenotypeGestator gene, List<XenotypeHolder> xenotypeDefs, float percent = 0.6f, bool useCurves = false)
        {
            List<Gene> pawnGenes = gene.GetPawnGenes();
            if (pawnGenes.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
            {
                return;
            }
            foreach (XenotypeHolder item in xenotypeDefs)
            {
                item.isOverriden = !GenesIsMatch(pawnGenes, item.genes, percent, out float matchPercent, useCurves);
                item.matchPercent = matchPercent;
            }
        }

        public static bool GenesIsMatch(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes, float reqPercent, out float matchPercent, bool useCurves = false, float geneticMaterial = 0f)
		{
			matchPercent = 0;
			if (xenotypeGenes.NullOrEmpty() || reqPercent <= 0f)
			{
				matchPercent = 1f;
				return true;
			}
			if (pawnGenes.NullOrEmpty())
			{
				return false;
			}
			List<GeneDef> pawnGeneDefs = pawnGenes.ConvertToDefs();
			int sameGenes = 0;
			foreach (GeneDef pawnGene in pawnGeneDefs)
			{
				foreach (GeneDef xenoGene in xenotypeGenes)
                {
					if (xenoGene == pawnGene)
					{
						sameGenes++;
					}
                    else if (xenoGene.ConflictsWith(pawnGene))
                    {
                        matchPercent++;
                    }
					else if (pawnGene.displayCategory == xenoGene.displayCategory)
					{
						matchPercent += 0.4f;
						//if (!WVC_Biotech.settings.hideXaGGenes)
						//{
						//}
						//else
						//{
						//	matchPercent += 0.05f;
						//}
                    }
                }
            }
			matchPercent *= 0.01f;
			if (useCurves)
			{
				XaG_GeneUtility.GetBiostatsFromList(pawnGeneDefs, out int cpx, out int met, out int arc);
				XaG_GeneUtility.GetBiostatsFromList(xenotypeGenes, out int cpxXeno, out int metXeno, out int arcXeno);
				SimpleCurve arcCurvePoints = new()
				{
					new CurvePoint(0f, 0.0f),
					new CurvePoint(arcXeno * 0.5f, 0.1f),
					new CurvePoint(arcXeno, 0f),
					new CurvePoint(arcXeno * 2f, 0.2f)
				};
				matchPercent += (float)Math.Round(arcCurvePoints.Evaluate(arc), 2);
				SimpleCurve metCurvePoints = new()
				{
					new CurvePoint(metXeno - 5f, 0.0f),
					new CurvePoint(metXeno, 0.1f),
					new CurvePoint(metXeno + 5f, 0.02f)
				};
				matchPercent += (float)Math.Round(metCurvePoints.Evaluate(met), 2);
				SimpleCurve cpxCurvePoints = new()
				{
					new CurvePoint(cpxXeno - 25f, 0.2f),
					new CurvePoint(cpxXeno, 0.1f),
					new CurvePoint(cpxXeno + 25f, 0f)
				};
				matchPercent += (float)Math.Round(cpxCurvePoints.Evaluate(cpx), 2);
			}
			matchPercent += sameGenes / xenotypeGenes.Count;
			matchPercent += geneticMaterial;
			if (matchPercent > 1f)
            {
				matchPercent = 1f;
			}
			if (matchPercent >= reqPercent)
			{
				return true;
			}
			return false;
		}

		protected override void Accept()
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneXenoGestator_GestationWarning".Translate(gene.pawn.LabelCap), StartChange));
		}

		public void StartChange()
		{
			gene.Notify_GestatorStart(selectedXenoHolder);
			SoundDefOf.MechGestatorCycle_Started.PlayOneShot(new TargetInfo(gene.pawn));
			if (hediffDefName != null && !gene.pawn.health.hediffSet.HasHediff(hediffDefName))
			{
				Hediff hediff = HediffMaker.MakeHediff(hediffDefName, gene.pawn);
				HediffComp_XenotypeGestator xeno_Gestator = hediff.TryGetComp<HediffComp_XenotypeGestator>();
				if (xeno_Gestator != null)
				{
					xeno_Gestator.SetupHolder(selectedXenoHolder);
					xeno_Gestator.gestationIntervalDays = GestationTime;
				}
				HediffComp_GeneHediff hediff_GeneCheck = hediff.TryGetComp<HediffComp_GeneHediff>();
				if (hediff_GeneCheck != null)
				{
					hediff_GeneCheck.geneDef = gene.def;
				}
				gene.pawn.health.AddHediff(hediff);
				if (cooldownHediffDef != null)
				{
					Hediff cooldownHediff = HediffMaker.MakeHediff(cooldownHediffDef, gene.pawn);
					HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
					if (hediffComp_Disappears != null)
					{
						hediffComp_Disappears.ticksToDisappear = ticksInDay * (cooldownDays + GestationTime);
					}
					HediffComp_GeneHediff cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_GeneHediff>();
					if (cooldownHediff_GeneCheck != null)
					{
						cooldownHediff_GeneCheck.geneDef = gene.def;
					}
					gene.pawn.health.AddHediff(cooldownHediff);
				}
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneXenoGestator_GestatorIsOnLetterLabel".Translate(), "WVC_XaG_GeneXenoGestator_GestatorIsOnLetterDesc".Translate(gene.pawn.Named("TARGET")), LetterDefOf.NeutralEvent, new LookTargets(gene.pawn));
				gene.Notify_GestatorPostStart(hediff);
			}
			Close(doCloseSound: false);
		}

	}

}
