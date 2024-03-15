// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromBuildingsOnMap : StatPart
	{

		public string label = "WVC_StatPart_GenesCombo_OffsetFromBuildings";

		public GeneDef reqGeneDef;

		public List<BuildingBandwidthWeight> buildings;

		public class BuildingBandwidthWeight
		{
			public ThingDef thingDef;
			public float bandwidth = 1f;
		}

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

		private bool TryGetOffset(StatRequest req, out float offset)
		{
			offset = 0f;
			if (!req.HasThing || req.Thing is not Pawn pawn || pawn.Map == null || pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (XaG_GeneUtility.HasActiveGene(reqGeneDef, pawn))
			{
				if (buildings.NullOrEmpty())
				{
					return false;
				}
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				foreach (Building item in allBuildingsColonist)
				{
					foreach (BuildingBandwidthWeight band in buildings)
					{
						if (band.thingDef == item.def && !item.def.IsFrame)
						{
							offset += band.bandwidth;
						}
					}
				}
				return true;
			}
			return false;
		}

	}
}
