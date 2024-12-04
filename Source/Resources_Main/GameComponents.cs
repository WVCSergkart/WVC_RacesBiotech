using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_GameComponent : GameComponent
	{

		public override void StartedNewGame()
		{
			if (!WVC_Biotech.settings.enable_StartingFoodPolicies)
			{
				return;
			}
			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef x) => x.GetStatValueAbstract(StatDefOf.Nutrition) > 0f).ToList();
			FoodPolicy bloodEaterFoodPolicy = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
			bloodEaterFoodPolicy.label = "WVC_XaG_BloodEaterFoodPolicy".Translate();
			FoodPolicy energyFoodPolicy = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
			energyFoodPolicy.label = "WVC_XaG_EnergyFoodPolicy".Translate();
			foreach (ThingDef item in thingDefs)
			{
				if (item.IsDrug)
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
					energyFoodPolicy.filter.SetAllow(item, allow: true);
				}
				else if (item.ingestible?.foodType == FoodTypeFlags.Fluid)
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
					energyFoodPolicy.filter.SetAllow(item, allow: false);
				}
				else
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: false);
					energyFoodPolicy.filter.SetAllow(item, allow: false);
				}
			}
		}

		private int nextRecache = 444;
		private int nextSecondRecache = 0;

		public Game currentGame;

		public XaG_GameComponent(Game game)
		{
			currentGame = game;
		}

		public override void LoadedGame()
		{
			DevFixes();
		}

		public override void GameComponentTick()
		{
			nextRecache--;
			if (nextRecache > 0)
			{
				return;
			}
			XaG_General();
			ResetCounter(new(40000, 70000));
		}

		public void XaG_General()
		{
			if (ModLister.IdeologyInstalled)
			{
				MiscUtility.CountAllPlayerControlledPawns_ForIdeology();
				//StaticCollectionsClass.cachedPawnsCount = MiscUtility.CountAllPlayerControlledColonistsExceptClonesAndQuests();
				//StaticCollectionsClass.cachedXenotypesCount = MiscUtility.CountAllPlayerXenos();
				//StaticCollectionsClass.cachedNonHumansCount = MiscUtility.CountAllPlayerNonHumanlikes();
				//Log.Error("Colonists: " + colonists + ". Xenos: " + xenos + ". Non-humans: " + nonHumans);
				nextSecondRecache++;
				if (nextSecondRecache >= 2)
                {
					MiscUtility.ForeverAloneDevelopmentPoints();
					nextSecondRecache = 1;
				}
			}
		}

		// DEV
		public void DevFixes()
		{
			FixGenesClasses();
			ResetGenes();
			FixGeneAbilities();
		}

		private static void FixGeneAbilities()
		{
			if (WVC_Biotech.settings.fixGeneAbilitiesOnLoad)
			{
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						// List<AbilityDef> fixedAbilities = new();
						Pawn_AbilityTracker abilities = item.abilities;
						if (abilities != null && !abilities.AllAbilitiesForReading.NullOrEmpty())
						{
							foreach (Ability ability in abilities.AllAbilitiesForReading.ToList())
							{
								if (XaG_GeneUtility.AbilityIsGeneAbility(ability))
								{
									abilities.RemoveAbility(ability.def);
									abilities.GainAbility(ability.def);
									Log.Message(item.Name + ": ABILITY FIXED: " + ability.def.label);
									// fixedAbilities.Add(ability.def);
								}
							}
						}
						AddMissingGeneAbilities(item);
					}
				}
				WVC_Biotech.settings.fixGeneAbilitiesOnLoad = false;
			}
		}

		public static void AddMissingGeneAbilities(Pawn item)
		{
			List<AbilityDef> pawnAbilities = MiscUtility.ConvertAbilitiesInAbilityDefs(item.abilities.AllAbilitiesForReading);
			foreach (Gene gene in item.genes.GenesListForReading)
			{
				if (gene.def?.abilities != null)
				{
					foreach (AbilityDef ability in gene.def.abilities)
					{
						if (!pawnAbilities.Contains(ability))
						{
							item.abilities.GainAbility(ability);
						}
					}
				}
			}
		}

		private static void ResetGenes()
		{
			if (WVC_Biotech.settings.resetGenesOnLoad)
			{
				// foreach (Pawn item in currentGame.World.worldPawns.AllPawnsAliveOrDead)
				// List<Pawn> pawns = currentGame.CurrentMap.mapPawns.AllPawns;
				// Log.Error("1");
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						Pawn_GeneTracker genes = item.genes;
						if (!genes.Endogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Endogenes.ToList())
							{
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: false);
							}
							Log.Message(item.Name + ": ENDOGENES FIXED: " + "\n" + genes.Endogenes.Select((Gene x) => x.def.label).ToLineList("	 - ", capitalizeItems: true));
						}
						if (!genes.Xenogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Xenogenes.ToList())
							{
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: true);
							}
							Log.Message(item.Name + ": XENOGENES FIXED: " + "\n" + genes.Xenogenes.Select((Gene x) => x.def.label).ToLineList("	 - ", capitalizeItems: true));
						}
						ReimplanterUtility.PostImplantDebug(item);
					}
				}
				WVC_Biotech.settings.resetGenesOnLoad = false;
			}
		}

		private static void FixGenesClasses()
		{
			if (WVC_Biotech.settings.fixGeneTypesOnLoad)
			{
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						Pawn_GeneTracker genes = item.genes;
						if (!genes.Endogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Endogenes.ToList())
							{
								if (gene.GetType() == gene.def.geneClass)
								{
									continue;
								}
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: false);
								Log.Message(item.Name + ": ENDOGENE TYPE FIXED: " + gene.def.defName);
							}
						}
						if (!genes.Xenogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Xenogenes.ToList())
							{
								if (gene.GetType() == gene.def.geneClass)
								{
									continue;
								}
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: true);
								Log.Message(item.Name + ": XENOGENE TYPE FIXED: " + gene.def.defName);
							}
						}
						ReimplanterUtility.PostImplantDebug(item);
					}
				}
				WVC_Biotech.settings.fixGeneTypesOnLoad = false;
			}
		}

		public void ResetCounter(IntRange interval)
		{
			nextRecache = interval.RandomInRange;
		}

	}

}