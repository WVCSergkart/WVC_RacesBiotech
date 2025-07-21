using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class QuestNode_Root_MechakinJoin_WalkIn : RimWorld.QuestGen.QuestNode_Root_WandererJoin_WalkIn
	{

		public override Pawn GeneratePawn()
		{
			Pawn pawn = base.GeneratePawn();
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, DefDatabase<XenotypeDef>.GetNamed("WVC_Meca"));
			return pawn;
		}

	}

}
