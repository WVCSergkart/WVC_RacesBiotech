using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_ShapeshiftToXenohuman : CompProperties
	{

		public XenotypeDef xenotypeDef;

		public CompProperties_ShapeshiftToXenohuman()
		{
			compClass = typeof(CompShapeshiftToXenohuman);
		}
	}

	public class CompShapeshiftToXenohuman : ThingComp
	{

		private CompProperties_ShapeshiftToXenohuman Props => (CompProperties_ShapeshiftToXenohuman)props;

		// public override void Initialize(CompProperties props)
		// {
			// base.Initialize(props);
			// AddHediff();
		// }

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			GenerateChild();
		}

		public void GenerateChild()
		{
			Pawn mechaEgg = parent as Pawn;
			Pawn overseer = mechaEgg.GetOverseer();
			GenerateXenoHumanPawn(overseer, mechaEgg, Props.xenotypeDef);
			// foreach (HediffDef hediff in Props.hediffDefs)
			// {
				// if (!pawn.health.hediffSet.HasHediff(hediff))
				// {
					// pawn.health.AddHediff(hediff);
				// }
			// }
			mechaEgg.Destroy(DestroyMode.Vanish);
		}

		public static void GenerateXenoHumanPawn(Pawn pawn, Pawn spawnPawn, XenotypeDef xenotype)
		{
			XenotypeExtension_SubXenotype modExtension = xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
			if (modExtension == null)
			{
				return;
			}
			SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
			Pawn pawnParent = pawn;
			Pawn pawnTarget = spawnPawn;
			PawnGenerationRequest generateNewBornPawn = new(pawnParent.kindDef, pawnParent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			// Pawn pawnNewBornChild = null;
			Pawn pawnNewBornChild = PawnGenerator.GeneratePawn(generateNewBornPawn);
			SubXenotypeUtility.RandomXenotype(pawnNewBornChild, subXenotypeDef, xenotype);
			if (PawnUtility.TrySpawnHatchedOrBornPawn(pawnNewBornChild, pawnTarget))
			{
				if (pawnNewBornChild.playerSettings != null && pawnParent.playerSettings != null)
				{
					pawnNewBornChild.playerSettings.AreaRestriction = pawnParent.playerSettings.AreaRestriction;
				}
				if (pawnNewBornChild.RaceProps.IsFlesh)
				{
					pawnNewBornChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawnParent);
				}
				if (pawnParent.Spawned)
				{
					pawnParent.GetLord()?.AddPawn(pawnNewBornChild);
				}
			}
			else
			{
				Find.WorldPawns.PassToWorld(pawnNewBornChild, PawnDiscardDecideMode.Discard);
			}
			TaleRecorder.RecordTale(TaleDefOf.GaveBirth, pawnParent, pawn);
			if (pawnTarget.Spawned)
			{
				FilthMaker.TryMakeFilth(pawnTarget.Position, pawnTarget.Map, ThingDefOf.Filth_AmnioticFluid, pawnTarget.LabelIndefinite(), 5);
				if (pawnTarget.caller != null)
				{
					pawnTarget.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
			// Messages.Message(completeMessage.Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
		}
	}

}
