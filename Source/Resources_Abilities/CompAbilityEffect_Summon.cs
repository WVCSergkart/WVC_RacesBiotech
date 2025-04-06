using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
    public class CompProperties_Summon : CompProperties_AbilityEffect
	{
		public QuestScriptDef quest;

		public IntRange spawnCountRange = new(1, 3);
	}

	[Obsolete]
	public class CompAbilityEffect_MechanoidsSummoning : CompAbilityEffect
	{
		public new CompProperties_Summon Props => (CompProperties_Summon)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			int countSpawn = Props.spawnCountRange.RandomInRange;
			for (int i = 0; i < countSpawn; i++)
			{
				MechanoidsUtility.MechSummonQuest(parent.pawn, Props.quest);
			}
			Messages.Message("WVC_RB_Gene_Summoner".Translate(parent.pawn.LabelIndefinite().CapitalizeFirst()), parent.pawn, MessageTypeDefOf.PositiveEvent);
		}

		public override bool GizmoDisabled(out string reason)
		{
			if (!MechanitorUtility.IsMechanitor(parent.pawn))
			{
				reason = "WVC_XaG_PawnShouldBeMechanitor".Translate(parent.pawn.Name.ToStringShort);
				return true;
			}
			reason = null;
			return false;
		}
	}

}
