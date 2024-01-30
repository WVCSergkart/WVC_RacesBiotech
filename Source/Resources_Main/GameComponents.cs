using RimWorld;
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

		public Game currentGame;

		public XaG_GameComponent(Game game)
		{
			currentGame = game;
		}

		public override void LoadedGame()
		{
			// base.LoadedGame();
			// foreach (Pawn item in StaticCollectionsClass.skillsNotDecayPawns.ToList())
			// {
				// if (item == null || item.Discarded || item.Destroyed)
				// {
					// StaticCollectionsClass.RemoveSkillDecayGenePawnFromList(item);
				// }
			// }
			// StaticCollectionsClass.skillsNotDecayPawns.Clear();
			DevFixes();
		}

		// public virtual void GameComponentTick()
		// {
		// }

		// public void Ideology()
		// {
		// }

		public void DevFixes()
		{
			if (WVC_Biotech.settings.fixGeneTypesOnLoad)
			{
				foreach (Pawn item in currentGame.CurrentMap.mapPawns.AllPawns.ToList())
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
				foreach (Pawn item in currentGame.CurrentMap.mapPawns.AllPawns.ToList())
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
				foreach (Pawn item in currentGame.CurrentMap.mapPawns.AllPawns.ToList())
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
		}

	}

}