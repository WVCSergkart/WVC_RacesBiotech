using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Morpher : Gene
	{

		//public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		private int nextTick = 1500;

		public List<PawnGeneSetHolder> pawnGeneSets = new();

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new IntRange(1200, 3400));
		}

		public override void Tick()
		{
			// base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			Morph();
		}

		public virtual void Morph(bool forced = false)
		{
			string phase = "";
			try
			{
				phase = "switch";
				if (!forced && !TryNocturnalSwitch())
				{
					ResetInterval(new IntRange(1200, 3400));
					return;
				}
				phase = "exclude list";
				List<XenotypeDef> exclude = new() { pawn.genes.Xenotype };
				if (pawnGeneSets.NullOrEmpty())
				{
					pawnGeneSets = new();
				}
				else
				{
					foreach (PawnGeneSetHolder set in pawnGeneSets)
					{
						exclude.Add(set.xenotypeDef);
					}
				}
				phase = "save old gene set";
				SaveGenes();
				phase = "get xenotype from pawn xenotype";
				XenotypeDef xenotypeDef = null;
				xenotypeDef = GetRandomXenotypeFromList(pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.xenotypeDefs, exclude, xenotypeDef);
				if (xenotypeDef == null && Giver != null)
				{
					phase = "get xenotype from gene xenotypes list";
					xenotypeDef = GetRandomXenotypeFromList(Giver?.xenotypeDefs, exclude, xenotypeDef);
				}
				if (xenotypeDef != null)
				{
					phase = "implant new xenotype";
					Reimplant(xenotypeDef);
				}
				else
				{
					phase = "implant saved xenotype";
					ImplantFromSet(pawnGeneSets.Where((PawnGeneSetHolder set) => set.xenotypeDef != null && set.xenotypeDef != pawn.genes.Xenotype).RandomElement());
				}
				pawn.Drawer?.renderer?.SetAllGraphicsDirty();
				phase = "do effects yay";
				DoEffects();
				ResetInterval(new IntRange(3200, 5000));

			}
			catch (Exception arg)
			{
				Log.Error($"Error while morphing during phase {phase}: {arg} (Gene: " + this.LabelCap + " | " + this.def.defName + ")");
				nextTick = 60000;
			}
        }

        public static XenotypeDef GetRandomXenotypeFromList(List<XenotypeDef> xenotypeDefs, List<XenotypeDef> exclude, XenotypeDef xenotypeDef)
		{
			if (xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			if (exclude == null)
			{
				exclude = new();
			}
			xenotypeDefs?.Where((XenotypeDef xenos) => !exclude.Contains(xenos))?.TryRandomElement(out xenotypeDef);
            return xenotypeDef;
        }

        public void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
		}

		private void ImplantFromSet(PawnGeneSetHolder pawnGeneSet)
		{
			ReimplanterUtility.SetXenotypeDirect(null, pawn, pawnGeneSet.xenotypeDef, true);
			foreach (Gene gene in pawnGeneSet.endogenes)
			{
				AddGene(gene.def, gene, true);
			}
			foreach (Gene gene in pawnGeneSet.xenogenes)
			{
				AddGene(gene.def, gene, false);
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
			pawnGeneSets.Remove(pawnGeneSet);
		}

		private void Reimplant(XenotypeDef xenotypeDef)
		{
			ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef, true);
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				AddGene(geneDef, null, xenotypeDef.inheritable);
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		}

		public void SaveGenes()
		{
			//if (pawnGeneSets.Where((PawnGeneSetHolder holder) => holder.xenotypeDef == pawn.genes.Xenotype).Any())
			//{
			//	return;
			//}
			PawnGeneSetHolder newSet = new();
			newSet.xenotypeDef = pawn.genes.Xenotype;
			newSet.xenogenes = new();
			newSet.endogenes = new();
			foreach (Gene gene in pawn.genes.Endogenes.ToList())
			{
				if (gene != this)
				{
					newSet.endogenes.Add(gene);
				}
			}
			foreach (Gene gene in pawn.genes.Endogenes.ToList())
			{
				RemoveGene(gene);
			}
			foreach (Gene gene in pawn.genes.Xenogenes.ToList())
			{
				if (gene != this)
				{
					newSet.xenogenes.Add(gene);
				}
			}
			foreach (Gene gene in pawn.genes.Xenogenes.ToList())
			{
				RemoveGene(gene);
			}
			pawnGeneSets.Add(newSet);
		}

		public virtual void AddGene(GeneDef geneDef, Gene gene, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
			}
			else
			{
				return;
			}
			if (gene != null)
			{
				if (inheritable)
				{
					pawn.genes.Endogenes.Remove(pawn.genes.GetGene(geneDef));
					pawn.genes.Endogenes.Add(gene);
				}
				else
				{
					pawn.genes.Xenogenes.Remove(pawn.genes.GetGene(geneDef));
					pawn.genes.Xenogenes.Add(gene);
				}
			}
		}

		public virtual void RemoveGene(Gene gene)
		{
			if (gene != this)
			{
				pawn?.genes?.RemoveGene(gene);
			}
		}

		private void ResetInterval(IntRange range)
        {
            nextTick = range.RandomInRange;
        }

		//public bool TryDamageSwitch()
  //      {
  //          if (pawn.genes.GetFirstGeneOfType<Gene_Morpher>() != null)
  //          {
  //              float summaryHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
  //              if (summaryHealthPercent < 0.8f)
  //              {
		//			return true;
  //              }
  //          }
		//	return false;
  //      }

		//public bool TrySeasonsSwitch()
  //      {
  //          if (pawn.genes.GetFirstGeneOfType<Gene_Morpher>() != null)
  //          {
  //              float num2 = GenLocalDate.DayOfYear(pawn);
  //              if (num2 > 31f && num2 < 60f)
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

        public bool TryNocturnalSwitch()
		{
			float num = GenLocalDate.DayTick(pawn);
			if (num > 45000f || num < 15000f)
			{
				return true;
			}
			return false;
		}

        public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryMorph",
					action = delegate
					{
						Morph(true);
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref pawnGeneSets, "pawnGeneSets", LookMode.Deep);
		}

	}

}
