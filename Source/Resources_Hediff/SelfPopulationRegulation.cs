using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffComp_GolemPopulationRegulation : HediffComp
    {
        // private readonly int ticksInday = 60000;
        // private readonly int ticksInday = 1500;

        // private int ticksCounter = 0;

        public HediffCompProperties_GolemPopulationRegulation Props => (HediffCompProperties_GolemPopulationRegulation)props;

        // protected int GestationIntervalDays => Props.gestationIntervalDays;

        // protected string customString => Props.customString;

        // protected bool produceEggs => Props.produceEggs;

        // protected string eggDef => Props.eggDef;

        // public override string CompLabelInBracketsExtra => GetLabel();

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (!Pawn.IsHashIntervalTick(60000))
            // if (!Pawn.IsHashIntervalTick(1500))
            {
                return;
            }
            Pawn golem = parent.pawn;
            // Log.Error("Проверяем на наличие надзирателя. Если надзирателя нету, убиваем");
            if (golem.GetOverseer() == null)
            {
                base.Pawn.Kill(null, parent);
                return;
            }
            // if (golem.Faction != Faction.OfPlayer)
            // {
            // base.Pawn.Kill(null, parent);
            // return;
            // }
            // Log.Error("Не трогаем тех кто в караване");
            if (golem.Map == null)
            {
                return;
            }
            if (!Pawn.IsHashIntervalTick(300000))
            // if (!Pawn.IsHashIntervalTick(3000))
            {
                return;
            }
            // Log.Error("Запрашиваем надзирателя");
            Pawn overseer = golem.GetOverseer();
            // if (overseer.Faction != Faction.OfPlayer)
            // {
            // return;
            // }
            // Log.Error("Проверяем есть ли у механитора нужный ген");
            // if (!MechanoidizationUtility.HasActiveGene(Props.geneDef, overseer))
            // {
            // base.Pawn.Kill(null, parent);
            // return;
            // }
            // Log.Error("Считаем механоидов одного типа");
            int connectedThingsCount = 0;
            List<Pawn> connectedThingThing = overseer.mechanitor.ControlledPawns;
            foreach (Pawn item in connectedThingThing)
            {
                if (!item.health.Downed && !item.health.Dead && item.kindDef.defName.Contains(golem.kindDef.defName))
                {
                    connectedThingsCount++;
                }
            }
            // Log.Error("Считаем все шрайны и прибавляем за каждый 0.2");
            // int naturalShrinesCount = 0;
            // List<Building> buildingsThing = golem.Map.listerBuildings.allBuildingsColonist;
            // foreach (Building item in buildingsThing)
            // {
            // if (item.def.defName.Contains("NatureShrine"))
            // {
            // naturalShrinesCount++;
            // }
            // }
            // Log.Error("Шрайны " + naturalShrinesCount);
            // float maxConnectedThings = overseer.GetStatValue(Props.statDef) + (naturalShrinesCount * 0.2f);
            // Log.Error("Проверяем количество големов, если их слишком много убиваем лишних");
            // Log.Error("Количество големов: " + connectedThingsCount + ", максимум может быть " + maxConnectedThings);
            if (connectedThingsCount > overseer.GetStatValue(Props.statDef))
            {
                base.Pawn.Kill(null, parent);
                // connectedThingsCount = 0;
                // return;
            }
            // return;
        }

        // private float NatureShrineCounting(Pawn golem, Pawn overseer)
        // {
        // float naturalShrinesCount = 0f;
        // List<Building> buildingsThing = golem.Map.listerBuildings.allBuildingsColonist;
        // foreach (Building item in buildingsThing)
        // {
        // if (item.def.defName.Contains("NatureShrine"))
        // {
        // _ = naturalShrinesCount + 0.2f;
        // }
        // }
        // }

    }

}
