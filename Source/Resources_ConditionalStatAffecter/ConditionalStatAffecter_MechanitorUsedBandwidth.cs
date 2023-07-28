using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ConditionalStatAffecter_MechanitorUsedBandwidth : ConditionalStatAffecter
    {
        public float controlledPawns;

        public override string Label => "WVC_StatsReport_MechanitorTotalBandwidth".Translate() + " " + controlledPawns;

        public override bool Applies(StatRequest req)
        {
            // if (!ModsConfig.BiotechActive)
            // {
            // return false;
            // }
            if (req.HasThing && req.Thing is Pawn pawn && MechanitorUtility.IsMechanitor(pawn))
            {
                // Log.Error("Создём переменную и запрашиваем количество подключенных механоидов");
                int connectedThingsCount = pawn.mechanitor.ControlledPawns.Count;
                // List<Pawn> connectedThingThing = pawn.mechanitor.ControlledPawns;
                // foreach (Pawn item in connectedThingThing)
                // {
                // connectedThingsCount++;
                // }
                // Log.Error("Механоидов подключено " + connectedThingsCount);
                // Log.Error("Сравниваем целевые значения с текущими");
                if (connectedThingsCount >= controlledPawns)
                {
                    // Log.Error("Возвращаем true");
                    return true;
                }
            }
            return false;
        }
    }

}
