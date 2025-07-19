using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public static class TraitsUtility
	{

		public static void RemoveAllTraits(Pawn pawn)
		{
			foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			{
				if (trait.sourceGene == null)
				{
					trait.RemoveTrait(pawn);
				}
			}
		}

		public static void AddTraitsFromList(Pawn pawn, List<TraitDefHolder> traits)
		{
			foreach (TraitDefHolder traitDefHolder in traits)
			{
				Trait trait = new(traitDefHolder.traitDef, traitDefHolder.traitDegree.Value);
				pawn.story.traits.GainTrait(trait, true);
			}
		}

		public static void RemoveGeneTraits(Pawn pawn, Gene gene)
		{
			foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			{
				if (trait.sourceGene == gene)
				{
					trait.RemoveTrait(pawn);
				}
			}
		}

		public static void AddGeneTraits(Pawn pawn, Gene gene)
		{
			if (gene.def.forcedTraits == null)
			{
				return;
			}
			List<TraitDef> checkedList = pawn.story.traits.allTraits.ConvertToDefs();
			foreach (GeneticTraitData geneticTraitData in gene.def.forcedTraits)
			{
				if (!checkedList.Contains(geneticTraitData.def))
				{
					Trait trait = new(geneticTraitData.def, geneticTraitData.degree);
					trait.sourceGene = gene;
					pawn.story.traits.GainTrait(trait, true);
				}
			}
		}

		public static void RemoveTrait(this Trait trait, Pawn pawn)
		{
			trait.sourceGene = null;
			trait.suppressedByGene = null;
			trait.suppressedByTrait = false;
			pawn.story.traits.RemoveTrait(trait, true);
			pawn.story.traits.allTraits.Remove(trait);
		}

		public static List<TraitDef> ConvertToDefs(this List<Trait> list)
		{
			List<TraitDef> newList = new();
			foreach (Trait item in list)
			{
				newList.Add(item.def);
			}
			return newList;
		}

		public static void FixGeneTraits(Pawn pawn, List<Gene> pawnGenes)
		{
			foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			{
				if (trait.sourceGene != null && !pawnGenes.Contains(trait.sourceGene))
				{
					trait.RemoveTrait(pawn);
				}
				if (trait.suppressedByGene != null && !pawnGenes.Contains(trait.sourceGene))
				{
					trait.suppressedByGene = null;
				}
			}
			//foreach (Trait trait in pawn.story.traits.allTraits.ToList())
			//{
			//	if (trait.suppressedByTrait && !TraitHasConflicts(pawn, trait))
			//	{
			//		trait.suppressedByTrait = false;
			//	}
			//}
		}

		//      public static bool TraitHasConflicts(Pawn pawn, Trait selectedTrait)
		//{
		//	foreach (Trait trait in pawn.story.traits.allTraits.ToList())
		//	{
		//		if (selectedTrait.def.ConflictsWith(trait))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

	}

}