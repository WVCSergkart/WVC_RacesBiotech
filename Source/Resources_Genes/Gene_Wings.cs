using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Wings : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			if (Active)
			{
				if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
				{
					pawn.health.AddHediff(Props.hediffDefName);
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
			if (pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDefName);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		private string FlyOrWalk()
		{
			if (pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			{
				return "WVC_XaG_Gene_Wings_On".Translate();
			}
			return "WVC_XaG_Gene_Wings_Off".Translate();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_Gene_Wings".Translate() + ": " + FlyOrWalk(),
				defaultDesc = "WVC_XaG_Gene_WingsDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					AddOrRemoveHediff();
					if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
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
