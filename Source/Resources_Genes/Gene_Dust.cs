using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Dust : Gene_Resource, IGeneResourceDrain
	{
		// public bool woundClottingAllowed = true;
		// public bool ageReversionAllowed = true;
		// public bool totalHealingAllowed = true;

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public Gene_Resource Resource => this;

		public Pawn Pawn => pawn;

		public bool CanOffset
		{
			get
			{
				if (Active)
				{
					return true;
				}
				return false;
			}
		}

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public float ResourceLossPerDay => ResourceLoss(pawn, def.resourceLossPerDay);

		public override float InitialResourceMax => 1.0f;

		// public override float Max => 5.0f;

		public override float MinLevelForAlert => 0.05f;

		public override float MaxLevelOffset => 0.20f;

		// protected override Color BarColor => new ColorInt(125, 122, 65).ToColor;
		// protected override Color BarHighlightColor => new ColorInt(156, 142, 46).ToColor;

		// protected override Color BarColor => new ColorInt(173, 142, 112).ToColor;
		// protected override Color BarHighlightColor => new ColorInt(173, 142, 112).ToColor;

		protected override Color BarColor => new ColorInt(150, 125, 85).ToColor;
		protected override Color BarHighlightColor => new ColorInt(150, 125, 85).ToColor;

		// public Color ColorFromValue()
		// {
			// if (Value > 50f)
			// {
				// return new ColorInt(150, 150, 85).ToColor;
			// }
			// return new ColorInt(150, 125, 85).ToColor;
		// }

		// Base
		public override void PostAdd()
		{
			base.PostAdd();
			Reset();
			ResetResourceValue();
			Gene_AddOrRemoveHediff.AddOrRemoveHediff(HediffDefName, pawn, this);
		}

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
			if (!pawn.IsHashIntervalTick(60000))
			{
				return;
			}
			Gene_AddOrRemoveHediff.AddOrRemoveHediff(HediffDefName, pawn, this);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Gene_AddOrRemoveHediff.RemoveHediff(HediffDefName, pawn);
		}
		// Base

		private void ResetResourceValue()
		{
			FloatRange floatRange = new(0.06f, 0.97f);
			Value = floatRange.RandomInRange;
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			// if (thing.def.IsMeat)
			// {
			// }
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				DustUtility.OffsetDust(pawn, 0.0625f * thing.GetStatValue(StatDefOf.Nutrition) * (float)numTaken);
			}
		}

		// public override void SetTargetValuePct(float val)
		// {
			// targetValue = Mathf.Clamp(val * Max, 0f, Max - MaxLevelOffset);
		// }

		// public bool ShouldChargeNow()
		// {
			// return Value < targetValue;
		// }

		public static bool PawnUnconscious(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || pawn.needs.rest.Resting)
			{
				return true;
			}
			return false;
		}

		public static float ResourceLoss(Pawn pawn, float def)
		{
			Gene_AngelicStability gene_AngelicStability = pawn.genes?.GetFirstGeneOfType<Gene_AngelicStability>();
			if (gene_AngelicStability != null)
			{
				return 0f;
			}
			if (PawnUnconscious(pawn))
			{
				return -1f * def;
			}
			return def;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			foreach (Gizmo resourceDrainGizmo in UndeadUtility.GetResourceDrainGizmos(this))
			{
				yield return resourceDrainGizmo;
			}
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref woundClottingAllowed, "woundClottingAllowed", defaultValue: true);
			// Scribe_Values.Look(ref ageReversionAllowed, "ageReversionAllowed", defaultValue: true);
			// Scribe_Values.Look(ref totalHealingAllowed, "totalHealingAllowed", defaultValue: true);
		// }
	}

}
