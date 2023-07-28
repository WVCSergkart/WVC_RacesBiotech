using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Gestator_TestTool : Gene
    {

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // DEV
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Spawn pawn",
                    action = delegate
                    {
                        GestationUtility.GenerateNewBornPawn(pawn);
                    }
                };
            }
        }

    }

    public class Gene_DustGestator_TestTool : Gene_DustDrain
    {

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // DEV
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Spawn pawn",
                    action = delegate
                    {
                        GestationUtility.GenerateNewBornPawn(pawn);
                    }
                };
            }
        }

    }
}
