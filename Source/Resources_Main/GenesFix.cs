using RimWorld;
using System.Collections.Generic;
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

		public FixBrokenGenes_GameComponent(Game game)
		{
		}

		// public FixBrokenGenes_GameComponent()
		// {
		// }

		public static void Apply()
		{
			if (!WVC_Biotech.settings.fixGenesOnLoad)
			{
				return;
			}
			// foreach (Pawn item in Current.Game.World.worldPawns.AllPawnsAliveOrDead)
			List<Pawn> pawns = Current.Game.CurrentMap.mapPawns.AllPawns;
			// Log.Error("1");
			foreach (Pawn item in pawns.ToList())
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
							Log.Message(item.Name + ": GENE FIXED: " + gene.def.label);
						}
					}
					if (!genes.Xenogenes.NullOrEmpty())
					{
						foreach (Gene gene in genes.Xenogenes.ToList())
						{
							genes.RemoveGene(gene);
							genes.AddGene(gene.def, xenogene: true);
							Log.Message(item.Name + ": GENE FIXED: " + gene.def.label);
						}
					}
				}
			}
			WVC_Biotech.settings.fixGenesOnLoad = false;
		}
	}
}