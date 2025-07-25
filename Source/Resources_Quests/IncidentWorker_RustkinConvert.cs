using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class IncidentWorker_RustkinConvert : RimWorld.IncidentWorker
	{

		public override float ChanceFactorNow(IIncidentTarget target)
		{
			return 1f + (StaticCollectionsClass.cachedColonistsCount * 0.35f);
		}

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            XenotypeDef xenotypeDef = DefDatabase<XenotypeDef>.GetNamed("WVC_GeneThrower");
            if (PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists.Where((colonist) => colonist.genes.Xenotype == xenotypeDef).ToList().Count > 5)
            {
                return base.CanFireNowSub(parms);
            }
            return false;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			XenotypeDef xenotypeDef = DefDatabase<XenotypeDef>.GetNamed("WVC_GeneThrower");
			List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists.Where((colonist) => colonist.genes.Xenotype == xenotypeDef).ToList();
            Pawn pawn = pawns.RandomElement();
            ReimplanterUtility.SetXenotype(pawn, DefDatabase<XenotypeDef>.GetNamed("WVC_Rustkind"));
			Find.LetterStack.ReceiveLetter("WVC_XaG_MetalkinConvertIncident".Translate(), "WVC_XaG_MetalkinConvertIncidentDesc".Translate(), LetterDefOf.NegativeEvent, new LookTargets(pawn));
			MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
			return true;
		}

	}

}
