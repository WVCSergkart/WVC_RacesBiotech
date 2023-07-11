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

	public class HediffCompProperties_PawnHaveWings : HediffCompProperties
	{

		public HediffCompProperties_PawnHaveWings()
		{
			compClass = typeof(HediffComp_PawnHaveWings);
		}

	}

	public class HediffComp_PawnHaveWings : HediffComp
	{

		private Gene_Wings cachedWingsGene;

		public HediffCompProperties_PawnHaveWings Props => (HediffCompProperties_PawnHaveWings)props;

		public override bool CompShouldRemove => ShouldRemove;

		public bool ShouldRemove
		{
			get
			{
				if (cachedWingsGene == null)
				{
					cachedWingsGene = base.Pawn.genes.GetFirstGeneOfType<Gene_Wings>();
				}
				if (cachedWingsGene != null && cachedWingsGene.Active)
				{
					return false;
				}
				return true;
			}
		}

	}

}
