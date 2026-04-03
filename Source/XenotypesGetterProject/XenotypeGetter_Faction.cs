using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Faction : XenotypeGetter
	{

		public override bool CanFire(Pawn pawn)
		{
			return pawn.Faction?.def == def.factionDef;
		}

	}

}
