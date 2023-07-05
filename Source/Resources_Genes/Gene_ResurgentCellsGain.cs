using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentCellsGain : Gene, IGeneResourceDrain
	{
		[Unsaved(false)]
		private Gene_ResurgentCells cachedHemogenGene;

		// private const float MinAgeForDrain = 3f;

		public Gene_Resource Resource
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes.GetFirstGeneOfType<Gene_ResurgentCells>();
				}
				return cachedHemogenGene;
			}
		}

		public bool CanOffset
		{
			get
			{
				if (Active)
				{
					return true;
				}
				return false;
			}
		}

		public float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo resourceDrainGizmo in UndeadUtility.GetResourceDrainGizmos(this))
			{
				yield return resourceDrainGizmo;
			}
		}
	}

}
