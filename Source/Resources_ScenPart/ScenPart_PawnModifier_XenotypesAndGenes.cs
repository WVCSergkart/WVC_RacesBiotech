// RimWorld.StatPart_Age
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_XenotypesAndGenes : ScenPart_PawnModifier
	{

		public List<XenotypeChance> xenotypeChances = new();
		public List<XenotypeDef> allowedXenotypes = new();
		public List<string> overridedXenotypes;
		public List<GeneDef> geneDefs = new();
		public bool addMechlink = false;
		public bool nullifyBackstory = false;
		public bool nullifySkills = false;
		public bool addSkipEffect = false;
		public List<GeneDef> chimeraGeneDefs = new();
		public List<GeneralHolder> chimeraGenesPerBiomeDef;
		public int startingDuplicates = 0;
		public IntRange additionalChronoAge = new(0, 0);
		public Gender gender = Gender.None;
		public bool startingPawnsIsPregnant = false;
		public ThingDef humanEggDef;
		public IntRange humanEggsCount = new(3, 5);
		public PrefabDef prefabDef;
		public List<SkillRange> skills;
		public List<TraitDefHolder> forcedTraits;
		public List<GeneralHolder> chimeraGenesPerXenotype;
		public bool archiveAllPawns = false;
		public bool isResurrected = false;

		//private List<string> startingXenotypes;
		//public List<string> StartingXenotypes
		//{
		//	get
		//	{
		//		if (startingXenotypes == null)
		//		{
		//			return new();
		//		}
		//		return startingXenotypes;
		//	}
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Collections.Look(ref startingXenotypes, "startingXenotypes", LookMode.Value);
		//	//Scribe_Values.Look(ref addMechlink, "addMechlink", false);
		//	//Scribe_Values.Look(ref nullifyBackstory, "nullifyBackstory", false);
		//	//Scribe_Values.Look(ref nullifySkills, "nullifySkills", false);
		//	//Scribe_Values.Look(ref addMechlink, "addMechlink", false);
		//}

		protected override void ModifyNewPawn(Pawn p)
		{
			SetTraits(p);
			SetXenotype(p);
			OverrideXenotype(p);
			SetGenes(p);
			SetGender(p);
			AddMechlink(p);
			NullifyBackstory(p);
			ChimeraGenes(p);
			AgeCorrection(p);
			Skills(p);
			SetPregnant(p);
			MiscUtility.Notify_DebugPawn(p);
			//ScribeInfo(p);
		}

		//private void ScribeInfo(Pawn pawn)
		//{
		//	if (pawn.genes != null)
		//	{
		//		if (startingXenotypes == null)
		//		{
		//			startingXenotypes = new();
		//		}
		//		startingXenotypes.AddSafe(pawn.genes.XenotypeLabel.UncapitalizeFirst());
		//	}
		//}

		private void SetTraits(Pawn pawn)
		{
			if (forcedTraits == null)
			{
				return;
			}
			TraitsUtility.RemoveAllTraits(pawn);
			TraitsUtility.AddTraitsFromList(pawn, forcedTraits);
		}

		private void SetGender(Pawn pawn)
		{
			if (gender == Gender.None || pawn.gender == gender)
			{
				return;
			}
			Gene_Gender.SetGender(pawn, gender);
		}

		private void SetPregnant(Pawn pawn)
		{
			if (!startingPawnsIsPregnant)
			{
				GestationUtility.TryUpdChildGenes(pawn);
				return;
			}
			GestationUtility.TryImpregnateOrUpdChildGenes(pawn);
		}

		public override IEnumerable<Thing> PlayerStartingThings()
		{
			Pawn pawn = Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount).First();
			if (humanEggDef != null && pawn.genes?.GetFirstGeneOfType<Gene_Ovipositor>() != null)
			{
				Thing thing = ThingMaker.MakeThing(humanEggDef);
				thing.stackCount = humanEggsCount.RandomInRange;
				CompHumanEgg egg = thing.TryGetComp<CompHumanEgg>(); 
				if (egg != null)
				{
					egg.SetupEgg(pawn);
				}
				yield return thing;
			}
		}

		public override void PostMapGenerate(Map map)
		{
			if (Find.GameInitData == null || !context.Includes(PawnGenerationContext.PlayerStarter))
			{
				return;
			}
			if (addSkipEffect)
			{
				foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
				{
					if (startingAndOptionalPawn.Spawned)
					{
						MiscUtility.DoSkipEffects(startingAndOptionalPawn.Position, startingAndOptionalPawn.Map);
					}
				}
			}
			DupePawns();
			ArchivePawns();
			MarkAsResurrected();
		}

		private void MarkAsResurrected()
		{
			if (!isResurrected)
			{
				return;
			}
			foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
			{
				startingAndOptionalPawn.HumanComponent()?.SetResurrected();
			}
		}

		private void DupePawns()
		{
			if (startingDuplicates <= 0)
			{
				return;
			}
			foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
			{
				for (int i = 0; i < startingDuplicates; i++)
				{
					if (startingAndOptionalPawn.Spawned)
					{
						FastDuplicatePawn(startingAndOptionalPawn);
					}
				}
			}

			void FastDuplicatePawn(Pawn startingAndOptionalPawn)
			{
				if (CellFinder.TryFindRandomCellNear(startingAndOptionalPawn.Position, startingAndOptionalPawn.Map, Mathf.FloorToInt(4.9f), IsValidSpawnCell, out var spawnCell, 100))
				{
					DuplicateUtility.TryDuplicatePawn(startingAndOptionalPawn, spawnCell, out Pawn duplicate);
					duplicate.ageTracker.AgeChronologicalTicks = new IntRange(additionalChronoAge.TrueMin, startingAndOptionalPawn.ageTracker.AgeChronologicalYears).RandomInRange * 3600000L;
					//duplicate.apparel.A
				}

				bool IsValidSpawnCell(IntVec3 pos)
				{
					if (pos.Standable(startingAndOptionalPawn.Map) && pos.Walkable(startingAndOptionalPawn.Map))
					{
						return !pos.Fogged(startingAndOptionalPawn.Map);
					}
					return false;
				}
			}
		}

		private void ArchivePawns()
		{
			if (!archiveAllPawns)
			{
				return;
			}
			Gene_Archiver archiver = null;
			foreach (Pawn item in Find.GameInitData.startingAndOptionalPawns)
			{
				Gene_Archiver gene = item.genes?.GetFirstGeneOfType<Gene_Archiver>();
				if (gene != null)
				{
					archiver = gene;
					break;
				}
			}
			if (archiver == null)
			{
				return;
			}
			GeneDef triggerDef = archiver.pawn.genes?.GetFirstGeneOfType<Gene_MorpherTrigger>()?.def;
			foreach (Pawn item in Find.GameInitData.startingAndOptionalPawns)
			{
				if (item == archiver.pawn)
				{
					continue;
				}
				archiver.TryArchiveSelectedPawn(item, archiver.pawn, archiver);
				if (XaG_GeneUtility.TryRemoveAllConflicts(item, triggerDef))
				{
					item.genes.AddGene(triggerDef, !item.genes.Xenotype.inheritable);
				}
			}
			archiver.pawn.genes?.GetFirstGeneOfType<Gene_Archiver_SkillsSync>()?.SyncSkills();
		}

		private void Skills(Pawn p)
		{
			if (nullifySkills)
			{
				DuplicateUtility.NullifySkills(p, true);
			}
			else if (skills != null)
			{
				DuplicateUtility.SetSkills(p, skills);
			}
		}

		private void AgeCorrection(Pawn p)
		{
			if (additionalChronoAge.max > 0)
			{
				p.ageTracker.AgeChronologicalTicks += additionalChronoAge.RandomInRange * 3600000L;
			}
		}

		private void ChimeraGenes(Pawn p)
		{
			if (chimeraGenesPerXenotype != null)
			{
				GetGenesSetPerXenotype(p.genes.Xenotype, out GeneralHolder genesHolder);
				if (genesHolder != null && genesHolder.genes != null)
				{
					XaG_GeneUtility.AddGenesToChimera(p, genesHolder.genes, true);
					return;
				}
			}
			if (chimeraGenesPerBiomeDef != null && Find.GameInitData != null)
			{
				GetGenesSetPerBiome(Find.World.grid[Find.GameInitData.startingTile].PrimaryBiome, out GeneralHolder genesHolder);
				if (genesHolder != null && genesHolder.genes != null)
				{
					XaG_GeneUtility.AddGenesToChimera(p, genesHolder.genes, true);
					return;
				}
			}
			if (!chimeraGeneDefs.NullOrEmpty())
			{
				XaG_GeneUtility.AddGenesToChimera(p, chimeraGeneDefs, true);
			}
		}

		private bool GetGenesSetPerXenotype(XenotypeDef xenotypeDef, out GeneralHolder genesHolder)
		{
			genesHolder = null;
			foreach (GeneralHolder holder in chimeraGenesPerXenotype)
			{
				if (holder.xenotypeDef == xenotypeDef)
				{
					genesHolder = holder;
					return true;
				}
			}
			return false;
		}

		private bool GetGenesSetPerBiome(BiomeDef biomeDef, out GeneralHolder genesHolder)
		{
			genesHolder = null;
			foreach (GeneralHolder holder in chimeraGenesPerBiomeDef)
			{
				foreach (BiomeDef item in holder.biomeDefs)
				{
					if (item == biomeDef)
					{
						genesHolder = holder;
						return true;
					}
				}
			}
			return false;
		}

		private void SetGenes(Pawn p)
		{
			if (geneDefs.NullOrEmpty())
			{
				return;
			}
			foreach (GeneDef geneDef in geneDefs)
			{
				if (XaG_GeneUtility.HasGene(geneDef, p))
				{
					continue;
				}
				p.genes.AddGene(geneDef, xenogene: !p.genes.Xenotype.inheritable);
			}
		}

		private void NullifyBackstory(Pawn p)
		{
			if (nullifyBackstory)
			{
				p.relations.ClearAllRelations();
				DuplicateUtility.NullifyBackstory(p);
			}
		}

		private void AddMechlink(Pawn p)
		{
			if (addMechlink)
			{
				GeneResourceUtility.TryAddMechlink(p);
			}
		}

		private void SetXenotype(Pawn p)
		{
			if (xenotypeChances.NullOrEmpty())
			{
				return;
			}
			List<XenotypeDef> xenotypes = new();
			foreach (XenotypeChance xenoChance in xenotypeChances)
			{
				xenotypes.Add(xenoChance.xenotype);
			}
			if (xenotypes.Contains(p.genes.Xenotype) || !allowedXenotypes.NullOrEmpty() && allowedXenotypes.Contains(p.genes.Xenotype))
			{
				return;
			}
			if (xenotypeChances.TryRandomElementByWeight((XenotypeChance xenoChance) => xenoChance.chance, out XenotypeChance xenotypeChance))
			{
				ReimplanterUtility.SetXenotype_DoubleXenotype(p, xenotypeChance.xenotype);
			}
		}

		private void OverrideXenotype(Pawn p)
		{
			if (overridedXenotypes.NullOrEmpty())
			{
				return;
			}
			XenotypeDef overridedXenotype = p.genes.Xenotype;
			if (overridedXenotypes.Contains(overridedXenotype.defName))
			{
				XenotypeDef xenotypeDef = xenotypeChances?.RandomElementByWeight(xenos => xenos.chance)?.xenotype;
				if (xenotypeDef.inheritable)
				{
					ReimplanterUtility.SetXenotype_DoubleXenotype(p, xenotypeDef);
				}
				else
				{
					ReimplanterUtility.SetXenotype_Safe(p, xenotypeDef);
				}
				foreach (GeneDef geneDef in overridedXenotype.genes)
				{
					if (geneDef.IsGeneDefOfType<Gene_Shapeshifter>())
					{
						if (XaG_GeneUtility.TryRemoveAllConflicts(p, geneDef))
						{
							p.genes.AddGene(geneDef, false);
							break;
						}
					}
				}
			}
		}

		private string cachedDesc = null;

		public override string Summary(Scenario scen)
		{
			try
			{
				ScenarioSummary();
			}
			catch
			{
				cachedDesc = "";
			}
			return cachedDesc;
		}

		private void ScenarioSummary()
		{
			if (cachedDesc != null)
			{
				return;
			}
			StringBuilder stringBuilder = new();
			if (!xenotypeChances.NullOrEmpty())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_AllowedXenotypes".Translate().CapitalizeFirst() + ":\n" + xenotypeChances.Select((XenotypeChance x) => x.xenotype.LabelCap.ToString()).ToLineList(" - ") + (!allowedXenotypes.NullOrEmpty() ? "\n" + allowedXenotypes.Select((XenotypeDef x) => x.LabelCap.ToString()).ToLineList(" - ") : ""));
			}
			if (addMechlink)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_AddMechlink".Translate().CapitalizeFirst());
			}
			if (nullifyBackstory)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_NullifyBackstory".Translate().CapitalizeFirst());
			}
			if (nullifySkills)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_NullifySkills".Translate().CapitalizeFirst());
			}
			if (!geneDefs.NullOrEmpty())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_StartingGenes".Translate().CapitalizeFirst() + ":\n" + geneDefs.Select((GeneDef x) => x.LabelCap.ToString()).ToLineList(" - "));
			}
			if (!chimeraGeneDefs.NullOrEmpty())
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_ChimeraStartingGenes".Translate().CapitalizeFirst() + ":\n" + chimeraGeneDefs.Select((GeneDef x) => x.LabelCap.ToString()).ToLineList(" - "));
			}
			if (additionalChronoAge.max > 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WVC_XaG_ScenPart_AddChronoAge".Translate(additionalChronoAge.min, additionalChronoAge.max).CapitalizeFirst());
			}
			cachedDesc = stringBuilder.ToString();
		}
	}

}
