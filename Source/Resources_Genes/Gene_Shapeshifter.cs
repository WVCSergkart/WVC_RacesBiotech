using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Shapeshifter : Gene
	{

		public GeneExtension_Shapeshifter Props => def?.GetModExtension<GeneExtension_Shapeshifter>();

		public bool xenogermComaAfterShapeshift = true;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.Spawned)
			{
				UndeadUtility.AddRandomTraitFromListWithChance(pawn, Props);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || !MiscUtility.PawnIsColonistOrSlave(pawn, true))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap,
				defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				// disabled = pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa) || pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating),
				// disabledReason = HediffDefOf.XenogermReplicating.description,
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_Shapeshifter(this));
				}
			};
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (WVC_Biotech.settings.shapeshifterGeneUnremovable)
			{
				pawn.genes.AddGene(this.def, false);
			}
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

	public class Gene_Shapeshifter_Rand : Gene_Shapeshifter
	{

		public override void PostAdd()
		{
			base.PostAdd();
			Shapeshift();
		}

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
