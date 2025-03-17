using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XenotypeHolder
	{

		public string name = null;

		public XenotypeIconDef iconDef = null;

		public XenotypeDef xenotypeDef = null;

		public List<GeneDef> genes = new();

		public bool inheritable;

		public float displayPriority;

		public bool shouldSkip = false;

		public bool isTrueShiftForm;

		public bool isOverriden;

		public float? matchPercent;

		//public string customEffectsDesc = null;

		public bool Baseliner => xenotypeDef == XenotypeDefOf.Baseliner && genes.NullOrEmpty();

		public bool CustomXenotype => xenotypeDef == XenotypeDefOf.Baseliner && !genes.NullOrEmpty();

		public XenotypeHolder()
		{

		}

		public XenotypeHolder(XenotypeDef xenotypeDef)
		{
			this.xenotypeDef = xenotypeDef;
			genes = xenotypeDef.genes;
			inheritable = xenotypeDef.inheritable;
		}

		[Unsaved(false)]
		private TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		private TaggedString cachedLabel = null;

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					if (name.NullOrEmpty())
					{
						cachedLabel = xenotypeDef.label;
					}
					else
					{
						cachedLabel = name;
					}
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		[Unsaved(false)]
		private string cachedDescription;

		public virtual string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					if (xenotypeDef != XenotypeDefOf.Baseliner)
					{
						stringBuilder.AppendLine(!xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.descriptionShort : xenotypeDef.description);
						if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine(("WVC_DoubleXenotypes".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + "\n" + xenotypeDef.doubleXenotypeChances.Select((XenotypeChance x) => "WVC_XaG_DoubleXenotypeWithChanceText".Translate(x.xenotype.LabelCap, (x.chance * 100f).ToString()).ToString()).ToLineList(" - "));
						}
					}
					else
					{
						stringBuilder.AppendLine("UniqueXenotypeDesc".Translate());
					}
					stringBuilder.AppendLine();
					//if (customEffectsDesc != null)
					//{
					//	stringBuilder.AppendLine(customEffectsDesc);
					//	stringBuilder.AppendLine();
					//}
					XaG_GeneUtility.GetBiostatsFromList(genes, out int biostatCpx, out int biostatMet, out int biostatArc);
					bool flag2 = false;
					if (biostatCpx != 0)
					{
						stringBuilder.AppendLineTagged("Complexity".Translate().Colorize(GeneUtility.GCXColor) + ": " + biostatCpx.ToStringWithSign());
						flag2 = true;
					}
					if (biostatMet != 0)
					{
						stringBuilder.AppendLineTagged("Metabolism".Translate().CapitalizeFirst().Colorize(GeneUtility.METColor) + ": " + biostatMet.ToStringWithSign());
						flag2 = true;
					}
					if (biostatArc != 0)
					{
						stringBuilder.AppendLineTagged("ArchitesRequired".Translate().Colorize(GeneUtility.ARCColor) + ": " + biostatArc.ToStringWithSign());
						flag2 = true;
					}
					if (flag2)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(("WVC_Inheritable".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + inheritable.ToStringYesNo());
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

	public class ThrallHolder : XenotypeHolder
	{

		public ThrallDef thrallDef;

		[Unsaved(false)]
		private string cachedDescription;

		public override string Description
		{
			get
			{
				if (cachedDescription == null)
				{
					StringBuilder stringBuilder = new();
					stringBuilder.AppendLine(LabelCap.Colorize(ColoredText.TipSectionTitleColor));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(thrallDef.description);
					if (thrallDef.xenotypeDef != null && !thrallDef.xenotypeDef.descriptionShort.NullOrEmpty())
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(thrallDef.xenotypeDef.descriptionShort);
					}
					if (thrallDef.reqGeneDef != null)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Requires".Translate() + ": " + thrallDef.reqGeneDef.LabelCap);
					}
					stringBuilder.AppendLine();
					stringBuilder.Append("WVC_XaG_AcceptableRotStages".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + thrallDef.acceptableRotStages.Select((RotStage x) => x.ToStringHuman()).ToLineList(" - "));
					cachedDescription = stringBuilder.ToString();
				}
				return cachedDescription;
			}
		}

	}

	public class GeneSetPresets : IExposable
	{

		public string name;

		public List<GeneDef> geneDefs = new();

		public void ExposeData()
		{
			Scribe_Values.Look(ref name, "name");
			Scribe_Collections.Look(ref geneDefs, "geneDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && geneDefs != null && geneDefs.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
		}

	}

	public class SaveableXenotypeHolder : XenotypeHolder, IExposable
	{

		public void ExposeData()
		{
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Values.Look(ref name, "name");
			Scribe_Values.Look(ref inheritable, "inheritable");
			Scribe_Collections.Look(ref genes, "genes", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && genes != null && genes.RemoveAll((GeneDef x) => x == null) > 0)
			{
				Log.Warning("Removed null geneDef(s)");
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                PostSetup();
            }
        }

        public void PostSetup()
        {
            if (xenotypeDef == null)
            {
                xenotypeDef = XenotypeDefOf.Baseliner;
            }
            if (genes == null)
            {
                genes = new();
            }
            if (xenotypeDef != XenotypeDefOf.Baseliner)
            {
                genes = xenotypeDef.genes;
                name = null;
                iconDef = null;
                inheritable = xenotypeDef.inheritable;
            }
        }

        public SaveableXenotypeHolder()
		{

		}

		public SaveableXenotypeHolder(XenotypeHolder holder)
		{
			xenotypeDef = holder.xenotypeDef;
			name = holder.name;
			iconDef = holder.iconDef;
			genes = holder.genes;
			inheritable = holder.inheritable;
		}

		public SaveableXenotypeHolder(XenotypeDef xenotypeDef, List<GeneDef> genes, bool inheritable, XenotypeIconDef icon, string name)
		{
			//genes = new();
			//SaveableXenotypeHolder newHolder = new();
			this.xenotypeDef = xenotypeDef;
			this.genes = genes;
			this.inheritable = inheritable;
			this.iconDef = icon;
			this.name = name;
			//xenotypeHolder = newHolder;
		}

	}

	public class PawnGeneSetHolder : IExposable
	{

		public int formId;

		public string name = null;

		public XenotypeIconDef iconDef = null;

		public List<Gene> endogenes = null;
		public List<Gene> xenogenes = null;

		public List<GeneDef> endogeneDefs = null;
		public List<GeneDef> xenogeneDefs = null;

		public List<GeneDefOverrideSaver> overrideSaverList = null;

		public XenotypeDef xenotypeDef = null;

		public Dictionary<NeedDef, float> savedPawnNeeds;
		public Dictionary<GeneDef, float> savedPawnResources;

		public bool SaveAndLoadGenes => WVC_Biotech.settings.enable_MorpherExperimentalMode;

		//public Color? savedHairColor;
		//public Color? savedSkinColorOverride;
		public HairDef savedHairDef;
		public BeardDef savedBeardDef;
		public TattooDef savedBodyTattooDef;
		public TattooDef savedFaceTattooDef;

		private void SaveAppearance(Pawn pawn)
		{
			//savedHairColor = pawn.story.HairColor;
			//savedSkinColorOverride = pawn.story.skinColorOverride;
			savedHairDef = pawn.story.hairDef;
			if (pawn.style != null)
			{
				savedBodyTattooDef = pawn.style.BodyTattoo;
				savedFaceTattooDef = pawn.style.FaceTattoo;
				savedBeardDef = pawn.style.beardDef;
			}
		}

		private void LoadAppearance(Pawn pawn)
		{
			//if (savedHairColor.HasValue)
			//{
			//	pawn.story.HairColor = savedHairColor.Value;
			//}
			//if (savedHairColor.HasValue)
			//{
			//	pawn.story.skinColorOverride = savedSkinColorOverride.Value;
			//}
			Color? hairColorOverride = XaG_GeneUtility.GetFirstHairColorGene(pawn)?.def?.hairColorOverride;
			if (hairColorOverride.HasValue)
			{
                pawn.story.HairColor = hairColorOverride.Value.ClampToValueRange(GeneTuning.HairColorValueRange);
			}
			Color? skinColorOverride = XaG_GeneUtility.GetFirstSkinColorOverrideGene(pawn)?.def?.skinColorOverride;
			if (skinColorOverride.HasValue)
			{
				pawn.story.skinColorOverride = skinColorOverride.Value.ClampToValueRange(GeneTuning.SkinColorValueRange);
			}
			if (savedHairDef != null)
			{
				pawn.story.hairDef = savedHairDef;
			}
			if (pawn.style != null)
			{
				if (savedBodyTattooDef != null)
				{
					pawn.style.BodyTattoo = savedBodyTattooDef;
				}
				if (savedFaceTattooDef != null)
				{
					pawn.style.FaceTattoo = savedFaceTattooDef;
				}
				if (savedBeardDef != null)
				{
					pawn.style.beardDef = savedBeardDef;
				}
			}
		}

		public void SaveGenes(Pawn pawn, Gene_Morpher morpher)
        {
            SaveAppearance(pawn);
            savedPawnNeeds = new();
            foreach (Need need in pawn.needs.AllNeeds)
            {
                if (!need.def.onlyIfCausedByGene)
                {
                    continue;
                }
                savedPawnNeeds[need.def] = need.CurLevel;
            }
            savedPawnResources = new();
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is Gene_Resource resource)
                {
                    savedPawnResources[resource.def] = resource.Value;
                }
            }
            overrideSaverList = new();
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene == morpher)
                {
                    continue;
                }
                if (gene.overriddenByGene != null)
				{
					GeneDefOverrideSaver newOverrideSaver = new();
					newOverrideSaver.overrided = gene.def;
					newOverrideSaver.overridedIsXenogene = pawn.genes.IsXenogene(gene);
					newOverrideSaver.overrider = gene.overriddenByGene.def;
                    newOverrideSaver.overriderIsXenogene = pawn.genes.IsXenogene(gene.overriddenByGene);
					overrideSaverList.Add(newOverrideSaver);
				}
            }
            if (SaveAndLoadGenes)
            {
                xenogenes = new();
                endogenes = new();
                foreach (Gene gene in pawn.genes.Endogenes.ToList())
                {
                    if (gene != morpher)
                    {
                        //gene.PostRemove();
                        endogenes.Add(gene);
                    }
                }
                foreach (Gene gene in pawn.genes.Endogenes.ToList())
                {
                    morpher.RemoveGene(gene);
                }
                foreach (Gene gene in pawn.genes.Xenogenes.ToList())
                {
                    if (gene != morpher)
                    {
                        //gene.PostRemove();
                        xenogenes.Add(gene);
                    }
                }
                foreach (Gene gene in pawn.genes.Xenogenes.ToList())
                {
                    morpher.RemoveGene(gene);
                }
                foreach (Gene gene in endogenes)
                {
                    gene.overriddenByGene = morpher;
                }
                foreach (Gene gene in xenogenes)
                {
                    gene.overriddenByGene = morpher;
                }
            }
            else
            {
                xenogeneDefs = new();
                endogeneDefs = new();
                foreach (Gene gene in pawn.genes.Endogenes.ToList())
                {
                    if (gene != morpher)
                    {
                        endogeneDefs.Add(gene.def);
                    }
                }
                foreach (Gene gene in pawn.genes.Endogenes.ToList())
                {
                    morpher.RemoveGene(gene);
                }
                foreach (Gene gene in pawn.genes.Xenogenes.ToList())
                {
                    if (gene != morpher)
                    {
                        xenogeneDefs.Add(gene.def);
                    }
                }
                foreach (Gene gene in pawn.genes.Xenogenes.ToList())
                {
                    morpher.RemoveGene(gene);
                }
            }
        }

        public void LoadGenes(Pawn pawn, Gene_Morpher morpher)
		{
			if (SaveAndLoadGenes)
			{
				try
				{
					Dictionary<Gene, int> savedEndogenesIDs = new();
					//Dictionary<Gene, Gene> savedEndogenesOverrides = new();
					foreach (Gene gene in endogenes)
					{
						morpher.AddGene(gene.def, true);
						Gene sourceGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == gene.def);
						savedEndogenesIDs[sourceGene] = sourceGene.loadID;
						//savedEndogenesOverrides[sourceGene] = sourceGene.overriddenByGene;
						morpher.CopyGeneID(gene, sourceGene, pawn.genes.Endogenes);
					}
					Dictionary<Gene, int> savedXenogenesIDs = new();
					//Dictionary<Gene, Gene> savedXenogenesOverrides = new();
					foreach (Gene gene in xenogenes)
					{
						morpher.AddGene(gene.def, false);
						Gene sourceGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == gene.def);
						savedXenogenesIDs[sourceGene] = sourceGene.loadID;
						//savedXenogenesOverrides[sourceGene] = sourceGene.overriddenByGene;
						morpher.CopyGeneID(gene, sourceGene, pawn.genes.Xenogenes);
					}
					foreach (var item in savedEndogenesIDs)
					{
						Gene targetGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.loadID = item.Value;
					}
					foreach (var item in savedXenogenesIDs)
					{
						Gene targetGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
						targetGene.loadID = item.Value;
					}
					//foreach (var item in savedEndogenesOverrides)
					//{
					//	Gene targetGene = pawn.genes.Endogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
					//	targetGene.overriddenByGene = item.Value;
					//}
					//foreach (var item in savedXenogenesOverrides)
					//{
					//	Gene targetGene = pawn.genes.Xenogenes.First((Gene oldGene) => oldGene.def == item.Key.def);
					//	targetGene.overriddenByGene = item.Value;
					//}
					savedEndogenesIDs = null;
					savedXenogenesIDs = null;
				}
				catch (Exception arg)
				{
					Log.Error("Failed copy genes. Reason: " + arg);
				}
			}
			else
			{
				foreach (GeneDef gene in endogeneDefs)
				{
					morpher.AddGene(gene, true);
				}
				foreach (GeneDef gene in xenogeneDefs)
				{
					morpher.AddGene(gene, false);
				}
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				gene.OverrideBy(null);
			}
			if (overrideSaverList != null)
			{
				foreach (GeneDefOverrideSaver saver in overrideSaverList)
				{
					if (saver.IsNull)
					{
						continue;
					}
					//if (saver.overrided == morpher.def || saver.overrider == morpher.def)
					//{
					//	continue;
					//}
					Gene overriderGene = saver.overriderIsXenogene ? XaG_GeneUtility.GetXenogene(saver.overrider, pawn) : XaG_GeneUtility.GetEndogene(saver.overrider, pawn);
					if (saver.overridedIsXenogene)
					{
						XaG_GeneUtility.GetXenogene(saver.overrided, pawn).OverrideBy(overriderGene);
					}
					else
					{
						XaG_GeneUtility.GetEndogene(saver.overrided, pawn).OverrideBy(overriderGene);
					}
				}
			}
            overrideSaverList = null;
			foreach (Need need in pawn.needs.AllNeeds)
			{
				foreach (var item in savedPawnNeeds)
				{
					if (need.def == item.Key)
					{
						need.CurLevel = item.Value;
					}
				}
			}
			savedPawnNeeds = null;
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if (gene is Gene_Resource resource)
				{
					foreach (var item in savedPawnResources)
					{
						if (resource.def == item.Key)
						{
							resource.Value = item.Value;
						}
					}
				}
			}
			savedPawnResources = null;
			LoadAppearance(pawn);
		}

		[Unsaved(false)]
		public TaggedString cachedLabelCap = null;

		[Unsaved(false)]
		public TaggedString cachedLabel = null;

		public virtual TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					if (name.NullOrEmpty())
					{
						cachedLabel = xenotypeDef.label;
					}
					else
					{
						cachedLabel = name.CapitalizeFirst();
					}
				}
				return cachedLabel;
			}
		}

		public virtual TaggedString LabelCap
		{
			get
			{
				if (cachedLabelCap == null)
				{
					cachedLabelCap = Label.CapitalizeFirst();
				}
				return cachedLabelCap;
			}
		}

		public int? cachedGenesCount;

		public virtual int AllGenesCount
		{
			get
			{
				if (!cachedGenesCount.HasValue)
                {
					if (SaveAndLoadGenes)
					{
						cachedGenesCount = endogenes.Count + xenogenes.Count;
					}
					else
					{
						cachedGenesCount = endogeneDefs.Count + xenogeneDefs.Count;
					}
				}
				return cachedGenesCount.Value;
			}
		}

        public virtual void ExposeData()
		{
			Scribe_Values.Look(ref formId, "formId");
			Scribe_Values.Look(ref name, "name");
			Scribe_Defs.Look(ref iconDef, "iconDef");
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Collections.Look(ref endogenes, "endogenes", LookMode.Deep);
			Scribe_Collections.Look(ref xenogenes, "xenogenes", LookMode.Deep);
			Scribe_Collections.Look(ref endogeneDefs, "endogeneDefs", LookMode.Def);
			Scribe_Collections.Look(ref xenogeneDefs, "xenogeneDefs", LookMode.Def);
			Scribe_Collections.Look(ref savedPawnNeeds, "savedPawnNeeds", LookMode.Def, LookMode.Value);
			Scribe_Collections.Look(ref savedPawnResources, "savedPawnResources", LookMode.Def, LookMode.Value);
			Scribe_Collections.Look(ref overrideSaverList, "overrideSaverList", LookMode.Deep);
			//Appearance
			//Scribe_Values.Look(ref savedHairColor, "savedHairColor");
			//Scribe_Values.Look(ref savedSkinColorOverride, "savedSkinColorOverride");
			Scribe_Defs.Look(ref savedHairDef, "savedHairDef");
			Scribe_Defs.Look(ref savedBeardDef, "savedBeardDef");
			Scribe_Defs.Look(ref savedBodyTattooDef, "savedBodyTattooDef");
			Scribe_Defs.Look(ref savedFaceTattooDef, "savedFaceTattooDef");
			//Load
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if ((xenogenes != null && xenogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0) || (endogenes != null && endogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0))
				{
					Log.Warning("Removed null gene(s)");
				}
				if ((endogeneDefs != null && endogeneDefs.RemoveAll((GeneDef x) => x == null) > 0) || (xenogeneDefs != null && xenogeneDefs.RemoveAll((GeneDef x) => x == null) > 0))
				{
					Log.Warning("Removed null geneDef(s)");
				}
				if (savedPawnNeeds != null)
				{
					foreach (var need in savedPawnNeeds.ToList())
					{
						if (need.Key == null)
						{
							savedPawnNeeds.Remove(need.Key);
						}
					}
				}
				if (savedPawnResources != null)
				{
					foreach (var geneDef in savedPawnResources.ToList())
					{
						if (geneDef.Key == null)
						{
							savedPawnResources.Remove(geneDef.Key);
						}
					}
				}
				//if (overrideSaverList != null && overrideSaverList.RemoveAll((GeneDefOverrideSaver saver) => saver.overrider == null || saver.overrided == null) > 0)
				//{
				//	Log.Warning("Removed null overrideSaver(s)");
				//}
			}
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (xenotypeDef == null)
                {
                    xenotypeDef = XenotypeDefOf.Baseliner;
                }
                //if (xenogenes == null)
                //{
                //    xenogenes = new();
                //}
                //if (endogenes == null)
                //{
                //    endogenes = new();
                //}
                //if (xenogeneDefs == null)
                //{
                //    xenogeneDefs = new();
                //}
                //if (endogeneDefs == null)
                //{
                //    endogeneDefs = new();
                //}
            }
        }

	}

	public class GeneDefOverrideSaver : IExposable
	{

		public bool overriderIsXenogene = false;
		public bool overridedIsXenogene = false;

		public GeneDef overrider = null;
		public GeneDef overrided = null;

		public bool IsNull => overrided == null || overrider == null;

		public void ExposeData()
		{
			Scribe_Values.Look(ref overriderIsXenogene, "overriderIsXenogene");
			Scribe_Values.Look(ref overridedIsXenogene, "overridedIsXenogene");
			Scribe_Defs.Look(ref overrider, "overrider");
			Scribe_Defs.Look(ref overrided, "overrided");
		}

	}

	public class PawnContainerHolder : PawnGeneSetHolder, IThingHolder, ISuspendableThingHolder
	{

		public override TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					cachedLabel = name;
				}
				return cachedLabel;
			}
		}

		public Pawn owner = null;
		public Pawn holded = null;

		public bool IsNullContainer => owner == null || holded == null;

		public ThingOwner innerContainer;

		public int lastTimeSeenByPlayer = -1;

		public override int AllGenesCount
		{
			get
			{
				if (!cachedGenesCount.HasValue)
				{
					cachedGenesCount = holded.genes.GenesListForReading.Count;
				}
				return cachedGenesCount.Value;
			}
		}

		public bool TrySetContainer(Pawn owner, Pawn toHold)
        {
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
			if (Accepts(toHold))
			{
				this.owner = owner;
				this.holded = toHold;
				name = toHold.Name.ToStringShort;
				xenotypeDef = toHold.genes.Xenotype;
				lastTimeSeenByPlayer = Find.TickManager.TicksGame;
				//endogeneDefs.AddRange(XaG_GeneUtility.ConvertGenesInGeneDefs(toHold.genes.Endogenes));
				//xenogeneDefs.AddRange(XaG_GeneUtility.ConvertGenesInGeneDefs(toHold.genes.Xenogenes));
				if (toHold.genes.UniqueXenotype)
				{
					iconDef = toHold.genes.iconDef;
				}
				toHold.DeSpawnOrDeselect();
				if (innerContainer.TryAdd(toHold))
				{
					return true;
				}
			}
			return false;
		}

        public IThingHolder ParentHolder => owner?.ParentHolder;

        public bool IsContentsSuspended => true;

        public bool Accepts(Thing thing)
		{
			return innerContainer.CanAcceptAnyOf(thing, canMergeWithExistingStacks: false);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public override void ExposeData()
		{
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_References.Look(ref owner, "owner", saveDestroyedThings: true);
			Scribe_References.Look(ref holded, "holded", saveDestroyedThings: true);
			base.ExposeData();
			Scribe_Values.Look(ref lastTimeSeenByPlayer, "lastTimeSeenByPlayer", -1);
		}
	}

}
