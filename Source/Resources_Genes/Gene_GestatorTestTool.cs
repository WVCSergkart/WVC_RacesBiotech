using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

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
						MechanoidizationUtility.GenerateNewBornPawn(pawn, "WVC_RB_Gene_MechaGestator", true);
					}
				};
			}
		}

	}
}
