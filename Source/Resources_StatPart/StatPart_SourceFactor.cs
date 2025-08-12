// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class StatPart_SourceFactor : StatPart
	{

		public StatDef stat;

		[MustTranslate]
		public string label;

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (TryGetFactor(req, out var offset))
			{
				val *= offset;
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			if (TryGetFactor(req, out var offset) && offset != 0f)
			{
				return label + ": +" + offset.ToStringPercent();
			}
			return null;
		}

		private bool TryGetFactor(StatRequest req, out float offset)
		{
			if (req.HasThing && req.Thing is Pawn pawn)
			{
				Pawn overseer = pawn.GetSourceCyclic();
				if (overseer != null && overseer != pawn)
				{
					offset = overseer.GetStatValue(stat);
					return true;
				}
			}
			offset = 0f;
			return false;
		}

	}

}
