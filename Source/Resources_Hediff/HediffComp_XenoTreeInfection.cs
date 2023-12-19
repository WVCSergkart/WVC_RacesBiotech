using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_XenoTreeInfection : HediffCompProperties
	{

		// public int gestationIntervalDays = -1;
		// public XenotypeDef xenotypeDef;

		// public string completeLetterLabel = "WVC_XaG_XenoTreeBirthLabel";

		// public string completeLetterDesc = "WVC_XaG_XenoTreeBirthDesc";

		// public bool endogeneTransfer = true;
		// public bool xenogeneTransfer = true;

		// public bool xenogerminationComa = false;

		public float requiredSeverity = 1.0f;

		public bool geneIsEndogene = true;

		public HediffDef hediffDef;

		// public string uniqueTag = "HediffGestator";

		public HediffCompProperties_XenoTreeInfection()
		{
			compClass = typeof(HediffComp_XenoTreeInfection);
		}

	}

	public class HediffComp_XenoTreeInfection : HediffComp
	{

		// private readonly int ticksInday = 60000;

		// private int ticksCounter = 0;

		public GeneDef geneDef;

		public Thing xenoTree;

		public HediffCompProperties_XenoTreeInfection Props => (HediffCompProperties_XenoTreeInfection)props;

		// protected Pawn Pawn => parent.pawn;

		// public override string CompLabelInBracketsExtra => GetLabel();

		// public override void CompPostMake()
		// {
			// if (Props.gestationIntervalDays > 0)
			// {
				// gestationIntervalDays = Props.gestationIntervalDays;
			// }
			// if (Props.xenotypeDef != null)
			// {
				// xenotypeDef = Props.xenotypeDef;
			// }
		// }

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_References.Look(ref xenoTree, "xenoTree");
			Scribe_Defs.Look(ref geneDef, "geneDef");
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			if (Pawn.Map == null)
			{
				return;
			}
			if (geneDef == null)
			{
				RemoveHediff();
				return;
			}
			if (Pawn.genes.HasGene(geneDef))
			{
				RemoveHediff();
				return;
			}
			// if (!MiscUtility.PawnIsColonistOrSlave(Pawn))
			// {
				// RemoveHediff();
				// return;
			// }
			if (parent.Severity >= Props.requiredSeverity)
			{
				Infect();
			}
		}

		private void Infect()
		{
			Pawn.genes.AddGene(geneDef, !Props.geneIsEndogene);
			if (xenoTree != null)
			{
				Gene_InfectedMind infector = Pawn?.genes?.GetFirstGeneOfType<Gene_InfectedMind>();
				if (infector != null)
				{
					infector.xenoTree = xenoTree;
				}
				xenoTree.TryGetComp<CompXenoTree>().AddedToConnectionList(Pawn);
			}
			RemoveHediff(true);
		}

		private void RemoveHediff(bool replaceWithOtherHediff = false)
		{
			Pawn.health.RemoveHediff(parent);
			if (replaceWithOtherHediff && Props.hediffDef != null)
			{
				Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, Pawn);
				Pawn.health.AddHediff(hediff);
			}
		}

		// public string GetLabel()
		// {
			// if (MiscUtility.PawnIsColonistOrSlave(Pawn))
			// {
				// float percent = (float)ticksCounter / (float)(ticksInday * gestationIntervalDays);
				// return percent.ToStringPercent();
			// }
			// return "";
		// }

		public override IEnumerable<Gizmo> CompGetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Infect pawn",
					action = delegate
					{
						Infect();
					}
				};
			}
		}

	}

}
