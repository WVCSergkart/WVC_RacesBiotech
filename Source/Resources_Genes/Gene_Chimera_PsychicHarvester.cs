using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ChimeraDependant_PsychicHarvester : Gene_ChimeraDependant
	{

		private int nextTick = 44344;
		private int updTick = 45454;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetTicker();
		}

		public override void Tick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, updTick))
			{
				return;
			}
			Harvest();
			ResetTicker();
		}

		private void ResetTicker()
		{
			//IntRange range = new(44444, 67565);
			updTick = Spawner.spawnIntervalRange.RandomInRange;
		}

		public virtual void Harvest()
		{

		}

		public virtual void GetGene(List<Pawn> pawns)
		{
			foreach (Pawn pawn in pawns)
			{
				if (Rand.Chance(Spawner.chance))
				{
					Chimera.GetGeneFromHuman(pawn);
				}
			}
		}

	}

	public class Gene_ChimeraPsychicHarvester_Lover : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest()
		{
			List<Pawn> pawns = new();
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (relation.def.reflexive)
				{
					pawns.Add(relation.otherPawn);
				}
			}
			//pawn.relations.GetDirectRelations(PawnRelationDefOf.Lover, ref pawns);
			GetGene(pawns);
		}
	}

	public class Gene_ChimeraPsychicHarvester_Friend : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest()
		{
			List<Pawn> pawns = new();
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (pawn.relations.OpinionOf(relation.otherPawn) >= 20)
				{
					pawns.Add(relation.otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Rival : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest()
		{
			List<Pawn> pawns = new();
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (pawn.relations.OpinionOf(relation.otherPawn) <= -20)
				{
					pawns.Add(relation.otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Family : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest()
		{
			List<Pawn> pawns = new();
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (relation.def.familyByBloodRelation)
				{
					pawns.Add(relation.otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Destructive : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest()
		{
			List<Pawn> pawns = new();
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (relation.otherPawn.Faction == pawn.Faction)
				{
					pawns.Add(relation.otherPawn);
				}
			}
			GetGene(pawns);
		}

		public override void GetGene(List<Pawn> pawns)
		{
			foreach (Pawn victim in pawns)
			{
				if (!Rand.Chance(Spawner.chance))
				{
					continue;
				}
				List<Gene> genes = victim?.genes?.GenesListForReading;
				if (genes.NullOrEmpty())
				{
					return;
				}
				if (Chimera.TryGetGene(XaG_GeneUtility.ConvertGenesInGeneDefs(genes), out GeneDef result))
				{
					victim.genes.RemoveGene(victim.genes.GetGene(result));
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
		}

	}

}
