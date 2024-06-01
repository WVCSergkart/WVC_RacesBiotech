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

		public bool xenogermComaAfterShapeshift = true;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.Spawned)
			{
				UndeadUtility.AddRandomTraitFromListWithChance(pawn, Props);
			}
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
			// yield return new Command_Action
			// {
				// defaultLabel = def.LabelCap,
				// defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// Find.WindowStack.Add(new Dialog_Shapeshifter(this));
				// }
			// };
			// if (pawn.Drafted)
			// {
				// yield break;
			// }
			// if (ModLister.CheckIdeology("Styling station") && WVC_Biotech.settings.shapeshifter_enableStyleButton)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "WVC_XaG_GeneShapeshifterStyles_Label".Translate(),
					// defaultDesc = "WVC_XaG_GeneShapeshifterStyles_Desc".Translate(),
					// icon = ContentFinder<Texture2D>.Get(def.iconPath),
					// action = delegate
					// {
						// Find.WindowStack.Add(new Dialog_StylingShift(pawn, this));
					// }
				// };
			// }
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediffs();
		}

		public void RemoveHediffs()
		{
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
		}

	}

	[Obsolete]
	public class Gene_Shapeshifter_Rand : Gene_Shapeshifter
	{

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// Shapeshift();
		// }

		// public void Shapeshift(float chance = 0.2f)
		// {
			// if (Active && Rand.Chance(chance))
			// {
				// RandomXenotype(pawn, this, null);
			// }
		// }

		// public void RandomXenotype(Pawn pawn, Gene gene, XenotypeDef xenotype)
		// {
			// if (xenotype == null)
			// {
				// List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.AllXenotypesExceptAndroids();
				// if (gene.def.GetModExtension<GeneExtension_Giver>() != null && gene.def.GetModExtension<GeneExtension_Giver>().xenotypeIsInheritable)
				// {
					// xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef.inheritable).RandomElement();
				// }
				// else
				// {
					// xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !randomXenotypeDef.inheritable).RandomElement();
				// }
				// if (xenotype == null)
				// {
					// Log.Error("Generated gene with null xenotype. Choose random.");
					// xenotype = xenotypeDef.RandomElement();
				// }
			// }
			// if (xenotype == null)
			// {
				// pawn.genes.RemoveGene(gene);
				// Log.Error("Xenotype is null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				// return;
			// }
			// List<GeneDef> dontRemove = new();
			// dontRemove.Add(gene.def);
			// ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotype);
		// }

		// public override void Tick()
		// {
			// base.Tick();
			// if (!pawn.IsHashIntervalTick(132520))
			// {
				// return;
			// }
			// Shapeshift(0.1f);
		// }

	}

}
