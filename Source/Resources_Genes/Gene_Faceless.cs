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
					// if (pawn?.story != null && !pawn.story.headType.defName.Contains("WVC_Faceless"))
					// {
						// return false;
					// }
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
				return true;
			}
			return false;
		}

	}
}
