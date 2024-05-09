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

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap,
				defaultDesc = "WVC_XaG_GeneShapeshifter_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_Shapeshifter(this));
				}
			};
			if (ModLister.CheckIdeology("Styling station") && WVC_Biotech.settings.shapeshifter_enableStyleButton)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_XaG_GeneShapeshifterStyles_Label".Translate(),
					defaultDesc = "WVC_XaG_GeneShapeshifterStyles_Desc".Translate(),
					icon = ContentFinder<Texture2D>.Get(def.iconPath),
					action = delegate
					{
						Find.WindowStack.Add(new Dialog_StylingShift(pawn, this));
					}
				};
			}
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

	public class Gene_Chimera : Gene
	{

		public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		private List<GeneDef> stolenGenes = new();

		private List<GeneDef> eatedGenes = new();

		public List<GeneDef> AllGenes
		{
			get
			{
				List<GeneDef> genes = new();
				genes.AddRange(eatedGenes);
				genes.AddRange(stolenGenes);
				return genes;
			}
		}

		public List<GeneDef> EatedGenes => eatedGenes;
		public List<GeneDef> StolenGenes => stolenGenes;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.Spawned)
			{
				return;
			}
			List<GeneDef> geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;
			while (stolenGenes.Count < 5)
			{
				if (geneDefs.Where((GeneDef x) => x.endogeneCategory == EndogeneCategory.None && x.selectionWeight > 0f && x.canGenerateInGeneSet && x.passOnDirectly && !AllGenes.Contains(x) && x.biostatArc == 0).TryRandomElementByWeight((GeneDef gene) => gene.selectionWeight, out GeneDef result))
				{
					AddGene(result);
				}
			}
		}

		private Gizmo genesGizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (genesGizmo == null)
			{
				genesGizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return genesGizmo;
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap,
				defaultDesc = "WVC_XaG_GeneGeneticThief_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_GeneticThief(this));
				}
			};
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: GetRandomGene",
					action = delegate
					{
						AddGene(DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.endogeneCategory == EndogeneCategory.None && x.selectionWeight > 0f && x.canGenerateInGeneSet && x.passOnDirectly && !AllGenes.Contains(x)).RandomElement());
					}
				};
			}
		}

		public void AddGene(GeneDef geneDef)
		{
			if (!stolenGenes.Contains(geneDef))
			{
				stolenGenes.Add(geneDef);
			}
		}

		public void EatGene(GeneDef geneDef)
		{
			if (!eatedGenes.Contains(geneDef))
			{
				eatedGenes.Add(geneDef);
				RemoveGene(geneDef);
			}
		}
		public void RemoveGene(GeneDef geneDef)
		{
			if (stolenGenes.Contains(geneDef))
			{
				stolenGenes.Remove(geneDef);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref eatedGenes, "eatedGenes", LookMode.Def);
			Scribe_Collections.Look(ref stolenGenes, "stolenGenes", LookMode.Def);
		}

		// public static float GetGeneWeight(GeneDef geneDef)
		// {
			// float weight = 1f / (geneDef.biostatCpx + geneDef.biostatMet + geneDef.biostatArc + 1f);
			// if (weight < 0f)
			// {
				// weight *= -1;
			// }
			// if (weight == 0f)
			// {
				// weight += 1f;
			// }
			// return weight;
		// }

		// public void Notify_PostStart(Gene_Shapeshifter shapeshiftGene)
		// {
		// }

		// public void Notify_PostShapeshift(Gene_Shapeshifter newShapeshiftGene)
		// {
			// foreach (Gene gene in pawn.genes.GenesListForReading)
			// {
				// AddGene(gene.def);
			// }
		// }

	}

	public class Gene_BloodChimera : Gene_Chimera, IGeneBloodfeeder
	{

		public void Notify_Bloodfeed(Pawn victim)
		{
			List<Gene> genes = victim?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				return;
			}
			if (genes.Where((Gene x) => x.def.selectionWeight > 0f && x.def.canGenerateInGeneSet && x.def.passOnDirectly && !AllGenes.Contains(x.def)).TryRandomElementByWeight((Gene gene) => gene.def.selectionWeight, out Gene result))
			{
				AddGene(result.def);
				Messages.Message("WVC_XaG_GeneGeneticThief_GeneCopied".Translate(pawn.NameShortColored, result.Label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

	}

}
