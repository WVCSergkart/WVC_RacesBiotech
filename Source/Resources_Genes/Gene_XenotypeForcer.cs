using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenotypeForcer : Gene
	{

		public XenotypeDef Xenotype => def.GetModExtension<GeneExtension_Giver>()?.xenotypeForcerDef;

		public override void PostAdd()
		{
			base.PostAdd();
			RandomXenotype(pawn, this, Xenotype);
		}

		public static void RandomXenotype(Pawn pawn, Gene gene, XenotypeDef xenotype)
		{
			if (pawn.genes.IsXenogene(gene))
			{
				pawn.genes.RemoveGene(gene);
				pawn.genes.AddGene(gene.def, false);
				return;
			}
			// bool geneIsXenogene = true;
			// List<Gene> endogenes = pawn.genes.Endogenes;
			// if (endogenes.Contains(gene))
			// {
			// geneIsXenogene = false;
			// }
			if (xenotype == null)
			{
				List<XenotypeDef> xenotypeDef = XenotypeFilterUtility.WhiteListedXenotypes(true, true);
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
			XenotypeForcer_SimpleVersion(pawn, xenotype);
			if (gene != null)
			{
				pawn.genes.RemoveGene(gene);
			}
		}

		public static void XenotypeForcer_SimpleVersion(Pawn pawn, XenotypeDef xenotype)
		{
			Pawn_GeneTracker genes = pawn.genes;
			if (!xenotype.inheritable)
			{
				for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
				{
					pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
				}
			}
			if (xenotype.inheritable)
			{
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
			}
			if (pawn.genes.Xenogenes.NullOrEmpty() && xenotype.inheritable || !xenotype.inheritable)
			{
				pawn.genes?.SetXenotypeDirect(xenotype);
				// pawn.genes.xenotypeName = xenotype.label;
				pawn.genes.iconDef = null;
			}
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			for (int i = 0; i < xenotype.genes.Count; i++)
			{
				pawn.genes?.AddGene(xenotype.genes[i], !xenotype.inheritable);
				if (xenotype.genes[i].skinColorBase != null || xenotype.genes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotype.genes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (xenotype.inheritable && !xenotypeHasSkinColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, xenogene: false);
			}
			if (xenotype.inheritable && !xenotypeHasHairColor || xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, xenogene: false);
			}
		}

	}

	public class Gene_Shapeshifter : Gene
	{

		// public XenotypeDef Xenotype => def.GetModExtension<GeneExtension_Giver>()?.xenotypeForcerDef;

		public override void PostAdd()
		{
			base.PostAdd();
			// if (pawn.genes != null && !pawn.genes.Xenogenes.NullOrEmpty() && pawn.genes.IsXenogene(this))
			// {
				// pawn.genes.RemoveGene(this);
				// pawn.genes.AddGene(this.def, false);
				// return;
			// }
			RandomXenotype(pawn, this, null);
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

}
