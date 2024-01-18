using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_BackstoryChanger : Gene
	{

		// public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>().childBackstoryDef;
		// public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>().adultBackstoryDef;
		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (Props != null)
			{
				BackstoryChanger(pawn, Props.childBackstoryDef, Props.adultBackstoryDef);
			}
		}

		public static void BackstoryChanger(Pawn pawn, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null)
		{
			if ((pawn?.story) == null)
			{
				return;
			}
			if (pawn.story.Childhood == null)
			{
				return;
			}
			List<BackstoryDef> blackListedBackstoryForChanger = XenotypeFilterUtility.BlackListedBackstoryForChanger();
			if (childBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Childhood))
			{
				pawn.story.Childhood = childBackstoryDef;
			}
			if (pawn.story.Adulthood != null && adultBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Adulthood))
			{
				pawn.story.Adulthood = adultBackstoryDef;
			}
		}

	}

	public class Gene_Learning : Gene_BackstoryChanger
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (General != null && General.noSkillDecay)
			{
				StaticCollectionsClass.AddSkillDecayGenePawnToList(pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!GeneFeaturesUtility.PawnSkillsNotDecay(pawn))
			{
				StaticCollectionsClass.RemoveSkillDecayGenePawnFromList(pawn);
			}
		}

	}

}
