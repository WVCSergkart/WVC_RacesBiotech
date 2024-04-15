using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// Rare Hemogen Drain
	public class Gene_HemogenOffset : Gene, IGeneResourceDrain
	{

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;

		public Gene_Resource Resource => Hemogen;

		public Gene_Hemogen Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public bool CanOffset
		{
			get
			{
				return Hemogen?.CanOffset == true;
			}
		}

		public float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public override void Tick()
		{
			base.Tick();
			if (pawn.IsHashIntervalTick(120))
			{
				TickHemogenDrain(this, 120);
			}
		}

		public static void TickHemogenDrain(IGeneResourceDrain drain, int tick = 1)
		{
			if (drain.Resource != null && drain.CanOffset)
			{
				GeneResourceDrainUtility.OffsetResource(drain, ((0f - drain.ResourceLossPerDay) / 60000f) * tick);
			}
		}

	}

	public class Gene_Bloodfeeder : Gene_HemogenOffset
	{

		public virtual void Notify_Bloodfeed(Pawn victim)
		{
		}

	}

	public class Gene_ImplanterFang : Gene_Bloodfeeder
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public override void Notify_Bloodfeed(Pawn victim)
		{
			// Log.Error("0");
			if (General == null)
			{
				return;
			}
			// Log.Error("1");
			if (victim?.health?.immunity?.AnyGeneMakesFullyImmuneTo(WVC_GenesDefOf.WVC_XaG_ImplanterFangsMark) == true)
			{
				return;
			}
			// Log.Error("2");
			float immunity = victim.GetStatValue(StatDefOf.ImmunityGainSpeed);
			if (!Rand.Chance(General.reimplantChance / (immunity > 0.01f ? immunity : 0.01f)))
			{
				// Log.Error("2 Chance: " + (General.reimplantChance / (immunity > 0.01f ? immunity : 0.01f)).ToString());
				return;
			}
			// Log.Error("3");
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
			{
				return;
			}
			// Log.Error("4");
			if (ReimplanterUtility.TryReimplant(pawn, victim, General.reimplantEndogenes, General.reimplantXenogenes))
			{
				victim.health.AddHediff(WVC_GenesDefOf.WVC_XaG_ImplanterFangsMark, ExecutionUtility.ExecuteCutPart(victim));
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(victim))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(pawn.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn, victim));
				}
			}
		}

	}

}
