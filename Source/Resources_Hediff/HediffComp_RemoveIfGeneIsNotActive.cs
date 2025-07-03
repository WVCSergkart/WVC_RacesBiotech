using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_GeneHediff : HediffCompProperties
	{

		public GeneDef geneDef;

		public List<GeneDef> geneDefs;

		// ~1 day
		public IntRange checkInterval = new (56720, 72032);

		public HediffCompProperties_GeneHediff()
		{
			compClass = typeof(HediffComp_GeneHediff);
		}

	}

	public class HediffComp_GeneHediff : HediffComp
	{

		public GeneDef geneDef;

		public HediffCompProperties_GeneHediff Props => (HediffCompProperties_GeneHediff)props;

		public int nextTick = 60000;

		// private bool geneIsRemoved = false;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			nextTick = Props.checkInterval.RandomInRange;
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			if (!Pawn.IsHashIntervalTick(nextTick, delta))
			{
				return;
			}
			if (geneDef == null && Props.geneDef != null)
			{
				geneDef = Props.geneDef;
			}
			if (!XaG_GeneUtility.HasActiveGene(geneDef, parent.pawn))
			{
				base.Pawn.health.RemoveHediff(parent);
			}
		}

		public override void CompPostPostRemoved()
		{
			if (geneDef == null)
			{
				geneDef = Props.geneDef;
			}
			if (XaG_GeneUtility.HasActiveGene(geneDef, Pawn))
			{
				BodyPartDef bodyPart = parent?.Part?.def;
				List<BodyPartDef> bodyparts = new();
				if (bodyPart != null)
				{
					bodyparts.Add(bodyPart);
				}
				if (HediffUtility.TryAddHediff(Def, Pawn, geneDef, bodyparts))
				{
					if (DebugSettings.ShowDevGizmos)
					{
						Log.Warning("Trying to remove " + Def.label + " hediff, but " + Pawn.Name.ToString() + " has the required gene. Hediff is added back.");
					}
				}
			}
		}

		// public virtual void Notify_GeneRemoved(Gene gene)
		// {
			// if (geneDef == gene.def || Props.geneDef == gene.def)
			// {
				// geneIsRemoved = true;
				// base.Pawn.health.RemoveHediff(parent);
			// }
		// }

		public override void CompExposeData()
		{
			Scribe_Defs.Look(ref geneDef, "geneDef");
			Scribe_Values.Look(ref nextTick, "nextTick", 60000);
		}

	}

	//[Obsolete]
	//public class HediffComp_ShapeshifterHediff : HediffComp
	//{

	//	public HediffCompProperties_GeneHediff Props => (HediffCompProperties_GeneHediff)props;

	//	public override void CompPostPostRemoved()
	//	{
	//		if (Pawn?.health?.hediffSet?.GetBrain() == null)
	//		{
	//			return;
	//		}
	//		if (XaG_GeneUtility.HasAnyActiveGene(Props.geneDefs, Pawn))
	//		{
	//			BodyPartDef bodyPart = parent?.Part?.def;
	//			List<BodyPartDef> bodyparts = new();
	//			if (bodyPart != null)
	//			{
	//				bodyparts.Add(bodyPart);
	//			}
	//			if (HediffUtility.TryAddHediff(Def, Pawn, null, bodyparts))
	//			{
	//				if (DebugSettings.ShowDevGizmos)
	//				{
	//					Log.Warning("Trying to remove " + Def.label + " hediff, but " + Pawn.Name.ToString() + " has the required gene. Hediff is added back.");
	//				}
	//			}
	//		}
	//	}

	//}

	// ========================================

	//[Obsolete]
	//public class HediffCompProperties_AlwaysRemove : HediffCompProperties
	//{

	//	public HediffCompProperties_AlwaysRemove()
	//	{
	//		compClass = typeof(HediffComp_AlwaysRemove);
	//	}

	//}

	//[Obsolete]
	//public class HediffComp_AlwaysRemove : HediffComp
	//{

	//	public HediffCompProperties_AlwaysRemove Props => (HediffCompProperties_AlwaysRemove)props;

	//	public override bool CompShouldRemove => true;

	//}

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

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			if (!Pawn.IsHashIntervalTick(nextTick, delta))
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
