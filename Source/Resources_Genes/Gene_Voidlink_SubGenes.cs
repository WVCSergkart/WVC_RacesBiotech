using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_VoidlinkDependant : Gene, IGeneOverridden
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

        public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			MasterGene?.CacheReset();
		}

        public virtual void Notify_Override()
		{
			MasterGene?.CacheReset();
		}

        public override void PostAdd()
		{
			base.PostAdd();
			MasterGene?.CacheReset();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			MasterGene?.CacheReset();
		}

	}

	public class Gene_VoidlinkOffset : Gene_VoidlinkDependant
	{

		public virtual float ResourceGain => def.resourceLossPerDay / 60000;

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				MasterGene?.OffsetResource(ResourceGain * 2500 * delta);
			}
		}

	}

	//public class Gene_VoidlinkMaxResource : Gene_VoidlinkDependant
	//{

	//}

}
