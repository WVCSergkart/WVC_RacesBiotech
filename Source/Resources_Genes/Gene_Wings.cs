using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Wings : Gene
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// AddOrRemoveHediff();
		// }

		// public override void Tick()
		// {
			// base.Tick();
			// if (!pawn.IsHashIntervalTick(60000))
			// {
				// return;
			// }
			// AddOrRemoveHediff();
		// }

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			if (Active)
			{
				if (!pawn.health.hediffSet.HasHediff(HediffDefName))
				{
					pawn.health.AddHediff(HediffDefName);
				}
				else
				{
					RemoveHediff();
				}
			}
			else
			{
				RemoveHediff();
			}
		}

		public void RemoveHediff()
		{
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefName);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		private string FlyOrWalk()
		{
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				return "WVC_XaG_Gene_Wings_On".Translate();
			}
			return "WVC_XaG_Gene_Wings_Off".Translate();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: Add Or Remove Hediff",
					// action = delegate
					// {
						// if (Active)
						// {
							// AddOrRemoveHediff();
						// }
					// }
				// };
			// }
			if (Active && Find.Selector.SelectedPawns.Count == 1 && pawn.Faction == Faction.OfPlayer || DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_XaG_Gene_Wings".Translate() + ": " + FlyOrWalk(),
					defaultDesc = "WVC_XaG_Gene_WingsDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get(def.iconPath),
					action = delegate
					{
						AddOrRemoveHediff();
						if (!pawn.health.hediffSet.HasHediff(HediffDefName))
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
						}
						else
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						}
					}
				};
			}
		}

	}

}
