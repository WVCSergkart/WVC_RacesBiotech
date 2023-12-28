using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Faceless : Gene
	{

		public List<HeadTypeDef> HeadTypeDefs => def.GetModExtension<GeneExtension_Giver>().headTypeDefs;

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					return HeadTypeIsCorrect(pawn);
				}
				return base.Active;
			}
		}

		public bool HeadTypeIsCorrect(Pawn pawn)
		{
			if (pawn?.genes == null || pawn?.story == null)
			{
				return false;
			}
			if (HeadTypeDefs.Contains(pawn.story.headType))
			{
				if (pawn?.health != null && pawn?.health?.hediffSet != null)
				{
					if (HasEyesGraphic(pawn) || AnyEyeIsMissing(pawn))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public bool AnyEyeIsMissing(Pawn pawn)
		{
			List<Hediff_MissingPart> missingPart = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
			for (int i = 0; i < missingPart.Count; i++)
			{
				if (missingPart[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasEyesGraphic(Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.eyeGraphicSouth != null || hediffs[i].def.eyeGraphicEast != null)
				{
					return true;
				}
			}
			return false;
		}

	}
}
