using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_Cellsrest : HediffWithComps
	{

		private Gene_Cellular cachedGene;

		private Gene_Cellular CellularGene => cachedGene ?? (cachedGene = pawn.genes?.GetFirstGeneOfType<Gene_Cellular>());

		public override string LabelInBrackets
		{
			get
			{
				if (Paused)
				{
					return base.LabelInBrackets;
				}
				return CellularGene.DeathrestPercent.ToStringPercent("F0");
			}
		}

		public override string TipStringExtra
		{
			get
			{
				string text = base.TipStringExtra;
				if (Paused)
				{
					if (!text.NullOrEmpty())
					{
						text += "\n";
					}
					text += "PawnWillKeepDeathrestingLethalInjuries".Translate(pawn.Named("PAWN")).Colorize(ColorLibrary.RedReadable);
				}
				return text;
			}
		}

		public override bool ShouldRemove
		{
			get
			{
				if (CellularGene == null)
				{
					return true;
				}
				return base.ShouldRemove;
			}
		}

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			CellularGene?.Notify_DeathrestStarted();
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			CellularGene?.Notify_DeathrestEnded();
			if (pawn.Spawned && pawn.CurJobDef == JobDefOf.Deathrest)
			{
				pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
			}
		}

		public override void PostTick()
		{
			base.PostTick();
			if (pawn.IsHashIntervalTick(120))
			{
				CellularGene?.TickDeathresting(120);
			}
		}

	}

}
