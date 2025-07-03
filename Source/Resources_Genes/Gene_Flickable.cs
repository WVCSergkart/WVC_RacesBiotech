using RimWorld;
using System;
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

		public override void Tick()
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
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
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
					XaG_UiUtility.FlickSound(!pawn.health.hediffSet.HasHediff(Props.hediffDefName));
                }
            };
		}

    }

	public class Gene_Wings : Gene_Flickable, IGeneRemoteControl
	{
		public string RemoteActionName => Wings();

		public TaggedString RemoteActionDesc => "WVC_XaG_Gene_WingsDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			AddOrRemoveHediff(pawn, Props.hediffDefName, this);
		}

		//public int nextFly = 15;

		//public override void Tick()
		//{
		//	nextFly--;
  //          if (nextFly > 0)
  //          {
  //              return;
  //          }
		//	nextFly = 15;
		//	pawn.flight?.StartFlying();
		//}

        public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		//=================

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
			//if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			//{
			//	yield break;
			//}
			//yield return new Command_Action
			//{
			//	defaultLabel = def.LabelCap + ": " + Wings(),
			//	defaultDesc = "WVC_XaG_Gene_WingsDesc".Translate(),
			//	icon = ContentFinder<Texture2D>.Get(def.iconPath),
			//	action = delegate
			//	{
			//		AddOrRemoveHediff(pawn, Props.hediffDefName, this);
			//		if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			//		{
			//			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			//		}
			//		else
			//		{
			//			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			//		}
			//	}
			//};
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		// public string GetInspectInfo
		// {
			// get
			// {
				// if (OnOrOff)
				// {
					// return "WVC_XaG_Gene_Wings_On_Info".Translate().Resolve();
				// }
				// return null;
			// }
		// }

	}

	public class Gene_Invisibility : Gene_Flickable
	{

		private int cooldown = -1;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
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
					Hediff hediff = pawn.health.hediffSet?.GetFirstHediffOfDef(Props.hediffDefName);
					if (WVC_Biotech.settings.invisibility_invisBonusTicks > 0)
					{
						HediffComp_Disappears hediffComp_Disappears = hediff?.TryGetComp<HediffComp_Disappears>();
						if (hediffComp_Disappears != null)
						{
							hediffComp_Disappears.SetDuration((int)WVC_Biotech.settings.invisibility_invisBonusTicks);
						}
					}
					cooldown = Find.TickManager.TicksGame + 450;
					XaG_UiUtility.FlickSound(hediff == null);
				}
			};
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cooldown, "cooldown", -1);
		}

	}

	[Obsolete]
	public class Gene_Gizmo : Gene
	{

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			{
				yield break;
			}
			if (!def.showGizmoWhenDrafted && pawn.Drafted)
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

	}

    public class Gene_RegenerationSleep : Gene_OverOverridable
	{

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFaction(pawn, this))
			{
				yield break;
			}
			if (!def.showGizmoWhenDrafted && pawn.Drafted)
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

	}

}
