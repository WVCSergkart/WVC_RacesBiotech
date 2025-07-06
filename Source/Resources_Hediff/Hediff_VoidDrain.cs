using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_VoidDrain : Hediff
	{

		//private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => true;

		[Unsaved(false)]
		private Gene_VoidHunger cachedGene;

		public Gene_VoidHunger Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = phylacteryOwner?.genes?.GetFirstGeneOfType<Gene_VoidHunger>();
				}
				return cachedGene;
			}
		}


		private int nextTick = 125;
        public override void TickInterval(int delta)
        {
			if (!GeneResourceUtility.CanTick(ref nextTick, 56756, delta))
            {
				return;
            }
			if (Gene == null || Gene.Victim != pawn)
			{
				pawn.health.RemoveHediff(this);
			}
		}

        public void Notify_VictimChanged()
        {
			pawn.health.RemoveHediff(this);
		}

		public override void Notify_PawnKilled()
		{
			Gene?.RemoveVictim();
		}

		public Pawn phylacteryOwner;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref phylacteryOwner, "phylacteryOwner");
		}

	}

}
