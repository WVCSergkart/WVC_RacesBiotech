using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityObsolete : CompProperties_AbilityEffect
	{

		public GeneDef geneDef;
	}

	public class CompAbilityEffect_Obsolete : CompAbilityEffect
	{
		public new CompProperties_AbilityObsolete Props => (CompProperties_AbilityObsolete)props;

		public override void CompTick()
		{
			if (Props.geneDef != null)
			{
				foreach (Gene gene in parent.pawn.genes.GenesListForReading.ToList())
				{
					if (gene.def == Props.geneDef)
					{
						bool xenogene = parent.pawn.genes.IsXenogene(gene);
						parent.pawn.genes.RemoveGene(gene);
						parent.pawn.genes.AddGene(gene.def, xenogene);
					}
				}
				if (Props.geneDef.IsGeneDefOfType<Gene_MechsSummon>())
				{
					parent.pawn.genes.GetFirstGeneOfType<Gene_MechsSummon>().nextTick = parent.CooldownTicksRemaining;
				}
			}
			parent.pawn?.abilities?.RemoveAbility(parent.def);
		}

	}

}
