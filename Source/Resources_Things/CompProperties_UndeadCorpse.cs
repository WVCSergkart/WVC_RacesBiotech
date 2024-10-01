using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_UndeadCorpse : CompProperties
	{

		public IntRange resurrectionDelay = new(6000, 9000);

		public string uniqueTag = "XaG_Undead";

		public CompProperties_UndeadCorpse()
		{
			compClass = typeof(CompUndeadCorpse);
		}

	}

	[Obsolete]
	public class CompUndeadCorpse : ThingComp
	{

		private int resurrectionDelay = 0;
		private bool shouldResurrect = false;

		private CompProperties_UndeadCorpse Props => (CompProperties_UndeadCorpse)props;

		// public override void Initialize(CompProperties props)
		// {
			// base.Initialize(props);
			// ResetCounter();
		// }

		public Corpse Corpse => parent is Corpse corpse ? corpse : null;

		public Pawn InnerPawn => Corpse?.InnerPawn;

		// public Gene_Undead Gene_Undead => InnerPawn.GetUndeadGene(out Gene_Undead gene_undead) ? gene_undead : null;

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
			// if (!respawningAfterLoad)
			// {
				// shouldResurrect = Gene_Undead?.UndeadCanResurrect == true && (InnerPawn.IsColonist || WVC_Biotech.settings.canNonPlayerPawnResurrect);
				// ResetCounter();
			// }
		// }

		public void SetUndead(bool resurrect, int delay)
		{
			shouldResurrect = resurrect;
			resurrectionDelay = delay;
			if (delay > 0)
			{
				resurrectionDelay += Props.resurrectionDelay.RandomInRange;
			}
		}

		public override void CompTick()
		{
			Tick();
		}

		public override void CompTickRare()
		{
			Tick();
		}

		public override void CompTickLong()
		{
			Tick();
		}

		public void Tick()
		{
			if (!shouldResurrect)
			{
				return;
			}
			if (Find.TickManager.TicksGame < resurrectionDelay)
			{
				return;
			}
			TryResurrect();
		}

		public void TryResurrect()
		{
			if (parent.Map == null)
			{
				return;
			}
			InnerPawn.GetUndeadGene(out Gene_Undead gene);
			if (gene != null)
			{
				if (Corpse?.CurRotDrawMode != RotDrawMode.Fresh)
				{
					if (ModLister.CheckAnomaly("Shambler"))
					{
						MutantUtility.ResurrectAsShambler(InnerPawn, -1, InnerPawn.Faction);
					}
				}
				else
				{
					GeneResourceUtility.RegenComaOrDeathrest(InnerPawn, gene);
				}
			}
			shouldResurrect = false;
		}

		// public void ResetCounter()
		// {
			// int delay = Props.resurrectionDelay.RandomInRange;
			// if (Gene_Undead != null)
			// {
				// delay += Gene_Undead.Giver.additionalDelay.RandomInRange;
			// }
			// resurrectionDelay = Find.TickManager.TicksGame + delay;
		// }

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref resurrectionDelay, "resurrectionDelay_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref shouldResurrect, "shouldResurrect_" + Props.uniqueTag, false);
		}

		// =================

		public override string CompInspectStringExtra()
		{
			if (shouldResurrect)
			{
				return "WVC_XaG_Gene_UndeadResurrectionCorpse_Info".Translate().Resolve() + ": " + (resurrectionDelay - Find.TickManager.TicksGame).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return null;
		}

	}

}
