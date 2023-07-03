using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_ResurgentCost : CompAbilityEffect
	{
		public new CompProperties_AbilityResurgentCost Props => (CompProperties_AbilityResurgentCost)props;

		private bool HasEnoughResurgent
		{
			get
			{
				Gene_ResurgentCells gene_Resurgent = parent.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (gene_Resurgent == null && Props.skipIfNotResurgent)
				{
					return true;
				}
				if (gene_Resurgent == null || gene_Resurgent.Value < Props.resurgentCost)
				{
					return false;
				}
				return true;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Gene_ResurgentCells gene_Resurgent = parent.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				UndeadUtility.OffsetResurgentCells(parent.pawn, 0f - Props.resurgentCost);
			}
		}

		public override bool GizmoDisabled(out string reason)
		{
			Gene_ResurgentCells gene_Resurgent = parent.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent == null && Props.skipIfNotResurgent)
			{
				reason = null;
				return false;
			}
			if (gene_Resurgent == null)
			{
				reason = "WVC_XaG_AbilityDisabledNoResurgentGene".Translate(parent.pawn);
				return true;
			}
			if (gene_Resurgent.Value < Props.resurgentCost)
			{
				reason = "WVC_XaG_AbilityDisabledNoResurgent".Translate(parent.pawn);
				return true;
			}
			float num = TotalResurgentCostOfQueuedAbilities();
			float num2 = Props.resurgentCost + num;
			if (Props.resurgentCost > float.Epsilon && num2 > gene_Resurgent.Value)
			{
				reason = "WVC_XaG_AbilityDisabledNoResurgent".Translate(parent.pawn);
				return true;
			}
			reason = null;
			return false;
		}

		public override bool AICanTargetNow(LocalTargetInfo target)
		{
			return HasEnoughResurgent;
		}

		private float TotalResurgentCostOfQueuedAbilities()
		{
			float num = (parent.pawn.jobs?.curJob?.verbToUse is not Verb_CastAbility verb_CastAbility) ? 0f : ResurgentCost(verb_CastAbility.ability.comps);
			if (parent.pawn.jobs != null)
			{
				for (int i = 0; i < parent.pawn.jobs.jobQueue.Count; i++)
				{
					if (parent.pawn.jobs.jobQueue[i].job.verbToUse is Verb_CastAbility verb_CastAbility2)
					{
						num += ResurgentCost(verb_CastAbility2.ability.comps);
					}
				}
			}
			return num;
		}

		public float ResurgentCost(List<AbilityComp> comps)
		{
			if (comps != null)
			{
				foreach (AbilityComp comp in comps)
				{
					if (comp is CompAbilityEffect_ResurgentCost compAbilityEffect_ResurgentCost)
					{
						return compAbilityEffect_ResurgentCost.Props.resurgentCost;
					}
				}
			}
			return 0f;
		}
	}

}
