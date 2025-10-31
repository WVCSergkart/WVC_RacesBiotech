using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityChimera : CompProperties_AbilityEffect
	{

		public ThoughtDef thoughtDefToGiveTarget;

		public ThoughtDef opinionThoughtDefToGiveTarget;

		public ThoughtDef allOtherPawnsAboutMe;

		public ThoughtDef allOtherPawns;

		public HediffDef hediffDef;

		// public float resistanceGain;

		// public float nutritionGain = 0.2f;

		// public float targetBloodLoss = 0.03f;

		// public IntRange bloodFilthToSpawnRange = new(1, 1);

		public CompProperties_AbilityChimera()
		{
			compClass = typeof(CompAbilityEffect_CopyGene);
		}

	}

	public class CompAbilityEffect_ChimeraDependant : CompAbilityEffect
	{

		public new CompProperties_AbilityChimera Props => (CompProperties_AbilityChimera)props;

		[Unsaved(false)]
		private Gene_Chimera cachedChimeraGene;

		public Gene_Chimera ChimeraGene
		{
			get
			{
				if (cachedChimeraGene == null || !cachedChimeraGene.Active)
				{
					cachedChimeraGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>();
				}
				return cachedChimeraGene;
			}
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (ChimeraGene == null)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_GeneChimera_DeActive".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return base.Valid(target, throwMessages);
		}

	}

	public class CompAbilityEffect_CopyGene : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null && ChimeraGene != null)
			{
				if (ChimeraGene.TryGetGene(XaG_GeneUtility.ConvertToDefs(pawn.genes.GenesListForReading), out GeneDef result))
				{
					if (Props.opinionThoughtDefToGiveTarget != null)
					{
						pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.opinionThoughtDefToGiveTarget, parent.pawn);
					}
					if (Props.thoughtDefToGiveTarget != null)
					{
						pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.thoughtDefToGiveTarget);
					}
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(parent.pawn.NameShortColored, result.label), parent.pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		//public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//	return Valid(target);
		//}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			//Pawn pawn = target.Pawn;
			//if (pawn == null)
			//{
			//	return false;
			//}
			//if (!pawn.IsHuman())
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			//if (pawn.genes.GenesListForReading.Where((gene) => !ChimeraGene.AllGenes.Contains(gene.def)).ToList().Count <= 0)
			//{
			//    if (throwMessages)
			//    {
			//        Messages.Message("WVC_XaG_GeneChimera_TargetNoGenes".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
			//    }
			//    return false;
			//}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, false) && base.Valid(target, throwMessages);
		}

		public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				string text = null;
				if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
				{
					text += "MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY"));
				}
				return text;
			}
			return null;
		}

	}

	public class CompAbilityEffect_CopyGeneFromGenepack : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			if (target.HasThing && target.Thing is Genepack genepack && ChimeraGene != null)
			{
				if (ChimeraGene.TryGetGene(genepack.GeneSet.GenesListForReading, out GeneDef result))
				{
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(parent.pawn.NameShortColored, result.label), parent.pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				genepack.Kill();
			}
		}

		//public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//	return Valid(target);
		//}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (!target.HasThing || target.Thing is not Genepack)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_MustTargetGenepack".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			//if (genepack.GeneSet.GenesListForReading.Where((gene) => !ChimeraGene.AllGenes.Contains(gene)).ToList().Count <= 0)
			//{
			//    if (throwMessages)
			//    {
			//        Messages.Message("WVC_XaG_GeneChimera_TargetNoGenes".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
			//    }
			//    return false;
			//}
			return base.Valid(target, throwMessages);
		}

	}

	public class CompAbilityEffect_CopyGenesFromXenogerm : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			if (target.HasThing && target.Thing is Xenogerm xenogerm && ChimeraGene != null)
			{
				if (ChimeraGene.TryAddGenesFromList(xenogerm.GeneSet.GenesListForReading))
				{
					Messages.Message("WVC_XaG_GenesCopied".Translate(), parent.pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				xenogerm.Kill();
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (!target.HasThing || target.Thing is not Xenogerm)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_MustTargetXenogerm".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return base.Valid(target, throwMessages);
		}

	}

	public class CompAbilityEffect_HarvestGenesFromPawn : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null && ChimeraGene != null)
			{
				if (ChimeraGene.TryAddGenesFromList(pawn.genes.GenesListForReading))
				{
					ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
					ReimplanterUtility.ExtractXenogerm(pawn);
					if (Props.opinionThoughtDefToGiveTarget != null)
					{
						pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.opinionThoughtDefToGiveTarget, parent.pawn);
					}
					if (Props.thoughtDefToGiveTarget != null)
					{
						pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.thoughtDefToGiveTarget);
					}
					Messages.Message("WVC_XaG_GeneGeneticThief_GenesHarvested".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		//public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//    return Valid(target);
		//}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			//Pawn pawn = target.Pawn;
			//if (pawn == null)
			//{
			//    return false;
			//}
			//if (!pawn.IsHuman())
			//{
			//    if (throwMessages)
			//    {
			//        Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
			//    }
			//    return false;
			//}
			//if (pawn.genes.GenesListForReading.Where((gene) => !ChimeraGene.AllGenes.Contains(gene.def)).ToList().Count <= 0)
			//{
			//    if (throwMessages)
			//    {
			//        Messages.Message("WVC_XaG_GeneChimera_TargetNoGenes".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
			//    }
			//    return false;
			//}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, false) && base.Valid(target, throwMessages);
		}

		public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				string text = null;
				if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
				{
					text += "MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY"));
				}
				return text;
			}
			return null;
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(target.Pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WVC_XaG_WarningPawnWillDieFromHarvesting".Translate(target.Pawn.Named("PAWN")), confirmAction, destructive: true);
			}
			return null;
		}

	}

	public class CompAbilityEffect_HarvestGenesFromCorpse : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			if (innerPawn != null && ChimeraGene != null)
			{
				if (ChimeraGene.TryAddGenesFromList(innerPawn.genes.GenesListForReading))
				{
					ReimplanterUtility.SetXenotype(innerPawn, XenotypeDefOf.Baseliner);
					//ReimplanterUtility.ExtractXenogerm(innerPawn);
					Messages.Message("WVC_XaG_GeneGeneticThief_GenesHarvested".Translate(innerPawn.NameShortColored), innerPawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

		//public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//    return Valid(target);
		//}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				Pawn innerPawn = corpse.InnerPawn;
				if (!innerPawn.IsHuman())
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				//if (innerPawn.genes.GenesListForReading.Where((gene) => !ChimeraGene.AllGenes.Contains(gene.def)).ToList().Count <= 0)
				//{
				//    if (throwMessages)
				//    {
				//        Messages.Message("WVC_XaG_GeneChimera_TargetNoGenes".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
				//    }
				//    return false;
				//}
			}
			return base.Valid(target, throwMessages);
		}

	}

}
