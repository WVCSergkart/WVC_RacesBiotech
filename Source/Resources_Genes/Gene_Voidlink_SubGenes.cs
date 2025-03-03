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

		//public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

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

		public override void PostAdd()
		{
			base.PostAdd();
			MasterGene?.UpdMaxResource();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			MasterGene?.UpdMaxResource();
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

	//public class Gene_VoidlinkMaxResource : Gene_VoidlinkDependant
	//{

	//}

}
