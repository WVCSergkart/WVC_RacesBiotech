using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ChimeraNetwork : Gene_ChimeraDependant, IGeneCustomChimeraEater
    {

        public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

        public string ChimeraEater_Name => "WVC_Share";
        public TaggedString ChimeraEater_Desc => "WVC_XaG_GeneChimeraNetwork_Desc";

        public void ChimeraEater(ref List<GeneDef> selectedGenes)
        {
            foreach (GeneDef geneDef in selectedGenes)
            {
                Chimera.RemoveCollectedGene(geneDef);
                try
                {
                    if (Rand.Chance(0.12f))
                    {
                        Gene_Chimera.TryGetUniqueGene(Chimera, pawn, Props.chimeraConditionalGenes);
                    }
                }
                catch (Exception arg)
                {
                    Log.Error("Failed obtaine gene. Reason: " + arg);
                }
            }
        }

        //public override bool BlockChimeraEat => true;

        public override void TickInterval(int delta)
        {
            //base.TickInterval(delta);
            if (!pawn.IsHashIntervalTick(69167, delta))
            {
                return;
            }
            if (!pawn.Faction.IsPlayer)
            {
                return;
            }
            ShareGenes();
        }

        public void ShareGenes()
        {
            List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists?.Where((target) => target != pawn && target.IsHuman() && XaG_GeneUtility.HasActiveGene(def, target))?.ToList();
            if (pawns.NullOrEmpty())
            {
                return;
            }
            foreach (Pawn item in pawns)
            {
                Gene_ChimeraNetwork network = item.genes.GetFirstGeneOfType<Gene_ChimeraNetwork>();
                if (network == null)
                {
                    continue;
                }
                if (Chimera.TryGetGene(network.Chimera.CollectedGenes, out GeneDef result))
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
                }
                else if (network.Chimera.TryGetGene(Chimera.CollectedGenes, out GeneDef result2))
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(item.NameShortColored, result2.label), item, MessageTypeDefOf.NeutralEvent, historical: false);
                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: ShareGenes",
                    action = delegate
                    {
                        ShareGenes();
                    }
                };
            }
        }
    }

}
