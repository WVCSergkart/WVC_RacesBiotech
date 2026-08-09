using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	/// <summary>
	/// Dummy.
	/// </summary>
	public class CompProperties_GenesSettings : CompProperties
	{

		public CompProperties_GenesSettings()
		{
			compClass = typeof(CompGenesSettings);
		}

	}

	/// <summary>
	/// For modders.
	/// Genes settings dev tools comp. Inherit from this or use IThingDevSettings
	/// They don't overwrite each other for the same pawn. You can use as many as you like.
	/// </summary>
	public class CompGenesSettings : ThingComp, IThingDevSettings
	{

		public virtual IEnumerable<Gizmo> DevGizmos()
		{
			return null;
		}

	}

}
