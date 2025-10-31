using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_FoodEfficiency : Gene
	{

		//public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			//base.Notify_IngestedThing(thing, numTaken);
			//if (Undead != null && Undead.specialFoodDefs.Contains(thing.def))
			//{
			//	return;
			//}
			//IngestibleProperties ingestible = thing.def?.ingestible;
			//if (ingestible == null)
			//{
			//	return;
			//}
			float nutrition = thing.def.GetStatValueAbstract(StatDefOf.Nutrition);
			if (nutrition > 0f)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * numTaken);
			}
		}

	}

	public class Gene_SuperMetabolism : Gene
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			GeneResourceUtility.IngestedThingWithFactor(this, thing, pawn, 5 * numTaken);
		}

	}

	public class Gene_SuperMetabolism_AddOrRemoveHediff : Gene_AddOrRemoveHediff
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			GeneResourceUtility.IngestedThingWithFactor(this, thing, pawn, 5 * numTaken);
		}

	}

	public class Gene_HungerlessStomach : Gene_AddOrRemoveHediff, IGeneNotifyGenesChanged
	{

		private float? cachedOffset;
		public float Offset
		{
			get
			{
				if (!cachedOffset.HasValue)
				{
					cachedOffset = GetFoodOffset(pawn);
				}
				return cachedOffset.Value;
			}
		}

		public static float GetFoodOffset(Pawn pawn)
		{
			float offset = 0.1f;
			float metabol = pawn.genes.GenesListForReading.Where((gene) => !gene.Overridden).Sum((gene) => gene.def.biostatMet);
			if (metabol < 0f)
			{
				float factor = 1f - ((metabol * -1) * 0.1f);
				offset *= factor > 0f ? factor : 0f;
			}
			else if (metabol > 0f)
			{
				float factor = 1f + (metabol * 0.1112f);
				offset *= factor;
			}
			return offset;
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			cachedOffset = null;
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(2919, delta))
			{
				return;
			}
			GeneResourceUtility.OffsetNeedFood(pawn, Offset);
		}

	}

}
