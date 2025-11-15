using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_Precept_PreferredXenotype_Social : RimWorld.ThoughtWorker_Precept_PreferredXenotype_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (otherPawn.genes == null)
			{
				return ThoughtState.Inactive;
			}
			//if (!IsPreferredXenotype(p, p.Ideo))
			//{
			//    return ThoughtState.Inactive;
			//}
			if (!p.Ideo.PreferredXenotypes.Any() && !p.Ideo.PreferredCustomXenotypes.Any())
			{
				return ThoughtState.Inactive;
			}
			if (PreferredXenotypesUtility.IsPreferredXenotype(otherPawn, p.Ideo))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.ActiveAtStage(1);
		}

	}

	public class ThoughtWorker_Precept_ColonyXenotypeMakeup : RimWorld.ThoughtWorker_Precept_ColonyXenotypeMakeup
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction == null)
			{
				return ThoughtState.Inactive;
			}
			if (!p.Ideo.PreferredXenotypes.Any() && !p.Ideo.PreferredCustomXenotypes.Any())
			{
				return ThoughtState.Inactive;
			}
			List<Pawn> list = p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction);
			int num = 0;
			int num2 = 0;
			bool flag = p.IsSlave || p.IsPrisoner;
			foreach (Pawn item in list)
			{
				bool flag2 = item.IsSlave || item.IsPrisoner;
				if (item.genes != null && flag == flag2)
				{
					num++;
					if (!PreferredXenotypesUtility.IsPreferredXenotype(item, p.Ideo))
					{
						num2++;
					}
				}
			}
			if (num2 == 0)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			float num3 = num2 / (float)num;
			if (num3 < 0.33f)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			if (num3 < 0.66f)
			{
				return ThoughtState.ActiveAtStage(2);
			}
			return ThoughtState.ActiveAtStage(3);
		}

	}

	public class ThoughtWorker_Precept_SelfDislikedXenotype : RimWorld.ThoughtWorker_Precept_SelfDislikedXenotype
	{

		public override string PostProcessDescription(Pawn p, string description)
		{
			return description.Formatted(p.genes.XenotypeLabel.Named("XENOTYPENAME"));
		}

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.genes == null)
			{
				return ThoughtState.Inactive;
			}
			return !PreferredXenotypesUtility.IsPreferredXenotype(p, p.Ideo);
		}

	}

}
