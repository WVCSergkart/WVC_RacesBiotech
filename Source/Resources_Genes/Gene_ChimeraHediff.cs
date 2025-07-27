using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ChimeraHediff : Gene_AddOrRemoveHediff
	{

		public virtual Hediff ChimeraHediff => pawn.health?.hediffSet?.GetFirstHediffOfDef(Props.hediffDefName); 

		//public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		[Unsaved(false)]
		private Gene_Chimera cachedChimeraGene;

		public Gene_Chimera Chimera
		{
			get
			{
				if (cachedChimeraGene == null || !cachedChimeraGene.Active)
				{
					cachedChimeraGene = pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>();
				}
				return cachedChimeraGene;
			}
		}

	}

	public class Gene_ChimeraBandwidth : Gene_ChimeraHediff, IGeneRemoteControl
	{

		public string RemoteActionName => "WVC_HideBandwidth".Translate();

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlHideBandwitdhDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
        {
            HideOrUnhideUI();
            //genesSettings.Close();
        }

        public bool RemoteControl_Hide => !WVC_Biotech.settings.enable_HideMechanitorButtonsPatch || !Active;

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
            UnhideUI();
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
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public bool shouldHideMechanitorUI = true;

        public override void Notify_OverriddenBy(Gene overriddenBy)
        {
            base.Notify_OverriddenBy(overriddenBy);
			UnhideUI();
		}

        public override void Notify_Override()
        {
            base.Notify_Override();
            Load();
        }

        private void Load()
        {
            if (!shouldHideMechanitorUI)
            {
                StaticCollectionsClass.AddHideMechanitors(pawn);
            }
        }

        private void HideOrUnhideUI()
		{
			shouldHideMechanitorUI = !shouldHideMechanitorUI;
            if (shouldHideMechanitorUI)
            {
                StaticCollectionsClass.RemoveHideMechanitors(pawn);
            }
            else
            {
                StaticCollectionsClass.AddHideMechanitors(pawn);
            }
            //StaticCollectionsClass.AddOrRemoveHideMechanitors(pawn);
        }

        private void UnhideUI()
		{
			shouldHideMechanitorUI = true;
			StaticCollectionsClass.RemoveHideMechanitors(pawn);
		}

		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref shouldHideMechanitorUI, "shouldHideMechanitorUI", defaultValue: true);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				Load();
			}
		}

    }

}
