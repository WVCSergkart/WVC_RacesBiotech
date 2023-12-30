using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class UndeadUtility
	{

		// Coma TEST

		[Obsolete]
		public static void ShouldUndeadRegenComaOrDeathrest(bool resurrect, Pawn pawn)
		{
			if (resurrect)
			{
				Gene_Undead undead = pawn?.genes?.GetFirstGeneOfType<Gene_Undead>();
				if (undead != null)
				{
					RegenComaOrDeathrest(pawn, undead);
				}
			}
		}

		public static void RegenComaOrDeathrest(Pawn pawn, Gene_Undead gene)
		{
			// Resurrect
			if (pawn?.health?.hediffSet?.GetBrain() == null)
			{
				Gene_BackstoryChanger.BackstoryChanger(pawn, gene.Giver.childBackstoryDef, gene.Giver.adultBackstoryDef);
				foreach (SkillRecord item in pawn.skills.skills)
				{
					if (!item.TotallyDisabled && item.XpTotalEarned > 0f)
					{
						float num = item.XpTotalEarned;
						item.Learn(0f - num, direct: true);
					}
				}
			}
			ResurrectionUtility.Resurrect(pawn);
			pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
			// Undead
			pawn.health.AddHediff(WVC_GenesDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
			pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
			// Evolve
			XenotypeDef mainXenotype = pawn.genes.CustomXenotype == null ? pawn.genes.Xenotype : null;
			string letterDesc = "WVC_LetterTextSecondChance_GeneUndead";
			SubXenotypeUtility.XenotypeShapeshifter(pawn);
			if (mainXenotype != null && mainXenotype != pawn.genes.Xenotype)
			{
				letterDesc = "WVC_LetterTextSecondChance_GeneUndeadShapeshift";
			}
			// Letter
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				string shapeshiftXenotype = pawn?.genes?.Xenotype != null ? pawn.genes.Xenotype.LabelCap : "ERROR";
				Find.LetterStack.ReceiveLetter(gene.LabelCap, letterDesc.Translate(pawn.Named("PAWN"), gene.LabelCap, shapeshiftXenotype), WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(pawn));
			}
		}

		// Resurrection

		public static void ResurrectWithSickness(Pawn pawn, ThoughtDef resurrectThought = null)
		{
			ResurrectionUtility.Resurrect(pawn);
			pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
			if (resurrectThought != null)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(resurrectThought);
			}
		}

		[Obsolete]
		public static void NewUndeadResurrect(Pawn pawn, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null, Gene_ResurgentCells resurgentGene = null, float resourceLossPerDay = 0f)
		{
			ResurrectWithSickness(pawn, WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
			if (resurgentGene == null)
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
				Gene_BackstoryChanger.BackstoryChanger(pawn, childBackstoryDef, adultBackstoryDef);
				foreach (SkillRecord item in pawn.skills.skills)
				{
					if (!item.TotallyDisabled && item.XpTotalEarned > 0f)
					{
						float num = item.XpTotalEarned;
						item.Learn(0f - num, direct: true);
					}
				}
                SubXenotypeUtility.XenotypeShapeshifter(pawn);
			}
			else
			{
				resurgentGene.Value -= resourceLossPerDay;
			}
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndeadResurgent".Translate(pawn.Named("TARGET")), WVC_GenesDefOf.WVC_XaG_UndeadEvent, new LookTargets(pawn));
			}
		}

		public static void OffsetResurgentCells(Pawn pawn, float offset)
		{
			if (!ModsConfig.BiotechActive)
			{
				return;
			}
			Gene_ResurgentCells gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Hemogen != null)
			{
				gene_Hemogen.Value += offset;
			}
		}

		// ResourceUtility
		public static void TickResourceDrain(IGeneResourceDrain drain)
		{
			if (drain.CanOffset && drain.Resource != null)
			{
				OffsetResource(drain, (0f - drain.ResourceLossPerDay) / 60000f);
			}
		}

		public static void OffsetResource(IGeneResourceDrain drain, float amnt)
		{
			// float value = drain.Resource.Value;
			drain.Resource.Value += amnt;
			// PostResourceOffset(drain, value);
		}

		public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
		{
			if (DebugSettings.ShowDevGizmos && drain.Resource != null)
			{
				Gene_Resource resource = drain.Resource;
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " -10",
					action = delegate
					{
						OffsetResource(drain, -0.1f);
					}
				};
				yield return command_Action;
				Command_Action command_Action2 = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " +10",
					action = delegate
					{
						OffsetResource(drain, 0.1f);
					}
				};
				yield return command_Action2;
			}
		}

		// public static void PostResourceOffset(IGeneResourceDrain drain, float oldValue)
		// {
		// if (oldValue > 0f && drain.Resource.Value <= 0f)
		// {
		// Pawn pawn = drain.Pawn;
		// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.ResurrectionSickness))
		// {
		// pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
		// }
		// }
		// }

		public static bool IsUndead(this Pawn pawn)
		{
			if (!ModsConfig.BiotechActive || pawn.genes == null)
			{
				return false;
			}
			return XaG_GeneUtility.HasActiveGene(pawn.genes?.GetFirstGeneOfType<Gene_Undead>()?.def, pawn);
		}

		public static bool GetUndeadGene(this Pawn pawn, out Gene_Undead gene)
		{
			gene = pawn.RaceProps.Humanlike && pawn?.genes?.GetFirstGeneOfType<Gene_Undead>() != null ? pawn.genes.GetFirstGeneOfType<Gene_Undead>() : null;
			return gene != null;
		}

	}
}
