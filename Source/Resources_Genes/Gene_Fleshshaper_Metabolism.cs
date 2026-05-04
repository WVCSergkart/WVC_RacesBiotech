namespace WVC_XenotypesAndGenes
{

	public class Gene_Fleshshaper_Metabolism : Gene_SharedMetabolism
	{

		public override void SetMetabolism()
		{
			def.biostatMet = Gene_Fleshshaper.FleshshaperGenes.Count;
		}

	}

}
