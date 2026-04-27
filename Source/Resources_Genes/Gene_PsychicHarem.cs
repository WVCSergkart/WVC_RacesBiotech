using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_PsychicHarem : XaG_Gene, IGeneOverriddenBy, IGeneRecacheable
	{

		private GeneExtension_Opinion cachedOpinionExtension;
		public GeneExtension_Opinion Opinion
		{
			get
			{
				if (cachedOpinionExtension == null)
				{
					cachedOpinionExtension = def?.GetModExtension<GeneExtension_Opinion>();
				}
				return cachedOpinionExtension;
			}
		}

		//private GeneExtension_Giver cachedGiverExtension;
		//public GeneExtension_Giver Giver
		//{
		//	get
		//	{
		//		if (cachedGiverExtension == null)
		//		{
		//			cachedGiverExtension = def.GetModExtension<GeneExtension_Giver>();
		//		}
		//		return cachedGiverExtension;
		//	}
		//}

		private static List<Pawn> cachedPawns;
		public static List<Pawn> Harem
		{
			get
			{
				if (cachedPawns == null)
				{
					List<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
					{
						if (!pawn.IsPsychicSensitive())
						{
							continue;
						}
						Gene_PsychicHarem packGene = pawn.genes?.GetFirstGeneOfType<Gene_PsychicHarem>();
						if (packGene != null)
						{
							list.Add(pawn);
						}
					}
					cachedPawns = list;
					shouldUpdate = !cachedPawns.NullOrEmpty();
				}
				return cachedPawns;
			}
		}

		public static bool InHarem(Pawn caller)
		{
			if (caller == null)
			{
				return false;
			}
			return Harem.Contains(caller);
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetCollection();
		}

		private static bool shouldUpdate = true;
		public static bool ShouldUpdate => shouldUpdate;

		public override void TickInterval(int delta)
		{
			if (ShouldUpdate)
			{
				UpdThoughts();
			}
		}

		private void UpdThoughts()
		{
			if (pawn?.Faction != Faction.OfPlayer)
			{
				return;
			}
			try
			{
				foreach (Pawn pawn in Harem)
				{
					foreach (Pawn member in Harem)
					{
						if (member != pawn)
						{
							pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.AboutMeThoughtDef, member);
							member.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.AboutMeThoughtDef, pawn);
						}
					}
					pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, null);
					//HediffUtility.TryAddHediff(Giver.hediffDef, pawn, def);
					//Hediff_PsychicHarem.curStage = null;
				}
			}
			catch (Exception arg)
			{
				Log.Warning("Failed update psychic harem. Reason: " + arg.Message);
			}
			shouldUpdate = false;
			Thought_PsychicHarem.cachedMoodOffset = null;
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

		public static void ResetCollection()
		{
			cachedPawns = null;
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			ResetCollection();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public void Notify_Override()
		{
			ResetCollection();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			ResetCollection();
			foreach (Pawn pawn in Harem)
			{
				Thought_Memory thought_Memory = pawn.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(Opinion.thoughtDef);
				if (thought_Memory != null && thought_Memory is Thought_PsychicHarem haremMood)
				{
					haremMood.badMoodTicks = Find.TickManager.TicksGame + (60000 * 5);
				}
			}
			//int ticksGame = Find.TickManager.TicksGame;
			//badMoodStartTicks = ticksGame;
			//badMoodEndTicks = ticksGame + (60000 * 5);
		}

		//private static int badMoodStartTicks = -1;
		//private static int badMoodEndTicks = -1;
		//public static bool HaremInBadMood
		//{
		//	get
		//	{
		//		if (badMoodStartTicks == -1 || badMoodEndTicks == -1)
		//		{
		//			return false;
		//		}
		//		int ticksGame = Find.TickManager.TicksGame;
		//		if (badMoodEndTicks < ticksGame || badMoodStartTicks > ticksGame)
		//		{
		//			return false;
		//		}
		//		return true;
		//	}
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	//Scribe_References.Look(ref deadPawnRef, "deadPawnRef", saveDestroyedThings: true);
		//	//Scribe_Values.Look(ref badMoodTicks, "lastBadMoodTick", -1);
		//	//if (Scribe.mode == LoadSaveMode.PostLoadInit)
		//	//{
		//	//	if (!HaremInBadMood)
		//	//	{
		//	//		badMoodTicks = -1;
		//	//	}
		//	//}
		//}

	}

}
