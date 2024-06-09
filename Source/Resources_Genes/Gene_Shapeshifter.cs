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

		[Obsolete]
		public bool xenogermComaAfterShapeshift = true;

		private ShapeshiftModeDef currentMode;

		private List<ShapeshiftModeDef> unlockedModes = new();

		public List<ShapeshiftModeDef> UnlockedModes => unlockedModes;

		public ShapeshiftModeDef ShiftMode => currentMode;

		public void SetMode(ShapeshiftModeDef newMode)
		{
			if (currentMode != null)
			{
				HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			}
			currentMode = newMode;
			// if (currentMode == null)
			// {
				// UnlockMode(Props.defaultShapeMode);
				// currentMode = Props.defaultShapeMode;
			// }
			if (currentMode != null)
			{
				foreach (HediffDef hediffDef in currentMode.hediffDefs)
				{
					HediffUtility.TryAddHediff(hediffDef, pawn, def);
				}
			}
		}

		public void UnlockMode(ShapeshiftModeDef newMode)
		{
			if (!unlockedModes.Contains(newMode))
			{
				unlockedModes.Add(newMode);
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.Spawned)
			{
				UndeadUtility.AddRandomTraitFromListWithChance(pawn, Props);
			}
			Reset();
		}

		public override void Reset()
		{
			unlockedModes = Props.initialShapeModes;
			currentMode = Props.defaultShapeMode;
			if (!xenogermComaAfterShapeshift)
			{
				UnlockMode(ShapeshiftModeDefOf.WVC_Safeshift);
				xenogermComaAfterShapeshift = true;
			}
			if (HediffUtility.HasAnyHediff(Props.duplicateHediffs, pawn))
			{
				UnlockMode(ShapeshiftModeDefOf.WVC_Duplicate);
			}
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: UnlockAllModes",
					action = delegate
					{
						foreach (ShapeshiftModeDef shapeshiftModeDef in DefDatabase<ShapeshiftModeDef>.AllDefsListForReading)
						{
							UnlockMode(shapeshiftModeDef);
						}
					}
				};
			}
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
			if (currentMode != null)
			{
				HediffUtility.RemoveHediffsFromList(pawn, currentMode.hediffDefs);
			}
			HediffUtility.RemoveHediffsFromList(pawn, Giver?.hediffDefs);
		}

		public void Notify_Override()
		{
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (WVC_Biotech.settings.shapeshifterGeneUnremovable)
			{
				pawn.genes.AddGene(this.def, false);
				UndeadUtility.TryTransferGeneStats(this, pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>());
			}
			RemoveHediffs();
		}

		// public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		// {
			// yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Label".Translate(), xenogermComaAfterShapeshift.ToStringYesNo(), "WVC_XaG_GeneShapeshifter_XenogermComaAfterShapeshift_Desc".Translate(), 200);
		// }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref xenogermComaAfterShapeshift, "xenogerminationComaAfterShapeshift", defaultValue: true);
			Scribe_Defs.Look(ref currentMode, "shapeshifterMode");
			Scribe_Collections.Look(ref unlockedModes, "unlockedModes", LookMode.Def);
		}

	}

}
