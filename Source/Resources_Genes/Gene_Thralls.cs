using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
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

	// public class Gene_ThrallMaster : Gene_ThrallMaker
	// {

		// private List<Pawn> controlledThralls = new();

		// public List<Pawn> ControlledThralls => controlledThralls;

		// private int cachedThrallsCount;

		// private int cachedTotalThrallBondConsumed = -1;

		// public int TotalThrallBondConsumed => GetAllThrallsBondCost();

		// public void Notify_RemoveThrall(Pawn thrall)
		// {
			// controlledThralls.Remove(thrall);
		// }

		// public void Notify_AddThrall(Pawn thrall)
		// {
			// controlledThralls.Add(thrall);
		// }

		// private int GetAllThrallsBondCost()
		// {
			// if (cachedThrallsCount == ControlledThralls.Count && cachedTotalThrallBondConsumed != -1)
			// {
				// return cachedTotalThrallBondConsumed;
			// }
			// cachedThrallsCount = ControlledThralls.Count;
			// cachedTotalThrallBondConsumed = 0;
			// foreach (Pawn item in ControlledThralls)
			// {
				// cachedTotalThrallBondConsumed += XaG_GeneUtility.GetPawn_Arc(item);
			// }
			// return cachedTotalThrallBondConsumed;
		// }

		// public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		// {
			// base.Notify_PawnDied(dinfo, culprit);
			// RemoveAllThralls();
		// }

		// public override void PostRemove()
		// {
			// base.PostRemove();
			// RemoveAllThralls();
		// }

		// public void RemoveAllThralls()
		// {
			// foreach (Pawn item in ControlledThralls)
			// {
				// Gene_Thrall geneThrall = item?.genes?.GetFirstGeneOfType<Gene_Thrall>();
				// if (geneThrall != null)
				// {
					// geneThrall.SetMaster(null);
				// }
			// }
		// }

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Collections.Look(ref controlledThralls, "controlledThralls", LookMode.Reference);
		// }

	// }

	// public class Gene_Thrall : Gene
	// {

		// public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		// private Pawn master;

		// private Hediff penaltyHediff;

		// public Pawn Master => master;

		// [Unsaved(false)]
		// private Gene_ThrallMaster cachedThrallMasterGene;

		// public Gene_ThrallMaster ThrallMaster
		// {
			// get
			// {
				// if (cachedThrallMasterGene == null || !cachedThrallMasterGene.Active)
				// {
					// cachedThrallMasterGene = master?.genes?.GetFirstGeneOfType<Gene_ThrallMaster>();
				// }
				// return cachedThrallMasterGene;
			// }
		// }

		// public override void Tick()
		// {
			// if (pawn.IsHashIntervalTick(1286))
			// {
				// if (ThrallMaster == null)
				// {
					// Notify_ChangeMaster();
					// return;
				// }
				// if (pawn.Faction != Faction.OfPlayer || Master?.Faction != Faction.OfPlayer)
				// {
					// Notify_ChangeMaster();
					// return;
				// }
				// RemoveHediff();
			// }
			// if (pawn.IsHashIntervalTick(38292))
			// {
				// if (!HasEnoughGolembond())
				// {
					// Notify_ChangeMaster();
					// return;
				// }
				// RemoveHediff(true);
			// }
		// }

		// public void Notify_ChangeMaster(bool giveHediff = true)
		// {
			// if (ThrallMaster != null)
			// {
				// ThrallMaster.Notify_RemoveThrall(pawn);
			// }
			// if (giveHediff)
			// {
				// AddHediff();
			// }
		// }

		// private void AddHediff()
		// {
			// HediffDef hediffDef = Giver?.hediffDefName;
			// if (hediffDef == null)
			// {
				// return;
			// }
			// if (penaltyHediff == null)
			// {
				// Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
				// HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
				// if (hediff_GeneCheck != null)
				// {
					// hediff_GeneCheck.geneDef = this.def;
				// }
				// pawn.health.AddHediff(hediff);
			// }
		// }

		// public bool HasEnoughGolembond()
		// {
			// if (ThrallMaster != null || Master != null)
			// {
				// return MechanoidsUtility.TotalGolembond(Master) >= ThrallMaster.TotalThrallBondConsumed;
			// }
			// return false;
		// }

		// private void RemoveHediff(bool extraRecovery = false)
		// {
			// if (!HasEnoughGolembond())
			// {
				// return;
			// }
			// if (extraRecovery)
			// {
				// penaltyHediff = pawn?.health?.hediffSet?.GetFirstHediffOfDef(Giver?.hediffDefName);
			// }
			// if (penaltyHediff != null)
			// {
				// pawn.health.RemoveHediff(penaltyHediff);
			// }
		// }

		// public void SetMaster(Pawn thrallMaster)
		// {
			// if (ThrallMaster != null)
			// {
				// ThrallMaster?.Notify_RemoveThrall(pawn);
			// }
			// master = thrallMaster;
			// if (thrallMaster != null)
			// {
				// ThrallMaster?.Notify_AddThrall(pawn);
			// }
		// }

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			// {
				// yield break;
			// }
			// if (ThrallMaster == null)
			// {
				// yield break;
			// }
			// yield return new Command_Action
			// {
				// defaultLabel = "WVC_XaG_RemoveThrallConnectionLabel".Translate(),
				// defaultDesc = "WVC_XaG_RemoveThrallConnectionDesc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// ThrallMaster?.Notify_RemoveThrall(pawn);
				// }
			// };
		// }

		// public override void PostRemove()
		// {
			// base.PostRemove();
			// Notify_ChangeMaster(false);
		// }

		// public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		// {
			// base.Notify_PawnDied(dinfo, culprit);
			// Notify_ChangeMaster(false);
		// }

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_References.Look(ref master, "master");
			// Scribe_References.Look(ref penaltyHediff, "penaltyHediff");
		// }

	// }

}
