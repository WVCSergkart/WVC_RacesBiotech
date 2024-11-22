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

		private bool? cachedOneTimeMorpher;

		public virtual bool IsOneTime
        {
			get
            {
				if (!cachedOneTimeMorpher.HasValue)
                {
					cachedOneTimeMorpher = pawn.genes?.GetFirstGeneOfType<Gene_MorpherOneTimeUse>() != null;
				}
				return cachedOneTimeMorpher.Value;

			}
        }

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

		//public List<PawnGeneSetHolder> GetGeneSets()
		//{
		//	if (savedGeneSets == null)
		//	{
		//		savedGeneSets = new();
		//	}
		//	return savedGeneSets;
		//}

		private string currentFormName = null;
		private int? formId;

		private List<PawnGeneSetHolder> savedGeneSets = new();

		public void AddSetHolder(PawnGeneSetHolder newSet)
		{
			if (!savedGeneSets.Contains(newSet))
			{
				savedGeneSets.Add(newSet);
			}
		}

		public void RemoveSetHolder(PawnGeneSetHolder oldSet)
		{
			if (savedGeneSets.Contains(oldSet))
			{
				savedGeneSets.Remove(oldSet);
			}
		}

		public void ResetAllSetHolders()
		{
			savedGeneSets = new();
		}

		public void TransferHolders(Gene_Morpher oldMorpher, Gene_Morpher newMorpher)
		{
			foreach (PawnGeneSetHolder holder in oldMorpher.savedGeneSets.ToList())
			{
				newMorpher.AddSetHolder(holder);
				oldMorpher.RemoveSetHolder(holder);
			}
			oldMorpher.ResetAllSetHolders();
		}

        public List<PawnGeneSetHolder> SavedGeneSets
        {
            get
			{
				if (savedGeneSets == null)
				{
					savedGeneSets = new();
				}
				return savedGeneSets;
            }
        }

        //public override string LabelCap
        //{
        //	get
        //	{
        //		if (currentFormName.NullOrEmpty())
        //		{
        //			return base.LabelCap;
        //		}
        //		return base.LabelCap + " (" + currentFormName.CapitalizeFirst() + ")";
        //	}
        //}

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
					LoadGenes(nextGeneSet);
				}
				phase = "debug genes";
				UpdSkinAndHair();
				UpdToolGenes();
				ReimplanterUtility.PostImplantDebug(pawn);
				phase = "post morph";
				//PostMorph();
				pawn.Drawer?.renderer?.SetAllGraphicsDirty();
				phase = "do effects yay";
				DoEffects(pawn);
				if (removeMorpher)
				{
					phase = "remove morpher :(";
					RemoveMorpher();
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
				if (XenotypeGiver != null)
				{
					if (!XenotypeGiver.morpherXenotypeDefs.NullOrEmpty())
					{
						xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromList(XenotypeGiver?.morpherXenotypeDefs, exclude));
					}
					else if (!XenotypeGiver.morpherXenotypeChances.NullOrEmpty())
					{
						xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(XenotypeGiver?.morpherXenotypeChances, exclude));
					}
				}
				if (xenotypeHolder == null && Giver != null)
				{
					if (!Giver.morpherXenotypeDefs.NullOrEmpty())
					{
						xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromList(Giver?.morpherXenotypeDefs, exclude));
					}
					else if (!Giver.morpherXenotypeChances.NullOrEmpty())
					{
						xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(Giver?.morpherXenotypeChances, exclude));
					}
				}
				if (xenotypeHolder == null)
				{
					xenotypeHolder = GetBestNewForm(this);
				}
			}
			if (xenotypeHolder != null)
			{
				Reimplant(xenotypeHolder);
			}
			else if (savedGeneSets.Where((PawnGeneSetHolder set) => set.xenotypeDef != null && set.formId != formId.Value).TryRandomElement(out PawnGeneSetHolder genesHolder))
			{
				LoadGenes(genesHolder);
			}
			else
			{
				Log.Error("Failed morph on phase: " + phase);
			}
		}

		public static XenotypeHolder GetBestNewForm(Gene gene)
		{
			List<XenotypeHolder> holders = ListsUtility.GetAllXenotypesHolders().Where((XenotypeHolder holder) => !holder.genes.HasGeneDefOfType<Gene_MorpherOneTimeUse>()).ToList();
			List<XenotypeHolder> result = new();
			foreach (XenotypeHolder holder in holders)
			{
				if (holder.shouldSkip || holder.genes.NullOrEmpty())
				{
					holder.matchPercent = 0.1f;
				}
				else if (holder.genes.HasGeneDefOfType<Gene_MorpherDependant>())
				{
					holder.matchPercent = 8f;
				}
				else if (holder.genes.HasGeneDefOfType<Gene_Morpher>())
				{
					holder.matchPercent = 4f;
				}
				else
				{
					holder.matchPercent = 1f;
				}
				if (holder.CustomXenotype)
				{
					holder.matchPercent += 2f;
				}
				XaG_GeneUtility.GetBiostatsFromList(holder.genes, out _, out int met, out int arc);
				if (arc > 0)
				{
					holder.matchPercent = (float)Math.Round((float)(holder.matchPercent.Value / (arc * 0.5f)), 2);
				}
				else
				{
					holder.matchPercent *= 1.2f;
				}
				if (met < 0)
				{
					holder.matchPercent *= 0.5f;
				}
				bool xenogene = gene.pawn.genes.IsXenogene(gene);
				if (xenogene && !holder.inheritable)
				{
					holder.matchPercent *= 10f;
				}
				else if (!xenogene && holder.inheritable)
				{
					holder.matchPercent *= 10f;
				}
				result.Add(holder);
			}
			//Log.Error("Xenotypes weights:" + "\n" + result.Select((XenotypeHolder x) => x.LabelCap + ": " + x.matchPercent).ToLineList(" - "));
			if (result.TryRandomElementByWeight((XenotypeHolder holder) => holder.matchPercent.Value, out XenotypeHolder newHolder))
			{
				return newHolder;
			}
			Log.Error("Failed get best holder. Trying randomize.");
			return holders.RandomElement();
		}

		//public void PostMorph()
		//{
		//	GeneResourceUtility.UpdMetabolism(pawn);
		//}

		//public GeneDef nextGeneTool = null;

		public void UpdToolGenes(bool forced = false, GeneDef nextGeneTool = null)
		{
			if (forced || pawn.genes?.GenesListForReading?.Any((Gene gene) => gene is Gene_MorpherTrigger) == false)
			{
				bool xenogene = pawn.genes.IsXenogene(this);
				if (nextGeneTool != null)
				{
					AddToolGene(nextGeneTool, xenogene);
					return;
				}
				GeneDef geneDef = DefDatabase<GeneDef>.GetNamed("WVC_MorphCondition_Deathrest");
				if (XenotypeGiver?.morpherTriggerGene != null)
				{
					AddToolGene(XenotypeGiver.morpherTriggerGene, xenogene);
				}
				else if (geneDef != null && pawn.needs?.TryGetNeed<Need_Deathrest>() != null)
				{
					AddToolGene(geneDef, xenogene);
				}
				else if (Giver != null && !Giver.morpherTriggerGenes.NullOrEmpty())
				{
					AddToolGene(Giver.morpherTriggerGenes.RandomElement(), xenogene);
				}
				else
				{
					Log.Error("Failed find morpherTriggerGene in xenotypeDef or geneDef.");
					//AddToolGene(DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef geneDef) => geneDef.prerequisite != null && geneDef.prerequisite == def).RandomElement(), xenogene);
				}
			}
		}

		private void UpdSkinAndHair()
		{
			//Log.Error("1");
			if (!savedGeneSets.NullOrEmpty())
			{
				//Log.Error("2");
				ReimplanterUtility.FindSkinAndHairGenes(pawn, out Pawn_GeneTracker recipientGenes, out bool xenotypeHasSkinColor, out bool xenotypeHasHairColor);
				if (!xenotypeHasSkinColor)
				{
					//Log.Error("2.1");
					//GeneDef skinDef = savedGeneSets?.FirstOrDefault()?.endogenes?.Where((Gene gene) => gene.def.skinColorBase != null || gene.def.skinColorOverride != null)?.ToList()?.First()?.def ?? savedGeneSets?.FirstOrDefault()?.endogeneDefs?.Where((GeneDef gene) => gene.skinColorBase != null || gene.skinColorOverride != null)?.ToList()?.First();
					GeneDef skinDef = GetSkinDef();
					if (skinDef != null)
					{
						//Log.Error("2.2");
						recipientGenes?.AddGene(skinDef, false);
					}
				}
				//Log.Error("3");
				if (!xenotypeHasHairColor)
				{
					//Log.Error("3.1");
					//GeneDef hairDef = savedGeneSets?.FirstOrDefault()?.endogenes?.Where((Gene gene) => gene.def.hairColorOverride != null)?.ToList()?.First()?.def ?? savedGeneSets?.FirstOrDefault()?.endogeneDefs?.Where((GeneDef gene) => gene.hairColorOverride != null)?.ToList()?.First();
					GeneDef hairDef = GetHairDef();
					if (hairDef != null)
					{
						//Log.Error("3.2");
						recipientGenes?.AddGene(hairDef, false);
					}
				}
				//Log.Error("4");
			}
			//Log.Error("5");
			ReimplanterUtility.TrySetSkinAndHairGenes(pawn);
		}

		private GeneDef GetHairDef()
		{
			PawnGeneSetHolder geneSet = savedGeneSets.First();
			if (geneSet == null)
			{
				Log.Error("PawnGeneSetHolder is null, but morph call debug sequence.");
				return null;
			}
			if (!geneSet.endogenes.NullOrEmpty())
			{
				List<Gene> geneDefs = geneSet.endogenes?.Where((Gene gene) => gene.def.hairColorOverride != null)?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First().def;
			}
			if (!geneSet.endogeneDefs.NullOrEmpty())
			{
				List<GeneDef> geneDefs = geneSet.endogeneDefs?.Where((GeneDef gene) => gene.hairColorOverride != null)?.ToList()?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First();
			}
			Log.Error("PawnGeneSetHolder saved genes is null, but morph call debug sequence.");
			return null;
		}

		private GeneDef GetSkinDef()
		{
			PawnGeneSetHolder geneSet = savedGeneSets.First();
			if (geneSet == null)
			{
				Log.Error("PawnGeneSetHolder is null, but morph call debug sequence.");
				return null;
			}
			if (!geneSet.endogenes.NullOrEmpty())
			{
				List<Gene> geneDefs = geneSet.endogenes?.Where((Gene gene) => gene.def.skinColorBase != null || gene.def.skinColorOverride != null)?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First().def;
			}
			if (!geneSet.endogeneDefs.NullOrEmpty())
			{
				List<GeneDef> geneDefs = geneSet.endogeneDefs?.Where((GeneDef gene) => gene.skinColorBase != null || gene.skinColorOverride != null)?.ToList();
				if (geneDefs.NullOrEmpty())
				{
					return null;
				}
				return geneDefs.First();
			}
			Log.Error("PawnGeneSetHolder saved genes is null, but morph call debug sequence.");
			return null;
		}

		public void DoEffects(Pawn pawn)
		{
			if (pawn.Map == null)
			{
				return;
			}
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
		}

		private void SaveGenes()
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
			//pawn.needs.AllNeeds.RemoveAll((Need need) => need.def.onlyIfCausedByGene);
			newSet.formId = formId.Value;
			newSet.xenotypeDef = pawn.genes.Xenotype;
			newSet.SaveGenes(pawn, this);
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

		private void LoadGenes(PawnGeneSetHolder pawnGeneSet)
        {
            ReimplanterUtility.SetXenotypeDirect(null, pawn, pawnGeneSet.xenotypeDef, true);
            if (pawn.genes.Xenotype == XenotypeDefOf.Baseliner)
            {
                pawn.genes.xenotypeName = pawnGeneSet.name;
                pawn.genes.iconDef = pawnGeneSet.iconDef;
            }
			pawnGeneSet.LoadGenes(pawn, this);
            //CopyGenesID(pawnGeneSet.endogenes, pawn.genes.Endogenes);
            //CopyGenesID(pawnGeneSet.xenogenes, pawn.genes.Xenogenes);
            //pawn.needs.AddOrRemoveNeedsAsAppropriate();
            currentFormName = pawnGeneSet.name;
            formId = pawnGeneSet.formId;
            savedGeneSets.Remove(pawnGeneSet);
        }

        public void CopyGeneID(Gene gene, Gene sourceGene, List<Gene> genes)
		{
			try
			{
				if (sourceGene == this)
				{
					return;
				}
				//gene.loadID = sourceGene.loadID;
				//gene.overriddenByGene = sourceGene.overriddenByGene;
				genes.Remove(sourceGene);
				genes.Add(gene);
				sourceGene = null;
			}
			catch (Exception arg)
			{
				Log.Warning("Failed copy gene: " + sourceGene.LabelCap + ". Reason: " + arg);
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
				//pawn.genes.SetXenotypeDirect(XenotypeDefOf.Baseliner);
				pawn.genes.xenotypeName = xenotypeDef.Label;
				pawn.genes.iconDef = xenotypeDef.iconDef;
			}
			formId = savedGeneSets.Count + 1;
			currentFormName = xenotypeDef.Label;
		}

        public virtual void AddGene(GeneDef geneDef, bool inheritable)
		{
			if (!geneDef.ConflictsWith(this.def) && (inheritable && !XaG_GeneUtility.HasEndogene(geneDef, pawn) || !XaG_GeneUtility.HasXenogene(geneDef, pawn)))
			{
				pawn.genes.AddGene(geneDef, !inheritable);
			}
			//else
			//{
			//	return;
			//}
			//if (gene != null)
			//{
			//	if (inheritable)
			//	{
			//		pawn.genes.Endogenes.Remove(pawn.genes.GetGene(geneDef));
			//		pawn.genes.Endogenes.Add(gene);
			//	}
			//	else
			//	{
			//		pawn.genes.Xenogenes.Remove(pawn.genes.GetGene(geneDef));
			//		pawn.genes.Xenogenes.Add(gene);
			//	}
			//}
		}

		public virtual void AddToolGene(GeneDef geneDef, bool xenogene)
		{
			if (Active && XaG_GeneUtility.TryRemoveAllConflicts(pawn, geneDef))
			{
				pawn.genes.AddGene(geneDef, xenogene);
			}
		}

		public virtual void RemoveGene(Gene gene)
		{
			if (gene != this)
			{
				pawn?.genes?.RemoveGene(gene);
			}
		}

		// Only for one time internal use. For other cases, the basic remove is used.
		private void RemoveMorpher()
		{
			try
			{
				foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
				{
					if (gene.def.IsGeneDefOfType<Gene_MorpherDependant>())
					{
						RemoveGene(gene);
					}
				}
				pawn?.genes?.RemoveGene(this);
				Messages.Message("WVC_XaG_GeneMorpherOneTimeMorph".Translate(pawn.Named("PAWN")), null, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove morpher properly. Reason: " + arg);
			}
		}

		public virtual TaggedString GizmoTootip => "WVC_XaG_MorpherGizmoTip".Translate();

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
				yield return new Command_Action
				{
					defaultLabel = "DEV: UpdTriggerGenes",
					action = delegate
					{
						UpdToolGenes(true);
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
