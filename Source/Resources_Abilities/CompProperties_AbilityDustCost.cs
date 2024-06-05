using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
    public class CompProperties_AbilityPawnNutritionCost : CompProperties_AbilityEffect
	{
		public float nutritionCost = 0.2f;

		public CompProperties_AbilityPawnNutritionCost()
		{
			compClass = typeof(CompAbilityEffect_PawnNutritionCost);
		}

		public override IEnumerable<string> ExtraStatSummary()
		{
			// yield return (string)("WVC_XaG_AbilityPawnNutritionCost".Translate() + ": ") + Mathf.RoundToInt(nutritionCost * 100f);
			yield return (string)("WVC_XaG_AbilityPawnNutritionCost".Translate() + ": ") + nutritionCost;
		}
	}

	[Obsolete]
	public class CompAbilityEffect_PawnNutritionCost : CompAbilityEffect
	{
		public new CompProperties_AbilityPawnNutritionCost Props => (CompProperties_AbilityPawnNutritionCost)props;

		private Need_Food need_Food = null;

		private void Cache()
		{
			if (need_Food == null)
			{
				need_Food = parent?.pawn?.needs?.food;
			}
		}

		private bool HasEnoughDust
		{
			get
			{
				Cache();
				if (need_Food == null)
				{
					return false;
				}
				return need_Food.CurLevel >= Props.nutritionCost;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			// Need_Food need_Food = parent.pawn.needs?.food;
			// Gene_Dust gene_Dust = parent.pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			// Cache();
			// if (need_Food != null)
			// {
			// }
			UndeadUtility.OffsetNeedFood(parent.pawn, 0f - Props.nutritionCost);
		}

		public override bool GizmoDisabled(out string reason)
		{
			// Gene_Dust gene_Dust = parent.pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			// Need_Food need_Food = parent.pawn.needs?.food;
			Cache();
			if (need_Food == null)
			{
				reason = "WVC_XaG_AbilityDisabledNoDustGene".Translate(parent.pawn);
				return true;
			}
			if (need_Food != null && need_Food.CurLevel < Props.nutritionCost)
			{
				reason = "WVC_XaG_AbilityDisabledNoDust".Translate(parent.pawn);
				return true;
			}
			float num = TotalDustCostOfQueuedAbilities();
			float num2 = Props.nutritionCost + num;
			if (Props.nutritionCost > float.Epsilon && num2 > need_Food.CurLevel)
			{
				reason = "WVC_XaG_AbilityDisabledNoDust".Translate(parent.pawn);
				return true;
			}
			reason = null;
			return false;
		}

		public override bool AICanTargetNow(LocalTargetInfo target)
		{
			return HasEnoughDust;
		}

		private float TotalDustCostOfQueuedAbilities()
		{
			float num = (parent.pawn.jobs?.curJob?.verbToUse is not Verb_CastAbility verb_CastAbility) ? 0f : NutritionCost(verb_CastAbility.ability.comps);
			if (parent.pawn.jobs != null)
			{
				for (int i = 0; i < parent.pawn.jobs.jobQueue.Count; i++)
				{
					if (parent.pawn.jobs.jobQueue[i].job.verbToUse is Verb_CastAbility verb_CastAbility2)
					{
						num += NutritionCost(verb_CastAbility2.ability.comps);
					}
				}
			}
			return num;
		}

		public float NutritionCost(List<AbilityComp> comps)
		{
			if (comps != null)
			{
				foreach (AbilityComp comp in comps)
				{
					if (comp is CompAbilityEffect_PawnNutritionCost compAbilityEffect_PawnNutritionCost)
					{
						return compAbilityEffect_PawnNutritionCost.Props.nutritionCost;
					}
				}
			}
			return 0f;
		}
	}

}
