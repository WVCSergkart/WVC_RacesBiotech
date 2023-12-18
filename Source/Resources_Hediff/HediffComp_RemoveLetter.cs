using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_RemoveLetter : HediffCompProperties
	{

		public string letterLabel = "WVC_XaG_GeneXenoGestator_CooldownLetterLabel";
		public string letterDesc = "WVC_XaG_GeneXenoGestator_CooldownLetterDesc";

		public HediffCompProperties_RemoveLetter()
		{
			compClass = typeof(HediffComp_RemoveLetter);
		}
	}

	public class HediffComp_RemoveLetter : HediffComp
	{

		public HediffCompProperties_RemoveLetter Props => (HediffCompProperties_RemoveLetter)props;

		public override void CompPostPostRemoved()
		{
			Find.LetterStack.ReceiveLetter(Props.letterLabel.Translate(), Props.letterDesc.Translate(Pawn.LabelCap), LetterDefOf.NeutralEvent, new LookTargets(Pawn));
		}

	}

}
