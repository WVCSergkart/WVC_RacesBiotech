using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
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

	public class GeneSetPresets : IExposable
	{

		public string name;

		public List<GeneDef> geneDefs = new();

		public void ExposeData()
		{
			Scribe_Values.Look(ref name, "name");
			Scribe_Collections.Look(ref geneDefs, "geneDefs", LookMode.Def);
		}

	}

	// public class CustomXenotypeWithXenotypeDef
	// {

		// private XenotypeDef xenotypeDef;

		// private CustomXenotype customXenotype;

		// public XenotypeDef XenotypeDef
		// {
			// get
			// {
				// if (customXenotype != null)
				// {
					// return XenotypeDefOf.Baseliner;
				// }
				// return xenotypeDef;
			// }
		// }

		// public CustomXenotype CustomXenotype => customXenotype;

		// public void SetXenotype(XenotypeDef xenotypeDef = null, CustomXenotype customXenotype = null)
		// {
			// if (customXenotype != null)
			// {
				// this.customXenotype = customXenotype;
			// }
			// if (xenotypeDef != null)
			// {
				// this.xenotypeDef = xenotypeDef;
			// }
		// }

		// [Unsaved(false)]
		// private Texture2D cachedIcon;

		// public Texture2D Icon
		// {
			// get
			// {
				// if (cachedIcon == null)
				// {
					// if (customXenotype != null)
					// {
						// cachedIcon = customXenotype.IconDef.Icon;
					// }
					// else if (xenotypeDef != null)
					// {
						// cachedIcon = xenotypeDef.Icon;
					// }
				// }
				// return cachedIcon;
			// }
		// }

		// public string LabelCap
		// {
			// get
			// {
				// if (customXenotype != null)
				// {
					// return customXenotype.name.CapitalizeFirst();
				// }
				// else if (xenotypeDef != null)
				// {
					// return xenotypeDef.LabelCap;
				// }
				// return null;
			// }
		// }

		// public float DisplayPriority
		// {
			// get
			// {
				// if (customXenotype != null)
				// {
					// return customXenotype.inheritable ? 1f : 0f;
				// }
				// else if (xenotypeDef != null)
				// {
					// return xenotypeDef.displayPriority;
				// }
				// return 0f;
			// }
		// }

		// public bool Inheritable
		// {
			// get
			// {
				// if (customXenotype != null)
				// {
					// return customXenotype.inheritable;
				// }
				// else if (xenotypeDef != null)
				// {
					// return xenotypeDef.inheritable;
				// }
				// return false;
			// }
		// }

		// public List<GeneDef> AllGenes
		// {
			// get
			// {
				// if (customXenotype != null)
				// {
					// return customXenotype.genes;
				// }
				// else if (xenotypeDef != null)
				// {
					// return xenotypeDef.AllGenes;
				// }
				// return new();
			// }
		// }

	// }

	// public class VirtualCategory
	// {

		// public string name;

	// }

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
