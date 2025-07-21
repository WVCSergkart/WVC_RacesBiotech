using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    // Hemogen
    public class Gene_EternalHunger : Gene_HemogenOffset, IGeneOverridden, IGeneBloodfeeder
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

        // Gene

        public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			AddOrRemoveHediff();
		}

		public void Notify_Override()
		{
			AddOrRemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			HediffUtility.TryAddOrRemoveHediff(Props?.hediffDefName, pawn, this);
		}


		public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn.IsHashIntervalTick(56421, delta))
            {
                AddOrRemoveHediff();
            }
            if (!pawn.IsHashIntervalTick(455, delta))
            {
                return;
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
            //if (pawn.Map == null)
            //{
            //	return;
            //}
            //if (pawn.Downed || pawn.Drafted || !pawn.Awake())
            //{
            //	return;
            //}
            //if (Resource is not Gene_Hemogen gene_hemogen || gene_hemogen.MinLevelForAlert + 0.3f < gene_hemogen.Value)
            //{
            //	return;
            //}
            //if (TryGetFood())
            //{
            //	return;
            //}
            //if (Gene_BloodHunter.TryHuntForFood(pawn))
            //{
            //	Messages.Message("WVC_XaG_Gene_EternalHunger_HuntWarning".Translate(pawn.NameShortColored.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
            //}
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
						AddOrRemoveHediff();
					}
				};
			}
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			//base.Notify_IngestedThing(thing, numTaken);
			if (!Active || Hemogen == null)
			{
				return;
			}
			if (thing.IsRawMeat())
			{
				Hemogen.Value += thing.def.GetStatValueAbstract(StatDefOf.Nutrition) * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor) * numTaken;
				//IngestibleProperties ingestible = thing.def.ingestible;
				//if (ingestible != null)
				//{
				//	GeneUtility.OffsetHemogen(pawn, GetHemogenCountFromFood(thing) * (float)numTaken * 1.2f);
				//}
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

        // Misc

        //public override bool Active
        //{
        //	get
        //	{
        //		if (!foodNeedDisabled)
        //		{
        //			return base.Active;
        //		}
        //		return false;
        //	}
        //}

        //public bool foodNeedDisabled = false;

        //public override void ExposeData()
        //{
        //	base.ExposeData();
        //	if (Scribe.mode == LoadSaveMode.PostLoadInit)
        //	{
        //		foodNeedDisabled = pawn.needs?.food == null;
        //	}
        //}

        //public bool TryGetFood()
        //{
        //	if (!pawn.TryGetNeedFood_WithRef(out Need_Food need_Food, ref foodNeedDisabled))
        //	{
        //		return false;
        //	}
        //	List<Thing> meatFood = new();
        //	foreach (Thing item in pawn.Map.listerThings.AllThings.ToList())
        //	{
        //		if (item.def.ingestible != null && item.def.IsMeat)
        //		{
        //			meatFood.Add(item);
        //		}
        //	}
        //	for (int j = 0; j < meatFood.Count; j++)
        //	{
        //		Thing specialFood = MiscUtility.GetSpecialFood(pawn, meatFood[j].def);
        //		if (specialFood == null)
        //		{
        //			continue;
        //		}
        //		int stack = GetFoodCount(specialFood) > 1 ? GetFoodCount(specialFood) : 1;
        //		GeneResourceUtility.OffsetNeedFood(pawn, GetHunger(stack, specialFood, need_Food));
        //		Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
        //		job.count = stack + 3;
        //		pawn.TryTakeOrderedJob(job, JobTag.Misc, true);
        //		return true;
        //	}
        //	return false;
        //}

        //public float GetHunger(int stack, Thing thing, Need_Food need_Food)
        //{
        //	float nutrition = stack * thing.def.ingestible.CachedNutrition * pawn.GetStatValue(StatDefOf.RawNutritionFactor, cacheStaleAfterTicks: 360000);
        //	float currentNeedFood = need_Food.CurLevel;
        //	float maxNeedFood = need_Food.MaxLevel;
        //	float targetFoodLevel = maxNeedFood - nutrition;
        //	float offset = -1 * (currentNeedFood - targetFoodLevel);
        //	if (offset > 0f)
        //	{
        //		offset = 0f;
        //	}
        //	return offset;
        //}

        //public int GetFoodCount(Thing thing)
        //{
        //	int foodCount = 0;
        //	float hemogen = GetHemogenCountFromFood(thing);
        //	float currentHemogenLvl = Resource.Value;
        //	float targetHemogenLvl = Resource.targetValue;
        //	while (currentHemogenLvl < targetHemogenLvl)
        //	{
        //		foodCount++;
        //		currentHemogenLvl += hemogen;
        //		if (foodCount >= 75)
        //		{
        //			break;
        //		}
        //	}
        //	return foodCount;
        //}

        //public float GetHemogenCountFromFood(Thing thing)
        //{
        //	return 0.175f *  thing.def.ingestible.CachedNutrition * pawn.GetStatValue(StatDefOf.RawNutritionFactor, cacheStaleAfterTicks: 360000) * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000);
        //}

    }

}
