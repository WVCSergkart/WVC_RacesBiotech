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

	public class Gene_FleshmassNucleus : Gene_OverOverridable
	{

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		private int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 150000;
		}

		public override void Tick()
		{
			// base.Tick();
			if (pawn.IsHashIntervalTick(600))
			{
				HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 600);
			}
			//new IntRange(60000, 150000).RandomInRange
			if (GeneResourceUtility.CanTick(ref nextTick, 150000))
			{
				TryGiveMutation();
			}
		}

		private void TryGiveMutation()
		{
			if (Gene_MorphMutations.TryGetBestMutation(pawn, out HediffDef mutation))
			{
				if (HediffUtility.TryGiveFleshmassMutation(pawn, mutation))
				{
					return;
				}
				if (TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
				{
					hediffWithComps_FleshmassHeart.LevelUp();
				}
				else
				{
					TrySpawnMeat(pawn);
				}
			}
			//Evolve();
		}

		private void Evolve()
		{
			if (!Rand.Chance(0.2f))
			{
				return;
			}
			if (StaticCollectionsClass.cachedColonistsCount > 1)
			{
				return;
			}
			if (DefDatabase<GeneDef>.AllDefsListForReading.Where((geneDef) => geneDef.GetModExtension<GeneExtension_General>()?.isFleshmass == true).TryRandomElement(out GeneDef geneDef))
			{
				XaG_GeneUtility.AddGeneToChimera(pawn, geneDef);
			}
		}

		private bool TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart)
		{
			hediffWithComps_FleshmassHeart = null;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where((Hediff hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff && massHediff.CurrentLevel < Undead.maxMutationLevel).OrderBy((hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff ? massHediff.CurrentLevel : 0f).ToList();
			foreach (Hediff hediff in hediffs)
			{
				if (hediff is HediffAddedPart_FleshmassNucleus massHediff)
				{
					hediffWithComps_FleshmassHeart = massHediff;
					break;
				}
			}
			return hediffWithComps_FleshmassHeart != null;
		}

		public static void TrySpawnMeat(Pawn pawn)
		{
			int num = Mathf.CeilToInt(20 * pawn.BodySize * pawn.GetStatValue(StatDefOf.MaxNutrition));
			int randomInRange = new IntRange(3, 6).RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				if (CellFinder.TryRandomClosewalkCellNear(pawn.PositionHeld, pawn.MapHeld, 3, out var result))
				{
					Thing thing = ThingMaker.MakeThing(ThingDefOf.Meat_Twisted);
					thing.stackCount = Rand.RangeInclusive(4, num);
					GenDrop.TryDropSpawn(thing, result, pawn.MapHeld, ThingPlaceMode.Near, out var _);
				}
			}
			HediffUtility.MutationMeatSplatter(pawn);
			//EffecterDefOf.MeatExplosion.Spawn(pawn.PositionHeld, pawn.MapHeld).Cleanup();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (ModsConfig.AnomalyActive && pawn.Corpse?.Map != null)
			{
				if (GenDrop.TryDropSpawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.FleshmassNucleus, Faction.OfEntities)), pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld, ThingPlaceMode.Near, out var resultingThing))
				{
					CompActivity activity = resultingThing.TryGetComp<CompActivity>();
					if (activity != null)
					{
						activity.AdjustActivity(1f);
					}
					//if (!pawn.Corpse.Destroyed)
					//{
					//	EffecterDefOf.MeatExplosion.Spawn(pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld).Cleanup();
					//	pawn.Corpse.Destroy();
					//}
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: FleshmassMutation",
					action = delegate
					{
						TryGiveMutation();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

	public class Gene_FleshmassBrain : Gene
	{

		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(3333))
			{
				DoResearch(3333);
			}
		}

		private float? cachedResearchSpeed;

		public void DoResearch(int tick)
		{
			if (!cachedResearchSpeed.HasValue)
			{
				cachedResearchSpeed = pawn.GetStatValue(StatDefOf.ResearchSpeed);
			}
			ResearchManager researchManager = Find.ResearchManager;
			ResearchProjectDef currentProject = researchManager.GetProject();
			if (currentProject != null)
			{
				pawn.skills.Learn(SkillDefOf.Intellectual, 0.04f * tick);
				float researchAmount = (cachedResearchSpeed.Value * tick) / (StaticCollectionsClass.cachedColonistsCount > 0 ? StaticCollectionsClass.cachedColonistsCount : 1f);
				researchManager.ResearchPerformed(researchAmount, pawn);
			}
		}

	}

	public class Gene_FleshmassArmor : Gene
	{

		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(16333))
			{
				DoApparelDamage(16333);
			}
		}

		public void DoApparelDamage(int tick)
		{
			List<Apparel> apparels = pawn.apparel.WornApparel;
			foreach (Apparel apparel in apparels)
			{
				if (apparel.def.apparel.countsAsClothingForNudity && apparel.def.useHitPoints)
				{
					apparel.HitPoints -= tick / 1500;
				}
			}
		}

	}

}
