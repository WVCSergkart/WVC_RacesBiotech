using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

    // public class GeneratorDef : Def
    // {

    // public bool isSkillGene = false;
    // public bool isXenoFrocer = false;
    // public bool isSpawnerGene = false;
    // public bool isDryadPatch = false;
    // public bool isColorGene = false;

    // public List<PawnKindDef> dryadDefs;

    // public override IEnumerable<string> ConfigErrors()
    // {
    // foreach (string item in base.ConfigErrors())
    // {
    // yield return item;
    // }
    // if (!isSkillGene && !isSpawnerGene && !isXenoFrocer && !isColorGene)
    // {
    // yield break;
    // }
    // if (!typeof(Gene).IsAssignableFrom(geneClass))
    // {
    // yield return "geneClass is not Gene or child thereof.";
    // }
    // }

    // }

    public class DummyDryadTemplateDef : Def
	{

		// public GeneDef geneDef;

		// public PawnKindDef dryadDef;

		public List<PawnKindDef> dryadDefs;

	}

}
