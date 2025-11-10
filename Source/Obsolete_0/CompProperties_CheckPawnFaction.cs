using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_InitialPawnFaction : CompProperties
	{
		public CompProperties_InitialPawnFaction()
		{
			compClass = typeof(CompInitialPawnFaction);
		}
	}

	[Obsolete]
	public class CompInitialPawnFaction : ThingComp
	{

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				CheckFaction();
			}
		}

		public void CheckFaction()
		{
			Pawn pawn = parent as Pawn;
			if (pawn.Faction != Faction.OfPlayer)
			{
				List<Pawn> mechanitors = MechanoidsUtility.GetAllMechanitors(parent.Map);
				if (!mechanitors.NullOrEmpty())
				{
					MechanoidsUtility.SetOverseer(mechanitors.RandomElement(), pawn);
					//pawn.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, pawn);
					//pawn.SetFaction(Faction.OfPlayer);
					//mechanitors.RandomElement().relations.AddDirectRelation(PawnRelationDefOf.Overseer, pawn);
				}
			}
		}
	}

}
