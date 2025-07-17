using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Rejuvenator : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => "WVC_DesiredAge".Translate();

		public TaggedString RemoteActionDesc => "Gene_RejuvenatorDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			List<FloatMenuOption> list = new();
			for (int i = 0; i < 60; i++)
			{
				int newAge = i + 18;
				list.Add(new FloatMenuOption(newAge.ToString(), delegate
				{
					desiredAge = newAge;
					Messages.Message("Gene_RejuvenatorMessage".Translate(), null, MessageTypeDefOf.NeutralEvent, historical: false);
					genesSettings.Close();
				}));
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
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Revers age",
					action = delegate
					{
						AgeReversal();
					}
				};
			}
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
				{
					yield return gizmo;
				}
			}
		}

		// =============

		public int desiredAge = 18;

		public override void PostAdd()
		{
			base.PostAdd();
			AgelessUtility.InitialRejuvenation(pawn);
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59675, delta))
			{
				return;
			}
			AgeReversal();
		}

		public void AgeReversal()
		{
			if (AgelessUtility.CanAgeReverse(pawn, desiredAge))
			{
				AgelessUtility.AgeReverse(pawn);
			}
		}

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref desiredAge, "desiredAge", 18);
		}

    }

}
