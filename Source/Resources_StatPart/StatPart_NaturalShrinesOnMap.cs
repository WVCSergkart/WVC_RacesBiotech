// RimWorld.StatPart_Age
using System.Collections.Generic;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class StatPart_OffsetFromBuildingsOnMap : StatPart
	{
		public string buildingDefName = "NatureShrine";

		public string label = "WVC_StatPart_GenesCombo_OffsetFromBuildings";

		public float buildingWeightFactor = 0.2f;

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
			if (ModsConfig.BiotechActive && req.HasThing && req.Thing is Pawn pawn && pawn.Map != null && MechanitorUtility.IsMechanitor(pawn))
			{
				// Pawn overseer = pawn.GetOverseer();
				// if (overseer != null && overseer.Map != null)
				// {
				// }
				int naturalShrinesCount = 0;
				List<Building> buildingsThing = pawn.Map.listerBuildings.allBuildingsColonist;
				foreach (Building item in buildingsThing)
				{
					if (item.def.defName.Contains(buildingDefName) && !item.def.IsFrame)
					{
						naturalShrinesCount++;
					}
				}
				offset = naturalShrinesCount * buildingWeightFactor;
				return true;
			}
			offset = 0f;
			return false;
		}
	}
}
