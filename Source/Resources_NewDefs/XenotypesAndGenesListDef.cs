using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{
    public class XenotypesAndGenesListDef : Def
	{
		public List<string> blackListedXenotypesForSerums = new();
		// public List<XenotypeDef> blackListedXenotypesForSingleSerums;
		// public List<XenotypeDef> blackListedXenotypesForHybridSerums;
		public List<XenotypeDef> whiteListedXenotypesForResurrectorSerums = new();
		public List<string> whiteListedXenotypesForFilter = new();
		// public List<GeneDef> whiteListedExoskinGenes;
		public List<BackstoryDef> blackListedBackstoryForChanger = new();
		// public List<ThingDef> listedGolems;
		// public List<GeneDef> gene_IsNotAcceptablePrey;
		// public List<GeneDef> gene_IsAngelBeauty;
		// public List<GeneDef> gene_PawnSkillsNotDecay;
		public List<string> mechDefNameShouldNotContain = new();
		// public List<GeneDef> xenoTree_PollutionReq_GeneDefs;
		public List<ThingDef> plantsToNotOverwrite_SpawnSubplant = new();
		// public List<TraitDef> shapeShift_ProhibitedTraits;
		// public List<PreceptDef> shapeShift_ProhibitedPrecepts;
		// public List<HediffDef> hediffsRemovedByGenesRestorationSerum;
		public List<MutantDef> xenoGenesMutantsExceptions = new();
		public List<GeneDef> anomalyXenoGenesExceptions = new();
		public List<GauranlenTreeModeDef> ignoredGauranlenTreeModeDefs = new();
		public List<XenotypeDef> devXenotypeDefs = new();
		public List<GeneDef> humanGeneDefs = new();

		//[Obsolete]
		//public List<XaG_CountWithChance> identicalGeneDefs = new();
		//[Obsolete]
		//public List<GeneDef> shapeshifterHeritableGenes = new();
		//[Obsolete]
		//public List<Type> shapeShift_IgnoredGeneClasses = new();
		// [Obsolete]
		// public List<HediffDef> hediffsThatPreventUndeadResurrection;
		// [Obsolete]
		// public List<HediffDef> blackListedHediffDefForReimplanter;
		// [Obsolete]
		// public List<ThingDef> blackListedDefsForSerums;
		// [Obsolete]
		// public List<GeneDef> perfectCandidatesForSerums;
		// [Obsolete]
		// public List<GeneDef> nonCandidatesForSerums;

	}
}
