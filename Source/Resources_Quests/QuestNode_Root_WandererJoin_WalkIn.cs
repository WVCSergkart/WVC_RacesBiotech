using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class QuestNode_Root_WandererJoin_WalkIn : RimWorld.QuestGen.QuestNode_Root_WandererJoin_WalkIn
	{

		public XenotypeDef xenotypeDef;

		public override Pawn GeneratePawn()
		{
			Pawn pawn = base.GeneratePawn();
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotypeDef ?? GetXenotype() ?? XenotypeDefOf.Baseliner);
			return pawn;
		}

		public static XenotypeDef GetXenotype()
		{
			XenotypeDef xenotypeDef = null;
			List<XenotypeGetterDef> xenotypeGetterDefs = ListsUtility.XenotypeGetterDefs;
			xenotypeGetterDefs.Shuffle();
			foreach (XenotypeGetterDef xenotypeGetterDef in xenotypeGetterDefs)
			{
				if (xenotypeDef != null)
				{
					break;
				}
				if (xenotypeGetterDef.Worker.CanFire())
				{
					xenotypeDef = xenotypeGetterDef.Worker.GetXenotype().xenotypeDef;
				}
			}
			return xenotypeDef;
		}

	}

}
