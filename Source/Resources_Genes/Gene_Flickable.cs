using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Flickable : Gene, IGeneOverridden
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public bool OnOrOff => pawn.health.hediffSet.HasHediff(Props.hediffDefName);

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff(pawn, Props.hediffDefName);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RemoveHediff(pawn, Props.hediffDefName);
		}

		public void Notify_Override()
		{
		}

		public static void AddOrRemoveHediff(Pawn pawn, HediffDef hediffDef, Gene gene)
		{
			if (gene.Active)
			{
				if (!pawn.health.hediffSet.HasHediff(hediffDef))
				{
					pawn.health.AddHediff(hediffDef);
				}
				else
				{
					RemoveHediff(pawn, hediffDef);
				}
			}
			else
			{
				RemoveHediff(pawn, hediffDef);
			}
		}

		public static void RemoveHediff(Pawn pawn, HediffDef hediffDef)
		{
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		public static string Flick(Pawn pawn, HediffDef hediffDef)
		{
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				return "WVC_XaG_Gene_DustMechlink_On".Translate().Colorize(ColorLibrary.Green);
			}
			return "WVC_XaG_Gene_DustMechlink_Off".Translate().Colorize(ColorLibrary.RedReadable);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + Flick(pawn, Props.hediffDefName),
				defaultDesc = Props.message.Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					AddOrRemoveHediff(pawn, Props.hediffDefName, this);
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

	public class Gene_Wings : Gene_Flickable, IGeneInspectInfo
	{

		private string Wings()
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
				defaultLabel = def.LabelCap + ": " + Wings(),
				defaultDesc = "WVC_XaG_Gene_WingsDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					AddOrRemoveHediff(pawn, Props.hediffDefName, this);
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

		public string GetInspectInfo
		{
			get
			{
				if (OnOrOff)
				{
					return "WVC_XaG_Gene_Wings_On_Info".Translate().Resolve();
				}
				return null;
			}
		}

	}

	public class Gene_Invisibility : Gene_Flickable
	{

		private int cooldown = -1;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap + ": " + Flick(pawn, Props.hediffDefName),
				defaultDesc = Props.message.Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				Disabled = cooldown > Find.TickManager.TicksGame,
				disabledReason = "WVC_XaG_Gene_InvisibilityCD".Translate(),
				action = delegate
				{
					AddOrRemoveHediff(pawn, Props.hediffDefName, this);
					cooldown = Find.TickManager.TicksGame + 450;
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cooldown, "cooldown", -1);
		}

	}

}
