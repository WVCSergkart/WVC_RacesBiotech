using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene, IGeneOverridden
	{

		public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		// [Obsolete]
		public bool xenogermComaAfterShapeshift = true;
		public bool genesRegrowAfterShapeshift = true;

		// private ShapeshiftModeDef currentMode;

		// private List<ShapeshiftModeDef> unlockedModes;

		// public List<ShapeshiftModeDef> UnlockedModes
		// {
			// get
			// {
				// if (unlockedModes == null)
				// {
					// unlockedModes = new();
				// }
				// return unlockedModes;
			// }
		// }

		// public ShapeshiftModeDef ShiftMode => currentMode;

		// public void SetMode(ShapeshiftModeDef newMode)
		// {
			// if (currentMode != null)
			// {
				// HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			// }
			// currentMode = newMode;
			// if (currentMode != null)
			// {
				// foreach (HediffDef hediffDef in currentMode.hediffDefs)
				// {
					// HediffUtility.TryAddHediff(hediffDef, pawn, def);
				// }
			// }
		// }

		// public void UnlockMode(ShapeshiftModeDef newMode)
		// {
			// if (!unlockedModes.Contains(newMode))
			// {
				// unlockedModes.Add(newMode);
			// }
		// }

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.Spawned)
			{
				UndeadUtility.AddRandomTraitFromListWithChance(pawn, Props);
			}
			// Reset();
		}

		// HeritGenes

		// public int heritableGenesSlots = 0;
		// public List<GeneDef> heritableGenes;
		// private List<GeneDef> oldHeritableGenes;

		// public void AddHeritableGenes()
		// {
			// if (heritableGenes.NullOrEmpty())
			// {
				// return;
			// }
			// List<GeneDef> pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading);
			// foreach (GeneDef heritableGeneDef in heritableGenes)
			// {
				// if (Dialog_CreateChimera.ConflictWith(heritableGeneDef, pawnGenes))
				// {
					// continue;
				// }
				// pawn.genes.AddGene(heritableGeneDef, false);
				// oldHeritableGenes.Add(heritableGeneDef);
			// }
		// }

		// public void RemoveHeritableGenes()
		// {
			// if (oldHeritableGenes.NullOrEmpty())
			// {
				// return;
			// }
			// foreach (GeneDef heritableGeneDef in oldHeritableGenes)
			// {
				// Gene gene = pawn.genes.GetGene(heritableGeneDef);
				// if (gene != null)
				// {
					// continue;
				// }
				// pawn.genes.RemoveGene(gene);
			// }
			// oldHeritableGenes = new();
		// }

		// HeritGenes

		// public override void Reset()
		// {
			// SetMode(Props.defaultShapeMode);
			// if (!xenogermComaAfterShapeshift)
			// {
				// UnlockMode(ShapeshiftModeDefOf.WVC_Safeshift);
				// xenogermComaAfterShapeshift = true;
			// }
			// if (HediffUtility.HasAnyHediff(Props.duplicateHediffs, pawn))
			// {
				// UnlockMode(ShapeshiftModeDefOf.WVC_Duplicate);
			// }
		// }

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: AddHeritableGeneSlot",
					// action = delegate
					// {
						// heritableGenesSlots++;
					// }
				// };
			// }
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
		}

		public void RemoveHediffs()
		{
			// if (currentMode != null)
			// {
				// HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			// }
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
		}

		public override void PostRemove()
		{
			base.PostRemove();
			// RemoveHeritableGenes();
			if (WVC_Biotech.settings.shapeshifterGeneUnremovable)
			{
				pawn.genes.AddGene(this.def, false);
				UndeadUtility.TryTransferGeneStats(this, pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>());
			}
			RemoveHediffs();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Label".Translate(), xenogermComaAfterShapeshift.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Desc".Translate(), 200);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref xenogermComaAfterShapeshift, "xenogerminationComaAfterShapeshift", defaultValue: true);
			Scribe_Values.Look(ref genesRegrowAfterShapeshift, "genesRegrowAfterShapeshift", defaultValue: true);
			// Scribe_Values.Look(ref heritableGenesSlots, "heritableGenesSlots", 0);
			// Scribe_Collections.Look(ref heritableGenes, "heritableGenes", LookMode.Def);
			// Scribe_Collections.Look(ref oldHeritableGenes, "oldHeritableGenes", LookMode.Def);
		}

		public virtual void UpdateForNewGene(Gene_Shapeshifter oldShapeshifter)
		{
			xenogermComaAfterShapeshift = oldShapeshifter.xenogermComaAfterShapeshift;
			genesRegrowAfterShapeshift = oldShapeshifter.genesRegrowAfterShapeshift;
			// heritableGenesSlots = oldShapeshifter.heritableGenesSlots;
			// heritableGenes = oldShapeshifter.heritableGenes;
			// oldHeritableGenes = oldShapeshifter.oldHeritableGenes;
		}

	}

}
