using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Chameleon : Gene, IGeneRemoteControl
	{
		public virtual string RemoteActionName => "WVC_Style".Translate();

		public virtual TaggedString RemoteActionDesc => "WVC_XaG_GeneShapeshifterStyles_Desc".Translate();

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Find.WindowStack.Add(new Dialog_StylingGene(pawn, this, true));
			genesSettings.Close();
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

		public override void TickInterval(int delta)
		{

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

	}

}
