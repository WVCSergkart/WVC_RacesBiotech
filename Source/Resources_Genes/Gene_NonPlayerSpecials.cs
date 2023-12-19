using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenotypeShapeshifter : Gene
	{

		// public bool geneIsXenogene = false;

		public XenotypeDef Xenotype => pawn.genes?.Xenotype;

		public override void PostAdd()
		{
			base.PostAdd();
			// SubXenotypeUtility.ShapeShift(pawn, Xenotype, this);
			SubXenotypeUtility.XenotypeShapeShift(pawn, Xenotype, this);
		}

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: Shapeshift",
					// action = delegate
					// {
						// SubXenotypeUtility.XenotypeShapeShift(pawn, Xenotype, this, 1.0f);
					// }
				// };
			// }
		// }

	}

	public class Gene_InfectedMind : Gene
	{

		public Thing xenoTree;

		public override bool Active
		{
			get
			{
				return base.Active && xenoTree != null;
			}
		}

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: Shapeshift",
					// action = delegate
					// {
						// SubXenotypeUtility.XenotypeShapeShift(pawn, Xenotype, this, 1.0f);
					// }
				// };
			// }
		// }

		public override void PostRemove()
		{
			base.PostRemove();
			TornConnection();
		}

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			TornConnection();
		}

		public void TornConnection()
		{
			if (xenoTree != null)
			{
				xenoTree.TryGetComp<CompXenoTree>().connectedPawns.Remove(pawn);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref xenoTree, "xenoTree");
		}

	}

}
