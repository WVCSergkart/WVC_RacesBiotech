using RimWorld;
using System.Security.Cryptography;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_CustomFloatMenu : CompProperties
	{

		public GeneDef geneDef;
		public XenotypeDef xenotypeDef;

		public JobDef jobDef;

		[NoTranslate]
		public string iconPath = "WVC/UI/Genes3/Gene_Backup";

		[NoTranslate]
		public string warningText = "WVC_XaG_GeneChimeraDevourFleshmassNucleusWarning";

		[MustTranslate]
		public string description = null;

		[NoTranslate]
		public string descriptionTranslated = null;

		public float factor = 1;

		//public CompProperties_CustomFloatMenu()
		//{
		//	compClass = typeof(CompEntitiesGenes);
		//}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (!description.NullOrEmpty())
			{
				parentDef.description += "\n\n" + description;
			}
			if (!descriptionTranslated.NullOrEmpty())
			{
				parentDef.description += "\n\n" + descriptionTranslated.Translate();
			}
		}

	}

}
