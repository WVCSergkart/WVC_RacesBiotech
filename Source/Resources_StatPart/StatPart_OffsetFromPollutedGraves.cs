// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromPollutedGraves : StatPart
	{
		// public string buildingDefName = "NatureShrine";

		public string label = "WVC_XaG_StatPart_OffsetFromPollutedGraves";

		// public float buildingWeightFactor = 0.2f;

		// public List<BuildingBandwidthWeight> buildings;

		// [MustTranslate]
		// private string label;

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
			// if (req.HasThing && req.Thing is Pawn pawn && pawn.GetOverseer() != null)
			// {
			// return "WVC_StatPart_GenesCombo_Static".Translate() + ": x" + pawn.GetOverseer().GetStatValue(stat);
			// }
			// return null;
		}

		private bool TryGetOffset(StatRequest req, out float offset)
		{
			offset = 0f;
			if (ModsConfig.BiotechActive && req.HasThing && req.Thing is Pawn pawn && pawn.Map != null && MechanitorUtility.IsMechanitor(pawn))
			{
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				if (allBuildingsColonist.NullOrEmpty())
				{
					return false;
				}
				foreach (Building item in allBuildingsColonist)
				{
					if (item is Building_Grave building_Grave && building_Grave.HasCorpse && item.Position.IsPolluted(item.Map))
					{
						offset += 1f;
					}
				}
				return true;
			}
			return false;
		}
	}
}
