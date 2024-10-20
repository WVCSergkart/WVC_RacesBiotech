using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_LightExposure : HediffWithComps
    {

		public override bool ShouldRemove => false;

		public override bool Visible
		{
			get
			{
				if (CurStage != null)
				{
					return CurStage.becomeVisible;
				}
				return false;
			}
		}

		public override void Tick()
        {
            //base.Tick();
			if (pawn.IsHashIntervalTick(878) && pawn.SpawnedOrAnyParentSpawned && !pawn.Dead)
            {
                Severity += HediffUtility.SeverityFromLit(pawn, 0.008f, -0.005f, 878);
            }
        }

	}

}
