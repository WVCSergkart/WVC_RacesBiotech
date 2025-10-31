using Verse;

namespace WVC_XenotypesAndGenes
{

	public struct GeneDefWithChance
	{
		public GeneDef geneDef;

		public float chance;

		public int reqDupesCount;

		public bool disabled;

		private int? cost;
		public int Cost
		{
			get
			{
				if (!cost.HasValue)
				{
					int newCost = 0;
					newCost += geneDef.biostatArc * 11;
					newCost += geneDef.biostatCpx * 2;
					newCost += geneDef.biostatMet * -1;
					if (!geneDef.statFactors.NullOrEmpty())
					{
						newCost += geneDef.statFactors.Count * 4;
					}
					if (!geneDef.statOffsets.NullOrEmpty())
					{
						newCost += geneDef.statOffsets.Count * 3;
					}
					if (!geneDef.forcedTraits.NullOrEmpty())
					{
						newCost += geneDef.forcedTraits.Count * 2;
					}
					if (!geneDef.abilities.NullOrEmpty())
					{
						newCost += geneDef.abilities.Count * 9;
					}
					if (!geneDef.capMods.NullOrEmpty())
					{
						newCost += geneDef.capMods.Count * 2;
					}
					cost = newCost;
				}
				if (cost.Value > 99)
				{
					cost = 99;
				}
				return cost.Value;
			}
			set
			{
				cost = value;
			}
		}

		public GeneCategoryDef displayCategory;

	}

}
