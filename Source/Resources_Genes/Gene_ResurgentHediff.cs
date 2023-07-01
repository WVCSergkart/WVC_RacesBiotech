using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentHediff : Gene
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(60000))
			{
				return;
			}
			AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			if (Active)
			{
				Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (gene_Resurgent != null)
				{
					if (gene_Resurgent.Value >= def.resourceLossPerDay)
					{
						if (!pawn.health.hediffSet.HasHediff(HediffDefName))
						{
							pawn.health.AddHediff(HediffDefName);
						}
					}
					else
					{
						RemoveHediff();
					}
				}
			}
		}

		public void RemoveHediff()
		{
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefName);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						if (Active)
						{
							AddOrRemoveHediff();
						}
					}
				};
			}
		}

	}

}
