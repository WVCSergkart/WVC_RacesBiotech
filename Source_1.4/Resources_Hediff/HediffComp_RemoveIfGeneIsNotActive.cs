using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_RemoveIfGeneIsNotActive : HediffCompProperties
	{

		public GeneDef geneDef;

		// ~1 day
		public IntRange checkInterval = new (56720, 72032);

		public HediffCompProperties_RemoveIfGeneIsNotActive()
		{
			compClass = typeof(HediffComp_RemoveIfGeneIsNotActive);
		}

	}

	public class HediffComp_RemoveIfGeneIsNotActive : HediffComp
	{

		public GeneDef geneDef;

		public HediffCompProperties_RemoveIfGeneIsNotActive Props => (HediffCompProperties_RemoveIfGeneIsNotActive)props;

		public int nextTick = 60000;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			nextTick = Props.checkInterval.RandomInRange;
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (!Pawn.IsHashIntervalTick(nextTick))
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
			Scribe_Defs.Look(ref geneDef, "geneDef");
			Scribe_Values.Look(ref nextTick, "nextTick", 60000);
		}

	}

	// ========================================

	[Obsolete]
	public class HediffCompProperties_AlwaysRemove : HediffCompProperties
	{

		public HediffCompProperties_AlwaysRemove()
		{
			compClass = typeof(HediffComp_AlwaysRemove);
		}

	}

	[Obsolete]
	public class HediffComp_AlwaysRemove : HediffComp
	{

		public HediffCompProperties_AlwaysRemove Props => (HediffCompProperties_AlwaysRemove)props;

		public override bool CompShouldRemove => true;

	}

	// ========================================

	public class HediffCompProperties_RemoveIf : HediffCompProperties
	{

		public int checkInterval = 5220;

		public List<GeneDef> anyOf_GeneDef;

		public bool alwaysRemove = false;

		public HediffCompProperties_RemoveIf()
		{
			compClass = typeof(HediffComp_RemoveIf);
		}

	}
	public class HediffComp_RemoveIf : HediffComp
	{

		public bool shouldRemove = false;

		public HediffCompProperties_RemoveIf Props => (HediffCompProperties_RemoveIf)props;

		public int nextTick = 60000;

		public override bool CompShouldRemove => shouldRemove;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			nextTick = Props.checkInterval;
			shouldRemove = Props.alwaysRemove;
			RemoveIf();
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (!Pawn.IsHashIntervalTick(nextTick))
			{
				return;
			}
			RemoveIf();
		}

		public void RemoveIf()
		{
			if (XaG_GeneUtility.HasAnyActiveGene(Props.anyOf_GeneDef, parent.pawn))
			{
				shouldRemove = true;
			}
		}

		public override void CompExposeData()
		{
			Scribe_Values.Look(ref nextTick, "nextTick", 60000);
		}

	}

}
