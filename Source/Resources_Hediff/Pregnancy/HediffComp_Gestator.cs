using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class HediffComp_Gestator : HediffComp, IHediffCustomPregnancy
	{
		private readonly int ticksInday = 60000;

		private int ticksCounter = 0;

		public HediffCompProperties_Gestator Props => (HediffCompProperties_Gestator)props;

		protected int GestationIntervalDays => Props.gestationIntervalDays;

		public override string CompLabelInBracketsExtra => GetLabel();

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
		}

		private GeneSet cachedGeneSet;
		public GeneSet GeneSet
		{
			get
			{
				if (cachedGeneSet == null)
				{
					cachedGeneSet = new();
					HediffUtility.AddGeneDefs(cachedGeneSet, Pawn.genes.GenesListForReading.ConvertToDefs());
					//Log.Error(cachedGeneSet.GenesListForReading.Count.ToString());
					cachedGeneSet.SortGenes();
				}
				return cachedGeneSet;
			}
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//base.CompPostTick(ref severityAdjustment);
			ticksCounter += delta;
			if (Pawn.Map == null)
			{
				return;
			}
			if (Pawn.Faction != Faction.OfPlayer || !Pawn.ageTracker.CurLifeStage.reproductive)
			{
				base.Pawn.health.RemoveHediff(parent);
				return;
			}
			if (ticksCounter < ticksInday * GestationIntervalDays)
			{
				return;
			}
			EndGestation();
		}

		private void EndGestation()
		{
			GestationUtility.GestateChild_WithGenes(parent.pawn, completeMessage: Props.completeLetterDesc, endogenes: Props.endogeneTransfer, xenogenes: Props.xenogeneTransfer);
			ticksCounter = 0;
			base.Pawn.health.RemoveHediff(parent);
		}

		public string GetLabel()
		{
			Pawn pawn = parent.pawn;
			if (pawn.Faction == Faction.OfPlayer && pawn.ageTracker.CurLifeStage.reproductive)
			{
				float percent = ticksCounter / (float)(ticksInday * GestationIntervalDays);
				return percent.ToStringPercent();
			}
			return "";
		}

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Complete gestation",
					action = delegate
					{
						EndGestation();
					}
				};
			}
			if (!Pawn.Drafted)
			{
				yield return XaG_UiUtility.ITab_InspectBabyGenes();
			}
		}
	}

}
