using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_Fleshmass_LoveEnhancer : Hediff
	{

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_FleshmassReproduction cachedGene;
		public Gene_FleshmassReproduction MainGene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_FleshmassReproduction>();
				}
				return cachedGene;
			}
		}

		public override void Notify_SurgicallyReplaced(Pawn surgeon)
		{
			TentacleAttack(surgeon);
		}

		public override void Notify_SurgicallyRemoved(Pawn surgeon)
		{
			TentacleAttack(surgeon);
		}

		private void TentacleAttack(Pawn surgeon)
		{
			if (ModsConfig.AnomalyActive)
			{
				//Pawn pawn = base.pawn;
				PawnKindDef fingerspike = PawnKindDefOf.Fingerspike;
				Faction ofEntities = Faction.OfEntities;
				float? fixedBiologicalAge = 0f;
				float? fixedChronologicalAge = 0f;
				Pawn beast = PawnGenerator.GeneratePawn(new PawnGenerationRequest(fingerspike, ofEntities, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, fixedBiologicalAge, fixedChronologicalAge));
				GenSpawn.Spawn(beast, CellFinder.StandableCellNear(pawn.Position, pawn.Map, 2f), pawn.Map);
				beast.stances.stunner.StunFor(new IntRange(120, 240).RandomInRange, surgeon);
				CompInspectStringEmergence compInspectStringEmergence = beast.TryGetComp<CompInspectStringEmergence>();
				if (compInspectStringEmergence != null)
				{
					compInspectStringEmergence.sourcePawn = pawn;
				}
				TaggedString label = "WVC_RemovedFleshmassPart_Label".Translate(pawn.Named("PAWN"));
				TaggedString text = "WVC_RemovedFleshmassPart_Desc".Translate(def.label, pawn.Named("PAWN"));
				Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatBig, beast);
			}
			MainGene?.Notify_TentacleRemoved();
			RimWorld.ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn, surgeon);
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(44464, delta))
			{
				if (MainGene == null)
				{
					pawn.health.RemoveHediff(this);
				}
			}
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is Gene_FleshmassReproduction && gene.Active))
			{
				if (Gene_FleshmassReproduction.TryAddLoveEnchancer(pawn))
				{
					if (DebugSettings.ShowDevGizmos)
					{
						Log.Warning("Trying to remove " + def.label + " hediff, but " + pawn.Name.ToString() + " has the required gene. Hediff is added back.");
					}
				}
			}
		}

	}

}
