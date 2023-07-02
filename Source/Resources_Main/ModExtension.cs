using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;
using WVC;
using System;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	public class GeneExtension_Background : DefModExtension
	{
		public string backgroundPathArchite;
	}

	public class GeneExtension_Spawner : DefModExtension
	{
		public ThingDef thingDefToSpawn;
		public int stackCount = 1;
		public IntRange spawnIntervalRange = new(120000, 300000);
		// public bool writeTimeLeftToSpawn = true;
		// public string customLabel = "Resource";
	}

	public class GeneExtension_General : DefModExtension
	{
		// public bool geneIsMechaskin = false;
		// public bool geneIsPowerSource = false;
		// public bool geneIsSubcore = false;
		public bool eyesShouldBeInvisble = false;
		public bool canBePredatorPrey = true;
		public bool noSkillDecay = false;
	}

	// public class ThoughtExtension_General : DefModExtension
	// {
		// public GeneDef geneDef;
	// }

	// public class GeneExtension_PawnPredatorPrey : DefModExtension
	// {
	// }

	// public class GeneExtension_Genepack : DefModExtension
	// {
		// public bool spawnGeneInGenepack = true;
	// }

	// public class GeneExtension_Conditions : DefModExtension
	// {
		// public bool pawnShouldBeImmortal = false;
		// public bool pawnShouldBeMechaskinned = false;
		// public bool pawnShouldHaveSubcore = false;
		// public bool pawnShouldBeMechalike = false;
	// }

	// public class GeneExtension_MechaClotting : DefModExtension
	// {
		// public bool preventBleeding = false;
	// }

	// public class GeneExtension_Giver : DefModExtension
	// {
		// public HediffDef hediffDef;
		// public List<BodyPartDef> bodyPartToAdd;
	// }

	// public class GeneExtension_Summoner : DefModExtension
	// {
		// public QuestScriptDef quest;
	// }

	public class GeneExtension_Giver : DefModExtension
	{
		public HediffDef hediffDefName;
		// public BodyPartDef bodypart;
		public List<BodyPartDef> bodyparts;
		public BackstoryDef childBackstoryDef;
		public BackstoryDef adultBackstoryDef;
		public List<GeneDef> randomizerGenesList;
		// public string exclusionTag;
		// public GeneDef randomizerGene;
		// public bool geneIsRandomized = false;
		// public IntRange intRange = new IntRange(1, 1);
		public XenotypeDef xenotypeForcerDef = null;
	}

	// public class HeadTypeExtension_Faceless : DefModExtension
	// {
		// public bool faceless = true;
	// }
	
	// public class BlackListedXenotypesDef : Def
	public class XenotypesAndGenesListDef : Def
	{
		public List<string> blackListedXenotypesForSerums;
		public List<XenotypeDef> whiteListedXenotypesForResurrectorSerums;
		public List<string> whiteListedXenotypesForFilter;
		public List<ThingDef> blackListedDefsForSerums;
		public List<GeneDef> perfectCandidatesForSerums;
		public List<GeneDef> whiteListedExoskinGenes;
		public List<BackstoryDef> blackListedBackstoryForChanger;
		// public List<ThingDef> listedGolems;
	}
	
	// public class WhiteListedGolemsDef : Def
	// {
		// public List<string> blackListedForSerums;
		// public List<string> perfectCandidatesForSerums;
	// }
	
	// public class Template_WhiteListedTraitsDef : Def
	// {
		// public List<string> whiteListedTraits;
	// }
}
