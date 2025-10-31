// RimWorld.ThoughtWorker_Pretty
using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class ThoughtWorker_GenderMale : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return false;
			}
			if (other.def != p.def)
			{
				return false;
			}
			if (other.gender != Gender.Male)
			{
				return false;
			}
			return true;
		}
	}

	[Obsolete]
	public class ThoughtWorker_GenderFemale : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return false;
			}
			if (other.def != p.def)
			{
				return false;
			}
			if (other.gender != Gender.Female)
			{
				return false;
			}
			return true;
		}
	}
}
