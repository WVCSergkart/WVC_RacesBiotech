// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class StatPart_ForceTo : StatPart
	{
		public StatDef statDefName;

		private bool ActiveFor(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return false;
			}
			if (statDefName != null)
			{
				return true;
			}
			return false;
		}

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (req.HasThing && req.Thing is Pawn pawn && ActiveFor(pawn))
			{
				if (pawn.GetStatValue(statDefName) != 0f)
				{
					val = pawn.GetStatValue(statDefName);
				}
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			if (req.HasThing && req.Thing is Pawn pawn && ActiveFor(pawn))
			{
				return "WVC_StatPart_GenesCombo_Static".Translate() + ": " + pawn.GetStatValue(statDefName);
			}
			return null;
		}
	}

}
