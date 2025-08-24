// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class StatPart_OffsetFromGenes : StatPart
	{

		public string label = "WVC_XaG_StatPart_OffsetFromGenes";

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
			if (!req.HasThing || req.Thing is not Pawn pawn || pawn.genes == null)
			{
				return false;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
            {
				float cost = 0;
				if (gene.def.biostatArc > 0)
				{
					cost += (500f * gene.def.biostatArc);
				}
				if (gene.def.biostatMet > 0)
				{
					cost += (50f * gene.def.biostatMet);
				}
				if (gene.def.biostatCpx > 0)
				{
					cost += (15f * gene.def.biostatCpx);
				}
				cost *= gene.def.marketValueFactor;
				offset += cost;
			}
            return true;
		}

	}

}
