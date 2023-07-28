using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    // public class WorkGiver_HaulToGeneBank_Base : WorkGiver_HaulToGeneBank
    // {

    // public virtual Thing FindGeneBank(Pawn pawn, Thing genepackThing)
    // {
    // Genepack genepack = genepackThing as Genepack;
    // if (genepack.targetContainer != null)
    // {
    // if (genepack.targetContainer.Map == genepack.Map)
    // {
    // CompGenepackContainer compGenepackContainer = genepack.targetContainer.TryGetComp<CompGenepackContainer>();
    // if (compGenepackContainer != null && !compGenepackContainer.Full)
    // {
    // return genepack.targetContainer;
    // }
    // }
    // return null;
    // }
    // return GenClosest.ClosestThingReachable(genepack.Position, genepack.Map, ThingRequest.ForGroup(ThingRequestGroup.GenepackHolder), PathEndMode.InteractionCell, TraverseParms.For(pawn), 9999f, delegate(Thing x)
    // {
    // if (x.IsForbidden(pawn) || !pawn.CanReserve(x))
    // {
    // return false;
    // }
    // CompGenepackContainer compGenepackContainer2 = x.TryGetComp<CompGenepackContainer>();
    // if (compGenepackContainer2 == null || compGenepackContainer2.Full || !compGenepackContainer2.autoLoad)
    // {
    // return false;
    // }
    // Thing targetContainer = genepack.targetContainer;
    // return (targetContainer == null || targetContainer == compGenepackContainer2.parent);
    // });
    // }

    // }

    public class WorkGiver_HaulToGeneBank_BasePack : WorkGiver_HaulToGeneBank
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(WVC_GenesDefOf.WVC_Genepack);
    }

    public class WorkGiver_HaulToGeneBank_UltraPack : WorkGiver_HaulToGeneBank
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(WVC_GenesDefOf.WVC_UltraGenepack);
    }

    public class WorkGiver_HaulToGeneBank_MechaPack : WorkGiver_HaulToGeneBank
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(WVC_GenesDefOf.WVC_MechaGenepack);
    }

}
