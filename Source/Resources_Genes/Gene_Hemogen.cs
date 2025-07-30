using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    // Rare Hemogen Drain
    public class Gene_HemogenDependant : Gene
	{

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;

		public Gene_Hemogen Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override void TickInterval(int delta)
		{

		}

	}

	// Rare Hemogen Drain
	public class Gene_HemogenOffset : Gene_HemogenDependant, IGeneResourceDrain
	{

		public Gene_Resource Resource => Hemogen;

		public virtual bool CanOffset
		{
			get
			{
				return Hemogen?.CanOffset == true;
			}
		}

		public virtual float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		//private int tick;

		//public override void Tick()
		//{
  //          tick--;
  //          if (tick <= 0)
  //          {
  //              tick = 360;
  //              Log.Error("Tick");
  //          }
  //      }

		public override void TickInterval(int delta)
		{
			//tick--;
			//if (tick <= 0)
			//{
			//	tick = 360;
			//	Log.Error("Tick");
			//}
			if (pawn.IsHashIntervalTick(360, delta))
			//if (GeneResourceUtility.CanTick(ref nextTick, 360))
			{
				//Log.Error("tick: " + 360);
				// Log.Error(tick.ToString() + " | 120");
				// tick = 0;
				//Log.Error("TickHemogenDrain");
                GeneResourceUtility.TickHemogenDrain(this, 360, CanOffset);
			}
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref nextTick, "nextTick", -1);
		//}

	}

	public class Gene_Bloodfeeder : Verse.Gene_Bloodfeeder
	{
		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	if (pawn.IsPrisonerOfColony && pawn.guest != null && pawn.guest.HasInteractionWith((PrisonerInteractionModeDef interaction) => interaction.hideIfNoBloodfeeders))
		//	{
		//		pawn.guest.SetNoInteraction();
		//	}
		//}

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;

		public Gene_Hemogen Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override void TickInterval(int delta)
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(10628, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (Hemogen?.ShouldConsumeHemogenNow() != true)
			{
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			if (pawn.Downed || pawn.Drafted || !pawn.Awake())
			{
				return;
			}
			GeneResourceUtility.TryHuntForFood(pawn);
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			pawns.Shuffle();
			for (int j = 0; j < pawns.Count; j++)
			{
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, pawns[j]))
				{
					continue;
				}
				SanguophageUtility.DoBite(pawn, pawns[j], 0.2f, 0.1f, 0.4f, 1f, new (0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
				break;
			}
		}

	}

	public class Gene_HemogenicMetabolism : Gene_HemogenOffset
	{

		public bool consumeHemogen = false;

		private float? cachedNutritionPerTick;

		public override float ResourceLossPerDay
		{
			get
			{
				if (consumeHemogen)
				{
					return def.resourceLossPerDay;
				}
				return 0f;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!consumeHemogen)
			{
				return;
			}
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(527, delta))
			{
				return;
			}
			ReplenishHunger();
			if (Hemogen == null || Hemogen.MinLevelForAlert > Hemogen.Value)
			{
				consumeHemogen = false;
				Messages.Message("WVC_XaG_HemogenicMetabolism_LowHemogen".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}

		public void ReplenishHunger()
		{
			if (!cachedNutritionPerTick.HasValue)
			{
				cachedNutritionPerTick = 0.02f + (pawn.needs?.food != null ? pawn.needs.food.FoodFallPerTick : 0f);
			}
			if (!GeneResourceUtility.TryOffsetNeedFood(pawn, cachedNutritionPerTick.Value, 0.05f))
			{
				consumeHemogen = false;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + XaG_UiUtility.OnOrOff(consumeHemogen),
				defaultDesc = "WVC_XaG_Gene_HemogenicMetabolismDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					consumeHemogen = !consumeHemogen;
					XaG_UiUtility.FlickSound(!consumeHemogen);
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref consumeHemogen, "consumeHemogen", false);
		}

	}

}
