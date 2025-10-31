using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_Ideo : Gene_Hivemind_Drone, IGeneRemoteControl
	{
		public string RemoteActionName
		{
			get
			{
				if (nextTick > 0)
				{
					return nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return "WVC_SetIdeo".Translate();
			}
		}

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControl_IdeoChangeDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			if (!Active || nextTick > 0)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			List<FloatMenuOption> list = new();
			List<Ideo> ideos = Find.IdeoManager.IdeosListForReading.Where((i) => !i.hidden).ToList();
			for (int i = 0; i < ideos.Count; i++)
			{
				Ideo newIdeo = ideos[i];
				list.Add(new FloatMenuOption(newIdeo.name.CapitalizeFirst(), delegate
				{
					if (!newIdeo.hidden)
					{
						Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Hivemind_IdeoChangeWarning".Translate(), delegate
						{
							SetIdeo(newIdeo);
							nextTick = 60000;
							Messages.Message("WVC_IdeoChanged".Translate(), new LookTargets(Hivemind), MessageTypeDefOf.PositiveEvent);
						});
						Find.WindowStack.Add(window);
					}
					else
					{
						SoundDefOf.ClickReject.PlayOneShotOnCamera();
					}
				}, orderInPriority: 0 - newIdeo.memes.Count));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

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

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

		//===========

		private int nextTick;

		//public override void TickInterval(int delta)
		//{
		//    if (!pawn.IsHashIntervalTick(59996, delta))
		//    {
		//        return;
		//    }
		//    SetHiveIdeo();
		//}

		//public override IEnumerable<Gizmo> GetGizmos()
		//{

		//}

		public void SetIdeo(Ideo newIdeo)
		{
			//if (!pawn.IsColonist)
			//{
			//    return;
			//}
			//ResetCollection(); // For sync node only. In short => Feature
			//if (Gene_Hivemind.HivemindPawns.NullOrEmpty())
			//{
			//    return;
			//}
			foreach (Pawn drone in Hivemind)
			{
				drone.ideo?.SetIdeo(newIdeo);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

}
