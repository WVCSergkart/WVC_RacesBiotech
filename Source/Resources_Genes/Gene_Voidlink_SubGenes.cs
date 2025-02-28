using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_VoidlinkDependant : Gene
	{

		[Unsaved(false)]
		private Gene_Voidlink cachedGene;

		public Gene_Voidlink MasterGene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Voidlink>();
				}
				return cachedGene;
			}
		}

	}

	public class Gene_VoidlinkOffset : Gene_VoidlinkDependant
	{

		public virtual float ResourceGain => def.resourceLossPerDay / 60000;

		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(2500))
			{
				MasterGene?.OffsetResource(ResourceGain * 2500);
			}
		}

	}

}
