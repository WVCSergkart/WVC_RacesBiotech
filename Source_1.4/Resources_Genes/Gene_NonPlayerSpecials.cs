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
			SubXenotypeUtility.XenotypeShapeShift(pawn, this);
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

	// public class Gene_InfectedMind : Gene
	// {

		// public Thing xenoTree;

		// public override bool Active
		// {
			// get
			// {
				// return base.Active && xenoTree != null;
			// }
		// }

		// public override void PostRemove()
		// {
			// base.PostRemove();
			// TornConnection();
		// }

		// public override void Notify_PawnDied()
		// {
			// base.Notify_PawnDied();
			// CompXenoTree compXenoTree = xenoTree?.TryGetComp<CompXenoTree>();
			// if (compXenoTree != null && !compXenoTree.AllConnectedPawns.NullOrEmpty())
			// {
				// foreach (Pawn pawn in compXenoTree.AllConnectedPawns)
				// {
					// if (pawn != base.pawn && compXenoTree.ConnectionCheck(pawn))
					// {
						// foreach (SkillRecord skill in pawn.skills.skills)
						// {
							// if (!skill.TotallyDisabled && skill.XpTotalEarned > 0f)
							// {
								// float num = skill.XpTotalEarned * 0.2f;
								// skill.Learn(0f - num, direct: true);
							// }
						// }
					// }
				// }
			// }
			// TornConnection();
		// }

		// public void TornConnection()
		// {
			// if (xenoTree != null)
			// {
				// xenoTree.TryGetComp<CompXenoTree>().RemoveFromConnectionList(pawn);
				// xenoTree = null;
			// }
		// }

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_References.Look(ref xenoTree, "xenoTree");
		// }

	// }

}
