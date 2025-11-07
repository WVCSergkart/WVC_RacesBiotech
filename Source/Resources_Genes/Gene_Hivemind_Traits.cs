using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Hivemind_Traits : Gene_Hivemind
	{

		public override void UpdGeneSync()
		{
			try
			{
				RemoveTraits();
				List<TraitDefHolder> allTraitDefs = new();
				foreach (Pawn hiver in Hivemind)
				{
					if (hiver == this.pawn)
					{
						continue;
					}
					List<Trait> traits = hiver.story?.traits?.allTraits;
					if (traits.NullOrEmpty())
					{
						continue;
					}
					foreach (Trait trait in traits)
					{
						if (!this.pawn.CanGetTrait(trait.def))
						{
							continue;
						}
						TraitDefHolder holder = new();
						holder.traitDef = trait.def;
						holder.traitDegree = trait.Degree;
						if (allTraitDefs.Any((tra) => tra.traitDef == holder.traitDef && tra.traitDegree == holder.traitDegree))
						{
							continue;
						}
						allTraitDefs.Add(holder);
					}
				}
				TraitsUtility.AddTraitsFromList(pawn, allTraitDefs, this);
			}
			catch (Exception arg)
			{
				Log.Error("Failed gain traits from hivemind. Reason: " + arg.Message);
			}
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			base.Notify_OverriddenBy(overriddenBy);
			RemoveTraits();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveTraits();
		}

		private void RemoveTraits()
		{
			TraitsUtility.RemoveGeneTraits(pawn, this);
		}

	}

}
