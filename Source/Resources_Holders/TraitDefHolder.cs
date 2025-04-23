using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class TraitDefHolder
	{

		public TraitDef traitDef;

		public TraitDegreeData traitDegreeData;

		public int? traitDegreeIndex;

		public Pawn targetPawn;

		public bool locked;

		[Unsaved(false)]
		private string cachedDescription;
		public string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.Append(traitDegreeData.description.Formatted(targetPawn.Named("PAWN")).AdjustedFor(targetPawn).Resolve());
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					cachedLabel = traitDegreeData.GetLabelFor(targetPawn);
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		public bool IsSame(TraitDef otherTrait)
		{
			if (otherTrait == traitDef)
			{
				return true;
			}
			return false;
		}

		public bool IsSame(Trait otherTrait)
		{
			if (IsSame(otherTrait.def) && otherTrait.CurrentData == traitDegreeData)
			{
				return true;
			}
			return false;
		}

		public bool IsSame(TraitDefHolder otherHolder)
		{
			if (IsSame(otherHolder.traitDef) && otherHolder.traitDegreeData == traitDegreeData)
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(TraitDef otherTrait)
		{
			if (otherTrait.ConflictsWith(traitDef))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(Trait otherTrait)
		{
			if (ConflictsWith(otherTrait.def))
			{
				return true;
			}
			return false;
		}

		public bool ConflictsWith(TraitDefHolder otherHolder)
        {
			if (otherHolder.traitDef.ConflictsWith(traitDef))
            {
				return true;
            }
            return false;
		}

		public bool ConflictsWithAny(List<TraitDefHolder> holders)
		{
			foreach (TraitDefHolder holder in holders)
            {
				if (ConflictsWith(holder))
                {
					return true;
                }
            }
			return false;
		}

		public bool CanAdd()
		{
			if (locked)
			{
				return false;
			}
			foreach (Trait trait in targetPawn.story.traits.allTraits)
			{
				if (IsSame(trait))
				{
					return false;
				}
				if (ConflictsWith(trait))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSame(List<TraitDefHolder> list)
		{
			foreach (TraitDefHolder trait in list)
			{
				if (IsSame(trait))
				{
					return true;
				}
			}
			return false;
		}

	}

}
