using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Hivemind_Traits : Gene_Hivemind
    {

        public override void UpdGeneSync()
        {
            try
            {
                List<TraitDefHolder> allTraitDefs = new();
                foreach (Pawn hiver in HivemindUtility.HivemindPawns)
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
                        allTraitDefs.Add(holder);
                    }
                }
                RemoveTraits();
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
