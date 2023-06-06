// RimWorld.StatPart_Age
using System.Collections.Generic;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class StatPart_FactorForGolems : StatPart
	{
		// public string pawnKindDef;

		public StatDef stat;

		// [MustTranslate]
		// private string label;

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (req.HasThing && req.Thing is Pawn pawn && MechanoidizationUtility.PawnIsGolem(pawn))
			{
				if (TryGetFactor(req, out var factor))
				{
					val *= factor;
				}
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			// if (TryGetFactor(req, out var factor) && factor != 0f)
			// {
				// return label + ": +" + factor.ToStringPercent();
			// }
			// return null;
			if (req.HasThing && req.Thing is Pawn pawn && pawn.GetOverseer() != null && MechanoidizationUtility.PawnIsGolem(pawn))
			{
				return "WVC_StatPart_GenesCombo_Static".Translate() + ": x" + pawn.GetOverseer().GetStatValue(stat);
			}
			return null;
		}

		private bool TryGetFactor(StatRequest req, out float factor)
		{
			if (ModsConfig.BiotechActive && req.HasThing && req.Thing is Pawn pawn)
			{
				Pawn overseer = pawn.GetOverseer();
				if (overseer != null)
				{
					factor = overseer.GetStatValue(stat);
					return true;
				}
			}
			factor = 0f;
			return false;
		}
	}
}
