using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter_Metalkin : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (Faction.OfPlayer.def.techLevel == TechLevel.Neolithic || Faction.OfPlayer.def.techLevel == TechLevel.Medieval || Faction.OfPlayer.def.techLevel == TechLevel.Animal)
			{
				return false;
			}
			if (Find.Maps.Any(map => map.mapPawns.AllPawns.Any(p => p.genes?.Xenotype?.IsXenoGenesDef() == true)))
			{
				return false;
			}
			return true;
		}

	}

}
