using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public abstract class ThoughtWorker_Precept_Social_Duplicates : ThoughtWorker_Precept_Social
	{

		public static List<Pawn> ignoredPawns = new();
		public static List<Pawn> duplicatePawns = new();

		public bool IsDuplicate(Pawn duplicate)
		{
			if (ignoredPawns.Contains(duplicate))
			{
				return false;
			}
			if (duplicatePawns.Contains(duplicate))
			{
				return true;
			}
			if (!duplicate.IsDuplicate)
			{
				ignoredPawns.Add(duplicate);
			}
			else
            {
				AddDuplicate(duplicate.GetSourceCyclic(), duplicate);
				duplicatePawns.Add(duplicate);
			}
			return true;
		}

		public static Dictionary<Pawn, DuplicateSet> duplicateSets = new();

		public static bool IsDuplicateOf(Pawn source, Pawn duplicate)
		{
			if (duplicateSets.ContainsKey(source))
			{
				return duplicateSets[source].Contains(duplicate);
			}
			return false;
		}

		public static bool IsSameDuplicate(Pawn duplicate1, Pawn duplicate2)
		{
			foreach (var item in duplicateSets)
            {
				if (item.Value.Contains(duplicate1) && item.Value.Contains(duplicate2))
                {
					return true;
                }
            }
			return false;
		}

		public static void AddDuplicate(Pawn source, Pawn duplicate)
		{
			if (duplicateSets.ContainsKey(source))
			{
				duplicateSets[source].Add(duplicate);
				return;
			}
			duplicateSets.Add(source, new DuplicateSet());
			duplicateSets[source].Add(duplicate);
		}

	}

	public class ThoughtWorker_Precept_Social_SourceAndDuplicate : ThoughtWorker_Precept_Social_Duplicates
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (IsDuplicate(p))
			{
				// My Source
				if (IsDuplicateOf(otherPawn, p))
				{
					return ThoughtState.ActiveAtStage(1);
				}
				// Same Source
				if (IsSameDuplicate(p, otherPawn))
				{
					return ThoughtState.ActiveAtStage(2);
				}
				return ThoughtState.Inactive;
			}
			if (IsDuplicate(otherPawn))
			{
				// My Duplicate
				if (IsDuplicateOf(p, otherPawn))
				{
					return ThoughtState.ActiveAtStage(0);
				}
			}
			return ThoughtState.Inactive;
		}

	}

	[Obsolete]
	public class ThoughtWorker_Precept_Social_MyDuplicate : ThoughtWorker_Precept_Social_Duplicates
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!IsDuplicate(otherPawn))
			{
				return ThoughtState.Inactive;
			}
			return IsDuplicateOf(p, otherPawn);
		}

	}

	[Obsolete]
	public class ThoughtWorker_Precept_Social_MySource : ThoughtWorker_Precept_Social_Duplicates
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!IsDuplicate(p))
			{
				return ThoughtState.Inactive;
			}
			return IsDuplicateOf(otherPawn, p);
		}

	}

	[Obsolete]
	public class ThoughtWorker_Precept_Social_SameSource : ThoughtWorker_Precept_Social_Duplicates
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!IsDuplicate(p))
			{
				return ThoughtState.Inactive;
			}
			return IsSameDuplicate(p, otherPawn);
		}

	}

}
