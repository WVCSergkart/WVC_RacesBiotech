using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AngelicStability : Gene_DustDrain
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(HediffDefName, pawn);;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				pawn.health.AddHediff(HediffDefName);
			}
		}

		// public override void Notify_IngestedThing(Thing thing, int numTaken)
		// {
			// IngestibleProperties ingestible = thing.def.ingestible;
			// float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			// if (ingestible != null && nutrition > 0f)
			// {
				// DustUtility.OffsetNeedFood(pawn, 0.5f * nutrition * (float)numTaken);
				// Log.Error("Stability: Additional " + (0.5f * nutrition * (float)numTaken) + " nutrition gain");
			// }
		// }
	}

}
