using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class IncidentWorker_MetalkinUpgrade : RimWorld.IncidentWorker
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
			if (pawns.NullOrEmpty())
            {
				return false;
            }
            Pawn pawn = pawns.RandomElement();
			//ReimplanterUtility.SetXenotype(pawn, DefDatabase<XenotypeDef>.GetNamed("WVC_Rustkind"));
			GeneExtension_Undead extension = def.GetModExtension<GeneExtension_Undead>();
			if (extension?.addedGenes != null)
            {
				foreach (GeneDef geneDef in extension.addedGenes)
                {
					pawn.genes.AddGene(geneDef, false);
                }
				ReimplanterUtility.PostImplantDebug(pawn);
            }
			else
            {
				return false;
			}
			Find.LetterStack.ReceiveLetter("WVC_XaG_MetalkinConvertIncident".Translate(), "WVC_XaG_MetalkinConvertIncidentDesc".Translate(), LetterDefOf.PositiveEvent, new LookTargets(pawn));
			MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
			return true;
		}

	}

}
