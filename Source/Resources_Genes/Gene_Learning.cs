using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

		// public override bool Active
		// {
			// get
			// {
				// if (pawn != null)
				// {
					// if (!Overridden)
					// {
						// if (General != null && General.noSkillDecay)
						// {
							// StaticCollectionsClass.AddSkillDecayGenePawnToList(pawn);
							// Log.Error("Skills not decay");
						// }
					// }
					// else
					// {
						// if (General != null && General.noSkillDecay)
						// {
							// StaticCollectionsClass.RemoveSkillDecayGenePawnFromList(pawn);
							// Log.Error("Skills decay");
						// }
					// }
				// }
				// return base.Active;
			// }
		// }

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

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get no skill decay pawns",
					action = delegate
					{
						Log.Error("All no skill decay pawns:" + "\n" + StaticCollectionsClass.skillsNotDecayPawns.Select((Pawn x) => x.Name.ToString()).ToLineList(" - "));
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (pawn != null && !PawnGenerator.IsBeingGenerated(pawn) && General != null && General.noSkillDecay)
			{
				StaticCollectionsClass.AddSkillDecayGenePawnToList(pawn);
			}
		}

	}

}
