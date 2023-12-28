using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Hediff_LabeledMechlink : Hediff_Mechlink
	{

		public override string LabelBase
		{
			get
			{
				StringBuilder stringBuilder = new();
				if (!comps.NullOrEmpty())
				{
					HediffComp_LabelFromGene labelFromGene = this.TryGetComp<HediffComp_LabelFromGene>();
					List<GeneDef> geneDefs = labelFromGene?.Props.geneDefs;
					if (labelFromGene != null && !geneDefs.NullOrEmpty())
					{
						for (int i = 0; i < geneDefs.Count; i++)
						{
							if (XaG_GeneUtility.HasActiveGene(geneDefs[i], pawn))
							{
								stringBuilder.Append(geneDefs[i].label);
								return stringBuilder.ToString();
							}
						}
					}
				}
				// for (int i = 0; i < comps?.Count; i++)
				// {
					// string compLabelPrefix = comps[i].CompLabelPrefix;
					// if (!compLabelPrefix.NullOrEmpty())
					// {
						// stringBuilder.Append(compLabelPrefix);
						// stringBuilder.Append(" ");
					// }
				// }
				stringBuilder.Append(base.LabelBase);
				return stringBuilder.ToString();
			}
		}

		public override string Description
		{
			get
			{
				StringBuilder stringBuilder = new();
				if (!comps.NullOrEmpty())
				{
					HediffComp_LabelFromGene labelFromGene = this.TryGetComp<HediffComp_LabelFromGene>();
					List<GeneDef> geneDefs = labelFromGene?.Props.geneDefs;
					if (labelFromGene != null && !geneDefs.NullOrEmpty())
					{
						for (int i = 0; i < geneDefs.Count; i++)
						{
							if (XaG_GeneUtility.HasActiveGene(geneDefs[i], pawn))
							{
								stringBuilder.Append("WVC_XaG_CustomLabeledMechlinkDesc".Translate());
								return stringBuilder.ToString();
							}
						}
					}
				}
				stringBuilder.Append(base.Description);
				return stringBuilder.ToString();
			}
		}

	}

	public class HediffCompProperties_LabelFromGene : HediffCompProperties
	{

		// public string label;

		// public string description;

		public List<GeneDef> geneDefs;

		public HediffCompProperties_LabelFromGene()
		{
			compClass = typeof(HediffComp_LabelFromGene);
		}
	}

	public class HediffComp_LabelFromGene : HediffComp
	{

		// public override string CompLabelPrefix => GetLabel();

		// public override string CompDescriptionExtra => GetDesc();

		public HediffCompProperties_LabelFromGene Props => (HediffCompProperties_LabelFromGene)props;

		// public string GetDesc()
		// {
			// Pawn pawn = parent.pawn;
			// if (pawn == null || Props.geneDef == null || !MechanoidizationUtility.HasActiveGene(Props.geneDef, pawn) || Props.description.NullOrEmpty())
			// {
				// return null;
			// }
			// return Props.description;
		// }

		// public string GetLabel()
		// {
			// Pawn pawn = parent.pawn;
			// if (pawn == null || Props.geneDef == null || !MechanoidizationUtility.HasActiveGene(Props.geneDef, pawn) || Props.label.NullOrEmpty())
			// {
				// return null;
			// }
			// return Props.label;
		// }

	}

}
