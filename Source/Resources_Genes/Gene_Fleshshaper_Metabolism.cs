using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Fleshshaper_Metabolism : Gene_SharedMetabolism
	{

		private static bool updated = false;
		public override bool Updated
		{
			get
			{
				return updated;
			}
			set
			{
				updated = value;
			}
		}

		protected override void ResetCache()
		{
			updated = false;
		}

		public override void SetMetabolism()
		{
			def.biostatMet = Gene_Fleshshaper.FleshshaperGenes.Count;
		}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{
		//	if (DebugSettings.ShowDevGizmos)
		//	{
		//		yield return new Command_Action
		//		{
		//			defaultLabel = "DEV: GetFleshshapersCount",
		//			action = delegate
		//			{
		//				SetMetabolism();
		//				Log.Error("Fleshshapers: " + Gene_Fleshshaper.FleshshaperGenes.Count);
		//			}
		//		};
		//	}
		//}

	}

}
