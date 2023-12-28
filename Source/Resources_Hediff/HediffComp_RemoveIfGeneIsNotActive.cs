using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_RemoveIfGeneIsNotActive : HediffCompProperties
	{
		public GeneDef geneDef;

		// 1 day
		public int checkInterval = 60000;

		public HediffCompProperties_RemoveIfGeneIsNotActive()
		{
			compClass = typeof(HediffComp_RemoveIfGeneIsNotActive);
		}
	}

	public class HediffComp_RemoveIfGeneIsNotActive : HediffComp
	{
		public GeneDef geneDef;

		public HediffCompProperties_RemoveIfGeneIsNotActive Props => (HediffCompProperties_RemoveIfGeneIsNotActive)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.checkInterval))
			{
				return;
			}
			if (geneDef == null && Props.geneDef != null)
			{
				// base.Pawn.health.RemoveHediff(parent);
				// return;
				geneDef = Props.geneDef;
			}
			if (!XaG_GeneUtility.HasActiveGene(geneDef, parent.pawn))
			{
				base.Pawn.health.RemoveHediff(parent);
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Defs.Look(ref geneDef, "geneDef");
		}

	}

	public class HediffCompProperties_AlwaysRemove : HediffCompProperties
	{

		public HediffCompProperties_AlwaysRemove()
		{
			compClass = typeof(HediffComp_AlwaysRemove);
		}

	}
	public class HediffComp_AlwaysRemove : HediffComp
	{

		public HediffCompProperties_AlwaysRemove Props => (HediffCompProperties_AlwaysRemove)props;

		public override bool CompShouldRemove => true;

	}

	public class HediffCompProperties_RemoveIf : HediffCompProperties
	{

		public List<GeneDef> anyOf_GeneDef;

		public HediffCompProperties_RemoveIf()
		{
			compClass = typeof(HediffComp_RemoveIf);
		}

	}
	public class HediffComp_RemoveIf : HediffComp
	{

		bool shouldRemove = true;

		public HediffCompProperties_RemoveIf Props => (HediffCompProperties_RemoveIf)props;

		// public override bool CompShouldRemove => base.Pawn.genes?.GetFirstGeneOfType<Gene_AngelicStability>() != null;

		public override bool CompShouldRemove => ShouldRemove;

		public bool ShouldRemove
		{
			get
			{
				if (shouldRemove)
				{
					if (!XaG_GeneUtility.HasAnyActiveGene(Props.anyOf_GeneDef, parent.pawn))
					{
						shouldRemove = false;
						return false;
					}
				}
				if (!shouldRemove)
				{
					return false;
				}
				return true;
			}
		}

	}

}
