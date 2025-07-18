using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Morpher : Gene, IGeneWithEffects, IGeneNotifyGenesChanged
	{

		//public GeneExtension_Undead Props => def?.GetModExtension<GeneExtension_Undead>();

		//private int nextTick = 1500;

		private bool? cachedOneTimeMorpher;

        public virtual bool CanMorphNow => true;

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
				return 0;
			}
		}

		public bool CanAddNewForm => currentLimit > savedGeneSets.Count;

		public void AddLimit(int count = 1)
		{
			currentLimit += count;
		}

		public void SetLimit(int count = 1)
		{
			currentLimit = count;
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

		public void SetFormID(int newId, string name)
		{
			currentFormName = name;
			formId = newId;
		}

		public void SaveFormID(PawnGeneSetHolder newSet)
		{
			if (!formId.HasValue)
			{
				formId = savedGeneSets.Count + 1;
			}
			newSet.formId = formId.Value;
		}

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

		public virtual void TransferHolders(Gene_Morpher oldMorpher, Gene_Morpher newMorpher, Pawn newOwner)
		{
			foreach (PawnGeneSetHolder holder in oldMorpher.savedGeneSets.ToList())
			{
				newMorpher.AddSetHolder(holder);
				oldMorpher.RemoveSetHolder(holder);
			}
			newMorpher.SetLimit(oldMorpher.currentLimit);
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

		private TaggedString cachedPossibleXenotypesString = null;
		public virtual TaggedString PossibleXenotypes
		{
			get
			{
				if (cachedPossibleXenotypesString == null)
				{
					cachedPossibleXenotypesString = pawn.genes.GetFirstGeneOfType<Gene_MorpherXenotypeTargeter>()?.Giver?.morpherXenotypeChances?.Select((XenotypeChance xenoChance) => xenoChance.xenotype.label).ToCommaList(true).CapitalizeFirst();
					if (cachedPossibleXenotypesString == null)
                    {
						cachedPossibleXenotypesString = "Random".Translate();
                    }
				}
				return cachedPossibleXenotypesString;
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedPossibleXenotypesString = null;
			cachedOneTimeMorpher = null;
			gizmo = null;
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

		public override void TickInterval(int delta)
		{

		}

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
				phase = "try get next xenotype";
				XenotypeHolder xenotypeHolder = null;
				if (savedGeneSets.Count < currentLimit + 1)
				{
					xenotypeHolder = GetBestNewFormForMorpher();
				}
				phase = "save old gene set";
				if (savedGeneSets.NullOrEmpty())
				{
					savedGeneSets = new();
				}
				SaveGenes();
				if (nextGeneSet == null)
				{
					phase = "create new form";
					TryCreateNewForm(xenotypeHolder);
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
				PostMorph();
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

        private void PostMorph()
        {
			foreach (Gene gene in pawn.genes.GenesListForReading)
            {
				if (gene is not Gene_MorpherDependant postMorphGene)
                {
					continue;
                }
				postMorphGene.PostMorph(pawn);
			}
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_Morph, pawn.Named(HistoryEventArgsNames.Doer)));
			}
		}

        private void TryCreateNewForm(XenotypeHolder xenotypeHolder)
		{
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
				Log.Error("Failed morph while trying create new/next form.");
			}
		}

		public XenotypeHolder GetBestNewFormForMorpher()
		{
			XenotypeHolder xenotypeHolder = null;
			List<XenotypeDef> exclude = new() { pawn.genes.Xenotype };
			foreach (PawnGeneSetHolder set in savedGeneSets)
			{
				exclude.Add(set.xenotypeDef);
			}
            Gene_MorpherXenotypeTargeter gene_MorpherXenotypeTargeter = pawn.genes.GetFirstGeneOfType<Gene_MorpherXenotypeTargeter>();
            if (gene_MorpherXenotypeTargeter != null)
            {
				//Log.Error("0");
				xenotypeHolder = gene_MorpherXenotypeTargeter.TargetedXenotype;
			}
			else if (XenotypeGiver != null)
			{
				if (!XenotypeGiver.morpherXenotypeDefs.NullOrEmpty())
				{
					xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromList(XenotypeGiver.morpherXenotypeDefs, exclude));
				}
				else if (!XenotypeGiver.morpherXenotypeChances.NullOrEmpty())
				{
					xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(XenotypeGiver.morpherXenotypeChances, exclude));
				}
			}
			if (xenotypeHolder == null && Giver != null)
			{
				if (!Giver.morpherXenotypeDefs.NullOrEmpty())
				{
					xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromList(Giver.morpherXenotypeDefs, exclude));
				}
				else if (!Giver.morpherXenotypeChances.NullOrEmpty())
				{
					xenotypeHolder = new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(Giver.morpherXenotypeChances, exclude));
				}
			}
			if (xenotypeHolder == null)
			{
				xenotypeHolder = GetBestNewForm(this);
			}
			return xenotypeHolder;
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
					holder.matchPercent = 1.3f;
				}
				else if (holder.genes.HasGeneDefOfType<Gene_Morpher>())
				{
					holder.matchPercent = 1.2f;
				}
				else
				{
					holder.matchPercent = 1f;
				}
				if (holder.CustomXenotype)
				{
					holder.matchPercent += 0.5f;
				}
				else if (gene.pawn.genes.Xenotype == holder.xenotypeDef)
				{
					holder.matchPercent *= 0.2f;
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
				if (met < -5)
				{
					holder.matchPercent *= 0.2f;
				}
				else if (met < 0)
				{
					holder.matchPercent *= 0.8f;
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
				AddToolGene(GetTriggerGene(), xenogene);
			}
		}

		private GeneDef GetTriggerGene()
		{
			if (XenotypeGiver?.morpherTriggerGene != null)
			{
				return XenotypeGiver.morpherTriggerGene;
			}
			List<GeneDef> list = new();
			List<GeneDef> database = DefDatabase<GeneDef>.AllDefsListForReading;
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is Gene_Deathrest)
				{
					if (database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_DeathrestMorph>()).TryRandomElement(out GeneDef triggerGene))
					{
						list.Add(triggerGene);
					}
				}
				else if (gene is Gene_Deathless)
				{
					if (database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_DeathlessMorph>()).TryRandomElement(out GeneDef triggerGene))
					{
						list.Add(triggerGene);
					}
				}
				else if (gene is Gene_Hemogen)
				{
					if (database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_HemogenMorph>()).TryRandomElement(out GeneDef triggerGene))
					{
						list.Add(triggerGene);
					}
				}
				else if (gene is Gene_Undead)
				{
					if (database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_UndeadMorph>()).TryRandomElement(out GeneDef triggerGene))
					{
						list.Add(triggerGene);
					}
				}
			}
			//if (pawn.needs?.TryGetNeed<Need_Deathrest>() != null)
			//{
			//}
			//if (pawn.genes?.GetFirstGeneOfType<Gene_Deathless>() != null)
			//{
			//}
			//if (pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>() != null)
			//{
			//}
			//if (pawn.genes?.GetFirstGeneOfType<Gene_Undead>() != null)
			//{
			//}
			if (pawn.psychicEntropy?.NeedsPsyfocus == true)
			{
				if (database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_PsyfocusMorph>()).TryRandomElement(out GeneDef triggerGene))
				{
					list.Add(triggerGene);
				}
			}
			if (Giver != null && !Giver.morpherTriggerGenes.NullOrEmpty())
			{
				list.AddRange(Giver.morpherTriggerGenes);
			}
			if (XenotypeGiver != null && !XenotypeGiver.morpherTriggerGenes.NullOrEmpty())
			{
				list.AddRange(XenotypeGiver.morpherTriggerGenes);
			}
			if (list.NullOrEmpty())
            {
				Log.Warning("Failed get trigger genes for pawn. Trying random from database.");
				return database.Where((GeneDef geneDef) => geneDef.IsGeneDefOfType<Gene_MorpherTrigger>()).RandomElement();
			}
			return list.RandomElement();
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
			//if (!geneSet.endogenes.NullOrEmpty())
			//{
			//	List<Gene> geneDefs = geneSet.endogenes?.Where((Gene gene) => gene.def.hairColorOverride != null)?.ToList();
			//	if (geneDefs.NullOrEmpty())
			//	{
			//		return null;
			//	}
			//	return geneDefs.First().def;
			//}
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
			//if (!geneSet.endogenes.NullOrEmpty())
			//{
			//	List<Gene> geneDefs = geneSet.endogenes?.Where((Gene gene) => gene.def.skinColorBase != null || gene.def.skinColorOverride != null)?.ToList();
			//	if (geneDefs.NullOrEmpty())
			//	{
			//		return null;
			//	}
			//	return geneDefs.First().def;
			//}
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

		public virtual void DoEffects(Pawn pawn)
		{
			if (pawn.Map == null)
			{
				return;
			}
			//WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
			MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
		}

		public void DoEffects()
		{
			DoEffects(pawn);
		}

		private void SaveGenes()
		{
			//if (pawnGeneSets.Where((PawnGeneSetHolder holder) => holder.xenotypeDef == pawn.genes.Xenotype).Any())
			//{
			//	return;
			//}
			PawnGeneSetHolder newSet = new();
			//if (!formId.HasValue)
			//{
			//	formId = savedGeneSets.Count + 1;
			//}
			//newSet.formId = formId.Value;
			SaveFormID(newSet);
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

        public void Reimplant(XenotypeHolder xenotypeDef)
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
			SetFormID(savedGeneSets.Count + 1, xenotypeDef.Label);
			//formId = savedGeneSets.Count + 1;
			//currentFormName = xenotypeDef.Label;
		}

        public virtual void AddGene(GeneDef geneDef, bool inheritable)
		{
			//if (!geneDef.ConflictsWith(this.def) && (inheritable && !XaG_GeneUtility.HasEndogene(geneDef, pawn) || !XaG_GeneUtility.HasXenogene(geneDef, pawn)))
			//{
			//	pawn.genes.AddGene(geneDef, !inheritable);
			//}
			pawn.TryAddOrRemoveGene(this, null, geneDef, inheritable);
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
			//if (gene != this)
			//{
			//	pawn?.genes?.RemoveGene(gene);
			//}
			pawn.TryAddOrRemoveGene(this, gene);
		}

		public virtual void ClearGenes()
		{
			DuplicateUtility.RemoveAllGenes(pawn.genes.GenesListForReading, new() { def });
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

		public virtual TaggedString WarningDesc => "WVC_XaG_GeneAbilityMorphWarning".Translate();

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

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

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
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
		}

		public virtual void StoreGeneSet(PawnGeneSetHolder geneSet, Gene_StorageImplanter storage)
		{
			List<GeneDef> selectedGenes = new();
			if (!geneSet.endogeneDefs.NullOrEmpty())
			{
				selectedGenes.AddRange(geneSet.endogeneDefs);
			}
			if (!geneSet.xenogeneDefs.NullOrEmpty())
			{
				selectedGenes.AddRange(geneSet.xenogeneDefs);
			}
			if (!selectedGenes.Contains(def))
			{
				selectedGenes.Add(def);
			}
			storage.SetupHolder(XenotypeDefOf.Baseliner, selectedGenes, geneSet.xenotypeDef.inheritable, geneSet.iconDef, null);
			RemoveSetHolder(geneSet);
		}
    }

}
