// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_PreferredExoskin_SingleThought : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return false;
			}
			if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
			{
				return false;
			}
			GeneDef geneDef = null;
			List<Precept> precept = pawn.ideo.Ideo.PreceptsListForReading;
			for (int i = 0; i < precept.Count; i++)
			{
				ThoughtExtension_General extension = precept[i].def.GetModExtension<ThoughtExtension_General>();
				if (extension != null)
				{
					geneDef = extension.geneDef;
				}
			}
			// GeneDef geneDef = def.GetModExtension<ThoughtExtension_General>().geneDef;
			if (MechanoidizationUtility.HasActiveGene(geneDef, pawn))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}

	// ===============================================================

	public class ThoughtWorker_PreferredExoskin_SingleThought_Social : ThoughtWorker_Precept_Social
	{
		protected override ThoughtState ShouldHaveThought(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
			{
				return false;
			}
			// GeneDef geneDef = def.GetModExtension<ThoughtExtension_General>().geneDef;
			GeneDef geneDef = null;
			// if (MechanoidizationUtility.TryGetGeneFromPrecept(pawn, out var gene))
			// {
				// geneDef = gene;
			// }
			List<Precept> precept = pawn.ideo.Ideo.PreceptsListForReading;
			// foreach (Precept item in precept)
			// {
				// ThoughtExtension_General extension = item.def.GetModExtension<ThoughtExtension_General>();
				// if (extension != null)
				// {
					// geneDef = extension.geneDef;
				// }
			// }
			for (int i = 0; i < precept.Count; i++)
			{
				ThoughtExtension_General extension = precept[i].def.GetModExtension<ThoughtExtension_General>();
				if (extension != null)
				{
					geneDef = extension.geneDef;
				}
			}
			// GeneDef geneDef = pawn.ideo.Ideo.PreceptsListForReading.def.GetModExtension<ThoughtExtension_General>().geneDef;
			if (MechanoidizationUtility.HasActiveGene(geneDef, other))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}
}
