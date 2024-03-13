// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromPollutedGraves : StatPart
	{

		public string label = "WVC_XaG_StatPart_OffsetFromPollutedGraves";

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
			if (MechanoidsUtility.MechanitorIsLich(pawn))
			{
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				if (allBuildingsColonist.NullOrEmpty())
				{
					return false;
				}
				foreach (Building item in allBuildingsColonist)
				{
					if (item.Position.IsPolluted(item.Map))
					{
						if (item is Building_Grave building_Grave && building_Grave.HasCorpse)
						{
							offset += 1f;
						}
						else if (item is Building_GibbetCage building_GibbetCage && building_GibbetCage.HasCorpse)
						{
							offset += 1.2f;
						}
						else if (item is Building_Skullspike)
						{
							offset += 0.2f;
						}
					}
				}
				return true;
			}
			return false;
		}

	}
}
