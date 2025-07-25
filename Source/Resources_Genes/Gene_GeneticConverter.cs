using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_GeneticConverter : Gene_ChimeraDependant
    {

        private int nextTick = 59109;

        public override void TickInterval(int delta)
        {
            if (!GeneResourceUtility.CanTick(ref nextTick, 59109, delta))
            {
                return;
            }
            HarvestAndConvert();
        }

        public void HarvestAndConvert()
        {
            List<Pawn> pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where((victim) => victim != pawn && victim.IsHuman() && (victim.HomeFaction == pawn.Faction || victim.IsPrisoner) && !XaG_GeneUtility.ConflictWith(def, victim.genes.GenesListForReading)).ToList();
            foreach (Pawn item in pawns)
            {
                if (IsMatch(item))
                {
                    continue;
                }
                if (HarvestAndReplaceRandomGene(item))
                {
                    //ReimplanterUtility.UnknownXenotype(pawn);
                    if (IsMatch(item))
                    {
                        SetXenotype(item);
                    }
                    else
                    {
                        ReimplanterUtility.TrySetSkinAndHairGenes(item);
                        ReimplanterUtility.PostImplantDebug(item);
                    }
                    break;
                }
                else
                {
                    SetXenotype(item);
                    break;
                }
            }

            bool IsMatch(Pawn item)
            {
                return XaG_GeneUtility.GenesIsMatch(item.genes.Endogenes.Where((gene) => gene.def != this.def).ToList(), pawn.genes.Endogenes.Where((g) => g.def != this.def && g.def.passOnDirectly).ToList().ConvertToDefs(), 1f);
            }

            void SetXenotype(Pawn item)
            {
                SaveableXenotypeHolder newHolder = new(pawn.genes.Xenotype, pawn.genes.Endogenes.ConvertToDefs(), true, pawn.genes.iconDef, pawn.genes.xenotypeName);
                ReimplanterUtility.SetXenotype(item, newHolder);
                MiscUtility.DoShapeshiftEffects_OnPawn(item);
            }
        }

        public bool HarvestAndReplaceRandomGene(Pawn target)
        {
            string phase = "start";
            try
            {
                phase = "get gene";
                List<GeneDef> allowedGenes = target.genes.GenesListForReading.Where((gene) => !XaG_GeneUtility.HasEndogene(gene.def, pawn)).ToList().ConvertToDefs();
                if (Chimera.TryGetGene(allowedGenes, out GeneDef result))
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
                    Gene gene = target.genes.GetGene(result);
                    if (!target.genes.IsXenogene(gene))
                    {
                        phase = "try add and remove gene";
                        target.genes.RemoveGene(gene);
                        AddNewGene(target);
                    }
                    return true;
                }
                else
                {
                    phase = "try add gene";
                    if (AddNewGene(target) && target.genes.Endogenes.Where((gene) => !XaG_GeneUtility.HasEndogene(gene.def, pawn)).TryRandomElement(out Gene oldGene))
                    {
                        target.genes.RemoveGene(oldGene);
                        return true;
                    }
                    else
                    {
                        phase = "try remove gene";
                        if (!target.genes.Xenogenes.NullOrEmpty() && target.genes.Xenogenes.TryRandomElement(out Gene gene))
                        {
                            target.genes.RemoveGene(gene);
                            return true;
                        }
                    }
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed harvest and replace on phase: " + phase + ". Reason: " + arg);
            }
            return false;

            bool AddNewGene(Pawn target)
            {
                Gene newGene = pawn.genes.Endogenes.Where((g) => g.def != this.def && g.def.passOnDirectly && !XaG_GeneUtility.HasEndogene(g.def, target))?.RandomElement();
                if (newGene != null)
                {
                    target.genes.AddGene(newGene.def, false);
                    Messages.Message("WVC_XaG_Chimera_GeneImplanted".Translate(newGene.def.label), target, MessageTypeDefOf.NeutralEvent, historical: false);
                }
                return newGene != null;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: HarvestAndConvert",
                    action = delegate
                    {
                        HarvestAndConvert();
                    }
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 59109);
        }

    }


}
