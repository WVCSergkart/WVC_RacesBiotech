using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompAbilityEffect_GenesComboImplanter : CompAbilityEffect
	{

		private static List<GeneDef> cachedHybridGenes;
		public static List<GeneDef> HybridGenes
		{
			get
			{
				if (cachedHybridGenes == null)
				{
					List<GeneDef> geneDefs = new();
					foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
					{
						if (geneDef.IsGeneDefOfType<Gene_HybridImplanter>() && !geneDefs.Contains(geneDef))
						{
							geneDefs.Add(geneDef);
						}
					}
					cachedHybridGenes = geneDefs;
				}
				return cachedHybridGenes;
			}
		}

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_HybridImplanter cachedGene;
		public Gene_HybridImplanter HybridGene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_HybridImplanter>();
				}
				return cachedGene;
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (HybridGene == null)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_Implanter_MissingGeneMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(target.Pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(target.Pawn.Named("PAWN")) + "\n\n" + "WVC_XaG_HybridImplanter_Warning".Translate(parent.pawn).Colorize(ColorLibrary.Gold), confirmAction, destructive: true);
			}
			return Dialog_MessageBox.CreateConfirmation("WVC_XaG_HybridImplanter_Warning".Translate(parent.pawn), confirmAction, destructive: true);
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

		public virtual void PostImplant(Pawn victim, Pawn caster, List<GeneDef> firstGenes, List<GeneDef> secondGenes)
		{
			if (Props.xenotypeDef != null && (caster.genes.Xenotype is not DevXenotypeDef hybrid || !hybrid.isHybrid))
			{
				ReimplanterUtility.SetXenotypeDirect(null, caster, Props.xenotypeDef);
			}
			ReimplanterUtility.UpdateXenogermReplication_WithComa(caster);
			ReimplanterUtility.ExtractXenogerm(victim);
			ReimplanterUtility.FleckAndLetter(victim, caster);
			//Gene.SetXenotypes(caster.genes.Xenotype, victim.genes.Xenotype);
		}

	}

	public class CompAbilityEffect_HybridImplanter : CompAbilityEffect_GenesComboImplanter
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn victim = target.Pawn;
			if (victim != null)
			{
				Pawn caster = parent.pawn;
				List<GeneDef> ignoredGenes = (HybridGene.IsEndogene ? caster.genes.Endogenes : caster.genes.Xenogenes).ConvertToDefs();
				ignoredGenes.AddRangeSafe(HybridGenes);
				//if (!ignoredGenes.Contains(HybridGene.def))
				//{
				//	ignoredGenes.Add(HybridGene.def);
				//}
				if (SubXenotypeUtility.TrySetHybridXenotype(caster, victim, ignoredGenes, false))
				{
					PostImplant(victim, caster, null, null);
				}
				else
				{
					Messages.Message("WVC_XaG_HybridImplanterFail".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
		}
	}

	public class CompAbilityEffect_MergeImplanter : CompAbilityEffect_GenesComboImplanter
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn victim = target.Pawn;
			if (victim != null)
			{
				Pawn caster = parent.pawn;
				List<GeneDef> firstXenotypeGenes = XaG_GeneUtility.ConvertToDefs(caster.genes.GenesListForReading);
				List<GeneDef> secondXenotypeGenes = XaG_GeneUtility.ConvertToDefs(victim.genes.GenesListForReading);
				if (SubXenotypeUtility.TrySetHybridXenotype(caster, HybridGenes, firstXenotypeGenes, secondXenotypeGenes, true, new()))
				{
					PostImplant(victim, caster, firstXenotypeGenes, secondXenotypeGenes);
				}
				else
				{
					Messages.Message("WVC_XaG_HybridImplanterFail".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
		}

		public override void PostImplant(Pawn victim, Pawn caster, List<GeneDef> firstGenes, List<GeneDef> secondGenes)
		{
			if (SubXenotypeUtility.TrySetHybridXenotype(victim, HybridGenes, firstGenes, secondGenes, true, new()))
			{
				ReimplanterUtility.SetXenotypeDirect(caster, victim);
				ReimplanterUtility.ExtractXenogerm(caster);
				ReimplanterUtility.ExtractXenogerm(victim);
				ReimplanterUtility.UpdateXenogermReplication_WithComa(caster);
				ReimplanterUtility.UpdateXenogermReplication_WithComa(victim);
			}
			ReimplanterUtility.FleckAndLetter(victim, caster);
		}

	}

}
