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

	public class Gene_DryadQueen_Dependant : Gene, IGeneDryadQueen
	{

		[Unsaved(false)]
		private Gene_DryadQueen cachedDryadsQueenGene;

		public Gene_DryadQueen Gauranlen
		{
			get
			{
				if (cachedDryadsQueenGene == null || !cachedDryadsQueenGene.Active)
				{
					cachedDryadsQueenGene = pawn?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedDryadsQueenGene;
			}
		}

		public virtual void Notify_DryadSpawned(Pawn dryad)
		{

		}

		public override void Tick()
		{

		}

	}

	public class Gene_GauranlenConnection : Gene
	{

		public override void Tick()
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(23195))
			{
				return;
			}
			Connection(23195);
		}

		public void Connection(int tick = 1)
		{
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
				treeConnection.ConnectionStrength += ((0.33f) / 60000) * tick;
			}
		}

	}

	public class Gene_GauranlenDryads_AddPermanentHediff : Gene_DryadQueen_Dependant
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public override void Notify_DryadSpawned(Pawn dryad)
		{
			HediffUtility.TryAddHediff(Props.hediffDefName, dryad, null, null, false);
		}

	}


}
