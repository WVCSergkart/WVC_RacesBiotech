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

	public class HediffComp_XenotypeGestatorLetter : HediffComp
	{

		public HediffCompProperties_RemoveLetter Props => (HediffCompProperties_RemoveLetter)props;

		public override bool CompShouldRemove => XenotypeGestator == null;

		[Unsaved(false)]
		private Gene_XenotypeGestator cachedGestatorGene;

		public Gene_XenotypeGestator XenotypeGestator
		{
			get
			{
				if (cachedGestatorGene == null || !cachedGestatorGene.Active)
				{
					cachedGestatorGene = Pawn?.genes?.GetFirstGeneOfType<Gene_XenotypeGestator>();
				}
				return cachedGestatorGene;
			}
		}

		// public override void CompPostTick(ref float severityAdjustment)
		// {
			// if (Pawn.IsHashIntervalTick(120))
			// {
				// if (XenotypeGestator == null)
				// {
					// Pawn.health.RemoveHediff(parent);
				// }
			// }
		// }

		// public override bool CompDisallowVisible()
		// {
			// return XenotypeGestator == null;
		// }

		public override void CompPostPostRemoved()
		{
			cachedGestatorGene = Pawn?.genes?.GetFirstGeneOfType<Gene_XenotypeGestator>();
			if (Pawn.Faction != Faction.OfPlayer || XenotypeGestator == null)
			{
				return;
			}
			Find.LetterStack.ReceiveLetter(Props.letterLabel.Translate(), Props.letterDesc.Translate(Pawn.LabelCap), LetterDefOf.NeutralEvent, new LookTargets(Pawn));
		}

	}

}
