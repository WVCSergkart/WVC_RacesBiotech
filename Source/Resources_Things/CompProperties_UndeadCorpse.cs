using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UndeadCorpse : CompProperties
	{

		public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "XaG_Undead";

		public CompProperties_UndeadCorpse()
		{
			compClass = typeof(CompUndeadCorpse);
		}
	}

	public class CompUndeadCorpse : ThingComp
	{

		public int resurrectionDelay;
		public bool shouldResurrect = false;

		private CompProperties_UndeadCorpse Props => (CompProperties_UndeadCorpse)props;

		// private Corpse Corpse => parent as Corpse;

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				shouldResurrect = parent is Corpse corpse && corpse.InnerPawn.IsUndead();
				ResetCounter();
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			Tick();
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			Tick();
		}

		public override void CompTickLong()
		{
			base.CompTickRare();
			Tick();
		}

		public void Tick()
		{
			if (!shouldResurrect || Find.TickManager.TicksGame < resurrectionDelay)
			{
				return;
			}
			TryResurrect();
		}

		public void TryResurrect()
		{
			Pawn pawn = parent is Corpse corpse ? corpse.InnerPawn : null;
			if (pawn != null && pawn.RaceProps.Humanlike && pawn.GetUndeadGene(out Gene_Undead gene_undead) && gene_undead.UndeadCanResurrect)
			{
				// UndeadUtility.NewUndeadResurrect(pawn, gene_undead.ChildBackstoryDef, gene_undead.AdultBackstoryDef, gene_undead.Gene_ResurgentCells, gene_undead.def.resourceLossPerDay);
				UndeadUtility.RegenComaOrDeathrest(pawn, gene_undead);
			}
		}

		public void ResetCounter()
		{
			// shouldResurrect = parent is Corpse corpse && corpse.InnerPawn.IsUndead();
			resurrectionDelay = Find.TickManager.TicksGame + Props.resurrectionDelay.RandomInRange;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref resurrectionDelay, "resurrectionDelay_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref shouldResurrect, "shouldResurrect_" + Props.uniqueTag, false);
		}

	}

}
