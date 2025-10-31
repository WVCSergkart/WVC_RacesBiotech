using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Hivemind_SyncNode : Gene_Hivemind_Drone, IGeneRemoteControl
    {

        public string RemoteActionName
        {
            get
            {
                if (cooldownTick > 0)
                {
                    return cooldownTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
                }
                return LabelCap;
            }
        }

        public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControl_HivemindSyncNode".Translate();

        public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
        {
            if (!Active || cooldownTick > 0 || !HivemindUtility.SuitableForHivemind(pawn))
            {
                SoundDefOf.ClickReject.PlayOneShotOnCamera();
                return;
            }
            Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_RemoteControl_HivemindSyncNodeWarning".Translate(), delegate
            {
                ResetCollection();
                cooldownTick = 60000 * 7;
                Messages.Message("WVC_XaG_RemoteControl_HivemindSyncNodeMessage".Translate(Hivemind.Count), new LookTargets(Hivemind), MessageTypeDefOf.PositiveEvent);
            });
            Find.WindowStack.Add(window);
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

        // =============

        public override void TickInterval(int delta)
        {
            if (cooldownTick > 0)
            {
                cooldownTick -= delta;
            }
            if (!pawn.IsHashIntervalTick(59997, delta))
            {
                return;
            }
            ResetCollection();
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            List<Pawn> hivemind = Hivemind;
            yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_HivemindPawns_Label".Translate(), hivemind.Count.ToString(), "WVC_XaG_HivemindPawns_Desc".Translate(), 100, hyperlinks: hivemind.Select((pawn) => new Dialog_InfoCard.Hyperlink(pawn)));
        }

        private int cooldownTick = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cooldownTick, "cooldownTick", -1);
        }

    }

}
