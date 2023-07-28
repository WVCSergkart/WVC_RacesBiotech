using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompAbilityEffect_Summon : CompAbilityEffect
    {
        public new CompProperties_Summon Props => (CompProperties_Summon)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            int countSpawn = Props.spawnCountRange.RandomInRange;
            for (int i = 0; i < countSpawn; i++)
            {
                MechanoidizationUtility.MechSummonQuest(parent.pawn, Props.quest);
            }
            Messages.Message("WVC_RB_Gene_Summoner".Translate(parent.pawn.LabelIndefinite().CapitalizeFirst()), parent.pawn, MessageTypeDefOf.PositiveEvent);
        }

        // private void Reset()
        // {
        // countSpawn = spawnCountRange.RandomInRange;
        // }
    }

    public class CompProperties_Summon : CompProperties_AbilityEffect
    {
        public QuestScriptDef quest;

        public IntRange spawnCountRange = new(1, 3);
    }

}
