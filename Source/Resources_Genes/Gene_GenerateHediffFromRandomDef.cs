using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_GenerateHediffWithRandomSeverity : Gene
	{

		public HediffDef HediffDef => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			// Hediff hediff = HediffMaker.MakeHediff(HediffDef, pawn);
			// FloatRange floatRange = new(HediffDef.minSeverity, HediffDef.maxSeverity);
			// hediff.Severity = floatRange.RandomInRange;
			// pawn.health.AddHediff(hediff);
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(60000))
			{
				return;
			}
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public static void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene)
		{
			if (gene.Active)
			{
				if (!pawn.health.hediffSet.HasHediff(hediffDef))
				{
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					FloatRange floatRange = new(hediffDef.minSeverity, hediffDef.maxSeverity);
					hediff.Severity = floatRange.RandomInRange;
					pawn.health.AddHediff(hediff);
					// pawn.health.AddHediff(hediffDef);
				}
			}
			else
			{
				Gene_AddOrRemoveHediff.RemoveHediff(hediffDef, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Gene_AddOrRemoveHediff.RemoveHediff(HediffDef, pawn);
		}

	}

}
