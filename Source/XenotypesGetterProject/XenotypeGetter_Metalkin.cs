using RimWorld;
using Verse;
using Verse.Noise;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeGetter_Metalkin : XenotypeGetter
	{

		public override bool CanFire()
		{
			if (ModsConfig.OdysseyActive)
			{
				if (Find.CurrentGravship != null)
				{
					return true;
				}
			}
			if (Faction.OfPlayerSilentFail?.def.techLevel == TechLevel.Neolithic || Faction.OfPlayerSilentFail?.def.techLevel == TechLevel.Medieval || Faction.OfPlayerSilentFail?.def.techLevel == TechLevel.Animal)
			{
				return false;
			}
			if (!CommsConsoleUtility.PlayerHasPoweredCommsConsole())
			{
				return false;
			}
			//if (Find.Maps.Any(map => map.mapPawns.AllPawns.Any(p => p.genes?.Xenotype?.IsXenoGenesDef() == true)))
			//{
			//	return false;
			//}
			return true;
		}

		public override float Chance()
		{
			if (Faction.OfMechanoids?.deactivated == true)
			{
				return 3;
			}
			return base.Chance();
		}

	}

}
