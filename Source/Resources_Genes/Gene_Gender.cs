using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Gender : Gene, IGeneOverridden
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeGender();
		}

		public void ChangeGender()
		{
			if (!Active)
			{
				return;
			}
			if (pawn.gender == Props.gender)
			{
				return;
			}
			if (Props.gender != Gender.None)
			{
				SetGender(pawn, Props.gender);
			}
		}

		public static void SetGender(Pawn pawn, Gender gender)
		{
			pawn.gender = gender;
			if (gender == Gender.Female && pawn.story?.bodyType == BodyTypeDefOf.Male)
			{
				pawn.story.bodyType = BodyTypeDefOf.Female;
			}
			else if (gender == Gender.Male && pawn.story?.bodyType == BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Male;
			}
            if (MiscUtility.GameNotStarted() && pawn.Name is NameTriple nameTriple)
            {
                pawn.Name = new NameTriple(nameTriple.First, nameTriple.First, nameTriple.Last);
			}
			if (!pawn.style.CanWantBeard && pawn.style.beardDef != BeardDefOf.NoBeard)
			{
				pawn.style.beardDef = BeardDefOf.NoBeard;
			}
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

        public void Notify_OverriddenBy(Gene overriddenBy)
        {

        }

        public void Notify_Override()
		{
			ChangeGender();
		}

    }

	public class Gene_Feminine : Gene_GauntSkin
	{

		public override void ChangeBodyType()
		{
			if (!Active)
			{
				return;
			}
			if (pawn?.DevelopmentalStage != DevelopmentalStage.Adult)
			{
				return;
			}
			if (pawn?.story?.bodyType != null && pawn.story.bodyType != BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Female;
			}
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			Remove();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Remove();
		}

		private void Remove()
		{
			if (pawn?.DevelopmentalStage != DevelopmentalStage.Adult)
			{
				return;
			}
			if (pawn.gender == Gender.Male && pawn.story?.bodyType == BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Male;
			}
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		// public override void ExposeData()
		// {
		// base.ExposeData();
		// ChangeBodyType();
		// }

	}

}
