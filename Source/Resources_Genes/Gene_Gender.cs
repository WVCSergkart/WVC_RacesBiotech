using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Gender : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (!Active)
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
				//if (!pawn.Spawned)
				//{
				//	GestationUtility.SetName(pawn, pawn.GetMother() ?? pawn.GetFather() ?? null);
				//}
				pawn.Drawer?.renderer?.SetAllGraphicsDirty();
			}
		}

	}

	public class Gene_Feminine : Gene_Exoskin, IGeneLifeStageStarted, IGeneOverridden
	{

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeBodyType();
		}

		public void ChangeBodyType()
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

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Remove();
		}

		public void Notify_Override()
		{
			ChangeBodyType();
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

		public void Notify_LifeStageStarted()
		{
			ChangeBodyType();
		}

	}

}
