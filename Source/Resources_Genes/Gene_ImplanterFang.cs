using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_ImplanterFang : Gene_HemogenOffset, IGeneBloodfeeder
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public virtual void Notify_Bloodfeed(Pawn victim)
		{
			// Log.Error("0");
			if (General == null || victim == null)
			{
				return;
			}
			if (victim.Dead)
			{
				return;
			}
			// Log.Error("1");
			if (victim?.health?.immunity?.AnyGeneMakesFullyImmuneTo(MainDefOf.WVC_XaG_ImplanterFangsMark) == true)
			{
				return;
			}
			// Log.Error("2");
			float? immunity = victim?.GetStatValue(StatDefOf.ImmunityGainSpeed, cacheStaleAfterTicks: 360000);
			float chance = (General.reimplantChance / (immunity.HasValue && immunity.Value > 0.01f ? immunity.Value : 0.01f)) * WVC_Biotech.settings.hemogenic_ImplanterFangsChanceFactor;
			if (!Rand.Chance(chance))
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
			TryReimplant(victim);
		}

		public virtual void TryReimplant(Pawn victim)
		{
			if (ReimplanterUtility.TryReimplant(pawn, victim, General.reimplantEndogenes, General.reimplantXenogenes))
			{
				victim.health.AddHediff(MainDefOf.WVC_XaG_ImplanterFangsMark, ExecutionUtility.ExecuteCutPart(victim));
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(victim))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(pawn.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn, victim));
				}
			}
		}
	}

	public class Gene_ImplanterFang_NonRemove : Gene_ImplanterFang
	{

		public bool IsXenogene => pawn.genes.IsXenogene(this);

		public override void TryReimplant(Pawn victim)
		{
			List<GeneDef> oldEndogenes = victim.genes.Endogenes.ConvertToDefs();
			List<GeneDef> oldXenogenes = victim.genes.Xenogenes.ConvertToDefs();
			bool isXenogene = IsXenogene;
			if (ReimplanterUtility.TryReimplant(pawn, victim, !isXenogene, isXenogene))
			{
				if (isXenogene)
				{
					foreach (GeneDef item in oldXenogenes)
					{
						if (!item.passOnDirectly)
						{
							continue;
						}
						if (!XaG_GeneUtility.HasGene(item, victim))
						{
							victim.genes.AddGene(item, false);
						}
					}
				}
				else
				{
					foreach (GeneDef item in oldEndogenes)
					{
						if (!item.passOnDirectly)
						{
							continue;
						}
						if (!XaG_GeneUtility.HasGene(item, victim))
						{
							victim.genes.AddGene(item, true);
						}
					}
				}
				victim.health.AddHediff(MainDefOf.WVC_XaG_ImplanterFangsMark, ExecutionUtility.ExecuteCutPart(victim));
				if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(victim))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(pawn.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn, victim));
				}
				ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
				ReimplanterUtility.PostImplantDebug(pawn);
			}
		}

	}

}
