using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene
	{

		// public XenotypeDef Xenotype => def.GetModExtension<GeneExtension_Giver>()?.xenotypeForcerDef;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// Shapeshift();
		// }

		public void Shapeshift(float chance = 0.2f)
		{
			if (Active && Rand.Chance(chance))
			{
				RandomXenotype(pawn, this, null);
			}
		}

		public void RandomXenotype(Pawn pawn, Gene gene, XenotypeDef xenotype)
		{
			// if (pawn.genes.IsXenogene(gene))
			// {
				// pawn.genes.RemoveGene(gene);
				// pawn.genes.AddGene(gene.def, false);
				// return;
			// }
			if (xenotype == null)
			{
				// List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true, true);
				List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.AllXenotypesExceptAndroids();
				if (gene.def.GetModExtension<GeneExtension_Giver>() != null && gene.def.GetModExtension<GeneExtension_Giver>().xenotypeIsInheritable)
				{
					xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef.inheritable).RandomElement();
				}
				else
				{
					xenotype = xenotypeDef.Where((XenotypeDef randomXenotypeDef) => !randomXenotypeDef.inheritable).RandomElement();
				}
				if (xenotype == null)
				{
					Log.Error("Generated gene with null xenotype. Choose random.");
					xenotype = xenotypeDef.RandomElement();
				}
			}
			if (xenotype == null)
			{
				pawn.genes.RemoveGene(gene);
				Log.Error("Xenotype is null. Do not report this to the developer, you yourself created this creepy world filled with bugs. To fix the situation, reset the filter in the " + "WVC_BiotechSettings".Translate() + " mod settings and restart the game.");
				return;
			}
			List<GeneDef> dontRemove = new();
			dontRemove.Add(gene.def);
			ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotype, dontRemove.ToList());
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// DEV
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || !MiscUtility.PawnIsColonistOrSlave(pawn, true))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_GeneShapeshifter_Label".Translate(),
				defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				disabled = pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa) || pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating),
				disabledReason = HediffDefOf.XenogermReplicating.description,
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_Shapeshifter(this));
				}
			};
		}

	}

	public class Gene_Shapeshifter_Rand : Gene_Shapeshifter
	{

		public override void PostAdd()
		{
			base.PostAdd();
			Shapeshift();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			yield break;
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(120000))
			{
				return;
			}
			Shapeshift(0.1f);
		}

	}

}