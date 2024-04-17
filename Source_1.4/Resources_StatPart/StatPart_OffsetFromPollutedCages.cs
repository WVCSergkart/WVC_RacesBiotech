// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromPollutedCages : StatPart
	{

		public string label = "WVC_XaG_StatPart_OffsetFromPollutedCages";

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
			if (ModsConfig.BiotechActive && req.HasThing && req.Thing is Pawn pawn && pawn.Map != null && MechanitorUtility.IsMechanitor(pawn))
			{
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				if (allBuildingsColonist.NullOrEmpty())
				{
					return false;
				}
				foreach (Building item in allBuildingsColonist)
				{
					if (item is Building_GibbetCage building_GibbetCage && building_GibbetCage.HasCorpse && item.Position.IsPolluted(item.Map))
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
