using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Inhumanized : Gene, IGeneOverridden
	{

		private int nextTick = 45679;
		private bool inhumanizedBeforeGene = false;

		public HediffDef Inhumanized => HediffDefOf.Inhumanized;
		public bool IsInhuman => pawn.Inhumanized();

		public override void PostAdd()
		{
			if (!Active)
            {
				return;
            }
			base.PostAdd();
			if (pawn.Inhumanized())
			{
				inhumanizedBeforeGene = true;
			}
			HediffUtility.TryAddHediff(Inhumanized, pawn, def, null);
		}

		public override void TickInterval(int delta)
        {
            // base.Tick();
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            Inhumanize(pawn);
            nextTick = 157889;
        }

        public static void Inhumanize(Pawn pawn)
        {
            if (ModsConfig.AnomalyActive && !pawn.Inhumanized() && Find.Anomaly.LevelDef != MonolithLevelDefOf.Disrupted)
            {
                pawn.mindState?.mentalBreaker?.TryDoMentalBreak("WVC_XaG_MentalBreakReason_Inhumanized".Translate(), MentalBreakDefOf.HumanityBreak);
            }
        }

		public virtual void Notify_OverriddenBy(Gene overriddenBy)
		{
			if (!inhumanizedBeforeGene)
			{
				HediffUtility.TryRemoveHediff(Inhumanized, pawn);
			}
		}

		public virtual void Notify_Override()
		{
			HediffUtility.TryAddHediff(Inhumanized, pawn, def, null);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!inhumanizedBeforeGene)
			{
				HediffUtility.TryRemoveHediff(Inhumanized, pawn);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref inhumanizedBeforeGene, "inhumanizedBeforeGene", false);
		}

	}

	public class Gene_Inhumanized_Switch : Gene_Inhumanized, IGeneRemoteControl
	{

		public string RemoteActionName => XaG_UiUtility.OnOrOff(IsInhuman);

		public TaggedString RemoteActionDesc => "WVC_XaG_InhumanizedSwitchDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Inhumanized);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
            }
			else
            {
				pawn.health.AddHediff(Inhumanized);
            }
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

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{

		}

		public override void Notify_Override()
		{

		}

		public override void TickInterval(int delta)
		{

		}

	}

}
