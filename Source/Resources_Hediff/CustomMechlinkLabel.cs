using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class Hediff_LabeledMechlink : Hediff_Mechlink
	{

		// public override string LabelBase
		// {
			// get
			// {
				// StringBuilder stringBuilder = new();
				// if (!comps.NullOrEmpty())
				// {
					// HediffComp_LabelFromGene labelFromGene = this.TryGetComp<HediffComp_LabelFromGene>();
					// List<GeneDef> geneDefs = labelFromGene?.Props.geneDefs;
					// if (labelFromGene != null && !geneDefs.NullOrEmpty())
					// {
						// for (int i = 0; i < geneDefs.Count; i++)
						// {
							// if (XaG_GeneUtility.HasActiveGene(geneDefs[i], pawn))
							// {
								// stringBuilder.Append(geneDefs[i].label);
								// return stringBuilder.ToString();
							// }
						// }
					// }
				// }
				// stringBuilder.Append(base.LabelBase);
				// return stringBuilder.ToString();
			// }
		// }

		// public override string Description
		// {
			// get
			// {
				// StringBuilder stringBuilder = new();
				// if (!comps.NullOrEmpty())
				// {
					// HediffComp_LabelFromGene labelFromGene = this.TryGetComp<HediffComp_LabelFromGene>();
					// List<GeneDef> geneDefs = labelFromGene?.Props.geneDefs;
					// if (labelFromGene != null && !geneDefs.NullOrEmpty())
					// {
						// for (int i = 0; i < geneDefs.Count; i++)
						// {
							// if (XaG_GeneUtility.HasActiveGene(geneDefs[i], pawn))
							// {
								// stringBuilder.Append("WVC_XaG_CustomLabeledMechlinkDesc".Translate());
								// return stringBuilder.ToString();
							// }
						// }
					// }
				// }
				// stringBuilder.Append(base.Description);
				// return stringBuilder.ToString();
			// }
		// }

	}

	// public class HediffCompProperties_LabelFromGene : HediffCompProperties
	// {

		// public List<GeneDef> geneDefs;

		// public HediffCompProperties_LabelFromGene()
		// {
			// compClass = typeof(HediffComp_LabelFromGene);
		// }
	// }

	// public class HediffComp_LabelFromGene : HediffComp
	// {

		// public HediffCompProperties_LabelFromGene Props => (HediffCompProperties_LabelFromGene)props;

	// }

}
