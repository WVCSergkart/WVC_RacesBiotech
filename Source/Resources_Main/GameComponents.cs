using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_GameComponent : GameComponent
	{

		// public override void StartedNewGame()
		// {
			// base.StartedNewGame();
			// Apply();
		// }

		private int nextRecache = 1500;

		public int cachedPawnsCount = 0;
		public int cachedXenotypesCount = 0;
		public int cachedNonHumansCount = 0;

		// public List<Pawn> cachedBloodHunters;

		public Game currentGame;

		public XaG_GameComponent(Game game)
		{
			currentGame = game;
		}

		public override void LoadedGame()
		{
			DevFixes();
			// XaG_General();
			// ResetCounter();
			// ResetCounter(1500);
		}

		public override void GameComponentTick()
		{
			nextRecache--;
			if (nextRecache > 0)
			{
				return;
			}
			XaG_General();
			ResetCounter(new(50000, 70000));
		}

		public void XaG_General()
		{
			// cachedBloodHunters = XaG_GeneUtility.GetAllBloodHunters();
			if (ModLister.IdeologyInstalled)
			{
				cachedPawnsCount = MiscUtility.CountAllPlayerControlledColonistsExceptClonesAndQuests();
				cachedXenotypesCount = MiscUtility.CountAllPlayerXenos();
				cachedNonHumansCount = MiscUtility.CountAllPlayerNonHumanlikes();
			}
		}

		public override void ExposeData()
		{
			Scribe_Values.Look(ref cachedPawnsCount, "cachedPawnsCount", 0);
			Scribe_Values.Look(ref cachedXenotypesCount, "cachedXenotypesCount", 0);
			Scribe_Values.Look(ref cachedNonHumansCount, "cachedNonHumansCount", 0);
			// Scribe_Collections.Look(ref cachedBloodHunters, "cachedBloodHunters", LookMode.Reference);
		}

		public void DevFixes()
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
					}
				}
				WVC_Biotech.settings.fixGeneTypesOnLoad = false;
			}
			if (WVC_Biotech.settings.fixGenesOnLoad)
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
							Log.Message(item.Name + ": ENDOGENES FIXED: " + "\n" + genes.Endogenes.Select((Gene x) => x.def.label).ToLineList("  - ", capitalizeItems: true));
						}
						if (!genes.Xenogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Xenogenes.ToList())
							{
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: true);
							}
							Log.Message(item.Name + ": XENOGENES FIXED: " + "\n" + genes.Xenogenes.Select((Gene x) => x.def.label).ToLineList("  - ", capitalizeItems: true));
						}
					}
				}
				WVC_Biotech.settings.fixGenesOnLoad = false;
			}
			if (WVC_Biotech.settings.fixGeneAbilitiesOnLoad)
			{
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
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
								}
							}
						}
					}
				}
				WVC_Biotech.settings.fixGeneAbilitiesOnLoad = false;
			}
			// if (WVC_Biotech.settings.fixThrallTypesOnLoad)
			// {
				// foreach (Pawn item in currentGame.CurrentMap.mapPawns.AllPawns.ToList())
				// {
					// if (item != null && item.RaceProps.Humanlike && item.genes != null)
					// {
					// }
				// }
				// WVC_Biotech.settings.fixGeneAbilitiesOnLoad = false;
			// }
		}

		public void ResetCounter(IntRange interval)
		{
			nextRecache = interval.RandomInRange;
		}

	}

}