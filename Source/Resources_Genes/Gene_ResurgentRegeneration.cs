// Verse.Gene_Healing
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class Gene_ResurgentTotalHealing : Gene_ResurgentDependent
	{
		private int ticksToHealBodyPart;

		private static readonly IntRange HealingIntervalTicksRange = new(60000, 120000);

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
					TryHealRandomPermanentWound(pawn, def);
				}
				ResetInterval();
			}
		}

		public static void TryHealRandomPermanentWound(Pawn pawn, GeneDef def)
		{
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if ((gene_Resurgent.Value - def.resourceLossPerDay) >= 0f)
				{
					TaggedString taggedString = HealthUtility.FixWorstHealthCondition(pawn);
					if (taggedString != null)
					{
						gene_Resurgent.Value -= def.resourceLossPerDay;
					}
					if (PawnUtility.ShouldSendNotificationAbout(pawn))
					{
						Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
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
							TryHealRandomPermanentWound(pawn, def);
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
}
