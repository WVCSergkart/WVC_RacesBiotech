using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_FleshmassGene : CompProperties
	{
		public CompProperties_FleshmassGene()
		{
			compClass = typeof(CompFleshmassGene);
		}
	}

	public class CompFleshmassGene : ThingComp
	{

		// public override void PostSpawnSetup(bool respawningAfterLoad)
		// {
			// base.PostSpawnSetup(respawningAfterLoad);
			// if (!respawningAfterLoad)
			// {
				// CheckFaction();
			// }
		// }

		// public void CheckFaction()
		// {
			// Pawn pawn = parent as Pawn;
			// if (pawn.Faction != Faction.OfPlayer)
			// {
				// List<Pawn> mechanitors = MechanoidsUtility.GetAllMechanitors(parent.Map);
				// if (!mechanitors.NullOrEmpty())
				// {
					// pawn.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, pawn);
					// pawn.SetFaction(Faction.OfPlayer);
					// mechanitors.RandomElement().relations.AddDirectRelation(PawnRelationDefOf.Overseer, pawn);
				// }
			// }
		// }
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			return Enumerable.Empty<FloatMenuOption>();
		}

	}

}
