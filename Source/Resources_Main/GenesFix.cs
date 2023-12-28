using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class FixBrokenGenes_GameComponent : GameComponent
	{
		// public override void StartedNewGame()
		// {
			// base.StartedNewGame();
			// Apply();
		// }

		public override void LoadedGame()
		{
			base.LoadedGame();
			Apply();
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
        public FixBrokenGenes_GameComponent(Game game)
        {
        }

		// public FixBrokenGenes_GameComponent()
		// {
		// }

		public static void Apply()
		{
			if (WVC_Biotech.settings.fixGenesOnLoad)
			{
				// foreach (Pawn item in Current.Game.World.worldPawns.AllPawnsAliveOrDead)
				// List<Pawn> pawns = Current.Game.CurrentMap.mapPawns.AllPawns;
				// Log.Error("1");
				foreach (Pawn item in Current.Game.CurrentMap.mapPawns.AllPawns.ToList())
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
				foreach (Pawn item in Current.Game.CurrentMap.mapPawns.AllPawns.ToList())
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