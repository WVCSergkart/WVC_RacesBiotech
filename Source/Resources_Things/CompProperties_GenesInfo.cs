using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_GenesDisplayInfo : CompProperties
	{

		public int recacheFrequency = 11435;

		//public Type golemGizmoType = typeof(GeneGizmo_Golems);

		public CompProperties_GenesDisplayInfo()
		{
			compClass = typeof(CompGenesDisplayInfo);
		}

	}

	public class CompGenesDisplayInfo : ThingComp
	{

		private CompProperties_GenesDisplayInfo Props => (CompProperties_GenesDisplayInfo)props;

		// =================

		public List<Gene_Spawner> cachedSpawnerGenes = null;
		public List<Gene_Mechlink> cachedMechlinkGenes = null;
		public Gene_Wings cachedWingGene = null;
		public Gene_Undead cachedUndeadGene = null;
		public Gene_GauranlenConnection cachedGauranlenConnectionGene = null;
		public Gene_Scarifier cachedScarifierGene = null;
		public Gene_GeneticInstability cachedGeneticInstabilityGene = null;

		// private bool cachedShouldShowGolemsInfo = false;

		private int nextRecache = -1;

		public void CacheInfoGenes(Pawn pawn)
		{
			ReCacheGenes(pawn);
			// if (WVC_Biotech.settings.enableGolemsInfo)
			// {
				// if (MechanitorUtility.IsMechanitor(pawn) && GolemsUtility.MechanitorHasAnyGolems(pawn))
				// {
					// cachedShouldShowGolemsInfo = true;
				// }
			// }
			nextRecache = Find.TickManager.TicksGame + Props.recacheFrequency;
		}

		// ==================

		public void ReCacheGenes(Pawn pawn)
		{
			// Nullify
			cachedSpawnerGenes = null;
			cachedWingGene = null;
			cachedUndeadGene = null;
			cachedGauranlenConnectionGene = null;
			cachedScarifierGene = null;
			cachedGeneticInstabilityGene = null;
			// Cache
			List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
			if (genesListForReading.NullOrEmpty())
			{
				return;
			}
			List<Gene_Spawner> spawners = new();
			List<Gene_Mechlink> mechlinks = new();
			foreach (Gene item in genesListForReading)
			{
				if (!item.Active)
				{
					continue;
				}
				if (WVC_Biotech.settings.enableGeneSpawnerGizmo && item is Gene_Spawner geneSpawner)
				{
					spawners.Add(geneSpawner);
				}
				else if (WVC_Biotech.settings.enableGeneBlesslinkInfo && item is Gene_Mechlink geneMechlink)
				{
					mechlinks.Add(geneMechlink);
				}
				else if (WVC_Biotech.settings.enableGeneGauranlenConnectionInfo && item is Gene_GauranlenConnection gene_GauranlenConnection)
				{
					cachedGauranlenConnectionGene = gene_GauranlenConnection;
				}
				else if (WVC_Biotech.settings.enableGeneWingInfo && item is Gene_Wings gene_Wings)
				{
					cachedWingGene = gene_Wings;
				}
				else if (WVC_Biotech.settings.enableGeneUndeadInfo && item is Gene_Undead gene_Undead)
				{
					cachedUndeadGene = gene_Undead;
				}
				else if (WVC_Biotech.settings.enableGeneScarifierInfo && item is Gene_Scarifier gene_Scarifier)
				{
					cachedScarifierGene = gene_Scarifier;
				}
				else if (WVC_Biotech.settings.enableGeneInstabilityInfo && item is Gene_GeneticInstability gene_GeneticInstability)
				{
					cachedGeneticInstabilityGene = gene_GeneticInstability;
				}
			}
			cachedSpawnerGenes = spawners;
			cachedMechlinkGenes = mechlinks;
		}

		// =================

		public override string CompInspectStringExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (WVC_Biotech.settings.enableGenesInfo)
			{
				if (parent is Pawn pawn)
				{
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
			// if (cachedBlesslinkGene != null && cachedBlesslinkGene.summonMechanoids)
			// {
				// if (!info.NullOrEmpty())
				// {
					// info += "\n";
				// }
				// info += "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + cachedBlesslinkGene.timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			// }
			if (!cachedMechlinkGenes.NullOrEmpty())
			{
				for (int i = 0; i < cachedMechlinkGenes.Count; i++)
				{
					if (!cachedMechlinkGenes[i].summonMechanoids || cachedMechlinkGenes[i].timeForNextSummon <= 0)
					{
						continue;
					}
					if (!info.NullOrEmpty() && i == 0)
					{
						info += "\n";
					}
					if (i > 0)
					{
						info += "\n";
					}
					info += "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + cachedMechlinkGenes[i].timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
			}
			if (pawn.Drafted)
			{
				return info;
			}
			if (!cachedSpawnerGenes.NullOrEmpty())
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
					info += "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(cachedSpawnerGenes[i]?.Props?.thingDefToSpawn, null, cachedSpawnerGenes[i].FinalStackCount)).Resolve() + ": " + cachedSpawnerGenes[i].ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
			}
			if (cachedGeneticInstabilityGene != null)
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_GeneticInstability_On_Info".Translate().Resolve() + ": " + cachedGeneticInstabilityGene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			if (cachedGauranlenConnectionGene != null)
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_GauranlenConnection_NextDryad_Info".Translate().Resolve() + ": " + cachedGauranlenConnectionGene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			if (cachedScarifierGene != null && cachedScarifierGene.CanScarify)
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_Scarifier_On_Info".Translate().Resolve() + ": " + cachedScarifierGene.scarifyInterval.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			if (cachedWingGene != null && cachedWingGene.OnOrOff)
			{
				if (!info.NullOrEmpty())
				{
					info += "\n";
				}
				info += "WVC_XaG_Gene_Wings_On_Info".Translate().Resolve();
			}
			return info;
		}

		// =================

		// private Gizmo_MaxGolems gizmo;

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (parent.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			Pawn pawn = parent as Pawn;
			if (Find.TickManager.TicksGame >= nextRecache)
			{
				CacheInfoGenes(pawn);
			}
			// if (cachedShouldShowGolemsInfo && WVC_Biotech.settings.enableGolemsInfo)
			// {
				// if (gizmo == null)
				// {
					// gizmo = (Gizmo_MaxGolems)Activator.CreateInstance(Props.golemGizmoType, pawn);
				// }
				// if (Find.Selector.SelectedPawns.Count == 1)
				// {
					// yield return gizmo;
				// }
			// }
		}

	}

}
