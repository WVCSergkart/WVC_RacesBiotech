using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_GauranlenConnection : Gene
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(1624))
			{
				return;
			}
			if (!ModsConfig.IdeologyActive)
			{
				return;
			}
			List<Thing> allBuildingsColonist = pawn?.connections?.ConnectedThings;
			if (allBuildingsColonist.NullOrEmpty())
			{
				return;
			}
			foreach (Thing item in allBuildingsColonist)
			{
				CompTreeConnection treeConnection = item.TryGetComp<CompTreeConnection>();
				if (treeConnection == null)
				{
					continue;
				}
				treeConnection.ConnectionStrength += 0.01f;
			}
		}

	}


}
