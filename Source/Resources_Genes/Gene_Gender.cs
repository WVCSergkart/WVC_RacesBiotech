using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Gender : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (Overridden)
			{
				return;
			}
			ChangeGender();
		}

		public void ChangeGender()
		{
			if (pawn.gender == Props.gender)
			{
				return;
			}
			if (Props.gender != Gender.None)
			{
				pawn.gender = Props.gender;
				if (Props.gender == Gender.Female && pawn.story?.bodyType == BodyTypeDefOf.Male)
				{
					pawn.story.bodyType = BodyTypeDefOf.Female;
				}
				else if (Props.gender == Gender.Male && pawn.story?.bodyType == BodyTypeDefOf.Female)
				{
					pawn.story.bodyType = BodyTypeDefOf.Male;
				}
				if (pawn.Map == null)
				{
					GestationUtility.GetBabyName(pawn, pawn.GetMother() ?? pawn.GetFather() ?? null);
				}
				pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
			}
		}

	}

}
