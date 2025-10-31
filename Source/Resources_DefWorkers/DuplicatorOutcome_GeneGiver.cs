using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class DuplicatorOutcome_GeneGiver : DuplicatorOutcome
	{

		public override bool CanFireNow(Pawn caster, Pawn source, Pawn duplicate)
		{
			if (def.geneDefs == null || XaG_GeneUtility.HasAllGenes(def.geneDefs, duplicate))
			{
				return false;
			}
			return true;
		}

		public override void DoOutcome(Pawn caster, Pawn source, Pawn duplicate, ref string customLetter, ref LetterDef letterDef)
		{
			GeneDef finalGeneDef = null;
			foreach (GeneDef geneDef in def.geneDefs)
			{
				if (XaG_GeneUtility.HasGene(geneDef, duplicate))
				{
					continue;
				}
				if (geneDef.prerequisite != null && !XaG_GeneUtility.HasActiveGene(geneDef.prerequisite, duplicate))
				{
					continue;
				}
				finalGeneDef = geneDef;
				break;
			}
			if (finalGeneDef == null)
			{
				letterDef = LetterDefOf.PositiveEvent;
				return;
			}
			if (finalGeneDef.prerequisite != null)
			{
				duplicate.genes.AddGene(finalGeneDef, duplicate.genes.IsXenogene(duplicate.genes.GetGene(finalGeneDef.prerequisite)));
			}
			else
			{
				duplicate.genes.AddGene(finalGeneDef, true);
			}
			customLetter += "\n\n" + "WVC_XaG_GeneGiverDuplicateLetter".Translate(duplicate.Named("PAWN"), source.Named("SOURCE"), finalGeneDef.label);
		}

	}

}
