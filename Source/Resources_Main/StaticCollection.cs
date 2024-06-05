using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// [StaticConstructorOnStartup]
	public static class StaticCollectionsClass
	{

		public static int cachedPawnsCount = 0;
		public static int cachedXenotypesCount = 0;
		public static int cachedNonHumansCount = 0;

	}

	// public class PawnContainer : IExposable
	// {

		// public Gender gender;

		// public string firstName;

		// public string secondName;

		// public string lastName;

		// public XenotypeDef xenotypeDef;

		// public List<GeneDef> genes;

		// public List<TraitDef> traits;

		// public List<SkillContainer> skills;

		// public BackstoryDef childhood;

		// public BackstoryDef adulthood;

		// public void ExposeData()
		// {
			// Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			// Scribe_Collections.Look(ref genes, "genes", LookMode.Def);
			// Scribe_Collections.Look(ref traits, "traits", LookMode.Def);
			// Scribe_Collections.Look(ref skills, "skills", LookMode.Deep);
			// Scribe_Defs.Look(ref childhood, "childhood");
			// Scribe_Defs.Look(ref adulthood, "adulthood");
			// Scribe_Values.Look(ref firstName, "firstName");
			// Scribe_Values.Look(ref secondName, "secondName");
			// Scribe_Values.Look(ref lastName, "lastName");
			// Scribe_Values.Look(ref gender, "gender", Gender.Male);
		// }

	// }

	// public class SkillContainer : IExposable
	// {

		// public SkillDef def;

		// public int levelInt;

		// public Passion passion;

		// public void ExposeData()
		// {
			// Scribe_Defs.Look(ref def, "def");
			// Scribe_Values.Look(ref levelInt, "level", 0);
			// Scribe_Values.Look(ref passion, "passion", Passion.None);
		// }

	// }


}
