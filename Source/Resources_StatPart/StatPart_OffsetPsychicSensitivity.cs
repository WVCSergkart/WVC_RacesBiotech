// RimWorld.StatPart_Age
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_OffsetFromPsychicSensitivity : StatPart
	{

		public string label = "WVC_XaG_StatPart_OffsetFromPsychicSensitivity";

		public SimpleCurve curve = new();

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
			if (!req.HasThing || req.Thing is not Pawn pawn)
			{
				return false;
			}
			if (!pawn.IsPsychicSensitive())
			{
				return false;
			}
			offset = curve.Evaluate(pawn.GetStatValue(StatDefOf.PsychicSensitivity, cacheStaleAfterTicks: 30000));
			return true;
		}

	}

}
