// Verse.AI.JobGiver_WanderNearConnectedTree
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    [Obsolete]
    public class ThinkNode_ConditionalAnyEnemyInMap : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (!pawn.Spawned)
            {
                return false;
            }
            Map map = pawn.Map;
            if (map.IsPlayerHome)
            {
                return GenHostility.AnyHostileActiveThreatToPlayer(map, countDormantPawnsAsHostile: false);
            }
            return false;
        }
    }

    [Obsolete]
    public class JobGiver_WanderNearTarget : JobGiver_Wander
    {
        public ThingDef targetDefName;

        public JobGiver_WanderNearTarget()
        {
            wanderRadius = 12f;
            ticksBetweenWandersRange = new IntRange(130, 250);
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(targetDefName);
            // foreach (Thing item in list)
            // {
            // if (item.Faction == Faction.OfPlayer)
            // {
            // hiveNumber++;
            // }
            // }
            if (list.Any((Thing x) => x.Spawned && x.Map == pawn.Map && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly)))
            {
                return base.TryGiveJob(pawn);
            }
            return null;
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(targetDefName);
            // Thing randomThing = list.RandomElement();
            // if (randomThing.Map == pawn.Map && pawn.CanReach(randomThing, PathEndMode.Touch, Danger.Deadly))
            // {
            // }
            return list.RandomElement().Position;
        }
    }

}
