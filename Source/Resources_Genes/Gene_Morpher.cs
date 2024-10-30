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

		private int currentLimit = 1;

		public int CurrentLimit => currentLimit;
		public int FormsCount
		{
			get
			{
				if (savedGeneSets != null)
				{
					return savedGeneSets.Count;
				}
				return 1;
			}
		}

		public List<PawnGeneSetHolder> GetGeneSets()
		{
			if (savedGeneSets == null)
			{
				savedGeneSets = new();
			}
			return savedGeneSets;
		}

		private string currentFormName = null;
		private int? formId;

		private List<PawnGeneSetHolder> savedGeneSets = new();

		public override string LabelCap
		{
			get
			{
				if (currentFormName.NullOrEmpty())
				{
					return base.LabelCap;
				}
				return base.LabelCap + " (" + currentFormName.CapitalizeFirst() + ")";
			}
		}

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
				if (savedGeneSets.NullOrEmpty())
				{
					savedGeneSets = new();
				}
				else
				{
					foreach (PawnGeneSetHolder set in savedGeneSets)
					{
						exclude.Add(set.xenotypeDef);
					}
				}
				phase = "save old gene set";
				SaveGenes();
				phase = "get xenotype";
				XenotypeDef xenotypeDef = null;
				if (savedGeneSets.Count < currentLimit + 1)
				{
					phase = "get xenotype from pawn xenotype";
					xenotypeDef = GetRandomXenotypeFromList(pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>()?.xenotypeDefs, exclude, xenotypeDef);
					if (xenotypeDef == null && Giver != null)
					{
						phase = "get xenotype from gene xenotypes list";
						xenotypeDef = GetRandomXenotypeFromList(Giver?.xenotypeDefs, exclude, xenotypeDef);
					}
					if (xenotypeDef == null)
					{
						phase = "get random xenotype from white list";
						xenotypeDef = ListsUtility.GetWhiteListedXenotypes().RandomElement();
					}
				}
				if (xenotypeDef != null)
				{
					phase = "implant new xenotype";
					Reimplant(xenotypeDef);
				}
				else if (savedGeneSets.Where((PawnGeneSetHolder set) => set.xenotypeDef != null && set.formId != formId.Value).TryRandomElement(out PawnGeneSetHolder genesHolder))
				{
					phase = "implant saved xenotype";
					ImplantFromSet(genesHolder);
				}
				else
				{
					Log.Error("Failed morph on phase: " + phase);
				}
				phase = "debug genes";
				UpdSkinAndHair();
				ReimplanterUtility.PostImplantDebug(pawn);
				pawn.Drawer?.renderer?.SetAllGraphicsDirty();
				phase = "do effects yay";
				DoEffects();
				ResetInterval(new IntRange(42000, 50000));

			}
			catch (Exception arg)
			{
				Log.Error($"Error while morphing during phase {phase}: {arg} (Gene: " + this.LabelCap + " | " + this.def.defName + ")");
				nextTick = 60000;
			}
		}

		private void UpdSkinAndHair()
		{
			if (!savedGeneSets.NullOrEmpty())
			{
				ReimplanterUtility.FindSkinAndHairGenes(pawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor);
				if (!xenotypeHasSkinColor)
				{
					GeneDef skinDef = savedGeneSets?.FirstOrDefault()?.endogenes.Where((Gene gene) => gene.def.skinColorBase != null || gene.def.skinColorOverride != null)?.ToList()?.First()?.def;
					if (skinDef != null)
					{
						recipientGenes?.AddGene(skinDef, false);
					}
				}
				if (!xenotypeHasHairColor)
				{
					GeneDef hairDef = savedGeneSets?.FirstOrDefault()?.endogenes?.Where((Gene gene) => gene.def.hairColorOverride != null)?.ToList()?.First()?.def;
					if (hairDef != null)
					{
						recipientGenes?.AddGene(hairDef, false);
					}
				}
			}
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
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
			if (pawn.genes.Xenotype == XenotypeDefOf.Baseliner)
			{
				pawn.genes.xenotypeName = pawnGeneSet.name;
				pawn.genes.iconDef = pawnGeneSet.iconDef;
			}
			foreach (Gene gene in pawnGeneSet.endogenes)
			{
				AddGene(gene.def, true);
			}
			foreach (Gene gene in pawnGeneSet.xenogenes)
			{
				AddGene(gene.def, false);
			}
			CopyGenesID(pawnGeneSet.endogenes, pawn.genes.Endogenes);
			CopyGenesID(pawnGeneSet.xenogenes, pawn.genes.Xenogenes);
			//DuplicateUtility.CopyGenesOverrides(pawn, pawn.genes.Endogenes, tmpOldEndogenes);
			//DuplicateUtility.CopyGenesOverrides(pawn, pawn.genes.Xenogenes, tmpOldXenogenes);
			currentFormName = pawnGeneSet.name;
			formId = pawnGeneSet.formId;
			savedGeneSets.Remove(pawnGeneSet);
		}

		public void CopyGenesID(List<Gene> newGenes, List<Gene> sourceGenes)
		{
			int i;
			for (i = 0; i < sourceGenes.Count; i++)
			{
				//Gene gene = newGenes[i];
				Gene sourceGene = sourceGenes[i];
				try
				{
					if (sourceGene == this || !newGenes.Where((Gene e) => e.def == sourceGene.def).TryRandomElement(out Gene gene))
					{
						continue;
					}
					gene.loadID = sourceGene.loadID;
					gene.overriddenByGene = sourceGene.overriddenByGene;
					sourceGenes.Remove(sourceGene);
					sourceGenes.Add(gene);
				}
				catch (Exception arg)
				{
					Log.Warning("Failed copy gene: " + sourceGene.LabelCap + ". Reason: " + arg);
				}
			}
		}

		private void Reimplant(XenotypeDef xenotypeDef)
		{
			ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef, true);
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				AddGene(geneDef, xenotypeDef.inheritable);
			}
			formId = savedGeneSets.Count + 1;
			currentFormName = xenotypeDef.label;
		}

		public void SaveGenes()
		{
			//if (pawnGeneSets.Where((PawnGeneSetHolder holder) => holder.xenotypeDef == pawn.genes.Xenotype).Any())
			//{
			//	return;
			//}
			PawnGeneSetHolder newSet = new();
			if (!formId.HasValue)
			{
				formId = savedGeneSets.Count + 1;
			}
			newSet.formId = formId.Value;
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
			foreach (Gene gene in newSet.endogenes)
			{
				gene.overriddenByGene = this;
			}
			foreach (Gene gene in newSet.xenogenes)
			{
				gene.overriddenByGene = this;
			}
			newSet.name = pawn.genes.xenotypeName;
			if (newSet.name.NullOrEmpty())
			{
				newSet.name = newSet.xenotypeDef.label;
			}
			if (pawn.genes.UniqueXenotype)
			{
				newSet.iconDef = pawn.genes.iconDef;
			}
			savedGeneSets.Add(newSet);
		}

		public virtual void AddGene(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
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
  //	  {
  //		  if (pawn.genes.GetFirstGeneOfType<Gene_Morpher>() != null)
  //		  {
  //			  float summaryHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
  //			  if (summaryHealthPercent < 0.8f)
  //			  {
		//			return true;
  //			  }
  //		  }
		//	return false;
  //	  }

		//public bool TrySeasonsSwitch()
  //	  {
  //		  if (pawn.genes.GetFirstGeneOfType<Gene_Morpher>() != null)
  //		  {
  //			  float num2 = GenLocalDate.DayOfYear(pawn);
  //			  if (num2 > 31f && num2 < 60f)
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

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
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
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref formId, "formId");
			Scribe_Values.Look(ref currentFormName, "currentFormName");
			Scribe_Values.Look(ref currentLimit, "currentLimit", 1);
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
			Scribe_Collections.Look(ref savedGeneSets, "pawnGeneSets", LookMode.Deep);
		}

	}

}
