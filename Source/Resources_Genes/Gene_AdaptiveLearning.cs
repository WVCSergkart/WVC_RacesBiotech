using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AdaptiveStudy : Gene_AddOrRemoveHediff
	{

		public static int MaxScore => 2000000;

		protected float beautyScore;
		protected float damageScore;
		protected float socialScore;
		protected float animalsScore;
		protected float cookingScore;
		protected float craftingScore;
		protected float intellectual;
		protected float plantsScore;
		protected float miningScore;

		public virtual int CookingSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(25, 2),
					new CurvePoint(100, 5),
					new CurvePoint(350, 8),
					new CurvePoint(1000, 16),
					new CurvePoint(5000, 20)
				};
				return (int)curvePoints.Evaluate(cookingScore);
			}
		}
		public virtual int CraftingAndConstructionSkills
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(500, 1),
					new CurvePoint(20000, 6),
					new CurvePoint(90000, 8),
					new CurvePoint(200000, 12),
					new CurvePoint(760000, 16),
					new CurvePoint(1600000, 20)
				};
				return (int)curvePoints.Evaluate(craftingScore);
			}
		}

		public virtual int MedicineSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(10, 2),
					new CurvePoint(40, 6),
					new CurvePoint(60, 8),
					new CurvePoint(400, 12),
					new CurvePoint(800, 16),
					new CurvePoint(3600, 20)
				};
				return (int)curvePoints.Evaluate(damageScore);
			}
		}

		public virtual int ArtSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(50, 2),
					new CurvePoint(320, 6),
					new CurvePoint(900, 8),
					new CurvePoint(1500, 12),
					new CurvePoint(5000, 16),
					new CurvePoint(10000, 20)
				};
				return (int)curvePoints.Evaluate(beautyScore);
			}
		}

		public virtual int WarSkills
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(2, 2),
					new CurvePoint(8, 6),
					new CurvePoint(16, 8),
					new CurvePoint(26, 12),
					new CurvePoint(50, 16),
					new CurvePoint(100, 20)
				};
				return (int)curvePoints.Evaluate(Find.StoryWatcher.statsRecord.colonistsKilled + Find.StoryWatcher.statsRecord.numRaidsEnemy + Find.StoryWatcher.statsRecord.numThreatBigs);
			}
		}

		public virtual int PlantsSkills
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(5, 2),
					new CurvePoint(8, 6),
					new CurvePoint(20, 8),
					new CurvePoint(60, 12),
					new CurvePoint(200, 16),
					new CurvePoint(1000, 20)
				};
				return (int)curvePoints.Evaluate(plantsScore);
			}
		}

		public virtual int SocialSkills
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(10, 2),
					new CurvePoint(16, 6),
					new CurvePoint(30, 8),
					new CurvePoint(120, 12),
					new CurvePoint(370, 16),
					new CurvePoint(1000, 20)
				};
				return (int)curvePoints.Evaluate(socialScore);
			}
		}

		public virtual int AnimalsSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(10, 2),
					new CurvePoint(18, 6),
					new CurvePoint(26, 8),
					new CurvePoint(50, 12),
					new CurvePoint(120, 16),
					new CurvePoint(600, 20)
				};
				return (int)curvePoints.Evaluate(animalsScore);
			}
		}

		public virtual int MiningSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(10, 2),
					new CurvePoint(30, 6),
					new CurvePoint(60, 8),
					new CurvePoint(200, 12),
					new CurvePoint(400, 16),
					new CurvePoint(1000, 20)
				};
				return (int)curvePoints.Evaluate(miningScore);
			}
		}

		public virtual int IntelectualSkill
		{
			get
			{
				SimpleCurve curvePoints = new()
				{
					new CurvePoint(0, 0),
					new CurvePoint(2, 2),
					new CurvePoint(6, 6),
					new CurvePoint(18, 8),
					new CurvePoint(50, 12),
					new CurvePoint(120, 16),
					new CurvePoint(240, 20)
				};
				return (int)curvePoints.Evaluate(intellectual);
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (pawn.IsHashIntervalTick(5000, delta))
			{
				UpdScores(5000);
			}
		}

		public void UpdScores(int delta)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (craftingScore < MaxScore)
			{
				craftingScore += Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor / 60000 * delta;
			}
			if (beautyScore < MaxScore)
			{
				beautyScore += pawn.needs?.beauty != null ? pawn.needs.beauty.CurLevel / 60000 * delta : 0;
			}
			if (damageScore < MaxScore)
			{
				damageScore += pawn.health != null ? 1f - pawn.health.summaryHealth.SummaryHealthPercent / 60000 * delta : 0;
			}
			if (pawn.MapHeld != null)
			{
				if (plantsScore < MaxScore)
				{
					plantsScore += pawn.MapHeld.Biome.plantDensity / 60000 * delta;
				}
				if (animalsScore < MaxScore)
				{
					animalsScore += pawn.MapHeld.Biome.animalDensity / 60000 * delta;
				}
				if (miningScore < MaxScore)
				{
					miningScore += pawn.MapHeld.roofGrid.Roofed(pawn.PositionHeld) ? 1 / 60000 * delta : 0;
				}
			}
			if (socialScore < MaxScore)
			{
				socialScore += pawn.relations != null ? pawn.relations.DirectRelations.Count / 60000 * delta : 0;
			}
			if (intellectual < MaxScore)
			{
				intellectual += 1 / 60000 * delta;
			}
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (cookingScore < MaxScore)
			{
				cookingScore += (thing.def.ingestible.CachedNutrition * numTaken);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref beautyScore, nameof(beautyScore));
			Scribe_Values.Look(ref damageScore, nameof(damageScore));
			Scribe_Values.Look(ref socialScore, nameof(socialScore));
			Scribe_Values.Look(ref animalsScore, nameof(animalsScore));
			Scribe_Values.Look(ref cookingScore, nameof(cookingScore));
			Scribe_Values.Look(ref craftingScore, nameof(craftingScore));
			Scribe_Values.Look(ref intellectual, nameof(intellectual));
			Scribe_Values.Look(ref plantsScore, nameof(plantsScore));
			Scribe_Values.Look(ref miningScore, nameof(miningScore));
		}

	}

}
