using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class RitualOutcomeComp_PawnGenes : RitualOutcomeComp_QualitySingleOffset
	{

		[NoTranslate]
		public string roleId;

		public List<GeneDef> anyGeneDefs = new();

		public float quality = 0f;

		public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
		{
			Pawn pawn = ritual?.PawnWithRole(roleId);
			if (pawn == null)
			{
				return 0f;
			}
			if (XaG_GeneUtility.HasAnyActiveGene(anyGeneDefs, pawn))
			{
				return quality;
			}
			return 0f;
		}

		public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
		{
			if (ritual == null)
			{
				return labelAbstract;
			}
			Pawn pawn = ritual?.PawnWithRole(roleId);
			if (pawn == null)
			{
				return null;
			}
			float num = quality;
			string text = ((num < 0f) ? "" : "+");
			return LabelForDesc.Formatted(pawn.Named("PAWN")) + ": " + "OutcomeBonusDesc_QualitySingleOffset".Translate(text + num.ToStringPercent()) + ".";
		}

		public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
		{
			Pawn pawn = assignments.FirstAssignedPawn(roleId);
			if (pawn == null)
			{
				return null;
			}
			if (!XaG_GeneUtility.HasAnyActiveGene(anyGeneDefs, pawn))
			{
				return null;
			}
			float num = quality;
			return new QualityFactor
			{
				label = label.Formatted(pawn.Named("PAWN")),
				// count = "1",
				qualityChange = ExpectedOffsetDesc(num > 0f, num),
				positive = (num > 0f),
				present = true,
				quality = num,
				priority = 0f
			};
		}

		public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
		{
			return 1f;
		}

		public override bool Applies(LordJob_Ritual ritual)
		{
			return true;
		}

	}

}
