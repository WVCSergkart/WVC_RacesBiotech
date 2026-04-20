using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	// Hemogen
	public class Gene_EternalHunger : Gene_HemogenOffset, IGeneOverriddenBy, IGeneBloodfeeder, IGeneRecacheable, IGeneAddOrRemoveHediff, IGeneNotifyLifeStageStarted
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		// Gene

		private float? cachedResourceLossPerDay;
		public override float ResourceLossPerDay
		{
			get
			{
				if (!cachedResourceLossPerDay.HasValue)
				{
					XaG_GeneUtility.GetBiostatsFromList(pawn.genes.GenesListForReading, out _, out int met, out _);
					cachedResourceLossPerDay = base.ResourceLossPerDay * (1f - (met * 0.1f));
				}
				return cachedResourceLossPerDay.Value;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			Local_AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			//Notify_GenesRecache(null);
			Local_AddOrRemoveHediff();
		}

		public void Notify_Override()
		{
			//Notify_GenesRecache(null);
			Local_AddOrRemoveHediff();
		}

		public void Local_AddOrRemoveHediff()
		{
			try
			{
				cachedResourceLossPerDay = null;
				HediffUtility.TryAddOrRemoveHediff(Props?.hediffDefName, pawn, this);
			}
			catch (Exception arg)
			{
				Log.Error("Error in Gene_EternalHunger in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
			}
			shouldUpdateHediff = false;
		}

		private bool shouldUpdateHediff = true;
		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			//if (pawn.IsHashIntervalTick(56421, delta))
			//{
			//	Local_AddOrRemoveHediff();
			//}
			if (!pawn.IsHashIntervalTick(455, delta))
			{
				return;
			}
			if (shouldUpdateHediff)
			{
				Local_AddOrRemoveHediff();
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (Hemogen == null)
			{
				return;
			}
			SyncNeedFoodWithHemogen();
		}

		private void SyncNeedFoodWithHemogen()
		{
			if (pawn.TryGetNeedFood(out Need_Food need_Food))
			{
				need_Food.CurLevelPercentage = Hemogen.ValuePercent;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Local_RemoveHediff();
		}

		public void Local_RemoveHediff()
		{
			HediffUtility.TryRemoveHediff(Props?.hediffDefName, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: AddOrRemoveHediff",
					action = delegate
					{
						Local_AddOrRemoveHediff();
					}
				};
			}
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active || Hemogen == null)
			{
				return;
			}
			if (thing.IsRawMeat())
			{
				Hemogen.Value += thing.def.GetStatValueAbstract(StatDefOf.Nutrition) * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor) * numTaken;
			}
			SyncNeedFoodWithHemogen();
		}

		public void Notify_Bloodfeed(Pawn victim)
		{
			if (Hemogen == null)
			{
				return;
			}
			Hemogen.Value += Hemogen.Max * victim.BodySize;
			SyncNeedFoodWithHemogen();
		}

		public void Notify_GenesRecache(Gene changedGene)
		{
			shouldUpdateHediff = true;
			cachedResourceLossPerDay = null;
		}

		public void Notify_LifeStageStarted()
		{
			shouldUpdateHediff = true;
			cachedResourceLossPerDay = null;
		}

	}

}
