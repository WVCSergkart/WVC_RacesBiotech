using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Hivemind_Regeneration : Gene_Hivemind_Drone
    {

        public static int? cachedRegenRate;
        public int RegenRate
        {
            get
            {
                if (!cachedRegenRate.HasValue)
                {
                    if (pawn.InHivemind())
                    {
                        cachedRegenRate = HivemindUtility.HivemindPawns.Count * 4;
                    }
                    else
                    {
                        cachedRegenRate = 0;
                    }
                }
                return cachedRegenRate.Value;
            }
        }

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(3335, delta))
            {
                return;
            }
            Regen();
        }

        public void Regen()
        {
            HealingUtility.Regeneration(pawn, RegenRate, 3335);
        }

    }

}
