using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ThrallMaker : Gene
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public ThrallDef thrallDef = null;

		private Gizmo gizmo;

		public override void PostAdd()
		{
			base.PostAdd();
			if (thrallDef == null)
			{
				thrallDef = DefDatabase<ThrallDef>.AllDefsListForReading.RandomElement();
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + (thrallDef != null ? thrallDef.LabelCap.ToString() : "ERR"),
				defaultDesc = "WVC_XaG_GeneThrallMaker_ButtonDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ThrallMaker(this));
				}
			};
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref thrallDef, "thrallDef");
		}

	}

	public class Gene_GeneticThrall : Gene_GeneticInstability
	{

		public override void DelayJob()
		{
			if (TryHuntForCells(pawn))
			{
				return;
			}
			base.DelayJob();
		}

		public static bool TryHuntForCells(Pawn pawn)
		{
			List<Pawn> colonists = pawn?.Map?.mapPawns?.SpawnedPawnsInFaction(pawn.Faction);
			colonists.Shuffle();
			for (int j = 0; j < colonists.Count; j++)
			{
				Pawn colonist = colonists[j];
				if (!GeneFeaturesUtility.CanCellsFeedNowWith(pawn, colonist))
				{
					continue;
				}
				if (MiscUtility.TryGetAbilityJob(pawn, colonist, WVC_GenesDefOf.WVC_XaG_Cellsfeed, out Job job))
				{
					pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
					return true;
				}
				return false;
			}
			return false;
		}

		public override void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			pawns.Shuffle();
			for (int j = 0; j < pawns.Count; j++)
			{
				if (!GeneFeaturesUtility.CanCellsFeedNowWith(pawn, pawns[j]))
				{
					continue;
				}
				CompProperties_AbilityCellsfeederBite cellsfeederComponent = GetAbilityCompProperties_CellsFeeder(WVC_GenesDefOf.WVC_XaG_Cellsfeed);
				GeneFeaturesUtility.DoCellsBite(pawn, pawns[j], cellsfeederComponent.daysGain, cellsfeederComponent.cellsConsumeFactor, cellsfeederComponent.nutritionGain, new(0, 0), cellsfeederComponent.targetBloodLoss);
				break;
			}
			base.InCaravan();
		}

		public static CompProperties_AbilityCellsfeederBite GetAbilityCompProperties_CellsFeeder(AbilityDef abilityDef)
		{
			if (abilityDef?.comps != null)
			{
				foreach (AbilityCompProperties comp in abilityDef.comps)
				{
					if (comp is CompProperties_AbilityCellsfeederBite cellsFeeder)
					{
						return cellsFeeder;
					}
				}
			}
			return null;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GeneticInstability",
					action = delegate
					{
						GeneticStuff();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Reduce instability ticker",
					action = delegate
					{
						nextTick -= 30000;
					}
				};
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_Gene_GeneticThrallLabel".Translate() + ": " + GeneUiUtility.OnOrOff(useStabilizerAuto),
				defaultDesc = "WVC_XaG_Gene_GeneticThrallDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					useStabilizerAuto = !useStabilizerAuto;
					if (useStabilizerAuto)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			};
		}

		// public override void PostRemove()
		// {
			// base.PostRemove();
			// pawn.genes.AddGene(this.def, false);
		// }

	}

}
