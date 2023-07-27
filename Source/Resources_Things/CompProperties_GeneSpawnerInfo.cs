using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	// This way of displaying spawner info is not the best, but it looks much nicer than the old one.
	public class CompProperties_GeneSpawnerInfo : CompProperties
	{

		public bool onlyPlayerFaction = true;

		public CompProperties_GeneSpawnerInfo()
		{
			compClass = typeof(CompGeneSpawnerInfo);
		}
	}

	public class CompGeneSpawnerInfo : ThingComp
	{

		private CompProperties_GeneSpawnerInfo Props => (CompProperties_GeneSpawnerInfo)props;

		public List<Gene_Spawner> cachedSpawnerGenes;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			CacheInfoGenes();
		}

		public void CacheInfoGenes()
		{
			if (parent is Pawn pawn)
			{
				if (Props.onlyPlayerFaction)
				{
					if (pawn.Faction != null && pawn.Faction == Faction.OfPlayer)
					{
						cachedSpawnerGenes = ActiveSpawnerGenes(pawn);
					}
				}
				else
				{
					cachedSpawnerGenes = ActiveSpawnerGenes(pawn);
				}
			}
		}

		public List<Gene_Spawner> ActiveSpawnerGenes(Pawn pawn)
		{
			List<Gene_Spawner> list = new();
			// Pawn pawn = parent as Pawn;
			List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true && genesListForReading[i].def.geneClass == typeof(Gene_Spawner))
				{
					list.Add((Gene_Spawner)genesListForReading[i]);
				}
			}
			// Log.Error("Pawn " + pawn.Name + " ActiveSpawnerGenes cached: " + list.Count);
			return list;
		}

		public override string CompInspectStringExtra()
		{
			if (WVC_Biotech.settings.enableGeneSpawnerGizmo == true && !cachedSpawnerGenes.NullOrEmpty())
			{
                if (parent is Pawn pawn && !pawn.Drafted)
                {
                    if (Props.onlyPlayerFaction)
                    {
                        if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
                        {
                            return null;
                        }
                    }
                    return Info();
                }
            }
			return null;
		}

		public string Info()
		{
			string info = null;
			for (int i = 0; i < cachedSpawnerGenes.Count; i++)
			{
				if (i > 0)
				{
					info += "\n";
				}
				info += "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(cachedSpawnerGenes[i].ThingDefToSpawn, null, cachedSpawnerGenes[i].StackCount)).Resolve() + ": " + cachedSpawnerGenes[i].ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return info;
		}
	}

}
