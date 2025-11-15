using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompAbilityEffect_HivemindAssimilator : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Pawn victim = target.Pawn;
			if (victim != null)
			{
				AddHivemindGenes(victim);
			}
		}

		public void AddHivemindGenes(Pawn pawn)
		{
			foreach (GeneDef item in Props.geneDefs)
			{
				pawn.genes?.AddGene(item, true);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (base.Valid(target, throwMessages))
			{
				Pawn victim = target.Pawn;
				if (IsHivemindDrone(victim))
				{
					if (throwMessages)
					{
						Messages.Message("WVC_XaG_HivemindAssimilator_PawnIsDrone".Translate(), victim, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, false);
			}
			return false;
		}

		private static bool IsHivemindDrone(Pawn victim)
		{
			return victim.genes.GenesListForReading.Any((gene) => HivemindUtility.IsHivemindGene(gene));
		}

	}

}
