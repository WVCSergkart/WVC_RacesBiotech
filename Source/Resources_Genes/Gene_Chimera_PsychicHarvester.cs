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
			TryHarvest();
		}

		private void ResetTicker()
		{
			//IntRange range = new(44444, 67565);
			updTick = Spawner.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryHarvestGenes",
					action = delegate
					{
						TryHarvest();
					}
				};
			}
		}

		public void TryHarvest()
		{
			if (pawn.Map != null)
			{
				Harvest(HumanFilter(pawn.Map.mapPawns.AllHumanlikeSpawned.ToList()));
			}
			ResetTicker();
		}

		public virtual void Harvest(List<Pawn> pawns)
		{

		}

		public virtual void GetGene(List<Pawn> pawns)
        {
			//Log.Error("Pawns: " + pawns.Count);
            foreach (Pawn pawn in pawns)
            {
                if (Rand.Chance(Spawner.chance))
                {
                    Chimera.GetGeneFromHuman(pawn);
                }
            }
        }

        private List<Pawn> HumanFilter(List<Pawn> pawns)
        {
			List<Pawn> filteredPawns = new();
			foreach (Pawn victim in pawns)
            {
				if (!victim.IsHuman() || !victim.IsPsychicSensitive() || victim.Faction != pawn.Faction)
                {
					continue;
				}
				filteredPawns.Add(victim);
			}
			return filteredPawns;

		}

		public bool IsLovers(Pawn one, Pawn two)
		{
			foreach (PawnRelationDef relation in one.GetRelations(two))
			{
				if (relation.reflexive)
				{
					return true;
				}
			}
			return false;
		}

	}

	public class Gene_ChimeraPsychicHarvester_Lover : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest(List<Pawn> potentialVictims)
		{
			List<Pawn> pawns = new();
			foreach (Pawn otherPawn in potentialVictims)
			{
				if (IsLovers(pawn, otherPawn))
				{
					pawns.Add(otherPawn);
				}
			}
			//pawn.relations.GetDirectRelations(PawnRelationDefOf.Lover, ref pawns);
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Friend : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest(List<Pawn> potentialVictims)
		{
			List<Pawn> pawns = new();
			foreach (Pawn otherPawn in potentialVictims)
			{
				if (pawn.relations.OpinionOf(otherPawn) >= 20 && otherPawn.relations.OpinionOf(pawn) >= 20)
				{
					pawns.Add(otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Rival : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest(List<Pawn> potentialVictims)
		{
			List<Pawn> pawns = new();
			foreach (Pawn otherPawn in potentialVictims)
			{
				if (pawn.relations.OpinionOf(otherPawn) <= -20 && otherPawn.relations.OpinionOf(pawn) <= -20)
				{
					pawns.Add(otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Family : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest(List<Pawn> potentialVictims)
		{
			List<Pawn> pawns = new();
			foreach (Pawn otherPawn in potentialVictims)
			{
				if (pawn.relations.FamilyByBlood.Contains(otherPawn))
				{
					pawns.Add(otherPawn);
				}
			}
			GetGene(pawns);
		}

	}

	public class Gene_ChimeraPsychicHarvester_Destructive : Gene_ChimeraDependant_PsychicHarvester
	{

		public override void Harvest(List<Pawn> potentialVictims)
		{
			List<Pawn> pawns = new();
			foreach (Pawn otherPawn in potentialVictims)
			{
				if (otherPawn.Faction == pawn.Faction)
				{
					pawns.Add(otherPawn);
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
