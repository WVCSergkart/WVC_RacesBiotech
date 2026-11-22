using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Noise;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class SacrificerMode
	{

		public SacrificerModeDef def;

		public virtual bool CanFireNow(Pawn pawn)
		{
			return pawn != null;
		}

		public virtual bool IsAllowed(Pawn pawn)
		{
			if (pawn.HomeFaction != Faction.OfPlayerSilentFail || pawn.IsQuestLodger())
			{
				return false;
			}
			//if (pawn.IsQuestLodger() || (pawn.HomeFaction != Faction.OfPlayer && pawn.HomeFaction != null && !pawn.HomeFaction.HostileTo(pawn.Faction)))
			//{
			//	return false;
			//}
			return true;
		}

		public virtual void SpawnReward(Pawn pawn, IntVec3 cell, Map map, Gene_Sacrificer gene)
		{
			if (def.rewards != null)
			{
				foreach (SacrificerModeDef.ThingDefCountWithChance reward in def.rewards)
				{
					if (Rand.Chance(reward.chance))
					{
						MiscUtility.SpawnItems(reward.thingDef, reward.count, cell, map);
					}
				}
				return;
			}
			int rewardType = new IntRange(1, 4).RandomInRange;
			switch (rewardType)
			{
				case 1:
					MiscUtility.SpawnItems(ThingDefOf.Steel, 10, cell, map);
					MiscUtility.SpawnItems(ThingDefOf.ComponentIndustrial, 1, cell, map);
					break;
				case 2:
					MiscUtility.SpawnItems(ThingDefOf.Bioferrite, 40, cell, map);
					if (Rand.Chance(0.1f))
					{
						MiscUtility.SpawnItems(ThingDefOf.Shard, 1, cell, map);
					}
					break;
				case 3:
					MiscUtility.SpawnItems(ThingDefOf.Steel, 25, cell, map);
					MiscUtility.SpawnItems(ThingDefOf.GravlitePanel, 5, cell, map);
					if (Rand.Chance(0.25f))
					{
						MiscUtility.SpawnItems(ThingDefOf.ComponentIndustrial, 1, cell, map);
					}
					if (Rand.Chance(0.1f))
					{
						MiscUtility.SpawnItems(ThingDefOf.Gravcore, 1, cell, map);
					}
					break;
				default:
					MiscUtility.SpawnItems(ThingDefOf.Steel, 40, cell, map);
					break;
			}
		}

		public virtual void GenesInteractions(Gene_Sacrificer gene, Pawn sacrifice)
		{
			if (sacrifice.RaceProps.IsFlesh)
			{
				gene.pawn?.genes?.GetFirstGeneOfType<Gene_FleshmassNucleus>()?.Notify_DevouredFlesh(sacrifice);
			}
		}

		public virtual void PostRemove(Gene_Sacrificer gene)
		{

		}

	}

	public class SacrificerMode_Disabled : SacrificerMode
	{

		public override bool CanFireNow(Pawn pawn)
		{
			return false;
		}

		public override bool IsAllowed(Pawn pawn)
		{
			return false;
		}

	}

	public class SacrificerMode_Biopunk : SacrificerMode
	{

		public override bool IsAllowed(Pawn pawn)
		{
			if (pawn.IsHuman())
			{
				return false;
			}
			return base.IsAllowed(pawn);
		}

		//public override void SpawnReward(Pawn pawn, IntVec3 cell, Map map)
		//{
		//	if (Rand.Chance())
		//	{
		//		MiscUtility.SpawnItems(ThingDefOf.Shard, 1, cell, map);
		//	}
		//	base.SpawnReward(pawn, cell, map);
		//}

	}

	public class SacrificerMode_Mechamancer : SacrificerMode
	{

		public override bool IsAllowed(Pawn pawn)
		{
			if (pawn.RaceProps.IsDrone || pawn.RaceProps.IsMechanoid || pawn.IsAndroid())
			{
				return false;
			}
			return base.IsAllowed(pawn);
		}

		public override void SpawnReward(Pawn pawn, IntVec3 cell, Map map, Gene_Sacrificer gene)
		{
			if (Rand.Chance(0.85f) || !HediffUtility.TryAddMechlink(pawn))
			{
				base.SpawnReward(pawn, cell, map, gene);
			}
		}

	}

	public class SacrificerMode_Animalist : SacrificerMode
	{

		public override bool IsAllowed(Pawn pawn)
		{
			if (pawn.IsAnimal)
			{
				return false;
			}
			return base.IsAllowed(pawn);
		}

	}

	public class SacrificerMode_Overlord : SacrificerMode
	{

		public override bool IsAllowed(Pawn pawn)
		{
			if (pawn.IsMutant)
			{
				return false;
			}
			return base.IsAllowed(pawn);
		}

		public override void SpawnReward(Pawn pawn, IntVec3 cell, Map map, Gene_Sacrificer gene)
		{
			if (Rand.Chance(0.07f) && HediffUtility.TryGetBestMutation(pawn, out HediffDef mutation) && HediffUtility.TryGiveFleshmassMutation(pawn, mutation))
			{
				return;
			}
			base.SpawnReward(pawn, cell, map, gene);
		}

	}

	public class SacrificerMode_ChosenOnes : SacrificerMode
	{

		private List<Pawn> chosenOnes;

		public override bool IsAllowed(Pawn pawn)
		{
			if (chosenOnes == null)
			{
				chosenOnes = new();
			}
			if (chosenOnes.Contains(pawn))
			{
				//Log.Error(pawn.Name.ToString());
				return false;
			}
			if (chosenOnes.Count < def.chosenOnesLimit && pawn.genes?.GetFirstGeneOfType<Gene_Sacrificer>() != null)
			{
				chosenOnes.Add(pawn);
				return false;
			}
			return base.IsAllowed(pawn);
		}

		public override void SpawnReward(Pawn pawn, IntVec3 cell, Map map, Gene_Sacrificer gene)
		{
			gene.SetMet(Find.StoryWatcher.statsRecord.colonistsKilled);
			pawn.Ideo?.development?.TryAddDevelopmentPoints(1);
		}

		//public override void PostRemove(Gene_Sacrificer gene)
		//{
		//	if (gene.Active)
		//	{
		//		gene.pawn?.genes?.AddGene(gene.def, false);
		//	}
		//}

	}

}
