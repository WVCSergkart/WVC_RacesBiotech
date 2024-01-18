using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class StaticCollectionsClass
	{

		public static HashSet<Pawn> skillsNotDecayPawns;

		static StaticCollectionsClass()
		{
			skillsNotDecayPawns = new HashSet<Pawn>();
		}

		public static void AddSkillDecayGenePawnToList(Pawn pawn)
		{
			if (!skillsNotDecayPawns.Contains(pawn))
			{
				skillsNotDecayPawns.Add(pawn);
			}
		}

		public static void RemoveSkillDecayGenePawnFromList(Pawn pawn)
		{
			if (skillsNotDecayPawns.Contains(pawn))
			{
				skillsNotDecayPawns.Remove(pawn);
			}
		}

	}

}
