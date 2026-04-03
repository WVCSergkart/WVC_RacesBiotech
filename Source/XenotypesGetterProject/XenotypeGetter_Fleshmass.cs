using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class XenotypeGetter_Fleshmass : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (!ModsConfig.AnomalyActive)
			{
				return false;
			}
			if (Find.Anomaly.LevelDef != MonolithLevelDefOf.Embraced)
			{
				if (Rand.Chance(0.34f) && Find.Maps?.Any(map => map?.listerThings?.AllThings?.Any(thing => thing?.TryGetComp<CompObelisk_Mutator>() != null) == true) == true)
				{
					return true;
				}
				if (Find.Maps?.Any(map => map?.mapPawns?.AllPawns?.Any(pawn => pawn.kindDef == PawnKindDefOf.FleshmassNucleus) == true) == true)
				{
					return true;
				}
				return false;
			}
			return true;
		}

	}

	public class XenotypeGetter_Hivemind : XenotypeGetter_Fleshmass
	{

		public override bool CanFire(Pawn pawn)
		{
			if (pawn.Faction != null && !pawn.Faction.def.rescueesCanJoin)
			{
				return false;
			}
			return base.CanFire(pawn);
		}

		public override bool CanFire()
		{
			if (HivemindUtility.IsInitialized)
			{
				return true;
			}
			return base.CanFire();
		}

	}

}
