using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class PawnGeneSetHolder : IExposable
	{

		public int formId;

		public string name = null;

		public XenotypeIconDef iconDef = null;

		//public List<Gene> endogenes = null;
		//public List<Gene> xenogenes = null;

		public List<GeneDef> endogeneDefs = null;
		public List<GeneDef> xenogeneDefs = null;

		public List<GeneDefOverrideSaver> overrideSaverList = null;

		public XenotypeDef xenotypeDef = null;

		public Dictionary<NeedDef, float> savedPawnNeeds;
		public Dictionary<GeneDef, float> savedPawnResources;

		//public bool SaveAndLoadGenes => WVC_Biotech.settings.enable_MorpherExperimentalMode;

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

		public void LoadGenes(Pawn pawn, Gene_Morpher morpher)
		{
			foreach (GeneDef gene in endogeneDefs)
			{
				morpher.AddGene(gene, true);
			}
			foreach (GeneDef gene in xenogeneDefs)
			{
				morpher.AddGene(gene, false);
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
					cachedGenesCount = endogeneDefs.Count + xenogeneDefs.Count;
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
			//Scribe_Collections.Look(ref endogenes, "endogenes", LookMode.Deep);
			//Scribe_Collections.Look(ref xenogenes, "xenogenes", LookMode.Deep);
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
				//if ((xenogenes != null && xenogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0) || (endogenes != null && endogenes.RemoveAll((Gene x) => x == null || x.def == null) > 0))
				//{
				//	Log.Warning("Removed null gene(s)");
				//}
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

}
