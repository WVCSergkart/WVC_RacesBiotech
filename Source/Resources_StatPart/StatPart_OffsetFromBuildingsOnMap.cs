// RimWorld.StatPart_Age
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromBuildingsOnMap : StatPart
	{

		public string label = "WVC_StatPart_GenesCombo_OffsetFromBuildings";

		public GeneDef reqGeneDef;

		public List<GeneralHolder> buildings;

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (TryGetOffset(req, out var offset))
			{
				val += offset;
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			if (TryGetOffset(req, out var offset) && offset != 0f)
			{
				return label.Translate() + ": +" + offset;
			}
			return null;
		}

		public virtual bool TryGetOffset(StatRequest req, out float offset)
		{
			offset = 0f;
			if (!req.HasThing || req.Thing is not Pawn pawn || pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (XaG_GeneUtility.HasActiveGene(reqGeneDef, pawn))
			{
				List<Building> allBuildingsColonist = GetAllColonistBuildings();
				if (buildings.NullOrEmpty() || allBuildingsColonist.NullOrEmpty())
				{
					return false;
				}
				foreach (Building item in allBuildingsColonist)
				{
					foreach (GeneralHolder band in buildings)
					{
						if (band.thingDef == item.def && !item.def.IsFrame)
						{
							offset += band.bandwidth;
						}
					}
				}
				return true;
			}
			// if (XaG_GeneUtility.HasActiveGene(reqGeneDef, pawn))
			// {
			// if (buildings.NullOrEmpty())
			// {
			// return false;
			// }
			// List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			// foreach (Building item in allBuildingsColonist)
			// {
			// foreach (XaG_CountWithChance band in buildings)
			// {
			// if (band.thingDef == item.def && !item.def.IsFrame)
			// {
			// offset += band.bandwidth;
			// }
			// }
			// }
			// return true;
			// }
			return false;
		}

		public List<Building> GetAllColonistBuildings()
		{
			List<Building> allBuildingsColonist = new();
			foreach (Map map in Find.Maps)
			{
				allBuildingsColonist.AddRange(map.listerBuildings.allBuildingsColonist);
			}
			return allBuildingsColonist;
		}

	}

	// public class StatPart_OffsetFromTreesOnMap : StatPart_OffsetFromBuildingsOnMap
	// {

	// public override bool TryGetOffset(StatRequest req, out float offset)
	// {
	// offset = 0f;
	// if (!req.HasThing || req.Thing is not Pawn pawn || pawn.Faction != Faction.OfPlayer)
	// {
	// return false;
	// }
	// List<Thing> allBuildingsColonist = new();
	// if (pawn.Map != null)
	// {
	// allBuildingsColonist = pawn.Map.listerThings.AllThings;
	// }
	// else if (pawn.CarriedBy?.Map != null)
	// {
	// allBuildingsColonist = pawn.CarriedBy?.Map.listerThings.AllThings;
	// }
	// if (buildings.NullOrEmpty() || allBuildingsColonist.NullOrEmpty())
	// {
	// return false;
	// }
	// foreach (Thing item in allBuildingsColonist)
	// {
	// if (item.def.plant == null)
	// {
	// continue;
	// }
	// foreach (XaG_CountWithChance band in buildings)
	// {
	// if (band.thingDef == item.def)
	// {
	// offset += band.bandwidth;
	// }
	// }
	// }
	// return true;
	// }

	// }

	public class StatPart_OffsetFromConnectedThings : StatPart_OffsetFromBuildingsOnMap
	{

		public override bool TryGetOffset(StatRequest req, out float offset)
		{
			offset = 0f;
			if (!req.HasThing || req.Thing is not Pawn pawn || pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (buildings.NullOrEmpty())
			{
				return false;
			}
			List<Thing> allBuildingsColonist = pawn.connections.ConnectedThings;
			foreach (Thing item in allBuildingsColonist)
			{
				if (item.def.plant == null)
				{
					continue;
				}
				foreach (GeneralHolder band in buildings)
				{
					if (band.thingDef == item.def)
					{
						offset += band.bandwidth;
					}
				}
			}
			return true;
		}

	}

}
