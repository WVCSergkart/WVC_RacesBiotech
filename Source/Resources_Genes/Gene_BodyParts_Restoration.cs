// Verse.Gene_Healing
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_RestoreMissingBodyParts : Gene
	{
		private int ticksToHealBodyPart;

		private static readonly IntRange HealingIntervalTicksRange = new(900000, 1800000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealBodyPart--;
			if (ticksToHealBodyPart <= 0)
			{
				if (Active)
				{
					HealingUtility.TryHealRandomPermanentWound(pawn, this, true);
				}
				ResetInterval();
			}
		}

		// public static void TryHealRandomPermanentWound(Pawn pawn)
		// {
			// TaggedString taggedString = HealthUtility.FixWorstHealthCondition(pawn);
			// if (PawnUtility.ShouldSendNotificationAbout(pawn))
			// {
				// Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			// }
		// }

		private void ResetInterval()
		{
			ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Restore lost limb",
					action = delegate
					{
						if (Active)
						{
							HealingUtility.TryHealRandomPermanentWound(pawn, this, true);
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		}
	}

	public class Gene_BodyPartsRestoration : Gene
	{
		private int ticksToHealBodyPart;

		// 15-30 days
		private static readonly IntRange HealingIntervalTicksRange = new(900000, 1800000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealBodyPart--;
			if (ticksToHealBodyPart <= 0)
			{
				if (Active)
				{
					HealingUtility.TryHealRandomPermanentWound(pawn, this);
				}
				ResetInterval();
			}
		}

		private void ResetInterval()
		{
			ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Restore lost limb",
					action = delegate
					{
						if (Active)
						{
							HealingUtility.TryHealRandomPermanentWound(pawn, this);
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		}
	}

	public class Gene_DustBodyPartsRestoration : Gene_FoodEfficiency
	{
		private int ticksToHealBodyPart;

		// 5-15 days
		private static readonly IntRange HealingIntervalTicksRange = new(300000, 900000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealBodyPart--;
			if (ticksToHealBodyPart <= 0)
			{
				if (Active)
				{
					HealingUtility.TryHealRandomPermanentWound(pawn, this);
				}
				ResetInterval();
			}
		}

		private void ResetInterval()
		{
			ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Restore lost limb",
					action = delegate
					{
						if (Active)
						{
							HealingUtility.TryHealRandomPermanentWound(pawn, this);
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		}
	}

	public class Gene_ResurgentTotalHealing : Gene_ResurgentDependent
	{
		private int ticksToHealBodyPart;

		private static readonly IntRange HealingIntervalTicksRange = new(300000, 900000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealBodyPart--;
			if (ticksToHealBodyPart <= 0)
			{
				if (Active)
				{
					TryHealRandomPermanentWound(pawn, this);
				}
				ResetInterval();
			}
		}

		private void TryHealRandomPermanentWound(Pawn pawn, Gene gene)
		{
			// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (Resurgent != null)
			{
				if (Resurgent.totalHealingAllowed)
				{
					if ((Resurgent.Value - gene.def.resourceLossPerDay) >= 0f)
					{
						TaggedString taggedString = HealingUtility.FixWorstHealthCondition(pawn, gene, true);
						if (taggedString != null)
						{
							Resurgent.Value -= gene.def.resourceLossPerDay;
						}
						if (PawnUtility.ShouldSendNotificationAbout(pawn))
						{
							Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
						}
					}
				}
			}
		}

		private void ResetInterval()
		{
			ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Restore lost limb",
					action = delegate
					{
						if (Active)
						{
							TryHealRandomPermanentWound(pawn, this);
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		}
	}

	public class Gene_GauranlenDryads_TotalHealing : Gene_DryadQueen_Dependant
	{

		private int ticksToHealDryads;

		public IntRange healingIntervalTicksRange = new(120000, 1800000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealDryads--;
			if (ticksToHealDryads > 0)
			{
				return;
			}
			HealDryads();
		}

		public void HealDryads()
		{
			List<Pawn> connectedThings = Gauranlen?.AllDryads;
			foreach (Pawn dryad in connectedThings)
			{
				HealingUtility.TryHealRandomPermanentWound(dryad, this, true);
			}
			ResetInterval();
		}

		private void ResetInterval()
		{
			ticksToHealDryads = healingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TotalHealDryads",
					action = delegate
					{
						HealDryads();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealDryads, "ticksToHealDryads", 0);
		}

	}

	public class Gene_HemogenScarless : Gene_HemogenOffset
	{
		private int ticksToHeal;

		private static readonly IntRange HealingIntervalTicksRange = new(900000, 1800000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			// base.Tick();
			ticksToHeal--;
			if (ticksToHeal <= 0)
			{
				TryConsumeHemogenAndHealWound();
			}
		}

		private void ResetInterval()
		{
			ticksToHeal = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: HealPermanentWound",
					action = delegate
					{
						TryConsumeHemogenAndHealWound();
					}
				};
			}
		}

		private void TryConsumeHemogenAndHealWound()
		{
			if (Hemogen == null)
			{
				return;
			}
			if ((Hemogen.Value - ResourceLossPerDay) <= 0f)
			{
				return;
			}
			if (HealingUtility.TryHealRandomPermanentWound(pawn, LabelCap))
			{
				GeneResourceDrainUtility.OffsetResource(this, 0f - ResourceLossPerDay);
			}
			ResetInterval();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHeal, "ticksToHeal", 0);
		}

	}

}
