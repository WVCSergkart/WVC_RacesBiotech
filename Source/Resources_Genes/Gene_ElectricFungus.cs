using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ElectricFungus : Gene_Resource, IGeneResourceDrain
	{
		// public bool woundClottingAllowed = true;
		// public bool ageReversionAllowed = true;
		// public bool totalHealingAllowed = true;

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

		public float ResourceLossPerDay => ResourceLossPerDay();

		public override float InitialResourceMax => 1.0f;

		// public override float Max => 5.0f;

		public override float MinLevelForAlert => 0.05f;

		public override float MaxLevelOffset => 0.20f;

		protected override Color BarColor => new ColorInt(125, 114, 37).ToColor;

		protected override Color BarHighlightColor => new ColorInt(156, 142, 46).ToColor;

		public override void PostAdd()
		{
			base.PostAdd();
			Reset();
			ResetResourceValue();
		}

		private void ResetResourceValue()
		{
			FloatRange floatRange = new(0.06f, 0.97f);
			Value = floatRange.RandomInRange;
		}

		// public override void Notify_IngestedThing(Thing thing, int numTaken)
		// {
			// if (thing.def.IsMeat)
			// {
				// IngestibleProperties ingestible = thing.def.ingestible;
				// if (ingestible != null && ingestible.sourceDef?.race?.Humanlike == true)
				// {
					// GeneUtility.OffsetHemogen(pawn, 0.0375f * thing.GetStatValue(StatDefOf.Nutrition) * (float)numTaken);
				// }
			// }
		// }

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
		}

		public override void SetTargetValuePct(float val)
		{
			targetValue = Mathf.Clamp(val * Max, 0f, Max - MaxLevelOffset);
		}

		public bool ShouldChargeNow()
		{
			return Value < targetValue;
		}

		public bool PawnUnconscious()
		{
			if (pawn.Downed || pawn.Deathresting || pawn.needs.rest.Resting)
			{
				return false;
			}
			return true;
		}

		public float ResourceLossPerDay()
		{
			if (PawnUnconscious())
			{
				return -1 * def.resourceLossPerDay;
			}
			return def.resourceLossPerDay;
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
