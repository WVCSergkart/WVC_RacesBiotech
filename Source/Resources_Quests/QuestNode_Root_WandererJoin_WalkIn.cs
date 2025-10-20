using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class QuestNode_Root_WandererJoin_WalkIn : RimWorld.QuestGen.QuestNode_Root_WandererJoin_WalkIn
	{

		public XenotypeDef xenotypeDef;

		public override Pawn GeneratePawn()
		{
			Pawn pawn = base.GeneratePawn();
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotypeDef ?? GetXenotype() ?? XenotypeDefOf.Baseliner);
			return pawn;
		}

		//public override void SendLetter_NewTemp(Quest quest, Pawn pawn, Map map)
		//{
		//	base.SendLetter_NewTemp(quest, pawn, map);
		//	MiscUtility.UpdateStaticCollection();
		//}

		public static XenotypeDef GetXenotype()
		{
			if (StaticCollectionsClass.cachedPlayerPawnsCount > 15)
			{
				return null;
			}
			if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount <= 3 && Find.CurrentMap?.mapTemperature?.OutdoorTemp < 0)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Meca");
			}
			if (!ThoughtWorker_Precept_Shapeshifter.AnyShapeshifters && StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount >= StaticCollectionsClass.cachedNonDeathrestingColonistsCount / 2)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Shadoweater");
			}
			if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount <= 1 && Find.TickManager.TicksGame > 12 * 60000)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_RogueFormer");
			}
			if (StaticCollectionsClass.cachedNonDeathrestingColonistsCount >= 2 || StaticCollectionsClass.cachedNonDeathrestingColonistsCount <= 6)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Bloodeater");
			}
			if (!StaticCollectionsClass.haveAssignedWork && StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 3 && Faction.OfPlayer.def.techLevel == TechLevel.Spacer)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Blank");
			}
			if (StaticCollectionsClass.cachedColonyMechsCount <= 0 && (Faction.OfPlayer.def.techLevel == TechLevel.Neolithic || Faction.OfPlayer.def.techLevel == TechLevel.Medieval || Faction.OfPlayer.def.techLevel == TechLevel.Animal))
			{
				return Rand.Chance(0.7f) || !ModsConfig.IdeologyActive ? DefDatabase<XenotypeDef>.GetNamed("WVC_Golemkind") : DefDatabase<XenotypeDef>.GetNamed("WVC_RuneDryad");
			}
			if (ModsConfig.RoyaltyActive && StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount == StaticCollectionsClass.cachedNonDeathrestingColonistsCount)
			{
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Lilith");
			}
			if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount <= 2 && GeneResourceUtility.AnyUndeads)
			{
				if (StaticCollectionsClass.cachedColonyMechsCount > 0)
				{
					return DefDatabase<XenotypeDef>.GetNamed("WVC_Undead");
				}
				return DefDatabase<XenotypeDef>.GetNamed("WVC_Featherdust");
			}
			if (ModsConfig.AnomalyActive)
			{
				if (HivemindUtility.HivemindPawns.Count <= 0)
				{
					if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount > 7)
					{
						return null;
					}
					return DefDatabase<XenotypeDef>.GetNamed("WVC_Fleshkind");
				}
				else
				{
					if (StaticCollectionsClass.cachedColonistsDuplicatesDeathrestingCount > 4)
					{
						return null;
					}
					return DefDatabase<XenotypeDef>.GetNamed("WVC_Overrider");
				}
			}
			return null;
		}

	}

}
