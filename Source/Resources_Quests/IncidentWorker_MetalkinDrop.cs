using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class IncidentWorker_MetalkinDrop : IncidentWorker
	{
		private static readonly Pair<int, float>[] CountChance = new Pair<int, float>[4]
		{
		new Pair<int, float>(1, 1f),
		new Pair<int, float>(2, 0.95f),
		new Pair<int, float>(3, 0.7f),
		new Pair<int, float>(4, 0.4f)
		};

		public override float ChanceFactorNow(IIncidentTarget target)
		{
			return 1f / PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Count;
		}

		private int RandomCountToDrop
		{
			get
			{
				float x2 = (float)Find.TickManager.TicksGame / 3600000f;
				float timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
				return CountChance.RandomElementByWeight((Pair<int, float> x) => (x.First == 1) ? x.Second : (x.Second * timePassedFactor)).First;
			}
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (StaticCollectionsClass.cachedColonistsCount > 6)
			{
				return false;
			}
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
            return TryFindShipChunkDropCell(map.Center, map, 999999, out _);
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!TryFindShipChunkDropCell(map.Center, map, 999999, out var pos))
			{
				return false;
			}
			SpawnShipChunks(pos, map, RandomCountToDrop);
			Messages.Message("MessageShipChunkDrop".Translate(), new TargetInfo(pos, map), MessageTypeDefOf.NeutralEvent);
			return true;
		}

		private void SpawnShipChunks(IntVec3 firstChunkPos, Map map, int count)
		{
			bool pawnSpawned = false;
			SpawnChunk(firstChunkPos, map);
			for (int i = 0; i < count - 1; i++)
			{
				if (!pawnSpawned)
				{
					SpawnPawn(map, DefDatabase<XenotypeDef>.GetNamed("WVC_GeneThrower"), firstChunkPos);
					pawnSpawned = true;
				}
				else if (TryFindShipChunkDropCell(firstChunkPos, map, 5, out var pos))
				{
					SpawnChunk(pos, map);
                }
			}
		}

        private void SpawnPawn(Map map, XenotypeDef xenotypeDef, IntVec3 loc)
        {
			if (TryFindShipChunkDropCell(loc, map, 4, out var result3))
			{
				Pawn pawn = ThingUtility.FindPawn(ThingSetMakerDefOf.RefugeePod.root.Generate());
				pawn.guest.Recruitable = true;
				pawn.apparel.DestroyAll();
				pawn.equipment.DestroyAllEquipment();
				pawn.SetFaction(null);
				DuplicateUtility.NullifyBackstory(pawn);
				AgelessUtility.Rejuvenation(pawn);
				pawn.ageTracker.AgeChronologicalTicks += (long)(new IntRange(333, 1111).RandomInRange * 3600000L);
				pawn.health.AddHediff(HediffDefOf.RegenerationComa);
				ReimplanterUtility.SetXenotype(pawn, xenotypeDef);
				MiscUtility.Notify_DebugPawn(pawn);
				//SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming_SmallExplosion, pawn, result3, map);
				MiscUtility.SummonDropPod(map, new() { pawn }, result3, true);
				Find.LetterStack.ReceiveLetter("WVC_XaG_MetalkinDropLabel".Translate(), "WVC_XaG_MetalkinDropDesc".Translate(), LetterDefOf.PositiveEvent, new LookTargets(pawn));
				//GenSpawn.Spawn(pawn, result3, map);
			}
        }

        private void SpawnChunk(IntVec3 pos, Map map)
		{
			SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, ThingDefOf.ShipChunk, pos, map);
		}

		private bool TryFindShipChunkDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
		{
			return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, ThingDefOf.ShipChunk.terrainAffordanceNeeded, out pos, 10, nearLoc, maxDist);
		}
	}

}
