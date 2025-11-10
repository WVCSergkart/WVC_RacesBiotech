using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_UseEffect_XenogermSerum : CompProperties_UseEffect
	{

		public XenotypeDef endotypeDef = null;
		public XenotypeDef xenotypeDef = null;

		public JobDef jobDef;

		public ThingDef moteDef;

		public List<GeneDef> possibleGenes;

		public List<XenotypeDef> possibleXenotypes;

		public XenotypeType xenotypeType = 0;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public bool removeSkinColor = true;

		public int daysDelay = 8;

		public bool canBeUsedInCaravan = false;

		// public List<ShapeshiftModeDef> unlockModes;

		public GeneDef geneDef;

		public HediffDef hediffDef;

		public List<HediffDef> hediffsToRemove;

		public bool disableShapeshiftComaAfterUse = false;

		public bool disableShapeshiftGenesRegrowAfterUse = false;

		public JobDef retuneJob;

		[MustTranslate]
		public string jobString;

		public List<ResearchProjectDef> researchPrerequisites;

		public enum XenotypeType
		{
			Base,
			Archite
		}

		public CompProperties_UseEffect_XenogermSerum()
		{
			compClass = typeof(CompUseEffect);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (compClass == typeof(CompUseEffect))
			{
				Log.Error(parentDef.defName + " has CompProperties_UseEffect_XenogermSerum with CompUseEffect compClass.");
			}
			if (compClass == typeof(CompUseEffect_GeneGiver) && possibleGenes.NullOrEmpty())
			{
				Log.Error(parentDef.defName + " has CompUseEffect_GeneGiver compClass with null possibleGenes.");
			}
			if (!hediffsToRemove.NullOrEmpty())
			{
				if (parentDef.descriptionHyperlinks.NullOrEmpty())
				{
					parentDef.descriptionHyperlinks = new();
				}
				foreach (HediffDef hediffDef in hediffsToRemove)
				{
					parentDef.descriptionHyperlinks.Add(new DefHyperlink(hediffDef));
				}
			}
			if (parentDef.stackLimit > 1)
			{
				parentDef.stackLimit = 1;
				Log.Warning(parentDef.defName + " should never stack. Stack size changed to 1");
			}
		}

	}

}
