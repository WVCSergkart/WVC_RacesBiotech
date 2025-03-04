using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class RitualOutcomeComp_PawnGenes : RitualOutcomeComp_QualitySingleOffset
	{

		[NoTranslate]
		public string roleId;

		// [Obsolete]
		// public List<GeneDef> anyGeneDefs = new();

		// public float quality = 0f;

		// [Unsaved(false)]
		// private float? cachedOffsetFromGenes;

		// public float OffsetFromGenes(Pawn pawn)
		// {
			// if (!cachedOffsetFromGenes.HasValue)
			// {
				// cachedOffsetFromGenes = GetBirthQualityOffsetFromGenes(pawn);
			// }
			// return cachedOffsetFromGenes.Value;
		// }

		public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
		{
			Pawn pawn = ritual?.PawnWithRole(roleId);
			if (pawn == null)
			{
				return 0f;
			}
			//Log.Error("QualityOffset");
			return GetBirthQualityOffsetFromGenes(pawn);
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
			float offset = GetBirthQualityOffsetFromGenes(pawn);
			//Log.Error("GetDesc");
			string text = ((offset < 0f) ? "" : "+");
			return LabelForDesc.Formatted(pawn.Named("PAWN")) + ": " + "OutcomeBonusDesc_QualitySingleOffset".Translate(text + offset.ToStringPercent()) + ".";
		}

		// public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
		// {
		// return null;
		// }

		private int nextTick = 0;
		private float offset = 0;

		public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
		{
			Pawn pawn = assignments.FirstAssignedPawn(roleId);
			if (pawn == null)
			{
				return null;
			}
			//float offset = 0;
			nextTick--;
			if (nextTick < 0)
			{
				offset = GetBirthQualityOffsetFromGenes(pawn);
				nextTick = 120;
			}
			if (offset == 0f)
			{
				return null;
			}
			return new QualityFactor
			{
				label = label.Formatted(pawn.Named("PAWN")),
				// count = "1",
				qualityChange = ExpectedOffsetDesc(offset > 0f, offset),
				positive = (offset > 0f),
				present = true,
				quality = offset,
				priority = 0f
			};
		}

		public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
		{
			return 1f;
		}

		public override bool Applies(LordJob_Ritual ritual)
		{
			// Pawn pawn = ritual?.PawnWithRole(roleId);
			// if (XaG_GeneUtility.HasAnyActiveGene(anyGeneDefs, pawn))
			// {
				// return true;
			// }
			return true;
		}

		public static float GetBirthQualityOffsetFromGenes(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return 0f;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			float offest = 0f;
			for (int j = 0; j < genesListForReading.Count; j++)
			{
				Gene gene = genesListForReading[j];
				if (!gene.Active)
				{
					continue;
				}
				GeneExtension_General general = gene.def?.GetModExtension<GeneExtension_General>();
				if (general == null)
				{
					continue;
				}
				offest += general.birthQualityOffset;
			}
			//Log.Error("GetBirthQualityOffsetFromGenes");
			return offest;
		}

	}

}
