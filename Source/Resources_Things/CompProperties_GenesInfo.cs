using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GenesDisplayInfo : CompProperties
	{

		public bool onlyPlayerFaction = true;

		public CompProperties_GenesDisplayInfo()
		{
			compClass = typeof(CompGenesDisplayInfo);
		}
	}

	public class CompGenesDisplayInfo : ThingComp
	{

		private CompProperties_GenesDisplayInfo Props => (CompProperties_GenesDisplayInfo)props;

		public List<Gene_Spawner> cachedSpawnerGenes = null;
		public Gene_Wings cachedWingGene = null;
		public Gene_Undead cachedUndeadGene = null;
		public Gene_DustMechlink cachedBlesslinkGene = null;
		public Gene_Scarifier cachedScarifierGene = null;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (parent is Pawn pawn)
			{
				if (Props.onlyPlayerFaction)
				{
					if (pawn.Faction != null && pawn.Faction == Faction.OfPlayer)
					{
						CacheInfoGenes(pawn);
					}
				}
				else
				{
					CacheInfoGenes(pawn);
				}
			}
		}

		public void CacheInfoGenes(Pawn pawn)
		{
			if (WVC_Biotech.settings.enableGeneSpawnerGizmo)
			{
				cachedSpawnerGenes = ActiveSpawnerGenes(pawn);
			}
			if (WVC_Biotech.settings.enableGeneWingInfo)
			{
				cachedWingGene = HaveWings(pawn);
			}
			if (WVC_Biotech.settings.enableGeneUndeadInfo)
			{
				cachedUndeadGene = Undead(pawn);
			}
			if (WVC_Biotech.settings.enableGeneBlesslinkInfo)
			{
				cachedBlesslinkGene = HaveBlesslink(pawn);
			}
			if (WVC_Biotech.settings.enableGeneBlesslinkInfo)
			{
				cachedScarifierGene = HaveScarifier(pawn);
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

		public Gene_Undead Undead(Pawn pawn)
		{
			Gene_Undead canResurrect = pawn.genes?.GetFirstGeneOfType<Gene_Undead>();
			if (canResurrect != null)
			{
				return canResurrect;
			}
			return null;
		}

		public Gene_Wings HaveWings(Pawn pawn)
		{
			Gene_Wings wings = pawn.genes?.GetFirstGeneOfType<Gene_Wings>();
			if (wings != null)
			{
				return wings;
			}
			return null;
		}

		public Gene_DustMechlink HaveBlesslink(Pawn pawn)
		{
			Gene_DustMechlink blesslink = pawn.genes?.GetFirstGeneOfType<Gene_DustMechlink>();
			if (blesslink != null)
			{
				return blesslink;
			}
			return null;
		}

		public Gene_Scarifier HaveScarifier(Pawn pawn)
		{
			Gene_Scarifier scarifier = pawn.genes?.GetFirstGeneOfType<Gene_Scarifier>();
			if (scarifier != null)
			{
				return scarifier;
			}
			return null;
		}

		public override string CompInspectStringExtra()
		{
			if (WVC_Biotech.settings.enableGenesInfo)
			{
				if (parent is Pawn pawn)
				{
					if (Props.onlyPlayerFaction)
					{
						if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
						{
							return null;
						}
					}
					return Info(pawn);
				}
			}
			return null;
		}

		public string Info(Pawn pawn)
		{
			string info = null;
			if (cachedUndeadGene != null && cachedUndeadGene.UndeadCanResurrect)
			{
				info += "WVC_XaG_Gene_Undead_On_Info".Translate().Resolve();
			}
			if (!pawn.Drafted && !cachedSpawnerGenes.NullOrEmpty())
			{
				for (int i = 0; i < cachedSpawnerGenes.Count; i++)
				{
					if (!info.NullOrEmpty() && i == 0)
					{
						info += "\n";
					}
					if (i > 0)
					{
						info += "\n";
					}
					info += "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(cachedSpawnerGenes[i].ThingDefToSpawn, null, cachedSpawnerGenes[i].StackCount)).Resolve() + ": " + cachedSpawnerGenes[i].ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
			}
			if (cachedBlesslinkGene != null && cachedBlesslinkGene.summonMechanoids)
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + cachedBlesslinkGene.timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			if (!pawn.Drafted && cachedScarifierGene != null && cachedScarifierGene.CanScarifyCheck())
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_Scarifier_On_Info".Translate().Resolve() + ": " + cachedScarifierGene.scarifyInterval.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			if (cachedWingGene != null && pawn.health.hediffSet.HasHediff(cachedWingGene.HediffDefName))
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_Wings_On_Info".Translate().Resolve();
			}
			return info;
		}
	}

}
