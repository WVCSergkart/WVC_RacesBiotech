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

		//private int nextTick = 1500;

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

        public void AddLimit(int count = 1)
        {
			currentLimit += count;
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

		public List<PawnGeneSetHolder> GeneSets => savedGeneSets;

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

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	ResetInterval(new IntRange(1200, 3400));
		//}

		//public override void Tick()
		//{
		//	nextTick--;
		//	if (nextTick > 0)
		//	{
		//		return;
		//	}
		//	Morph();
		//}

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public GeneExtension_Giver XenotypeGiver => pawn.genes?.Xenotype?.GetModExtension<GeneExtension_Giver>();

		public virtual bool TryMorph(PawnGeneSetHolder nextGeneSet, bool shouldMorph = false, bool removeMorpher = false)
		{
			string phase = "";
			try
            {
                phase = "switch";
                if (!shouldMorph)
                {
                    //ResetInterval(new IntRange(1200, 3400));
                    return false;
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
				if (nextGeneSet == null)
                {
                    phase = "create new form";
                    TryCreateNewForm(phase, exclude);
                }
                else
                {
                    phase = "implant saved form";
                    ImplantFromSet(nextGeneSet);
                }
                phase = "debug genes";
                UpdSkinAndHair();
                UpdToolGenes();
				ReimplanterUtility.PostImplantDebug(pawn);
				pawn.Drawer?.renderer?.SetAllGraphicsDirty();
                phase = "do effects yay";
                DoEffects();
				if (removeMorpher)
				{
					phase = "remove morpher";
					pawn?.genes?.RemoveGene(this);
				}
				//ResetInterval(new IntRange(42000, 50000));
				return true;

            }
            catch (Exception arg)
			{
				Log.Error($"Error while morphing during phase {phase}: {arg} (Gene: " + this.LabelCap + " | " + this.def.defName + ")");
				//nextTick = 60000;
			}
			return false;
		}

        private void TryCreateNewForm(string phase, List<XenotypeDef> exclude)
        {
            XenotypeHolder xenotypeHolder = null;
            if (savedGeneSets.Count < currentLimit + 1)
            {
                XenotypeDef xenotypeFromXenotype = GetRandomXenotypeFromList(XenotypeGiver?.xenotypeDefs, exclude);
                if (xenotypeFromXenotype != null)
                {
                    xenotypeHolder = new(xenotypeFromXenotype);
                }
                XenotypeDef xenotypeFromGene = GetRandomXenotypeFromList(Giver?.xenotypeDefs, exclude);
                if (xenotypeHolder == null && xenotypeFromGene != null)
                {
                    xenotypeHolder = new(xenotypeFromGene);
                }
                if (xenotypeHolder == null)
                {
                    xenotypeHolder = GetBestNewForm(def);
                }
            }
            if (xenotypeHolder != null)
            {
                Reimplant(xenotypeHolder);
            }
            else if (savedGeneSets.Where((PawnGeneSetHolder set) => set.xenotypeDef != null && set.formId != formId.Value).TryRandomElement(out PawnGeneSetHolder genesHolder))
            {
                ImplantFromSet(genesHolder);
            }
            else
            {
                Log.Error("Failed morph on phase: " + phase);
            }
        }

        private static XenotypeHolder GetBestNewForm(GeneDef geneDef)
        {
            List<XenotypeHolder> holders = ListsUtility.GetAllXenotypesHolders();
			List<XenotypeHolder> result = new();
			foreach (XenotypeHolder holder in holders)
			{
				if (holder.shouldSkip || holder.genes.NullOrEmpty())
                {
					holder.matchPercent = 0.1f;
                }
				else if (holder.genes.Contains(geneDef))
				{
					holder.matchPercent = 5f;
				}
				else if (XaG_GeneUtility.AnyGeneDefIsSubGeneOf(holder.genes, geneDef))
				{
					holder.matchPercent = 2f;
				}
				else
				{
					holder.matchPercent = 1f;
				}
				if (holder.CustomXenotype)
				{
					holder.matchPercent += 2f;
				}
			}
			if (result.TryRandomElementByWeight((XenotypeHolder holder) => holder.matchPercent.Value, out XenotypeHolder newHolder))
			{
				return newHolder;
			}
			return holders.RandomElement();
        }

        public void UpdToolGenes()
        {
            if (pawn.genes?.GenesListForReading?.Any((Gene gene) => gene is Gene_MorpherDependant) == false)
            {
                pawn.genes?.AddGene(DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef geneDef) => geneDef.prerequisite != null && geneDef.prerequisite == def).RandomElement(), pawn.genes.IsXenogene(this));
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

		public static XenotypeDef GetRandomXenotypeFromList(List<XenotypeDef> xenotypeDefs, List<XenotypeDef> exclude)
		{
			XenotypeDef xenotypeDef = null;
			if (!xenotypeDefs.NullOrEmpty())
			{
				if (exclude == null)
				{
					exclude = new();
				}
				xenotypeDefs?.Where((XenotypeDef xenos) => !exclude.Contains(xenos))?.TryRandomElement(out xenotypeDef);
			}
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
			foreach (Need need in pawn.needs.AllNeeds)
			{
				foreach (var item in pawnGeneSet.savedPawnNeeds)
				{
					if (need.def == item.Key)
					{
						need.CurLevel = item.Value;
					}
				}
			}
			pawnGeneSet.savedPawnNeeds = null;
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

		private void Reimplant(XenotypeHolder xenotypeDef)
		{
			ReimplanterUtility.SetXenotypeDirect(null, pawn, xenotypeDef.xenotypeDef, true);
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				AddGene(geneDef, xenotypeDef.inheritable);
			}
			if (xenotypeDef.CustomXenotype)
			{
				pawn.genes.SetXenotypeDirect(XenotypeDefOf.Baseliner);
				pawn.genes.xenotypeName = xenotypeDef.Label;
				pawn.genes.iconDef = xenotypeDef.iconDef;
			}
			formId = savedGeneSets.Count + 1;
			currentFormName = xenotypeDef.Label;
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
			newSet.savedPawnNeeds = new();
			foreach (Need need in pawn.needs.AllNeeds)
			{
				if (!need.def.onlyIfCausedByGene)
				{
					continue;
				}
				newSet.savedPawnNeeds[need.def] = need.CurLevel;
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
						TryMorph(null, true);
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
			//Scribe_Values.Look(ref nextTick, "nextTick", 0);
			Scribe_Collections.Look(ref savedGeneSets, "pawnGeneSets", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (savedGeneSets == null)
				{
					savedGeneSets = new();
				}
			}
		}

	}

}
