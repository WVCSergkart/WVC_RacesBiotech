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
		public Gene gene;

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

		public Dialog_XenotypeGestator(Gene thisGene)
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
			matchPercent = WVC_Biotech.settings.xenotypeGestator_GestationMatchPercent;
			SetMatchedHolders(gene.pawn, allXenotypes, matchPercent);
			disabled = HediffUtility.GetFirstHediffPreventsPregnancy(gene.pawn.health.hediffSet.hediffs) != null;
			OnGenesChanged();
		}

		private int GetGestationTime()
		{
			return (int)(((GetXenotype_Cpx(selectedXenoHolder) * xenotypeComplexityFactor) + gestationPeriodDays) * WVC_Biotech.settings.xenotypeGestator_GestationTimeFactor);
		}

		public static int GetXenotype_Cpx(XenotypeHolder xenotypeDef)
		{
			XaG_GeneUtility.GetBiostatsFromList(xenotypeDef.genes, out int cpx, out _, out _);
			return cpx;
		}

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

		public static void SetMatchedHolders(Pawn pawn, List<XenotypeHolder> xenotypeDefs, float percent = 0.6f)
		{
			List<Gene> pawnGenes = pawn?.genes?.GenesListForReading;
			if (pawnGenes.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return;
			}
			foreach (XenotypeHolder item in xenotypeDefs)
            {
                item.isOverriden = !GenesIsMatch(pawnGenes, item.genes, percent, out float matchPercent);
				item.matchPercent = matchPercent;
            }
        }

		private static bool GenesIsMatch(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes, float reqPercent, out float matchPercent)
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
						if (!WVC_Biotech.settings.hideXaGGenes)
						{
							matchPercent += 0.4f;
						}
						else
						{
							matchPercent += 0.05f;
						}
                    }
                }
            }
			matchPercent *= 0.01f;
			matchPercent += sameGenes / xenotypeGenes.Count;
			if (matchPercent > 1f)
            {
				matchPercent = 1f;
			}
			//List<GeneDef> matchingGenes = GetMatchingGenesList(pawnGenes, xenotypeGenes);
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
			SoundDefOf.MechGestatorCycle_Started.PlayOneShot(new TargetInfo(gene.pawn));
			if (hediffDefName != null && !gene.pawn.health.hediffSet.HasHediff(hediffDefName))
			{
				Hediff hediff = HediffMaker.MakeHediff(hediffDefName, gene.pawn);
				HediffComp_XenotypeGestator xeno_Gestator = hediff.TryGetComp<HediffComp_XenotypeGestator>();
				if (xeno_Gestator != null)
				{
					xeno_Gestator.SetupHolder(selectedXenoHolder);
					xeno_Gestator.gestationIntervalDays = GetGestationTime();
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
						hediffComp_Disappears.ticksToDisappear = ticksInDay * (cooldownDays + GetGestationTime());
					}
					HediffComp_GeneHediff cooldownHediff_GeneCheck = cooldownHediff.TryGetComp<HediffComp_GeneHediff>();
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

	}

}
