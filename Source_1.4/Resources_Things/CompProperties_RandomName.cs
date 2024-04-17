using Verse;
using Verse.Grammar;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_RandomName : CompProperties
	{

		public RulePackDef nameMaker;

		public CompProperties_RandomName()
		{
			compClass = typeof(CompRandomName);
		}

	}

	public class CompRandomName : ThingComp
	{

		public string thingName = null;

		public CompProperties_RandomName Props => (CompProperties_RandomName)props;

		public string GenerateName()
		{
			GrammarRequest request = default;
			request.Includes.Add(Props.nameMaker);
			return GenText.CapitalizeAsTitle(GrammarResolver.Resolve("r_totem_name", request));
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (thingName == null)
			{
				thingName = GenerateName();
			}
		}

		public override string TransformLabel(string label)
		{
			return thingName;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref thingName, "thingName");
		}

	}

}
